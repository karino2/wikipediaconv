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
            int chapterNumber = 1;
            foreach (var file in files)
            {
                using (StreamReader sr = new StreamReader(file.FullName))
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
                        Content = contents
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
