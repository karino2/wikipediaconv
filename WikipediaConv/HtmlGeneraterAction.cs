using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace WikipediaConv
{
    public class HtmlGeneraterAction : IDecodedAction
    {
        DirectoryInfo _base;
        internal INotifyDecoder _notify;
        public HtmlGeneraterAction(string bzipPath)
        {
            _base = new FileInfo(bzipPath).Directory;
        }
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
                DirectoryInfo di = new DirectoryInfo(Path.Combine(_base.FullName, "ePub/"));
                if (!di.Exists)
                {
                    di.Create();
                }
                return di;
            }
        }

        public ILoadAndDecodeBlocker Decoder { get; set; }
        string _path;

        public void Action(object state)
        {
            PageInfo pi = (PageInfo)state;
            pi.Decoder = Decoder;
            if (pi.Beginnings.Length > 1)
            {
                Array.Sort(pi.Beginnings);
                Array.Sort(pi.Ends);
            }

            try
            {
                pi.TreatRedirectException = true;
                string formattedContent = pi.GetFormattedContent();
                DirectoryInfo di = TargetPath(pi);
                string path = GetHtmlName(pi, di);
                _path = path; // for debug.
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write(formattedContent);
                }
            }
            catch (PageInfo.RedirectException re)
            {
                Debug.WriteLine("skip redirect: (" + re.Message + ")");
            }
            catch (Exception ex)
            {
                Debugger.Break();
                Debug.WriteLine("path:" + _path);
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace.ToString());
            }
            _notify.NotifyEnd();
        }

        private static string GetHtmlName(PageInfo pi, DirectoryInfo di)
        {
            string wikiName = pi.Name;
            string baseName = WikiNameToFileBaseName(wikiName);
            string fname = baseName + ".html";
            string path = Path.Combine(di.FullName, fname);
            return path;
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
