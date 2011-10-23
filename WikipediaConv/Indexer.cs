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
    public interface ILongTask
    {
        event ProgressChangedEventHandler ProgressChanged;
        void Start();
        void Abort();
    }
    public interface ILoadAndDecodeBlocker
    {
        string LoadAndDecodeBlock(long[] beginnings, long[] ends);
    }
    public class Indexer : ILoadAndDecodeBlocker
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
            _action._notify = _bzipReader;
        }

        public ILongTask LongTask { get { return _bzipReader; } }

        #region ILongTask
        public void CreateIndex() { _bzipReader.StartDecodeThread(); }
        public void AbortIndex()
        {
            _bzipReader.AbortDecoding();
        }
        /// <summary>
        /// Occurs when the indexing is happening and the progress changes
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged
        {
            add
            {
                _bzipReader.ProgressChanged += value;
            }
            remove
            {
                _bzipReader.ProgressChanged -= value;
            }
        }
        #endregion

        #region delegate to IndexAction and BzipReader
        public bool IndexExists { get { return _action.IndexExists; } }
        public IndexSearcher Searcher { get { return _action.Searcher; } }

        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            _bzipReader.OnProgressChanged(e);
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

        public string File { get { return _bzipReader.FilePath.ToLowerInvariant(); } }

        public string LoadAndDecodeBlock(long[] beginnings, long[] ends)
        {
            return _bzipReader.LoadAndDecodeBlock(beginnings, ends);
        }

        internal void Close()
        {
            _bzipReader.Close();
        }
    }

    public class IndexerAction : IDecodedAction
    {
        internal INotifyDecoder _notify;

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


        public void FlashAction()
        {
            // Flush the memory indexer to disk from time to time

            if (memoryIndexer.DocCount() >= MAX_RAMDIRECTORY_DOCS)
            {
                _notify.ReportProgress(0, DecodingProgress.State.Running, Properties.Resources.FlushingDocumentsToDisk);

                while (_notify.IsActive())
                {
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }

                Lucene.Net.Store.Directory dir = memoryIndexer.GetDirectory();

                memoryIndexer.Close();

                indexer.AddIndexes(new Lucene.Net.Store.Directory[] { dir });

                memoryIndexer = new IndexWriter(new RAMDirectory(), textAnalyzer, true);
            }
        }
        /// <summary>
        /// Tokenizes and adds the specified PageInfo object to Lucene index
        /// </summary>
        /// <param name="state">PageInfo object</param>
        public bool Action(PageInfo pi)
        {
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
            return true;
        }



        public void PostAction()
        {
            Lucene.Net.Store.Directory dir = memoryIndexer.GetDirectory();

            memoryIndexer.Close();

            indexer.AddIndexes(new Lucene.Net.Store.Directory[] { dir });

            memoryIndexer = null;
            _notify.ReportProgress(0, DecodingProgress.State.Running, Properties.Resources.OptimizingIndex);

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
    }
}
