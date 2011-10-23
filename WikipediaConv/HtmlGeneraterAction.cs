using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace WikipediaConv
{
    public class DumpAction : IDecodedAction
    {
        DirectoryInfo _base;
        internal INotifyDecoder _notify;
        Func<PageInfo, string> _getContent;
        public DumpAction(string bzipPath, string extension, string relativePath, Func<PageInfo, string> getContent)
        {
            _base = new FileInfo(bzipPath).Directory;
            Extension = extension;
            _getContent = getContent;
            RelativePath = relativePath;
        }

        public static DumpAction CreateHtmlGeneraterAction(string bzipPath)
        {
            return new DumpAction(bzipPath, ".html", "ePub/", (pi) => pi.GetFormattedContent());
        }

        public static DumpAction CreateRawDumpAction(string bzipPath)
        {
            return new DumpAction(bzipPath, ".wiki", "pdf/", (pi) => pi.GetRawContent());
        }

        public string Extension { get; set; }
        public string RelativePath { get; set; }
        public void PreAction()
        {
        }

        internal DirectoryInfo TargetPath(PageInfo pi)
        {
            return WorkingFolder;
        }

        public DirectoryInfo WorkingFolder
        {
            get
            {
                DirectoryInfo di = new DirectoryInfo(Path.Combine(_base.FullName, RelativePath));
                if (!di.Exists)
                {
                    di.Create();
                }
                return di;
            }
        }

        public ILoadAndDecodeBlocker Decoder { get; set; }
        string _path;

        public bool Action(PageInfo pi)
        {
            pi.Decoder = Decoder;

            try
            {
                pi.TreatRedirectException = true;
                string formattedContent = _getContent(pi);
                DirectoryInfo di = TargetPath(pi);
                string path = GetHtmlName(pi, di);
                _path = path; // for debug.
                // ignore redirect.
                if (!IsSkipCandidate(path, formattedContent))
                {
                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        sw.Write(formattedContent);
                    }
                    return true;
                }
            }
            catch (PageInfo.RedirectException re)
            {
                Debug.WriteLine("skip redirect: (" + re.Message + ")");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("path:" + _path);
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace.ToString());
            }
            return false;
        }

        static internal bool IsSkipCandidate(string wikiName, string formattedContent)
        {
            if (IsRedirect(formattedContent))
                return true;
            if (IsSakujoIrai(wikiName))
                return true;
            if (IsTemplate(wikiName))
                return true;
            if (IsCategory(wikiName))
                return true;
            return false;
        }

        internal static bool IsCategory(string wikiName)
        {
            return wikiName.StartsWith("Category:");
        }

        internal static bool IsTemplate(string wikiName)
        {
            return wikiName.StartsWith("Template:");
        }

        internal static bool IsSakujoIrai(string wikiName)
        {
            return wikiName.StartsWith("Wikipedia:削除依頼");
        }

        static internal bool IsRedirect(string formattedContent)
        {
            return formattedContent.StartsWith("#REDIRECT ") ||
                formattedContent.StartsWith("#redirect ") ||
                formattedContent.StartsWith("#転送 ");
        }


        private string GetHtmlName(PageInfo pi, DirectoryInfo di)
        {
            string wikiName = pi.Name;
            string baseName = WikiNameToFileBaseName(wikiName);
            string fname = "_" + baseName + Extension;
            if (!String.IsNullOrEmpty(pi.Yomi))
                fname = pi.Yomi + fname;
            try
            {
                string path = Path.Combine(di.FullName, fname);
                return path;
            }
            catch (System.NotSupportedException ex)
            {
                Debug.WriteLine("combine: ");
                Debug.WriteLine("dir=" + di.FullName);
                Debug.WriteLine("fname=" + fname);
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace.ToString());
                throw;
            }
        }

        internal static string WikiNameToFileBaseName(string wikiName)
        {
            string baseName = wikiName.Replace("/", "／").Replace(">", "＞").Replace("<", "＜").Replace("?", "？").Replace(":", "：").Replace("\"", "”").Replace("\\", "￥").Replace("*", "＊").Replace("|", "｜").Replace(";", "；");
            if (baseName == "Con")
                return "Con_";
            return baseName;
        }


        public void FlashAction()
        {
        }

        public void PostAction()
        {
        }

        public void FinalizeAction(bool failedOrAbort)
        {
        }

        public void Close()
        {
        }

    }
}
