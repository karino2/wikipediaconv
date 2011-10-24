using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using Ionic.Zip;
using System.IO;

namespace EPubLib
{
    public class Book
    {
        public Book()
        {
            //this.Chapters = new List<Chapter>();
            //this.Images = new List<FileItem>();
            this.FileItems = new List<FileItem>();
        }

        public string Title { get; set; }

        public string Identifier { get; set; }

        public string Language { get; set; }

        public List<FileItem> FileItems { get; private set; }

        //public List<Chapter> Chapters { get; private set;  }

        //public List<FileItem> Images { get; private set; }

        public IEnumerable<Chapter> GetChapters()
        {
            return FileItems.OfType<Chapter>();
        }

        public IEnumerable<Image> GetImages()
        {
            return FileItems.OfType<Image>();
        }
 
        /// <summary>
        /// Create a TOC from the chapters and create the file
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename)
        {
            using (ZipFile zippedBook = new ZipFile())
            {
                zippedBook.ForceNoCompression = true;
                ZipEntry mimetype = zippedBook.AddEntry("mimetype", "", "application/epub+zip");

                zippedBook.ForceNoCompression = false;

                zippedBook.AddEntry("container.xml", "META-INF", GetResource("EPubLib.Files.META_INF.container.xml"));
                zippedBook.AddEntry("content.opf", "OEBPS", GetContentOpf(), Encoding.UTF8);
                zippedBook.AddEntry("toc.ncx", "OEBPS", GetToc(), Encoding.UTF8);

                foreach (Chapter chapter in GetChapters())
                {
                    zippedBook.AddEntry("chapter" + chapter.Number.ToString("000") + ".xhtml", "OEBPS", chapter.Content, Encoding.UTF8);
                }
                foreach (FileItem image in GetImages())
                {
                    zippedBook.AddEntry(image.Name, "OEBPS/images", image.Data);
                }
                
                zippedBook.Save(filename);
            }
        }

        /// <summary>
        /// Inspects the Chapters an constructs /OEBPS/content.opf
        /// </summary>
        /// <returns>Xml string of the file contents</returns>
        private string GetContentOpf()
        {
            string OPF = "http://www.idpf.org/2007/opf";
            XmlDocument content = new XmlDocument();
            
            //content.PreserveWhitespace = true;
            XmlNamespaceManager namespaces = new XmlNamespaceManager(content.NameTable);
            namespaces.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
            namespaces.AddNamespace("opf", OPF);
            //namespaces.AddNamespace("", "http://www.idpf.org/2007/opf");

            content.Load(GetResource("EPubLib.Files.OEBPS.content.opf"));

            content.SelectSingleNode("//dc:title", namespaces).InnerText = this.Title;
            content.SelectSingleNode("//dc:identifier", namespaces).InnerText = "urn:uuid:" + this.Identifier;
            content.SelectSingleNode("//dc:language", namespaces).InnerText = this.Language ?? "en-US";

            //Get the manifest & spine nodes.
            XmlNode manifest = content.SelectSingleNode("/opf:package/opf:manifest", namespaces);
            XmlNode spine = content.SelectSingleNode("/opf:package/opf:spine", namespaces);

            //<item id="ncx" href="toc.ncx" media-type="application/x-dtbncx+xml" />
            XmlElement ncx = content.CreateElement("item", OPF);
            ncx.SetAttribute("id", "ncx");
            ncx.SetAttribute("href", "toc.ncx");
            ncx.SetAttribute("media-type", "application/x-dtbncx+xml");
            manifest.AppendChild(ncx);

            //Sort the chapters and then write them to the TOC
            List<Chapter> chapters = GetChapters().ToList<Chapter>();
            chapters.Sort();
            foreach (Chapter chapter in chapters)
            {
                //<item id="chapter001" href="chapter001.xhtml" media-type="application/xhtml+xml" />
                XmlElement xmlChapter = content.CreateElement("item", OPF);
                
                xmlChapter.SetAttribute("id", "chapter" + chapter.Number.ToString("000"));
                xmlChapter.SetAttribute("href", "chapter" + chapter.Number.ToString("000") + ".xhtml");
                xmlChapter.SetAttribute("media-type", chapter.ContentType);
                
                manifest.AppendChild(xmlChapter);

                //<itemref idref="chapter001" />
                XmlElement itemref = content.CreateElement("itemref", OPF);
                itemref.SetAttribute("idref", "chapter" + chapter.Number.ToString("000"));

                spine.AppendChild(itemref);
            }
            foreach (FileItem image in GetImages())
            {
                //<item id="imgl" href="images/sample.jpg" media-type="image/jpeg" />
                XmlElement imageNode = content.CreateElement("item", OPF);
                imageNode.SetAttribute("id", image.Name);
                imageNode.SetAttribute("href", "images/" + image.Name);
                imageNode.SetAttribute("media-type", image.ContentType);
                manifest.AppendChild(imageNode);
            }

            return XmlDocumentToString(content);
        }

        /// <summary>
        /// Inspects the Chapters an constructs /OEBPS/toc.ncx
        /// </summary>
        /// <returns>Xml string of the file contents</returns>
        private string GetToc()
        {
            string URI = "http://www.daisy.org/z3986/2005/ncx/";
            XmlDocument toc = new XmlDocument();
            toc.Load(GetResource("EPubLib.Files.OEBPS.toc.ncx"));

            XmlNamespaceManager namespaces = new XmlNamespaceManager(toc.NameTable);
            namespaces.AddNamespace("n", "http://www.daisy.org/z3986/2005/ncx/");

            //Set the title
            XmlNode title = toc.SelectSingleNode("/n:ncx/n:docTitle/n:text", namespaces);
            title.InnerText = this.Title;

            //Set the ID
            XmlNode id = toc.SelectSingleNode("/n:ncx/n:head/n:meta[@name='dtb:uid']/@content", namespaces);
            id.Value = this.Identifier;

            //Grab the navMap node
            XmlNode navMap = toc.SelectSingleNode("/n:ncx/n:navMap", namespaces);

            List<Chapter> chapters = GetChapters().ToList<Chapter>();
            chapters.Sort();
            //Append one child per chapter
            foreach (Chapter chapter in chapters)
            {
                XmlElement navPoint = toc.CreateElement("navPoint", URI);
                navPoint.SetAttribute("id", "chapter" + chapter.Number.ToString("000"));
                navPoint.SetAttribute("playOrder", chapter.Number.ToString());

                XmlElement navLabel = toc.CreateElement("navLabel", URI);

                //The title of the chapter
                XmlElement text = toc.CreateElement("text", URI);
                text.InnerText = chapter.Title;

                XmlElement content = toc.CreateElement("content", URI);
                content.SetAttribute("src", "chapter" + chapter.Number.ToString("000") + ".xhtml");

                navLabel.AppendChild(text);
                navPoint.AppendChild(navLabel);
                navPoint.AppendChild(content);
                navMap.AppendChild(navPoint);
            }

            return XmlDocumentToString(toc);
        }

        private Stream GetResource(string p)
        {
            return Assembly.GetAssembly(this.GetType()).GetManifestResourceStream(p);
        }

        /// <summary>
        /// Helper method to serialize the DOM with formatting and in UTF8
        /// </summary>
        private static string XmlDocumentToString(XmlDocument dom)
        {
            StringWriterWithEncoding writer = new StringWriterWithEncoding(Encoding.UTF8);

            dom.Save(writer);

            return writer.ToString();
        }

    }
}
