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
        public DumpAction(string bzipPath, string extension,  Func<PageInfo, string> getContent, DirectoryInfo outputPath)
        {
            _base = new FileInfo(bzipPath).Directory;
            Extension = extension;
            _getContent = getContent;
            OutputRoot = outputPath;
        }

        public static DumpAction CreateHtmlGeneraterAction(string bzipPath, DirectoryInfo result)
        {
            return new DumpAction(bzipPath, ".html", (pi) => pi.GetFormattedContent(), result);
        }

        public static DumpAction CreateRawDumpAction(string bzipPath, DirectoryInfo result)
        {
            return new DumpAction(bzipPath, ".wiki", (pi) => pi.GetRawContent(), result);
        }

        public string Extension { get; set; }
        public void PreAction()
        {
        }

        internal DirectoryInfo TargetPath(PageInfo pi)
        {
            return OutputRoot;
        }

        DirectoryInfo _workingFolder = null;

        public DirectoryInfo OutputRoot { get; set; }
            /*
        {
            get
            {
                if (_workingFolder != null)
                    return _workingFolder;
                DirectoryInfo di = new DirectoryInfo(Path.Combine(_base.FullName, RelativePath));
                if (!di.Exists)
                {
                    di.Create();
                }
                _workingFolder = di;
                return _workingFolder;
            }
            set
            {
                _workingFolder = value;
            }
        }
             * */

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
                if (!IsSkipCandidate(pi.Name, formattedContent))
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
            if (IsFileDescription(wikiName))
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

        internal static bool IsFileDescription(string wikiName)
        {
            return wikiName.StartsWith("ファイル:");
        }

        static internal bool IsRedirect(string formattedContent)
        {
            return formattedContent.StartsWith("#REDIRECT", StringComparison.CurrentCultureIgnoreCase) ||
                formattedContent.StartsWith("#転送");
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
            catch (System.Exception ex)
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
