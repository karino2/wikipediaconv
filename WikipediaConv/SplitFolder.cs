using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace WikipediaConv
{
    public class SplitFolder
    {
        public virtual FileInfo[] Files
        {
            get
            {
                return Current.GetFiles();
            }
        }

        DirectoryInfo Base { get; set; }
        DirectoryInfo Current { get; set; }
        public int UpperFileLimit { get; set; }


        public SplitFolder(DirectoryInfo baseDi)
            : this(baseDi, baseDi)
        { }

        public SplitFolder(DirectoryInfo baseDi, DirectoryInfo current)
        {
            Base = baseDi;
            Current = current;
        }

        public bool NeedSplit
        {
            get
            {
                return Files.Length >= UpperFileLimit;
            }
        }

        public void Split()
        {
            if (SubFolderExists)
                throw new Exception("Already splited");
            CreateSubdirectories();
            SortToSubdirectories();
        }

        internal virtual void MoveTo(FileInfo target, string destPath)
        {
            target.MoveTo(destPath);
        }

        private void SortToSubdirectories()
        {
            foreach (var file in Files)
            {
                string dest = GetMatchedSubdirectoryPath(file);
                if(dest != Current.FullName)
                    MoveTo(file, Path.Combine(dest, file.Name));
            }
        }

        internal string FileNameHeadUntilCurrent
        {
            get
            {
                if(Base == Current)
                    return "";
                List<String> dirs = new List<String>();
                DirectoryInfo di = Current;
                while (Base.FullName != di.FullName)
                {
                    dirs.Add(di.Name);
                    di = di.Parent;
                }
                dirs.Reverse();
                StringBuilder bldr = new StringBuilder();
                dirs.ForEach((d) => bldr.Append(d));
                return bldr.ToString();
            }
        }

        internal string GetMatchedSubdirectoryPath(FileInfo file)
        {
            string untilCur = FileNameHeadUntilCurrent;
            if (file.Name == untilCur)
                return untilCur;
            Debug.Assert(file.Name.StartsWith(untilCur));
            string nextHead = file.Name.Substring(untilCur.Length, 1);
            if(InsideNormalRange(nextHead))
                return Path.Combine(Current.FullName, file.Name.Substring(0, untilCur.Length + 1));
            throw new Exception("NYI: special character handling.");
        }

        private bool InsideNormalRange(string nextHead)
        {
            return nextHead[0] >= 'a' && nextHead[0] <= 'z';
        }

        void CreateSubdirectories()
        {
            for (char c = 'A'; c <= 'Z'; c++)
            {
                CreateSubdirectory(c);
            }
        }

        internal virtual void CreateSubdirectory(char c)
        {
            Current.CreateSubdirectory(Path.Combine(Current.FullName, c.ToString()));
        }

        public bool SubFolderExists
        {
            get
            {
                return Current.GetDirectories().Length != 0;
            }
        }

    }
}
