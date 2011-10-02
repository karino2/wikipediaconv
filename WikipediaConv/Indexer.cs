using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Snowball;
using Lucene.Net.Analysis.Cn;
using Lucene.Net.Analysis.CJK;
using Lucene.Net.Analysis.Cz;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Index;
using Lucene.Net.Search;
using RAMDirectory = Lucene.Net.Store.RAMDirectory;
using FSDirectory = Lucene.Net.Store.FSDirectory;

namespace WikipediaConv
{
    public class Indexer
    {
        /// <summary>
        /// The maximum hits to return per query
        /// </summary>
        public const int MAX_SEARCH_HITS = 100;

        IndexerAction _action;
        BzipReader _bzipReader;
        public Indexer(string path)
        {
            _action = new IndexerAction(path);
            _bzipReader = new BzipReader(path, _action);
            _action._report = _bzipReader;
        }

        #region delegate to IndexAction and BzipReader
        public bool IndexExists { get { return _action.IndexExists; } }
        public IndexSearcher Searcher { get { return _action.Searcher; } }
        public void CreateIndex() { _bzipReader.CreateIndex(); }
        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            _bzipReader.OnProgressChanged(e);
        }
        /// <summary>
        /// Occurs when the indexing is happening and the progress changes
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged {
            add {
                _bzipReader.ProgressChanged += value;
            }
            remove
            {
                _bzipReader.ProgressChanged -= value;
            }
        }
        public string FilePath { get { return _bzipReader.FilePath; } }

        #endregion

        #region Searching methods

        private bool searchRunning;

        private struct SearchItem
        {
            public string SearchRequest;
            public int MaxResults;
            public HitCollection Hits;
            public Queue<Exception> Errors;
        }

        /// <summary>
        /// Searches for the specified term in the index.
        /// </summary>
        /// <param name="term">The term.</param>
        public static HitCollection Search(string term, IEnumerable<Indexer> indexers, int maxResults)
        {
            foreach (Indexer ixr in indexers)
            {
                if (!ixr.IndexExists ||
                    ixr.Searcher == null)
                {
                    throw new Exception(String.Format(Properties.Resources.IndexDoesNotExist, ixr.FilePath));
                }
            }

            string searchRequest = String.Format("title:\"{0}\" AND -\"Image:\"", term);

            HitCollection ret = new HitCollection();

            SearchItem si = new SearchItem();

            si.Hits = ret;
            si.SearchRequest = searchRequest;
            si.MaxResults = maxResults;
            si.Errors = new Queue<Exception>();

            int indexersNumber = 0;

            // I can't really be sure about the thread safety of Lucene

            try
            {
                lock (typeof(Indexer))
                {
                    foreach (Indexer ixr in indexers)
                    {
                        indexersNumber++;

                        int i = 0;

                        while (ixr.searchRunning &&
                            i < 30)
                        {
                            Thread.Sleep(100);

                            i++;
                        }

                        if (i >= 30)
                        {
                            throw new Exception(Properties.Resources.FailedStartingSearch);
                        }

                        ixr.searchRunning = true;
                    }

                    foreach (Indexer ixr in indexers)
                    {
                        ThreadPool.QueueUserWorkItem(ixr.Search, si);
                    }

                    foreach (Indexer ixr in indexers)
                    {
                        int i = 0;

                        while (ixr.searchRunning &&
                            i < 30)
                        {
                            Thread.Sleep(100);

                            i++;
                        }

                        if (i >= 30)
                        {
                            throw new Exception(Properties.Resources.FailedFinishingSearch);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lock (si.Errors)
                {
                    si.Errors.Enqueue(ex);
                }
            }

            StringCollection sc = new StringCollection();

            lock (si.Errors)
            {
                while (si.Errors.Count > 0)
                {
                    Exception ex = si.Errors.Dequeue();

                    if (!sc.Contains(ex.Message))
                    {
                        sc.Add(ex.Message);
                    }
                }
            }

            StringBuilder sb = new StringBuilder();

            foreach (string message in sc)
            {
                sb.AppendLine(message);
            }

            ret.ErrorMessages = sb.Length > 0 ? sb.ToString() : String.Empty;

            if (indexersNumber > 1)
            {
                PageInfo[] arr = new PageInfo[ret.Count];

                for (int i = 0; i < ret.Count; i++)
                {
                    arr[i] = ret[i];
                }

                Array.Sort<PageInfo>(arr,
                    delegate(PageInfo a, PageInfo b)
                    {
                        return (a.Score < b.Score ? 1 : (a.Score > b.Score ? -1 : 0));
                    });

                ret.Clear();

                foreach (PageInfo pi in arr)
                {
                    ret.Add(pi);
                }
            }

            return ret;
        }

        /// <summary>
        /// The searches are always executed on the thread pools
        /// </summary>
        /// <param name="state">The query request</param>
        private void Search(object state)
        {
            SearchItem si = (SearchItem)state;

            try
            {
                Query q = _action.QueryParser.Parse(si.SearchRequest);

                IEnumerator hits = _action.Searcher.Search(q).Iterator();

                int i = 0;

                while (hits.MoveNext() &&
                    i < si.MaxResults)
                {
                    Hit hit = (Hit)hits.Current;

                    PageInfo pi = new PageInfo(this, hit);

                    lock (si.Hits)
                    {
                        si.Hits.Add(pi);
                    }

                    i++;
                }

                if (i == si.MaxResults &&
                    hits.MoveNext())
                {
                    lock (si.Hits)
                    {
                        si.Hits.HadMoreHits = true;
                    }
                }
            }
            catch (Exception ex)
            {
                lock (si.Errors)
                {
                    si.Errors.Enqueue(ex);
                }
            }

            searchRunning = false;
        }

        #endregion

        internal void AbortIndex()
        {
            _bzipReader.AbortIndex();
        }

        public string File { get { return _bzipReader.FilePath.ToLowerInvariant(); } }

        internal string LoadAndDecodeBlock(long[] beginnings, long[] ends)
        {
            return _bzipReader.LoadAndDecodeBlock(beginnings, ends);
        }

        internal void Close()
        {
            _bzipReader.Close();
        }
    }

    public class IndexerAction : IArchiveAction
    {
        internal IReportProgress _report;

        private const int MAX_RAMDIRECTORY_DOCS = 500000;
        /// <summary>
        /// The path to the dump file
        /// </summary>
        private string filePath;
        /// <summary>
        /// The path to the index
        /// </summary>
        private string indexPath;
        /// <summary>
        /// The index searcher
        /// </summary>
        private IndexSearcher searcher;
        /// <summary>
        /// Whether the index exists at the specified location
        /// </summary>
        private bool indexExists;
        /// <summary>
        /// The analyzer to use for indexing and query tokenizing
        /// </summary>
        private Analyzer textAnalyzer;
        /// <summary>
        /// The query parser for the searches in this index
        /// </summary>
        private QueryParser queryParser;

        /// <summary>
        /// Gets a value indicating whether index exists.
        /// </summary>
        /// <value><c>true</c> if index exists; otherwise, <c>false</c>.</value>
        public bool IndexExists
        {
            get
            {
                return indexExists;
            }
        }

        /// <summary>
        /// The index writer which is used for indexing
        /// </summary>
        private IndexWriter indexer;
        private IndexWriter memoryIndexer;

        /// <summary>
        /// Whether to use multiple threads while indexing the documents
        /// </summary>
        private bool multithreadedIndexing = false;

        /// <summary>
        /// Store the offsets of the previous block in case of the Wiki topic carryover
        /// </summary>
        private long previousBlockBeginning = -1;
        private long previousBlockEnd = -1;
        private int activeThreads = 0;

        public IndexerAction(string path)
        {
            filePath = path;
            indexPath = Path.ChangeExtension(filePath, ".idx");

            if (Directory.Exists(indexPath) &&
                IndexReader.IndexExists(indexPath))
            {
                indexExists = true;
            }

            if (indexExists)
            {
                searcher = new IndexSearcher(indexPath);
            }

            textAnalyzer = GuessAnalyzer(filePath);

            queryParser = new QueryParser("title", textAnalyzer);

            queryParser.SetDefaultOperator(QueryParser.Operator.AND);
            multithreadedIndexing = (Environment.ProcessorCount > 1);
            AbortIndexing = false;
        }


        public void PreAction()
        {
            // Close any searchers

            if (searcher != null)
            {
                searcher.Close();

                searcher = null;
            }

            indexExists = false;

            // Create the index writer

            indexer = new IndexWriter(indexPath, textAnalyzer, true);
            memoryIndexer = new IndexWriter(new RAMDirectory(), textAnalyzer, true);

            memoryIndexer.SetMaxBufferedDocs(1000);
            memoryIndexer.SetMergeFactor(100);

            indexer.SetMaxBufferedDocs(1000);
            indexer.SetMergeFactor(100);
        }

        /// <summary>
        /// Indexes the provided string
        /// </summary>
        /// <param name="currentText">The string to index</param>
        /// <param name="beginning">The beginning offset of the block</param>
        /// <param name="end">The end offset of the block</param>
        /// <param name="charCarryOver">Whether there was a Wiki topic carryover from previous block</param>
        /// <param name="lastBlock">True if this is the last block</param>
        /// <returns>The number of characters in the end of the string that match the header entry</returns>
        public int IndexString(string currentText, long beginning, long end, int charCarryOver, bool lastBlock)
        {
            bool firstRun = true;

            int topicStart = currentText.IndexOf("<title>", StringComparison.InvariantCultureIgnoreCase);

            int titleEnd, idStart, idEnd, topicEnd = -1;

            string title = String.Empty;
            long id = -1;

            while (topicStart >= 0 &&
                !AbortIndexing)
            {
                titleEnd = -1;
                idStart = -1;
                idEnd = -1;
                topicEnd = -1;

                titleEnd = currentText.IndexOf("</title>", topicStart, StringComparison.InvariantCultureIgnoreCase);

                if (titleEnd < 0)
                {
                    break;
                }

                title = currentText.Substring(topicStart + "<title>".Length, titleEnd - topicStart - "<title>".Length);

                idStart = currentText.IndexOf("<id>", titleEnd, StringComparison.InvariantCultureIgnoreCase);

                if (idStart < 0)
                {
                    break;
                }

                idEnd = currentText.IndexOf("</id>", idStart, StringComparison.InvariantCultureIgnoreCase);

                if (idEnd < 0)
                {
                    break;
                }

                id = Convert.ToInt64(currentText.Substring(idStart + "<id>".Length, idEnd - idStart - "<id>".Length));

                topicEnd = currentText.IndexOf("</text>", idEnd, StringComparison.InvariantCultureIgnoreCase);

                if (topicEnd < 0)
                {
                    break;
                }

                // Start creating the object for the tokenizing ThreadPool thread

                long[] begins = new long[1];
                long[] ends = new long[1];

                // Was there a carryover?

                if (firstRun)
                {
                    // Did the <title> happen in the carryover area?

                    if (charCarryOver > 0 &&
                        topicStart < charCarryOver)
                    {
                        if (previousBlockBeginning > -1 &&
                            previousBlockEnd > -1)
                        {
                            begins = new long[2];
                            ends = new long[2];

                            begins[1] = previousBlockBeginning;
                            ends[1] = previousBlockEnd;
                        }
                        else
                        {
                            throw new Exception(Properties.Resources.CarryoverNoPrevBlock);
                        }
                    }
                }

                begins[0] = beginning;
                ends[0] = end;

                PageInfo pi = new PageInfo(id, title, begins, ends);

                Interlocked.Increment(ref activeThreads);

                if (multithreadedIndexing)
                {
                    ThreadPool.QueueUserWorkItem(TokenizeAndAdd, pi);
                }
                else
                {
                    TokenizeAndAdd(pi);
                }

                // Store the last successful title start position

                int nextTopicStart = currentText.IndexOf("<title>", topicStart + 1, StringComparison.InvariantCultureIgnoreCase);

                if (nextTopicStart >= 0)
                {
                    topicStart = nextTopicStart;
                }
                else
                {
                    break;
                }

                firstRun = false;
            }

            // Now calculate how many characters we need to save for next block

            int charsToSave = 0;

            if (topicStart == -1)
            {
                if (!lastBlock)
                {
                    throw new Exception(Properties.Resources.NoTopicsInBlock);
                }
            }
            else
            {
                if (!lastBlock)
                {
                    if (topicEnd == -1)
                    {
                        charsToSave = currentText.Length - topicStart;
                    }
                    else
                    {
                        if (topicStart < topicEnd)
                        {
                            charsToSave = currentText.Length - topicEnd - "</text>".Length;
                        }
                        else
                        {
                            charsToSave = currentText.Length - topicStart;
                        }
                    }
                }
            }

            // Flush the memory indexer to disk from time to time

            if (memoryIndexer.DocCount() >= MAX_RAMDIRECTORY_DOCS)
            {
                _report.ReportProgress(0, IndexingProgress.State.Running, Properties.Resources.FlushingDocumentsToDisk);

                while (activeThreads != 0)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }

                Lucene.Net.Store.Directory dir = memoryIndexer.GetDirectory();

                memoryIndexer.Close();

                indexer.AddIndexes(new Lucene.Net.Store.Directory[] { dir });

                memoryIndexer = new IndexWriter(new RAMDirectory(), textAnalyzer, true);
            }

            previousBlockBeginning = beginning;
            previousBlockEnd = end;

            return charsToSave;
        }
        /// <summary>
        /// Tokenizes and adds the specified PageInfo object to Lucene index
        /// </summary>
        /// <param name="state">PageInfo object</param>
        private void TokenizeAndAdd(object state)
        {
            PageInfo pi = (PageInfo)state;

            Document d = new Document();

            // Store title

            d.Add(new Field("title", pi.Name, Field.Store.YES, Field.Index.TOKENIZED));

            // Current block offsets

            d.Add(new Field("beginning", BitConverter.GetBytes(pi.Beginnings[0]), Field.Store.YES));
            d.Add(new Field("end", BitConverter.GetBytes(pi.Ends[0]), Field.Store.YES));

            if (pi.Beginnings.Length == 2)
            {
                // Previous block offsets

                d.Add(new Field("beginning", BitConverter.GetBytes(pi.Beginnings[1]), Field.Store.YES));
                d.Add(new Field("end", BitConverter.GetBytes(pi.Ends[1]), Field.Store.YES));
            }

            // Topic ID

            d.Add(new Field("topicid", pi.TopicId.ToString(), Field.Store.YES, Field.Index.UN_TOKENIZED));

            memoryIndexer.AddDocument(d);

            Interlocked.Decrement(ref activeThreads);
        }

        public void WaitTillFinish()
        {
            while (activeThreads != 0)
            {
                _report.ReportProgress(0, IndexingProgress.State.Running, String.Format(Properties.Resources.WaitingForTokenizers, activeThreads));

                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
            _report.ReportProgress(0, IndexingProgress.State.Running, Properties.Resources.FlushingDocumentsToDisk);
        }


        public void PostAction()
        {
            Lucene.Net.Store.Directory dir = memoryIndexer.GetDirectory();

            memoryIndexer.Close();

            indexer.AddIndexes(new Lucene.Net.Store.Directory[] { dir });

            memoryIndexer = null;
            _report.ReportProgress(0, IndexingProgress.State.Running, Properties.Resources.OptimizingIndex);

            indexer.Optimize();

            indexExists = true;
        }

        public void FinalizeAction(bool failedOrAbort)
        {
            if (indexer != null)
            {
                indexer.Close();

                indexer = null;
            }

            if (failedOrAbort)
            {
                Directory.Delete(indexPath, true);

                indexExists = false;
            }
            else
            {
                if (indexExists)
                {
                    searcher = new IndexSearcher(indexPath);
                }
            }
        }

        /// <summary>
        /// Closes the searcher object
        /// </summary>
        public void Close()
        {
            if (indexExists &&
                searcher != null)
            {
                searcher.Close();

                searcher = null;
            }
        }

        private Analyzer GuessAnalyzer(string filePath)
        {
            Analyzer ret = null;

            switch (Path.GetFileName(filePath).Substring(0, 2).ToLowerInvariant())
            {
                case "zh":
                    ret = new ChineseAnalyzer();
                    break;
                case "cs":
                    ret = new CzechAnalyzer();
                    break;
                case "da":
                    ret = new SnowballAnalyzer("Danish");
                    break;
                case "nl":
                    ret = new SnowballAnalyzer("Dutch");
                    break;
                case "en":
                    ret = new SnowballAnalyzer("English");
                    break;
                case "fi":
                    ret = new SnowballAnalyzer("Finnish");
                    break;
                case "fr":
                    ret = new SnowballAnalyzer("French");
                    break;
                case "de":
                    ret = new SnowballAnalyzer("German");
                    break;
                case "it":
                    ret = new SnowballAnalyzer("Italian");
                    break;
                case "ja":
                    ret = new CJKAnalyzer();
                    break;
                case "ko":
                    ret = new CJKAnalyzer();
                    break;
                case "no":
                    ret = new SnowballAnalyzer("Norwegian");
                    break;
                case "pt":
                    ret = new SnowballAnalyzer("Portuguese");
                    break;
                case "ru":
                    ret = new SnowballAnalyzer("Russian");
                    break;
                case "es":
                    ret = new SnowballAnalyzer("Spanish");
                    break;
                case "se":
                    ret = new SnowballAnalyzer("Swedish");
                    break;
                default:
                    ret = new StandardAnalyzer();
                    break;
            }

            return ret;
        }

        public IndexSearcher Searcher { get { return searcher; } }
        public QueryParser QueryParser { get { return queryParser; } }


        public bool AbortIndexing { get; set; }
    }
}
