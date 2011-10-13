using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace WikipediaConv
{
    public interface ISplitTactics
    {
        string RemoveUnsupportCharacter(string fname);

        IEnumerable<char> Alphabets { get; }

        string Lookup(string p);

        string FileNameToYomi(string fname);
    }
    public class EnglishTactics : ISplitTactics
    {
        public string RemoveUnsupportCharacter(string fname)
        {
            return Regex.Replace(fname, "[^a-zA-Z0-9]", "");
        }

        public IEnumerable<char> Alphabets
        {
            get
            {
                for (char c = 'a'; c <= 'z'; c++)
                {
                    yield return c;
                }
                for (char c = '0'; c <= '9'; c++)
                {
                    yield return c;
                }
            }
        }


        public string Lookup(string p)
        {
            return p;
        }


        public string FileNameToYomi(string fname)
        {
            return fname;
        }
    }

    public class JapaneseTactics : ISplitTactics
    {
        public string RemoveUnsupportCharacter(string fname)
        {
            return Regex.Replace(fname, "[^a-zA-Z0-9あ-ん]", "");
        }

        public IEnumerable<char> Alphabets
        {
            get
            {
                yield return 'a';
                yield return '0';
                yield return 'あ';
                yield return 'か';
                yield return 'さ';
                yield return 'た';
                yield return 'な';
                yield return 'は';
                yield return 'ま';
                yield return 'や';
                yield return 'ら';
                yield return 'わ';
            }
        }

        bool Inside(char beg, char end, char target)
        {
            return target >= beg && target <= end;
        }

        public string Lookup(string str)
        {
            char c = str[0];
            if(Inside('a', 'z', c))
                return "a";
            if(Inside('0', '9', c))
                return "0";
            if(Inside('あ', 'お', c))
                return "あ";
            if (Inside('か', 'こ', c))
                return "か";
            if (Inside('さ', 'そ', c))
                return "さ";
            if (Inside('た', 'と', c))
                return "た";
            if (Inside('な', 'の', c))
                return "な";
            if (Inside('は', 'ほ', c))
                return "は";
            if (Inside('ま', 'も', c))
                return "ま";
            if (Inside('や', 'よ', c))
                return "や";
            return "わ";
        }


        public string FileNameToYomi(string fname)
        {
            return fname.Split('_')[0];
        }
    }

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

        public string Extension { get; set; }

        ISplitTactics _tactics;

        public SplitFolder(DirectoryInfo baseDi, ISplitTactics tactics)
            : this(baseDi, baseDi, tactics)
        { }

        public SplitFolder(DirectoryInfo baseDi)
            : this(baseDi, baseDi)
        { }

        public SplitFolder(DirectoryInfo baseDi, DirectoryInfo current) : this(baseDi, current, new EnglishTactics())
        {
        }
        public SplitFolder(DirectoryInfo baseDi, DirectoryInfo current, ISplitTactics tactics)
        {
            Abort = false;
            _tactics = tactics;
            Base = new DirectoryInfo(baseDi.FullName.TrimEnd('\\'));
            Current = current;
            MaxFileNum = 100;
            Extension = ".html";
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
            if (key.Length == untilCur.Length)
                return Current.FullName;
            Debug.Assert(key.StartsWith(untilCur));
            string nextHead = _tactics.Lookup(key.Substring(untilCur.Length, 1));
            return Path.Combine(Current.FullName, nextHead);
        }

        internal string FileNameToSortKey(FileInfo file)
        {
            var fname = Path.GetFileNameWithoutExtension(file.Name);
            fname = _tactics.FileNameToYomi(fname);
            fname = Regex.Replace(fname, "&[^;]*;", "");
            fname = _tactics.RemoveUnsupportCharacter(fname);
            return fname.ToLowerInvariant().Replace(" ", "");
        }

        /*
        private bool InsideNormalRange(string nextHead)
        {
            return nextHead[0] >= 'a' && nextHead[0] <= 'z';
        }*/

        void CreateSubdirectories()
        {
            foreach (char c in _tactics.Alphabets)
            {
                EnsureSubdirectory(c);
            }
            /*
            for (char c = 'a'; c <= 'z'; c++)
            {
                EnsureSubdirectory(c);
            }
            for (char c = '0'; c <= '9'; c++)
            {
                EnsureSubdirectory(c);
            }
            if (Japanese)
            {
                for (char c = 'あ'; c <= 'ん'; c++)
                {
                    EnsureSubdirectory(c);
                }
            }
             * */
        }

        void EnsureSubdirectory(char c)
        {
            var dis = Current.GetDirectories(c.ToString());
            if (dis.Length == 0)
                CreateSubdirectory(c);
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
                return Current.GetFiles("*" + Extension).Length > MaxFileNum;
            }
        }
    }
}
