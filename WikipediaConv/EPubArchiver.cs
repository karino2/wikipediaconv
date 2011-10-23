using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using EPubLib;

namespace WikipediaConv
{
    public class EPubArchiver
    {
        public void Archive(IEnumerable<FileInfo> files, string epubPath)
        {
            Book book = new Book();
            book.Language = "ja";
            int chapterNumber = 1;
            foreach (var file in files)
            {
                using (StreamReader sr = new StreamReader(file.FullName, System.Text.Encoding.GetEncoding("utf-8")))
                {
                    string contents = sr.ReadToEnd();
                    string title = GetTitle(contents);
                    // For redirect case.
                    title = title == "" ? Path.GetFileNameWithoutExtension(file.Name) : title;
                    Chapter chapter1 = new Chapter()
                    {
                        Title = title,

                        //You are responsible for setting chapter numbers appropriately, starting with 1.
                        Number = chapterNumber,

                        //Be sure your Content is XHTML 1.1 compliant!
                        Content = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
" + contents
                    };
                    book.FileItems.Add(chapter1);
                }
                chapterNumber++;
            }
            book.Save(epubPath);
        }

        private string GetTitle(string headContent)
        {
            int beg = headContent.IndexOf("<title>");
            if (beg == -1)
                return "";
            beg += "<title>".Length;
            int end = headContent.IndexOf("</title>");
            if (end == -1)
                return "";
            return headContent.Substring(beg, end-beg);
        }
    }
}
