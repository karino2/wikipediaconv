#define DIRTY

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


    public class DirectoryInfoCache
    {
        public DirectoryInfoCache Parent;
        public DirectoryInfo Item;
        private DirectoryInfo[] _children = null;
        public DirectoryInfoCache(DirectoryInfoCache parent, DirectoryInfo item)
        {
            Parent = parent;
            Item = item;
        }

        public int ChildrenLength
        {
            get { return DirectoryInfoChildren.Length; }
        }

        public DirectoryInfoCache GetChild(int i)
        {
            return new DirectoryInfoCache(this, DirectoryInfoChildren[i]);
        }

        public int ChildIndex
        {
            get
            {
                for (int i = 0; i < Parent.ChildrenLength; i++)
                {
                    if (Parent.GetChild(i).Item.FullName == Item.FullName)
                        return i;
                }
                return -1;
            }
        }

        private DirectoryInfo[] DirectoryInfoChildren
        {
            get
            {
                if (_children == null)
                    _children = Item.GetDirectories();
                return _children;
            }
        }
        public static ForestNode<DirectoryInfoCache> Forest(DirectoryInfo cur)
        {
            var dic = new DirectoryInfoCache(null, cur);
            return Forest(dic);
        }

        public static ForestNode<DirectoryInfoCache> Forest(DirectoryInfoCache dic)
        {
            ForestNode<DirectoryInfoCache> root = new ForestNode<DirectoryInfoCache>(ForestNode<DirectoryInfoCache>.Edge.Leading,
                dic,
                (x, i) => x.GetChild(i),
                (x) => x.Parent,
                (x) => x.ChildrenLength,
                (x) => x.ChildIndex,
                (x, y) => PathEqual(x.Item, y.Item));
            return root;
        }
        public static bool PathEqual(DirectoryInfo x, DirectoryInfo y)
        {
            return NormalizedFullName(x.FullName) == NormalizedFullName(y.FullName);
        }

        private static string NormalizedFullName(string fullName)
        {
            return fullName.TrimEnd(new char[] { '\\', '/' });
        }


        public override int GetHashCode()
        {
            return NormalizedFullName(Item.FullName).GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (other == null)
                return ((object)this) == null;
            if (other is DirectoryInfoCache)
            {
                DirectoryInfoCache dic = other as DirectoryInfoCache;
                return PathEqual(Item, dic.Item);
            }
            return false;
        }

        public static bool operator ==(DirectoryInfoCache a, DirectoryInfoCache b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(DirectoryInfoCache a, DirectoryInfoCache b)
        {
            return !a.Equals(b);
        }
    }

    public class SplitFolder
    {
        public virtual IEnumerable<FileInfo> FileEnum
        {
            get
            {
                return Current.Item.EnumerateFiles();
            }
        }
        DirectoryInfoCache Base { get; set; }
        internal DirectoryInfoCache Current { get; set; }

        public string Extension { get; set; }

        ISplitTactics _tactics;


        public SplitFolder(DirectoryInfo baseDi)
            : this(baseDi, new EnglishTactics())
        { }


        public DirectoryInfoCache StartDirectory { get; set; }

        public Dictionary<string, bool> _dirty;

        public SplitFolder(DirectoryInfo baseDi, ISplitTactics tactics)
        {
            Abort = false;
            _tactics = tactics;
            Base = new DirectoryInfoCache(null, baseDi);
            Current = Base;
            StartDirectory = Current;
            MaxFileNum = WikipediaConv.Properties.Settings.Default.OneFolderMaxFileNum;
            Extension = ".html";
            _dirty = new Dictionary<string, bool>();
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

        ForestWalker<DirectoryInfoCache> _walker;

        public void StartSplit()
        {
            Abort = false;
            Current = StartDirectory;
            ForestNode<DirectoryInfoCache> root = DirectoryInfoCache.Forest(StartDirectory);
            _walker = root.Walker;
            _dirty.Clear();
            SetDirty(StartDirectory.Item.FullName);
        }

        void SetDirty(string fullName)
        {
            _dirty[NormalizedFullName(fullName)] = true;
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
            if (node.CurrentEdge == ForestNode<DirectoryInfoCache>.Edge.Trailing)
                return; // continue;
#if DIRTY
            if(!IsDirty(node.Element.Item.FullName))
            {
                _walker.SkipChildren();
                return; // continue;
            }
#endif
            Current = node.Element;
            bool needToWalkDown = false;
            if (AlreadySplited || TooMuchFile)
            {
                needToWalkDown = SortToSubdirectories();
            }
            if (!needToWalkDown)
            {
                _walker.SkipChildren();
            }

        }

        private bool IsDirty(string fullName)
        {
            return _dirty.ContainsKey(NormalizedFullName(fullName));
        }

        public void Split()
        {
            StartSplit();
            while (IsRunning)
            {
                SplitOne();
            }
        }

        private static string NormalizedFullName(string fullName)
        {
            return fullName.TrimEnd(new char[] { '\\', '/' });
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

        internal virtual void MoveTo(FileInfo target, string dest)
        {
            var destPath = Path.Combine(dest, target.Name);
            EnsureDirectory(dest);
#if DIRTY
            SetDirty(dest);
#endif
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
                if (dest != Current.Item.FullName)
                {
                    moveSomething = true;
                    MoveTo(file, dest);
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
                DirectoryInfoCache dic = Current;
                while (Base != dic)
                {
                    dirs.Add(dic.Item.Name);
                    dic = dic.Parent;
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
                return Current.Item.FullName;
            string nextHead = LookupSortChar(key, untilCur.Length);
            return Path.Combine(Current.Item.FullName, nextHead);
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


        void EnsureDirectory(string path)
        {
            var di = new DirectoryInfo(path);
            if (!di.Exists)
                di.Create();
        }



        public int MaxFileNum { get; set; }

        public bool TooMuchFile 
        {
            get
            {
                return BiggerThanLimit(Current.Item.EnumerateFiles("*" + Extension), MaxFileNum);
            }
        }

        public bool AlreadySplited { 
            get 
            {

                var dirs = Current.Item.EnumerateDirectories();
                foreach(var dir in dirs)
                {
                    return true; // at least one dir exist.
                }
                return false;
            } 
        }
    }
}
