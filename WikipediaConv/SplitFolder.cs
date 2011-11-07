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

        private List<DirectoryInfoCache> _children = new List<DirectoryInfoCache>();
        private string _fullName;

        public bool PossiblyMoveFromOutside { get; set; }

        private int _fileCount;
        public int FileCount {
            get
            {
                if (PossiblyMoveFromOutside)
                {
                    // always use native value. only used at root folder.
                    // Warning! do not cashed. very slow.
                    Debug.Assert(Parent == null);
                    return Item.EnumerateFiles(InterestedFilePattern).Count();
                }
                return _fileCount;
            }
            set { _fileCount = value; }
        }
        public string InterestedFilePattern { get; set; }
        public DirectoryInfoCache(DirectoryInfoCache parent, DirectoryInfo item)
        {
            Parent = parent;
            if (parent != null)
                InterestedFilePattern = parent.InterestedFilePattern;
            PossiblyMoveFromOutside = false;
            Item = item;
            _fullName = NormalizedFullName(item.FullName);
            FileCount = 0;
        }

        public int ChildrenLength
        {
            get { return _children.Count; }
        }

        public DirectoryInfoCache GetChild(int i)
        {
            return _children[i];
        }

        public List<DirectoryInfoCache> Children { get { return _children; } }

        public int ChildIndex
        {
            get
            {
                return Parent.Children.FindIndex((x) => this == x);
            }
        }

        public void SyncAllToFileSystem()
        {
            Debug.Assert(this.Parent == null);
            var walker = DirectoryInfoCache.Forest(this).Walker;
            while (walker.HasNext)
            {
                walker.MoveNext();
                var node = walker.Current;
                if (node.CurrentEdge == ForestNode<DirectoryInfoCache>.Edge.Trailing)
                    continue;
                node.Element.SyncToFileSystem();
            }
        }

        // do not sync children.
        public void SyncToFileSystem()
        {
            foreach (var di in Item.EnumerateDirectories())
            {
                SaveToCache(di);
            }
            if(!PossiblyMoveFromOutside && !String.IsNullOrEmpty(InterestedFilePattern))
                FileCount = Item.EnumerateFiles(InterestedFilePattern).Count();
        }


        public void MoveTo(FileInfo target, DirectoryInfoCache to, string newName)
        {
            // Debug.Assert(Array.Exists(Item.GetFiles(InterestedFilePattern), (x) => x == target));
            RawMoveTo(target, to.FullName, newName);
            to.FileCount++;
            FileCount--;
        }

        public DirectoryInfoCache CreateSubdirectory(string name)
        {
            var di = RawCreateSubdirectory(name);
            var diCache = SaveToCache(di);
            return diCache;
        }

        private DirectoryInfoCache SaveToCache(DirectoryInfo di)
        {
            var diCache = new DirectoryInfoCache(this, di);
            _children.Add(diCache);
            return diCache;
        }


        protected virtual DirectoryInfo RawCreateSubdirectory(string name)
        {
            return Item.CreateSubdirectory(name);
        }

        protected virtual void RawMoveTo(FileInfo target, string destPath, string newName)
        {
            target.MoveTo(Path.Combine(destPath, newName));
        }

        public static ForestNode<DirectoryInfoCache> ForestWithSync(DirectoryInfo cur)
        {
            var dic = new DirectoryInfoCache(null, cur);
            dic.SyncAllToFileSystem();
            return Forest(dic);
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
                (x, y) => x.FullName == y.FullName);
            return root;
        }

        private static string NormalizedFullName(string fullName)
        {
            return fullName.TrimEnd(new char[] { '\\', '/' });
        }


        public override int GetHashCode()
        {
            return _fullName.GetHashCode();
        }

        public string FullName { get { return _fullName; } }

        public override bool Equals(object other)
        {
            if (other == null)
                return ((object)this) == null;
            if (other is DirectoryInfoCache)
            {
                DirectoryInfoCache dic = other as DirectoryInfoCache;
                return _fullName == dic.FullName;
            }
            return false;
        }

        public static bool operator ==(DirectoryInfoCache a, DirectoryInfoCache b)
        {
            if (null == (object)a)
                return null == (object)b;
            return a.Equals(b);
        }
        public static bool operator !=(DirectoryInfoCache a, DirectoryInfoCache b)
        {
            if (null == (object)a)
                return null != (object)b;
            return !a.Equals(b);
        }

        internal DirectoryInfoCache EnsureSubdirectory(string relativeSub)
        {
            var sub = Children.Find((di) => Path.Combine(FullName, relativeSub) == di.FullName);
            if (sub != null)
                return sub;
            return CreateSubdirectory(relativeSub);
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
            Base.PossiblyMoveFromOutside = true;
            InterestedFilePattern = "*.html";

            Current = Base;
            StartDirectory = Current;
            MaxFileNum = WikipediaConv.Properties.Settings.Default.OneFolderMaxFileNum;
            _dirty = new Dictionary<string, bool>();
        }

        // Warning! call this function before creating sub node!
        public string InterestedFilePattern { set { Base.InterestedFilePattern = value; } }

        public bool Abort { get; set; }

        ForestWalker<DirectoryInfoCache> _walker;
        private bool _splitDictInit = false;

        public void StartSplit()
        {
            Abort = false;
            Current = StartDirectory;
            var root = DirectoryInfoCache.Forest(StartDirectory);
            _walker = root.Walker;
            _dirty.Clear();
            SetDirty(StartDirectory.Item.FullName);
            SyncDirectoryInfoCacheToFileSystem();
        }

        // for profiling purpose only.
        private void SyncDirectoryInfoCacheToFileSystem()
        {
            if (_splitDictInit)
                return;
            _splitDictInit = true;
            StartDirectory.SyncAllToFileSystem();
        }

        void SetDirty(string fullName)
        {
            _dirty[fullName] = true;
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
            return _dirty.ContainsKey(fullName);
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

        internal virtual void MoveTo(DirectoryInfoCache from, FileInfo target, string destRelative)
        {
            var newName = target.Name;
            var destDIC = EnsureSubdirectory(from, destRelative);
#if DIRTY
            SetDirty(destDIC.FullName);
#endif
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    from.MoveTo(target, destDIC, newName);
                    return;
                }
                catch (IOException)
                {
                    var fname = Path.GetFileNameWithoutExtension(newName);
                    var extension = Path.GetExtension(newName);
                    fname += 'X';
                    newName = fname + extension;
                }
            }
        }

        private bool SortToSubdirectories()
        {
            bool moveSomething = false;
            foreach (var file in FileEnum)
            {
                string destRelative = GetMatchedSubcirectoryRelativePath(file);
                if (!String.IsNullOrEmpty(destRelative))
                {
                    moveSomething = true;
                    MoveTo(Current, file, destRelative);
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

        internal string GetMatchedSubcirectoryRelativePath(FileInfo file)
        {
            string untilCur = FileNameHeadUntilCurrent;
            string key = FileNameToSortKey(file);
            if (key.Length == untilCur.Length)
                return "";
            return LookupSortChar(key, untilCur.Length);
        }

        internal string GetMatchedSubdirectoryPath(FileInfo file)
        {
            var relative = GetMatchedSubcirectoryRelativePath(file);
            if(String.IsNullOrEmpty(relative))
                return Current.Item.FullName;
            return Path.Combine(Current.Item.FullName, relative);
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


        DirectoryInfoCache EnsureSubdirectory(DirectoryInfoCache parent, string relativeSub)
        {
            return parent.EnsureSubdirectory(relativeSub);
        }

        public int MaxFileNum { get; set; }

        public bool TooMuchFile 
        {
            get
            {
                return Current.FileCount > MaxFileNum;
            }
        }

        public bool AlreadySplited { 
            get 
            {
                return Current.ChildrenLength != 0;
            } 
        }

    }
}
