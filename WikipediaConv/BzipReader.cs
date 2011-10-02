﻿using System;
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
    public interface IArchiveAction
    {
        void PreAction();
        int IndexString(string currentText, long beginning, long end, int charCarryOver, bool lastBlock);
        void PostAction();
        void FinalizeAction(bool failedOrAbort);

        void Close();

        void WaitTillFinish();

        bool AbortIndexing { get; set; }
    }
    public interface IReportProgress
    {
        void ReportProgress(int percentage, IndexingProgress.State status, string message);
    }
    public class BzipReader : IReportProgress
    {

        IArchiveAction _action;
        /// <summary>
        /// The maximum number of decoded blocks to keep in memory. A single block is roughly 1 M characters = 2 Mb
        /// </summary>
        private const int MAX_CACHED_BLOCKS_NO = 30;
        /// <summary>
        /// The maximum number of blocks we would be prepared to process in bz2 file.
        /// 200000 blocks = 160 Gb uncompressed, hope this is enough
        /// </summary>
        private const int MAX_BLOCKS_NO = 200000;
        /// <summary>
        /// UTF8 decoder which would throw on unknown character
        /// </summary>
        private Decoder utf8 = new UTF8Encoding(true, false).GetDecoder();
        /// <summary>
        /// The path to the dump file
        /// </summary>
        private string filePath;
        public string FilePath { get { return filePath; } }
        /// <summary>
        /// Occurs when the indexing is happening and the progress changes
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;
        /// <summary>
        /// The total number of blocks in the file
        /// </summary>
        private long totalBlocks = MAX_BLOCKS_NO;
        /// <summary>
        /// The buffer for the beginnings of the blocks
        /// </summary>
        private long[] beginnings = new long[MAX_BLOCKS_NO];
        /// <summary>
        /// The buffer for the ends of the blocks
        /// </summary>
        private long[] ends = new long[MAX_BLOCKS_NO];
        /// <summary>
        /// The buffer for the current block
        /// </summary>
        private byte[] blockBuf;
        /// <summary>
        /// The character buffer for the current block
        /// </summary>
        private char[] charBuf;
        /// <summary>
        /// The indexing thread
        /// </summary>
        private Thread indexingThread;
        /// <summary>
        /// The bzip2-watching thread
        /// </summary>
        private Thread bzip2WatchThread;
        /// <summary>
        /// Whether the indexing process should be aborted
        /// </summary>
        private bool abortIndexing;
        /// <summary>
        /// Percent done for bzip2 block location progress
        /// </summary>
        private int bz2_blocks_pct_done;
        /// <summary>
        /// Total bz2 file size, for progress calculation
        /// </summary>
        private long bz2_filesize;
        /// <summary>
        /// The cache of decoded blocks from file
        /// </summary>
        private Dictionary<long, byte[]> blocksCache = new Dictionary<long, byte[]>();
        /// <summary>
        /// start time for operations
        /// </summary>
        private DateTime startTime;
        /// <summary>
        /// elapsed time for operations
        /// </summary>
        private TimeSpan elapsed;

        /// <summary>
        /// Initializes a new instance of the <see cref="BzipReader"/> class.
        /// </summary>
        /// <param name="path">The path to the .xml.bz2 dump of wikipedia</param>
        public BzipReader(string path, IArchiveAction action)
        {
            filePath = path;
            _action = action;


            abortIndexing = false;

        }

        #region Indexing methods
        /// <summary>
        ///  watch the bzip2 block locator and report progress
        /// </summary>
        private void BzipBlockLocatorProgressMonitor()
        {
            while (true) // we expect to be aborted externally
            {
                Thread.Sleep(500);
                ReportProgress(bz2_blocks_pct_done, IndexingProgress.State.Running, String.Empty);
            }
        }

        /// <summary>
        /// Creates the index for the bz2 file on a separate thread.
        /// </summary>
        private void CreateIndexAsync()
        {
            bool failed = false;
            startTime = DateTime.Now;

            try
            {
                InitializeIndexer();

                // Locate the bzip2 blocks in the file

                LocateBlocks();

                // Two times more than the first block but not less than 100 bytes

                long bufSize = ((ends[0] - beginnings[0]) / 8) * 2 + 100;

                // Buffers for the current and next block

                blockBuf = new byte[bufSize];
                charBuf = new char[bufSize];

                // Whether there was a Wiki topic carryover from current block to the next one

                char[] charCarryOver = new char[0];

                // The length of the currently loaded data

                long loadedLength = 0;

                StringBuilder sb = new StringBuilder();

                // Starting indexing

                startTime = DateTime.Now;
                elapsed = new TimeSpan(0);
                ReportProgress(0, IndexingProgress.State.Running, Properties.Resources.ProgressIndexing);
                for (long i = 0; i < totalBlocks && !abortIndexing; i++)
                {
                    ReportProgress((int)((double)(i * 100) / (double)totalBlocks), IndexingProgress.State.Running, String.Empty);

                    #region Indexing logic

                    loadedLength = LoadBlock(beginnings[i], ends[i], ref blockBuf);

                    if (charBuf.Length < blockBuf.Length)
                    {
                        charBuf = new char[blockBuf.Length];
                    }

                    int bytesUsed = 0;
                    int charsUsed = 0;
                    bool completed = false;

                    // Convert the text to UTF8

                    utf8.Convert(blockBuf, 0, (int)loadedLength, charBuf, 0, charBuf.Length, i == totalBlocks - 1, out bytesUsed, out charsUsed, out completed);

                    if (!completed)
                    {
                        throw new Exception(Properties.Resources.UTFDecoderError);
                    }

                    // Construct a current string

                    sb.Length = 0;

                    if (charCarryOver.Length > 0)
                    {
                        sb.Append(charCarryOver);
                    }

                    sb.Append(charBuf, 0, charsUsed);

                    int carryOverLength = charCarryOver.Length;

                    int charsMatched = IndexString(sb.ToString(), beginnings[i], ends[i], carryOverLength, i == totalBlocks - 1);

                    // There's a Wiki topic carryover, let's store the characters which need to be carried over 

                    if (charsMatched > 0)
                    {
                        charCarryOver = new char[charsMatched];

                        sb.CopyTo(charsUsed + carryOverLength - charsMatched, charCarryOver, 0, charsMatched);
                    }
                    else
                    {
                        charCarryOver = new char[0];
                    }

                    #endregion
                }

                // Wait till all the threads finish
                WaitTillFinish();

                PostAction();
            }
            catch (Exception ex)
            {
                ReportProgress(0, IndexingProgress.State.Failure, ex.ToString());

                failed = true;
            }

            // Try to release some memory
            FinalizeAction(failed);
            ReportProgress(0, IndexingProgress.State.Finished, String.Empty);
        }

        private void WaitTillFinish()
        {
            _action.WaitTillFinish();
            /*
            while (activeThreads != 0)
            {
                ReportProgress(0, IndexingProgress.State.Running, String.Format(Properties.Resources.WaitingForTokenizers, activeThreads));

                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
            ReportProgress(0, IndexingProgress.State.Running, Properties.Resources.FlushingDocumentsToDisk);
             * */
        }

        #region Indexer Related
        private void FinalizeAction(bool failed)
        {
            _action.FinalizeAction(failed || abortIndexing);
            /*
            if (indexer != null)
            {
                indexer.Close();

                indexer = null;
            }

            if (failed ||
                abortIndexing)
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
             * */
        }

        private void PostAction()
        {
            _action.PostAction();
            /*
            Lucene.Net.Store.Directory dir = memoryIndexer.GetDirectory();

            memoryIndexer.Close();

            indexer.AddIndexes(new Lucene.Net.Store.Directory[] { dir });

            memoryIndexer = null;
            ReportProgress(0, IndexingProgress.State.Running, Properties.Resources.OptimizingIndex);

            indexer.Optimize();

            indexExists = true;
             * */
        }

        private void InitializeIndexer()
        {
            _action.PreAction();
            /*
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
             * */

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
        private int IndexString(string currentText, long beginning, long end, int charCarryOver, bool lastBlock)
        {
            return _action.IndexString(currentText, beginning, end, charCarryOver, lastBlock);
        }
        /// <summary>
        /// Closes the searcher object
        /// </summary>
        public void Close()
        {
            _action.Close();
            /*
            if (indexExists &&
                searcher != null)
            {
                searcher.Close();

                searcher = null;
            }
             * */
        }

        #endregion 




        #endregion

        #region Block methods

        /// <summary>
        /// Class-level variable to do the allocation just once
        /// </summary>
        private byte[] decompressionBuf;

        /// <summary>
        /// Loads the bzip2 block from the file
        /// </summary>
        /// <param name="beginning">The beginning of the block, in bits.</param>
        /// <param name="end">The end of the block, in bits.</param>
        /// <param name="buf">The buffer to load into.</param>
        /// <returns>The length of the loaded data, in bytes</returns>
        private long LoadBlock(long beginning, long end, ref byte[] buf)
        {
            long bufSize = buf.LongLength;

            bzip2.StatusCode status = bzip2.BZ2_bzLoadBlock(filePath, beginning, end, buf, ref bufSize);

            if (status != bzip2.StatusCode.BZ_OK)
            {
                throw new Exception(String.Format(Properties.Resources.FailedLoadingBlock, filePath, beginning, status));
            }

            // Just some initial value, we will reallocate the buffer as needed

            if (decompressionBuf == null)
            {
                decompressionBuf = new byte[buf.Length * 4];
            }

            int intBufSize = (int)bufSize;
            int intDecompSize = decompressionBuf.Length;

            status = bzip2.BZ2_bzDecompress(buf, intBufSize, decompressionBuf, ref intDecompSize);

            // Limit a single uncompressed block size to 32 Mb

            while (status == bzip2.StatusCode.BZ_OUTBUFF_FULL &&
                decompressionBuf.Length < 32000000)
            {
                decompressionBuf = new byte[decompressionBuf.Length * 2];

                intDecompSize = decompressionBuf.Length;

                status = bzip2.BZ2_bzDecompress(buf, intBufSize, decompressionBuf, ref intDecompSize);
            }

            if (decompressionBuf.Length > 32000000)
            {
                throw new Exception(String.Format(Properties.Resources.FailedUncompressingMemory, beginning));
            }

            if (status != bzip2.StatusCode.BZ_OK)
            {
                throw new Exception(String.Format(Properties.Resources.FailedUncompressingStatus, beginning, status));
            }

            // Exchange the raw buffer and the uncompressed one

            byte[] exch = buf;

            buf = decompressionBuf;

            decompressionBuf = exch;

            return intDecompSize;
        }

        /// <summary>
        /// Locates the bzip2 blocks in the file
        /// </summary>
        private void LocateBlocks()
        {
            ReportProgress(0, IndexingProgress.State.Running, Properties.Resources.ProgressLocatingBlocks);
            FileInfo fi = new FileInfo(filePath);
            bz2_filesize = fi.Length;
            startTime = DateTime.Now;
            elapsed = new TimeSpan(0);
            bzip2WatchThread = new Thread(BzipBlockLocatorProgressMonitor);
            bzip2WatchThread.Start(); // start the monitor, to report progress

            bzip2.StatusCode status = bzip2.BZ2_bzLocateBlocks(filePath, beginnings, ends, ref totalBlocks, ref bz2_blocks_pct_done);
            bzip2WatchThread.Abort();

            if (status != bzip2.StatusCode.BZ_OK)
            {
                throw new Exception(String.Format(Properties.Resources.FailedLocatingBlocks, filePath, status));
            }

            if (totalBlocks < 1)
            {
                throw new Exception(String.Format(Properties.Resources.NoBlocksFound, filePath));
            }
        }

        /// <summary>
        /// Loads a block from the file, decompresses it and decodes to string
        /// </summary>
        /// <param name="begin">The list of block beginnings</param>
        /// <param name="end">The list of block ends</param>
        /// <returns>The decoded string</returns>
        public string LoadAndDecodeBlock(long[] begin, long[] end)
        {
            byte[] currentBuf = null;

            utf8.Reset();

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < begin.Length; i++)
            {
                if (blocksCache.ContainsKey(begin[i]))
                {
                    currentBuf = blocksCache[begin[i]];
                }
                else
                {
                    if (blockBuf == null)
                    {
                        blockBuf = new byte[((end[i] - begin[i]) / 8) * 2 + 100];
                    }

                    long loadedLen = LoadBlock(begin[i], end[i], ref blockBuf);

                    currentBuf = new byte[loadedLen];

                    Array.Copy(blockBuf, 0, currentBuf, 0, (int)loadedLen);

                    blocksCache[begin[i]] = currentBuf;
                }

                if (charBuf == null ||
                    charBuf.Length < currentBuf.Length)
                {
                    charBuf = new char[currentBuf.Length];
                }

                int bytesUsed = 0;
                int charsUsed = 0;
                bool completed = false;

                utf8.Convert(currentBuf, 0, currentBuf.Length, charBuf, 0, charBuf.Length, i == begin.Length, out bytesUsed, out charsUsed, out completed);

                sb.Append(charBuf, 0, charsUsed);
            }

            // Purge the cache if needed

            if (blocksCache.Count > MAX_CACHED_BLOCKS_NO)
            {
                List<long> listBegins = new List<long>();

                listBegins.AddRange(blocksCache.Keys);

                Random r = new Random();

                int indexToRemove = r.Next(blocksCache.Count);

                while (blocksCache.Count > MAX_CACHED_BLOCKS_NO)
                {
                    blocksCache.Remove(listBegins[indexToRemove]);

                    listBegins.RemoveAt(indexToRemove);

                    indexToRemove = r.Next(blocksCache.Count);
                }
            }

            return sb.ToString();
        }

        #endregion

        #region Misc

        public void ReportProgress(int percentage, IndexingProgress.State status, string message)
        {
            int eta;
            IndexingProgress ip = new IndexingProgress();

            ip.IndexingState = status;
            ip.Message = message;

            // a naive ETA formula: ETA = ElapsedMinutes * 100 / percentDone - Elapsed

            if (percentage > 0)
            {
                elapsed = (DateTime.Now.Subtract(startTime));
                eta = (int)(elapsed.TotalMinutes * 100 / percentage - elapsed.TotalMinutes);

                if (eta <= 0)
                {
                    ip.ETA = Properties.Resources.FinishingSoon;
                }
                else
                {
                    TimeSpan remaining = TimeSpan.FromMinutes(eta + 1);
                    ip.ETA = String.Format(Properties.Resources.FinishingInHours, (int)remaining.TotalHours, remaining.Minutes);
                }
            }
            else
            {
                ip.ETA = "n/a";
            }

            OnProgressChanged(new ProgressChangedEventArgs(percentage, ip));
        }

        public void CreateIndex()
        {
            indexingThread = new Thread(CreateIndexAsync);

            indexingThread.Start();
        }

        public void AbortIndex()
        {
            if (indexingThread != null &&
                indexingThread.IsAlive)
            {
                abortIndexing = true;
                _action.AbortIndexing = true;
            }
        }

        public virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(this, e);
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

        /// <summary>
        /// The name of the dump file this index is associated
        /// </summary>
        public string File
        {
            get { return filePath.ToLowerInvariant(); }
        }

        #endregion
    }
}