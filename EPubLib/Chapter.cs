using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPubLib
{
    public class Chapter : FileItem, IComparable<Chapter>
    {
        public Chapter() : base("application/xhtml+xml")
        {
        }

        /// <summary>
        /// The title of the chapter
        /// </summary>
        public string Title { get; set; }

                /// <summary>
        /// The chapter number, starting from 1.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// The HTML body
        /// </summary>
        public string Content { get; set; }


        #region IComparable<Chapter> Members

        /// <summary>
        /// Used for sorting by chapter number
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Chapter other)
        {
            return Number.CompareTo(other.Number);
        }

        #endregion
    }
}
