using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPubLib
{
    public abstract class Image : FileItem
    {
        protected Image(string contentType)
            : base(contentType)
        {
        }
    }

    public class ImageJpeg : Image
    {
        internal ImageJpeg() : base("image/jpeg")
        {
            base.DefaultExtension = ".jpg";
        }
    }

    public class ImageGif : Image
    {
        internal ImageGif() : base("image/gif")
        {
            base.DefaultExtension = ".gif";
        }
    }

    public class ImagePng : Image
    {
        internal ImagePng() : base("image/png")
        {
            base.DefaultExtension = ".png";
        }
    }

}
