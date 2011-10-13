﻿using System;
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
                string formattedContent = _getContent(pi);
                DirectoryInfo di = TargetPath(pi);
                string path = GetHtmlName(pi, di);
                _path = path; // for debug.
                // ignore redirect.
                if (!IsRedirect(formattedContent))
                {
                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        sw.Write(formattedContent);
                    }
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

        private bool IsRedirect(string formattedContent)
        {
            return formattedContent.StartsWith("#REDIRECT ") ||
                formattedContent.StartsWith("#転送 ");
        }


        private string GetHtmlName(PageInfo pi, DirectoryInfo di)
        {
            string wikiName = pi.Name;
            string baseName = WikiNameToFileBaseName(wikiName);
            string fname = "_" + baseName + Extension;
            if (!String.IsNullOrEmpty(pi.Yomi))
                fname = pi.Yomi.Replace("*", "") + fname;
            else
                Debugger.Break();
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