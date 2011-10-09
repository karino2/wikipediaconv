using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

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
        internal DirectoryInfo Current { get; set; }
        public int UpperFileLimit { get; set; }


        public SplitFolder(DirectoryInfo baseDi)
            : this(baseDi, baseDi)
        { }

        public SplitFolder(DirectoryInfo baseDi, DirectoryInfo current)
        {
            Abort = false;
            Base = new DirectoryInfo(baseDi.FullName.TrimEnd('\\'));
            Current = current;
            MaxFileNum = 100;
        }

        public bool NeedSplit
        {
            get
            {
                return Files.Length >= UpperFileLimit;
            }
        }

        static int ChildIndex(DirectoryInfo di)
        {
            DirectoryInfo[] dirs = di.Parent.GetDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {
                if (dirs[i].FullName == di.FullName)
                    return i;
            }
            return -1;
        }

        public bool Abort { get; set; }

        ForestWalker<DirectoryInfo> _walker;

        public void StartSplit()
        {
            Abort = false;
            ForestNode<DirectoryInfo> root = DirectoryForest(Current);
            _walker = root.Walker;
        }

        public bool IsRunning
        {
            get
            {
                return _walker.HasNext;
            }
        }

        // sometime, not split one, bug don't care.
        public void SplitOne()
        {
            _walker.MoveNext();
            var node = _walker.Current;
            if (node.CurrentEdge == ForestNode<DirectoryInfo>.Edge.Trailing)
                return; // continue;
            Current = node.Element;
            if (TooMuchFile)
            {
                CreateSubdirectories();
                SortToSubdirectories();
                RemoveUnusedDirectories();
            }
        }

        public void Split()
        {
            Abort = false;
            ForestNode<DirectoryInfo> root = DirectoryForest(Current);
            var walker = root.Walker;
            foreach(var node in walker)
            {
                if (Abort)
                    return;
                if (node.CurrentEdge == ForestNode<DirectoryInfo>.Edge.Trailing)
                    continue;
                Current = node.Element;
                if (TooMuchFile)
                {
                    CreateSubdirectories();
                    SortToSubdirectories();
                    RemoveUnusedDirectories();
                }
            }

        }

        public static ForestNode<DirectoryInfo> DirectoryForest(DirectoryInfo cur)
        {
            ForestNode<DirectoryInfo> root = new ForestNode<DirectoryInfo>(ForestNode<DirectoryInfo>.Edge.Leading,
                cur,
                (x, i) => x.GetDirectories()[i],
                (x) => x.Parent,
                (x) => x.GetDirectories().Length,
                ChildIndex,
                (x, y) => x.FullName.TrimEnd(new char[]{'\\', '/'}) == y.FullName.TrimEnd(new char[]{'\\', '/'}));
            return root;
        }

        private void RemoveUnusedDirectories()
        {
            foreach (var dir in Current.GetDirectories())
            {
                if (dir.GetFiles().Length == 0)
                    dir.Delete();
            }
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
                while (Base.FullName.TrimEnd(new char[] { '\\', '/' }) != di.FullName.TrimEnd(new char[] { '\\', '/' }))
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
            string key = FileNameToSortKey(file);
            if (key == untilCur)
                return Current.FullName;
            Debug.Assert(key.StartsWith(untilCur));
            string nextHead = key.Substring(untilCur.Length, 1);
            if(InsideNormalRange(nextHead))
                return Path.Combine(Current.FullName, key.Substring(untilCur.Length, 1));
            throw new Exception("NYI: special character handling.");
        }

        static internal string FileNameToSortKey(FileInfo file)
        {
            var fname = Path.GetFileNameWithoutExtension(file.Name);
            fname = Regex.Replace(fname, "&[^;]*;", "");
            fname = Regex.Replace(fname, "[^a-zA-Z]", "");
            return fname.ToLowerInvariant().Replace(" ", "");
        }

        private bool InsideNormalRange(string nextHead)
        {
            return nextHead[0] >= 'a' && nextHead[0] <= 'z';
        }

        void CreateSubdirectories()
        {
            if (SubFolderExists)
                return; // we should ensure folder name, but OK for normal case.
            for (char c = 'a'; c <= 'z'; c++)
            {
                CreateSubdirectory(c);
            }
            // CreateSubdirectory('X');
        }

        internal virtual void CreateSubdirectory(char c)
        {
            Current.CreateSubdirectory(c.ToString());
        }

        public bool SubFolderExists
        {
            get
            {
                return Current.GetDirectories().Length != 0;
            }
        }

        public int MaxFileNum { get; set; }

        public bool TooMuchFile 
        {
            get
            {
                return Current.GetFiles("*.html").Length > MaxFileNum;
            }
        }
    }
}
