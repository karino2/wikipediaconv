using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using ScrewTurn.Wiki;
using System.Diagnostics;

namespace WikipediaConv
{
    public class PageInfo
    {
        public class RedirectException : Exception {
            public RedirectException(string message)
                : base(message)
            {
            }
        }
        /// <summary>
        /// The cached formatted content
        /// </summary>
        private string formattedContent;
        /// <summary>
        /// Whether this topic is being redirected to a different one
        /// </summary>
        private string redirectToTopic;
        /// <summary>
        /// The indexer this hit belongs to
        /// </summary>
        public readonly Indexer Indexer;

        ILoadAndDecodeBlocker _decoder;
        public ILoadAndDecodeBlocker Decoder { get { return _decoder; }
            set
            {
                if (Beginnings.Length > 1)
                {
                    Array.Sort(Beginnings);
                    Array.Sort(Ends);
                }

                _decoder = value;
            }
        }
        /// <summary>
        /// The title of the topic
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// The list of block beginnings
        /// </summary>
        public readonly long[] Beginnings;
        /// <summary>
        /// The list of block ends
        /// </summary>
        public readonly long[] Ends;
        /// <summary>
        /// The ID of the Wiki topic
        /// </summary>
        public readonly long TopicId;
        /// <summary>
        /// The score of the hit
        /// </summary>
        public readonly float Score;

        /// <summary>
        /// Whether this topic is being redirected to a different one
        /// </summary>
        public string RedirectToTopic
        {
            get { return redirectToTopic; }
        }

        /// <summary>
        /// This constructor is used while indexing the dump to pass the information about topic to tokenizing and
        /// indexing ThreadPool thread
        /// </summary>
        /// <param name="id">Topic ID</param>
        /// <param name="title">Topic title</param>
        /// <param name="begin">The list of the beginnings of the blocks the topic belongs to</param>
        /// <param name="end">The list of the ends of the blocks the topic belongs to</param>
        public PageInfo(long id, string title, long[] begin, long[] end)
        {
            TopicId = id;
            Name = title;
            Beginnings = begin;
            Ends = end;
            TreatRedirectException = false;
        }

        public bool TreatRedirectException { get; set; }

        /// <summary>
        /// This constructor is used while retrieving the hit from the dump
        /// </summary>
        /// <param name="ltask">The dump indexer this Wiki topic belongs to</param>
        /// <param name="hit">The Lucene Hit object</param>
        public PageInfo(Indexer ixr, Hit hit)
        {
            TreatRedirectException = false;
            Indexer = ixr;

            // Decoder setter sort Beginnings and Ends.
            _decoder = ixr;

            Score = hit.GetScore();

            Document doc = hit.GetDocument();

            TopicId = Convert.ToInt64(doc.GetField("topicid").StringValue());

            Name = doc.GetField("title").StringValue();

            Beginnings = new long[doc.GetFields("beginning").Length];
            Ends = new long[doc.GetFields("end").Length];

            int i = 0;
            
            foreach (byte[] binVal in doc.GetBinaryValues("beginning"))
            {
                Beginnings[i] = BitConverter.ToInt64(binVal, 0);

                i++;
            }

            i = 0;

            foreach (byte[] binVal in doc.GetBinaryValues("end"))
            {
                Ends[i] = BitConverter.ToInt64(binVal, 0);

                i++;
            }

            Array.Sort(Beginnings);
            Array.Sort(Ends);
        }

        public string GetFormattedContent()
        {
            if (!String.IsNullOrEmpty(formattedContent))
            {
                return formattedContent;
            }

            string raw = GetRawContent();

            formattedContent = FormatContent(raw); 

            return formattedContent;
        }

        string rawContent = null;

        public string GetRawContent(string contains)
        {
            if (!String.IsNullOrEmpty(rawContent))
                return rawContent;
            /*
            var rawContentCand = GetTextNodeInnerTextFromIdPos(contains, 0);
            var tmp = GetRawContent();
            Debug.Assert(rawContentCand == tmp);
            rawContent = rawContentCand;
             * */
            rawContent = GetTextNodeInnerTextFromIdPos(contains, 0);
            return rawContent;
        }

        public string GetRawContent()
        {
            if (!String.IsNullOrEmpty(rawContent))
                return rawContent;
            string raw =  Decoder.LoadAndDecodeBlock(Beginnings, Ends);
            rawContent =  GetTextNodeInnerText(raw);
            return rawContent;
        }

        public string FormatContent(string toFormat)
        {
            string tmp = Formatter.Format(Name, toFormat, this, Settings.IsRTL, out redirectToTopic);
            if (redirectToTopic != null && TreatRedirectException)
                throw new RedirectException("redirected, name=" + Name + " as " + redirectToTopic);
            return tmp;
        }


        static int IndexOf(string target, string searchfor, int pos)
        {
            return target.IndexOf(searchfor, pos, StringComparison.InvariantCultureIgnoreCase);
            // a little faster, but I don't know the risk.
            // return target.IndexOf(searchfor, pos);
        }

        private string GetTextNodeInnerText(string raw)
        {
            int pos = IndexOfId(raw);
            if (pos < 0)
            {
                throw new Exception(String.Format(Properties.Resources.NoTopicInBlock, Name));
            }

            return GetTextNodeInnerTextFromIdPos(raw, pos);
        }

        private string GetTextNodeInnerTextFromIdPos(string raw, int idPos)
        {
            int textStart = IndexOf(raw, "<text", idPos);

            if (textStart < 0)
            {
                throw new Exception(String.Format(Properties.Resources.NoTextMarkerInBlock, Name));
            }

            int extractionStart = IndexOf(raw, ">", textStart);

            if (extractionStart < 0)
            {
                throw new Exception(String.Format(Properties.Resources.NoTextStartInBlock, Name));
            }

            int extractionEnd = IndexOf(raw, "</text>", extractionStart);

            if (extractionEnd < 0)
            {
                throw new Exception(String.Format(Properties.Resources.NoTextEndInBlock, Name));
            }

            string toFormat = Substring(raw, extractionStart, extractionEnd);

            return HtmlDecode(toFormat);
        }

        static int _hint = 0;

        private int IndexOfId(string raw)
        {
            string searchfor = String.Format("<id>{0}</id>", TopicId);

            if (_hint > raw.Length)
                _hint = 0;
            int pos = IndexOf(raw, searchfor, _hint);
            if (pos != -1)
            {
                _hint = pos;
                return pos;
            }

            pos = raw.IndexOf(searchfor, 0, Math.Min(raw.Length, _hint + searchfor.Length));
            if (pos != -1)
                _hint = pos;

            return pos;
        }

        private static string HtmlDecode(string toFormat)
        {
            return HttpUtility.HtmlDecode(toFormat);
        }

        private static string Substring(string raw, int extractionStart, int extractionEnd)
        {
            string toFormat = raw.Substring(extractionStart + 1, extractionEnd - extractionStart - 1);
            return toFormat;
        }


        public override string ToString()
        {
            return Name;
        }

        public string Yomi { get; set; }
    }
}
