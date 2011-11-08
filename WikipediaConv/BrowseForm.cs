using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WikipediaConv
{
    public partial class BrowseForm : Form
    {
        /// <summary>
        /// The number of milliseconds to wait before an automatic search is initiated on the query string
        /// </summary>
        private const int AUTOSEARCH_DELAY = 400;
        /// <summary>
        /// The list of the currently opened indexes
        /// </summary>
        Dictionary<string, Indexer> indexes = new Dictionary<string, Indexer>();
        /// <summary>
        /// Delayed automatic searching support
        /// </summary>
        private DateTime? lastTextChange;
        /// <summary>
        /// Whether the search using the current text has already been executed
        /// </summary>
        private bool searchLaunched;
        /// <summary>
        /// Set to true when HitsList.SelectedValueChanged is due to us loading the new result set
        /// </summary>
        private bool loadingResults;

        public BrowseForm()
        {
            InitializeComponent();

            if (Properties.Settings.Default.Dumps != null)
            {
                foreach (string file in Properties.Settings.Default.Dumps)
                {
                    LoadIndexer(file);
                }
            }

            WebServer.Instance.UrlRequested += new UrlRequestedHandler(WebServer_UrlRequested);
            webBrowser.DocumentTitleChanged += new EventHandler(webBrowser_DocumentTitleChanged);
            webBrowser.CanGoBackChanged += new EventHandler(webBrowser_CanGoBackChanged);
            webBrowser.CanGoForwardChanged += new EventHandler(webBrowser_CanGoForwardChanged);
            
            searchBox.TextBox.PreviewKeyDown += new PreviewKeyDownEventHandler(searchBox_PreviewKeyDown);

            SyncCloseMenuItem();

            searchBox.Width = hitsBox.Width;
        }

        /// <summary>
        /// The ID of the search (to be able to cancel and identify them)
        /// </summary>
        private int searchIdentifier = 0;

        /// <summary>
        /// The search item
        /// </summary>
        private struct SearchItem
        {
            public int SearchIdentifier;
            public string SearchText;
            public HitCollection Hits;
        };

        /// <summary>
        /// Executes the search using the currently entered text
        /// </summary>
        private void LaunchSearch(bool interactive)
        {
            if (indexes.Count == 0)
            {
                return;
            }

            searchStatusLabel.Text = String.Format(Properties.Resources.SearchingForTerm, searchBox.Text);

            searchLaunched = true;

            searchIdentifier++;

            if (!interactive)
            {
                SearchItem si = new SearchItem();

                si.SearchIdentifier = searchIdentifier;
                si.SearchText = searchBox.Text;

                ThreadPool.QueueUserWorkItem(BackgroundSearch, si);

                return;
            }

            HitCollection hits = Indexer.Search(searchBox.Text, indexes.Values, Indexer.MAX_SEARCH_HITS);

            searchStatusLabel.Text = String.Empty;

            if (String.IsNullOrEmpty(hits.ErrorMessages))
            {
                loadingResults = true;

                hitsBox.DataSource = hits;
                hitsBox.SelectedItem = null;

                loadingResults = false;

                if (hits.HadMoreHits)
                {
                    searchStatusLabel.Text = String.Format(Properties.Resources.ShowingXTopResults, Indexer.MAX_SEARCH_HITS.ToString());
                }

                if (hits.Count > 0)
                {
                    hitsBox.SetSelected(0, true);

                    PageInfo page = hitsBox.SelectedItem as PageInfo;

                    webBrowser.Navigate(WebServer.Instance.GenerateUrl(page));
                }
            }
            else
            {
                MessageBox.Show(this, hits.ErrorMessages, Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Does the Lucene search in background and returns the results, if any
        /// </summary>
        /// <param name="state">The state object</param>
        private void BackgroundSearch(object state)
        {
            SearchItem si = (SearchItem)state;

            try
            {
                si.Hits = Indexer.Search(si.SearchText, indexes.Values, Indexer.MAX_SEARCH_HITS);
            }
            catch (Exception ex)
            {
                si.Hits = new HitCollection();

                si.Hits.ErrorMessages = ex.Message;
            }

            Invoke(new BackgroundSearchFinishedDelegate(BackgroundSearchFinished), si);
        }

        /// <summary>
        /// The delegate for the callback on the 'search finished' event
        /// </summary>
        /// <param name="si">Search item</param>
        private delegate void BackgroundSearchFinishedDelegate(SearchItem si);

        /// <summary>
        /// Gets called from the background thread whenever the background search finishes
        /// </summary>
        /// <param name="si">Search item</param>
        private void BackgroundSearchFinished(SearchItem si)
        {
            if (searchIdentifier == si.SearchIdentifier &&
                String.IsNullOrEmpty(si.Hits.ErrorMessages))
            {
                loadingResults = true;

                hitsBox.DataSource = si.Hits;
                hitsBox.SelectedItem = null;

                loadingResults = false;

                searchStatusLabel.Text = si.Hits.HadMoreHits ? String.Format(Properties.Resources.ShowingXTopResults, Indexer.MAX_SEARCH_HITS.ToString()) : String.Empty;
            }
            else
            {
                searchStatusLabel.Text = String.Empty;
            }
        }

        /// <summary>
        /// Cancels any pending search request. Just increments the ID to make any pending search results irrelevant
        /// </summary>
        private void CancelSearch()
        {
            searchStatusLabel.Text = String.Empty;
            
            searchIdentifier++;
        }

        /// <summary>
        /// The stack of the automatic redirects. Contains the list of automatic redirects which happened
        /// for current request. Needed to avoid the loops in redirects.
        /// </summary>
        private Stack<string> autoRedirects = new Stack<string>();

        /// <summary>
        /// Gets called whenever the browser control requests a URL from the web server
        /// </summary>
        /// <param name="sender">Web server instance</param>
        /// <param name="e">Request parameters</param>
        private void Web_UrlRequested(object sender, UrlRequestedEventArgs e)
        {
            string response = "Not found";
            string redirect = String.Empty;

            if (!String.IsNullOrEmpty(e.TeXEquation))
            {
                return;
            }

            string topic = e.TopicName.Replace('_', ' ').Trim();

            if (topic.Contains("#"))
            {
                topic = topic.Substring(0, topic.IndexOf('#')).Trim();
            }

            PageInfo page = hitsBox.SelectedItem as PageInfo;

            if (page != null &&
                topic.Equals(page.Name, StringComparison.InvariantCultureIgnoreCase) &&
                e.IndexName.Equals(page.Indexer.File, StringComparison.InvariantCultureIgnoreCase) &&
                !IsCircularRedirect(page))
            {
                response = page.GetFormattedContent();
                redirect = page.RedirectToTopic;
            }
            else
            {
                List<Indexer> searchArea = new List<Indexer>();

                // This is an internal link

                if (String.IsNullOrEmpty(e.IndexName))
                {
                    if (page != null)
                    {
                        searchArea.Add(page.Indexer);
                    }
                    else
                    {
                        foreach (Indexer ixr in indexes.Values)
                        {
                            searchArea.Add(ixr);
                        }
                    }
                }
                else
                {
                    foreach (Indexer ixr in indexes.Values)
                    {
                        if (ixr.File.Equals(e.IndexName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            searchArea.Add(ixr);

                            break;
                        }
                    }
                }

                if (searchArea.Count > 0)
                {
                    HitCollection hits = Indexer.Search(topic, searchArea, 100);

                    bool exactTopicLocated = false;

                    foreach (PageInfo pi in hits)
                    {
                        if (pi.Name.Trim().Equals(topic, StringComparison.InvariantCultureIgnoreCase) &&
                            !IsCircularRedirect(pi))
                        {
                            response = pi.GetFormattedContent();
                            redirect = pi.RedirectToTopic;

                            exactTopicLocated = true;

                            break;
                        }
                    }

                    if (hits.Count > 0 &&
                        !exactTopicLocated)
                    {
                        foreach (PageInfo pi in hits)
                        {
                            if (String.IsNullOrEmpty(pi.RedirectToTopic))
                            {
                                response = pi.GetFormattedContent();
                                redirect = pi.RedirectToTopic;

                                exactTopicLocated = true;

                                break;
                            }
                        }

                        if (!exactTopicLocated)
                        {
                            foreach (PageInfo pi in hits)
                            {
                                if (!IsCircularRedirect(pi))
                                {
                                    response = pi.GetFormattedContent();
                                    redirect = pi.RedirectToTopic;

                                    break;
                                }
                            }
                        }
                    }
                }
            }

            e.Redirect = !String.IsNullOrEmpty(redirect);
            e.RedirectTarget = redirect;
            e.Response = Encoding.UTF8.GetBytes(response);
            e.MimeType = "text/html";

            if (String.IsNullOrEmpty(redirect))
            {
                autoRedirects.Clear();
            }
            else
            {
                autoRedirects.Push(redirect);
            }
        }

        /// <summary>
        /// Checks whether the specified page would be a circular redirect
        /// </summary>
        /// <param name="pi">Page to check for circular redirect</param>
        /// <returns>True if the page has already been encountered in the list of redirects</returns>
        private bool IsCircularRedirect(PageInfo pi)
        {
            // We need to generate the formatted content to know whether the topic is redirected

            pi.GetFormattedContent();

            if (!String.IsNullOrEmpty(pi.RedirectToTopic))
            {
                foreach (string s in autoRedirects)
                {
                    if (s.Equals(pi.RedirectToTopic, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Calls the Web browser control to specify new location to load
        /// </summary>
        /// <param name="sender">The HitsBox list</param>
        /// <param name="e">Event arguments</param>
        private void hitsBox_SelectedValueChanged(object sender, EventArgs e)
        {
            PageInfo page = hitsBox.SelectedItem as PageInfo;

            if (page != null &&
                !loadingResults)
            {
                webBrowser.Navigate(WebServer.Instance.GenerateUrl(page));
            }
        }

        #region Helper methods

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            if (searchLaunched)
            {
                CancelSearch();
            }

            searchLaunched = false;

            lastTextChange = DateTime.Now;
        }

        private void searchBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Return ||
                e.KeyCode == Keys.Enter)
            {
                LaunchSearch(true);
            }
        }

        private void webBrowser_DocumentTitleChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(webBrowser.DocumentTitle))
            {
                Text = String.Format(Properties.Resources.WindowTitle, webBrowser.DocumentTitle);
            }
        }
        
        private void rtlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsi = (ToolStripMenuItem)sender;

            Settings.IsRTL = !Settings.IsRTL;
            tsi.Checked = Settings.IsRTL; 
        }
        
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BrowseForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            WebServer.Instance.Stop();

            StringCollection sc = new StringCollection();

            foreach (Indexer ixr in indexes.Values)
            {
                sc.Add(ixr.File);
            }

            Properties.Settings.Default.Dumps = sc;

            Properties.Settings.Default.Save();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = CreateIndexPickupDialog();
            if (fd.ShowDialog(this) == DialogResult.OK)
            {
                foreach (string file in fd.FileNames)
                {
                    if (indexes.ContainsKey(file.ToLowerInvariant()))
                    {
                        continue;
                    }

                    LoadIndexer(file);
                }

                SyncCloseMenuItem();
            }
        }

        private static OpenFileDialog CreateIndexPickupDialog()
        {
            OpenFileDialog fd = new OpenFileDialog();

            fd.CheckFileExists = true;
            fd.CheckPathExists = true;
            fd.DereferenceLinks = true;
            fd.Multiselect = true;
            fd.SupportMultiDottedExtensions = true;
            fd.ValidateNames = true;
            fd.Filter = Properties.Resources.OpenDumpFilter;

            return fd;
        }

        private void LoadIndexer(string file)
        {
            if (!File.Exists(file))
            {
                MessageBox.Show(this, String.Format(Properties.Resources.DumpFileDoesNotExist, file), Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            Indexer ixr = new Indexer(file);

            if (!ixr.IndexExists)
            {
                if (new ProgressDialog(ixr.LongTask, _counter).ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
            }

            indexes.Add(ixr.File, ixr);
        }

        private void SyncCloseMenuItem()
        {
            foreach (ToolStripItem tsi in closeToolStripMenuItem.DropDownItems)
            {
                tsi.Click -= new EventHandler(dumpFileClose_Click);
            }

            closeToolStripMenuItem.DropDownItems.Clear();

            SortedDictionary<string, string> sd = new SortedDictionary<string,string>();

            foreach (Indexer ixr in indexes.Values)
            {
                sd.Add(Path.GetFileNameWithoutExtension(ixr.File), ixr.File);
            }

            foreach (string file in sd.Keys)
            {
                ToolStripItem tsi = new ToolStripMenuItem(file);

                tsi.Name = sd[file];

                closeToolStripMenuItem.DropDownItems.Add(tsi);
            }

            foreach (ToolStripItem tsi in closeToolStripMenuItem.DropDownItems)
            {
                tsi.Click += new EventHandler(dumpFileClose_Click);
            }

            closeToolStripMenuItem.Enabled = (closeToolStripMenuItem.DropDownItems.Count > 0);
        }

        private void dumpFileClose_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = (ToolStripItem)sender;

            indexes[tsi.Name].Close();

            indexes.Remove(tsi.Name);

            SyncCloseMenuItem();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (searchLaunched)
            {
                return;
            }

            if (lastTextChange.HasValue &&
                DateTime.Now.Subtract(lastTextChange.Value) > TimeSpan.FromMilliseconds(AUTOSEARCH_DELAY) &&
                searchBox.Text.Length > 2)
            {
                LaunchSearch(false);
            }
        }

        private void WebServer_UrlRequested(object sender, UrlRequestedEventArgs e)
        {
            Invoke(new UrlRequestedHandler(Web_UrlRequested), sender, e);
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            LaunchSearch(true);
        }

        private void webBrowser_CanGoForwardChanged(object sender, EventArgs e)
        {
            nextButton.Enabled = webBrowser.CanGoForward;
        }

        private void webBrowser_CanGoBackChanged(object sender, EventArgs e)
        {
            backButton.Enabled = webBrowser.CanGoBack;
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            webBrowser.GoForward();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            webBrowser.GoBack();
        }

        private void hitsBox_SizeChanged(object sender, EventArgs e)
        {
            searchBox.Width = hitsBox.Width;
        }

        #endregion

        internal static DecodingProgress CreateProgress(string message, DecodingProgress.State state)
        {
            DecodingProgress ip = new DecodingProgress();
            ip.Message = message;
            ip.DecodingState = state;
            ip.ETA = "n/a";
            return ip;
        }

        public class ArchiveTask : ILongTask
        {
            // no progress information now.
            public event ProgressChangedEventHandler ProgressChanged;
            ForestWalker<DirectoryInfoCache> _walker;

            bool _abort = false;
            int _epubChapterNum = Properties.Settings.Default.ArchiveNum;

            Action<IEnumerable<FileInfo>, string> Archive;
            public string Extension { get; set; }

            public ArchiveTask(DirectoryInfoCache workDir, Action<IEnumerable<FileInfo>, string> archive, string extension)
            {
                Archive = archive;
                var node = DirectoryInfoCache.Forest(workDir);
                _walker = node.Walker;
                Extension = extension;
                SourceExtension = ".html";
            }

            public string SourceExtension { get; set; }

            Thread archiveThread;

            public void Start()
            {
                archiveThread = new Thread(ArchiveAsync);

                archiveThread.Start();

            }


            public void ArchiveAsync()
            {
                ReportProgress(0, DecodingProgress.State.Running, "start archive");
                int count = 0;
                _abort = false;
                foreach (var node in _walker)
                {
                    if (_abort)
                        break; // to call ReportProgress Finished for closing dialog.
                    if (node.CurrentEdge == ForestNode<DirectoryInfoCache>.Edge.Trailing)
                        continue;
                    var di = node.Element.Item;
                    var fisEnum = di.EnumerateFiles("*"+ SourceExtension).GetEnumerator();
                    List<FileInfo> flist = new List<FileInfo>();
                    while(fisEnum.MoveNext())
                    {
                        if (_abort)
                            break;
                        var dirname = fisEnum.Current.Directory.FullName;
                        flist.Clear();
                        CopyRange(flist, fisEnum, _epubChapterNum);
                        string epubName = RemoveYomi(Path.GetFileNameWithoutExtension(flist[0].Name)) + "-" +
                            RemoveYomi(Path.GetFileNameWithoutExtension(flist[flist.Count-1].Name)) + Extension;

                        Archive(flist, Path.Combine(dirname, epubName));
                        ReportProgress(0, DecodingProgress.State.Running, "archive: " + count++ + ":" + flist[0].Name);
                        flist.ForEach(fi => fi.Delete());
                    }

                }
                ReportProgress(0, DecodingProgress.State.Finished, "finish archive");
            }

            private string RemoveYomi(string fname)
            {
                var ar = fname.Split('_');
                List<string> lstr = new List<string>(ar);
                lstr.RemoveAt(0);
                return String.Join("_", lstr);
            }

            private void CopyRange(List<FileInfo> flist, IEnumerator<FileInfo> fisEnum, int num)
            {
                int count = 0;
                do
                {
                    flist.Add(fisEnum.Current);
                    count++;
                    if (num <= count)
                        return;
                } while (fisEnum.MoveNext());
            }

            public void Abort()
            {
                if (archiveThread != null &&
                    archiveThread.IsAlive)
                {
                    _abort = true;
                }
            }
            public void ReportProgress(int percentage, DecodingProgress.State state, string message)
            {
                DecodingProgress ip = CreateProgress(message, state);

                ProgressChanged(this, new ProgressChangedEventArgs(percentage, ip));
            }
        }

        DirectoryInfoCache _workingDir = null;
        DirectoryInfoCache GetWorkingDirectory(string bzipPath, string dirName, string interestedFilePattern)
        {
            if (_workingDir != null)
                return _workingDir;
            var parent = new FileInfo(bzipPath).Directory;
            DirectoryInfo di = new DirectoryInfo(Path.Combine(parent.FullName, dirName));
            if (!di.Exists)
            {
                di.Create();
            }
            _workingDir = new DirectoryInfoCache(null, di);
            _workingDir.PossiblyMoveFromOutside = true;
            _workingDir.InterestedFilePattern = interestedFilePattern;
            _workingDir.SyncAllToFileSystem();
            return _workingDir;
        }

        private void toEPubButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = CreateIndexPickupDialog();

            if (fd.ShowDialog(this) == DialogResult.OK)
            {
                // picked dump bzip file should place in the same folder.

                var archiver = new EPubArchiver();
                var workingDir = GetWorkingDirectory(fd.FileNames[0], "ePub/", "*.html");
                DumpFileToArchive(fd.FileNames, archiver.Archive, Dumper.CreateHtmlGenerater, workingDir, ".epub", ".html");
            }

        }

        private bool EnableAutoLogging = true;

        private void DumpFileToArchive(string[] files, Action<IEnumerable<FileInfo>, string> archive, Func<string, bool, DirectoryInfo, PerfCounter, Dumper> createDumper, DirectoryInfoCache workingDir, string outputExtension, string sourceExtension)
        {
            using (var uwAll = _counter.UsingWatch("All"))
            {
                foreach (string file in files)
                {
                    bool isJapanese = IsJapanese(file);
                    Dumper gen = createDumper(file, isJapanese, workingDir.Item, _counter);
                    gen.EnableAutoLogging = EnableAutoLogging;
                    if (DialogResult.OK != new ProgressDialog(gen.LongTask, _counter).ShowDialog(this))
                    {
                        // MessageBox.Show("generate html cancelled");
                        // tmp fall through. 
                        // return;
                    }
                }

                using (var uwArchive = _counter.UsingWatch("Archive"))
                {
                    ArchiveTask archiveTask = new ArchiveTask(workingDir, archive, outputExtension);
                    archiveTask.SourceExtension = sourceExtension;
                    if (DialogResult.OK != new ProgressDialog(archiveTask, _counter).ShowDialog(this))
                    {
                        MessageBox.Show("generate epub cancelled");
                        return;
                    }
                }
            }
        }

        private bool IsJapanese(string file)
        {
            return Path.GetFileName(file).StartsWith("ja");
        }

        PerfCounter _counter = new PerfCounter();

        private void toPdfButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = CreateIndexPickupDialog();

            if (fd.ShowDialog(this) == DialogResult.OK)
            {
                var archiver = new PdfArchiver();
                var workingDir = GetWorkingDirectory(fd.FileNames[0], "pdf", "*.wiki");
                DumpFileToArchive(fd.FileNames, archiver.Archive, Dumper.CreateRawDumper, workingDir,  ".pdf", ".wiki");
            }
        }

        private void profileMenuItem_Click(object sender, EventArgs e)
        {
            var pd = new ProfileDialog(_counter);
            pd.Show(this);
        }
    }
}
