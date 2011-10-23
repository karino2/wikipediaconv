using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPubLib
{
    /// <summary>
    /// An abstract factory. Use Image.Create().
    /// </summary>
    public abstract class FileItem
    {
        private FileItem()
        {
        }

        public FileItem(string contentType)
        {
            this.ContentType = contentType;
        }

        /// <summary>
        /// The actual content of the image
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// The file name. Note that it will be saved into /images/, so it should be name and extension only,
        /// not path.
        /// </summary>
        public string Name { get; set; }

        public virtual string ContentType { get; protected set; }

        public virtual string DefaultExtension { get; protected set; }

        /// <summary>
        /// Creates a concrete type based on an enum. 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FileItem Create(FileItemType type)
        {
            switch (type)
            {
                case FileItemType.JPEG:
                    return new ImageJpeg();
                    break;
                case FileItemType.GIF:
                    return new ImageGif();
                    break;
                case FileItemType.PNG:
                    return new ImagePng();
                    break;
                case FileItemType.XHTML:
                    return new Chapter();
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }
        }

        /// <summary>
        /// Create a specific concrete type by filename. Usefule when you know you've done something like
        /// parse and &ltimg /&gt; tag--just pass the src in here. 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static FileItem Create(string filename)
        {
            filename = filename.ToUpperInvariant();
            if (filename.EndsWith(".JPG") || filename.EndsWith(".JPEG"))
            {
                return FileItem.Create(FileItemType.JPEG);
            }
            if (filename.EndsWith(".GIF"))
            {
                return FileItem.Create(FileItemType.GIF);
            }
            if (filename.EndsWith(".PNG"))
            {
                return FileItem.Create(FileItemType.PNG);
            }
            if (filename.EndsWith("HTML") || filename.EndsWith("XHTML"))
            {
                return FileItem.Create(FileItemType.XHTML);
            }
            throw new NotImplementedException();
        }

    }

    

}
