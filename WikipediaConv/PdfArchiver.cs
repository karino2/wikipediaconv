using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;

namespace WikipediaConv
{
    public class PdfArchiver
    {
        static Font _font;
        static PdfArchiver()
        {
            BaseFont.AddToResourceSearch("iTextAsian.dll");
            _font = new Font(
               BaseFont.CreateFont("HeiseiKakuGo-W5", "UniJIS-UCS2-H", BaseFont.NOT_EMBEDDED)
               , 16);
        }

        public void Archive(IEnumerable<FileInfo> files, string epubPath)
        {
            Document doc = new Document();
            int chapterNumber = 1;
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(epubPath, FileMode.Create));

            doc.Open();
            foreach (var file in files)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(file.FullName))
                    {
                        string contents = sr.ReadToEnd();
                        /*
                        string title = GetTitle(contents);
                        // For redirect case.
                        title = title == "" ? Path.GetFileNameWithoutExtension(file.Name) : title;
                         * */
                        string title = GetTitleFromFilePath(file.Name);

                        Chapter chapter1 = new Chapter(new Paragraph(title, _font), 1 /* chapterNumber */);

                        /*

                        var lists = HTMLWorker.ParseToList(new StringReader(contents), new StyleSheet());
                        lists.All((e) => chapter1.Add(e));
                         * */
                        chapter1.Add(new Paragraph(contents, _font));
                        try
                        {
                            doc.Add(chapter1);
                        }
                        catch (IndexOutOfRangeException)
                        {
                            //used unknown font. 
                            // just skip.
                            continue;
                        }

                    }
                    chapterNumber++;
                }
                catch (FileNotFoundException)
                {
                    // in some Greeth filename, StreamReader return file not found exception.
                    // Just ignore it.
                }
            }
            try
            {
                doc.Close();
            }
            catch (IOException)
            {
                // if nothing add, close return IOException.
                // this is FileNotFoundException case only and just ignore it.
            }
        }

        private string GetTitleFromFilePath(string p)
        {
            var title = Path.GetFileNameWithoutExtension(p);
            var titleList = new List<string>(title.Split('_'));
            titleList.RemoveAt(0);
            return String.Join("_", titleList);
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
