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
            return Regex.Replace(fname, "[^a-zA-Z0-9ぁ-んァ-ンヵヶヴ]", "");
        }

        public IEnumerable<char> Alphabets
        {
            get
            {
                /*
                yield return 'a';
                yield return '0';
                 * */
                for (char c = 'a'; c <= 'z'; c++)
                {
                    yield return c;
                }
                for (char c = '0'; c <= '9'; c++)
                {
                    yield return c;
                }
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
                return c.ToString();
            if(Inside('0', '9', c))
                return c.ToString();
            if(Inside('ぁ', 'お', c) ||
               Inside('ァ', 'オ', c) ||
                c == 'ヴ')
                return "あ";
            if (Inside('か', 'ご', c) ||
                Inside('カ', 'ゴ', c) ||
                c == 'ヵ' || c == 'ヶ')
                return "か";
            if (Inside('さ', 'ぞ', c) ||
                Inside('サ', 'ゾ', c))
                return "さ";
            if (Inside('た', 'ど', c) ||
                Inside('タ', 'ド', c))
                return "た";
            if (Inside('な', 'の', c) ||
                Inside('ナ', 'ノ', c))
                return "な";
            if (Inside('は', 'ぽ', c) ||
                Inside('ハ', 'ポ', c))
                return "は";
            if (Inside('ま', 'も', c) ||
                Inside('マ', 'モ', c))
                return "ま";
            if (Inside('ゃ', 'よ', c) ||
                Inside('ャ', 'ヨ', c))
                return "や";
            if (Inside('ら', 'ろ', c) ||
                Inside('ラ', 'ロ', c))
                return "ら";
            /*
            if (Inside('わ', 'ん', c) ||
                Inside('ワ', 'ン', c))
                return "ら";
             * */
            return "わ";
        }


        public string FileNameToYomi(string fname)
        {
            return fname.Split('_')[0];
        }
    }

    public class SplitFolder
    {
        public virtual IEnumerable<FileInfo> FileEnum
        {
            get
            {
                return Current.EnumerateFiles();
            }
        }
        DirectoryInfo Base { get; set; }
        internal DirectoryInfo Current { get; set; }

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

        public DirectoryInfo StartDirectory { get; set; }
        public SplitFolder(DirectoryInfo baseDi, DirectoryInfo current, ISplitTactics tactics)
        {
            Abort = false;
            _tactics = tactics;
            Base = new DirectoryInfo(baseDi.FullName.TrimEnd('\\'));
            Current = current;
            StartDirectory = current;
            MaxFileNum = WikipediaConv.Properties.Settings.Default.OneFolderMaxFileNum;
            Extension = ".html";
        }


        private static bool BiggerThanLimit(IEnumerable<FileInfo> fileEnum, int upperLimit)
        {
            int count = 0;
            foreach (var f in fileEnum)
            {
                count++;
                if (count >= upperLimit)
                    return true;
            }
            return false;
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
            Current = StartDirectory;
            ForestNode<DirectoryInfo> root = DirectoryForest(StartDirectory);
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
            bool needToWalkDown = false;
            if (TooMuchFile || AlreadySplited)
            {
                CreateSubdirectories();
                needToWalkDown = SortToSubdirectories();
                RemoveUnusedDirectories();
            }
            if (!needToWalkDown)
            {
                _walker.SkipChildren();
            }

        }

        public void Split()
        {
            StartSplit();
            while (IsRunning)
            {
                SplitOne();
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

        bool IsEmpty(DirectoryInfo di)
        {
            foreach (var f in di.EnumerateFiles())
            {
                return false;
            }
            foreach (var d in di.EnumerateDirectories())
            {
                return false;
            }
            return true;
        }

        private void RemoveUnusedDirectories()
        {
            foreach (var dir in Current.GetDirectories())
            {
                if (IsEmpty(dir))
                    dir.Delete();
            }
        }

        internal virtual void MoveTo(FileInfo target, string destPath)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    target.MoveTo(destPath);
                    return;
                }
                catch (IOException)
                {
                    var fname = Path.GetFileNameWithoutExtension(destPath);
                    var targetPath = Path.GetDirectoryName(destPath);
                    var extension = Path.GetExtension(destPath);
                    fname += 'X';
                    destPath = Path.Combine(targetPath, fname + extension);
                }
            }
        }

        private bool SortToSubdirectories()
        {
            bool moveSomething = false;
            foreach (var file in FileEnum)
            {
                string dest = GetMatchedSubdirectoryPath(file);
                if (dest != Current.FullName)
                {
                    moveSomething = true;
                    MoveTo(file, Path.Combine(dest, file.Name));
                }
            }
            return moveSomething;
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
            string nextHead = LookupSortChar(key, untilCur.Length);
            return Path.Combine(Current.FullName, nextHead);
        }

        internal string LookupSortChar(string key, int curLen)
        {
            string nextHead = _tactics.Lookup(key.Substring(curLen, 1));
            return nextHead;
        }

        internal string FileNameToSortKey(FileInfo file)
        {
            var fname = Path.GetFileNameWithoutExtension(file.Name);
            fname = _tactics.FileNameToYomi(fname);
            fname = Regex.Replace(fname, "&[^;]*;", "");
            fname = _tactics.RemoveUnsupportCharacter(fname);
            return fname.ToLowerInvariant().Replace(" ", "");
        }

        void CreateSubdirectories()
        {
            foreach (char c in _tactics.Alphabets)
            {
                EnsureSubdirectory(c);
            }
        }

        void EnsureSubdirectory(char c)
        {
            /* // this code is very slow.
            var dis = Current.GetDirectories(c.ToString());
            if (dis.Length == 0)
             * */
            var di = GetDirectoryInfo(c);
            if (!di.Exists) 
                CreateSubdirectory(c);
        }

        private DirectoryInfo GetDirectoryInfo(char c)
        {
            return new DirectoryInfo(Path.Combine(Current.FullName, c.ToString()));
        }

        internal virtual void CreateSubdirectory(char c)
        {
            var di = GetDirectoryInfo(c);
            di.Create();
            // Current.CreateSubdirectory(c.ToString());
        }


        public int MaxFileNum { get; set; }

        public bool TooMuchFile 
        {
            get
            {
                return BiggerThanLimit(Current.EnumerateFiles("*" + Extension), MaxFileNum);
            }
        }

        public bool AlreadySplited { 
            get 
            {

                var dirs = Current.EnumerateDirectories();
                foreach(var dir in dirs)
                {
                    return true; // at least one dir exist.
                }
                return false;
            } 
        }
    }
}
