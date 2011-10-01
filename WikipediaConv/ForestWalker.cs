using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikipediaConv
{
    class ForestWalker<E> : IEnumerator<ForestNode<E>> 
        where E : class
    {
        ForestNode<E> _root;
        ForestNode<E> _current;

        public ForestWalker(ForestNode<E> root)
        {
            _root = root;
        }


        public bool HasNext
        {
            get
            {
                return (_current == null) || !(_current.CurrentEdge == ForestNode<E>.Edge.Trailing &&
                                _current.Element == _root.Element);
            }
        }

        public void SkipChildren()
        {
            _current = _current.NewEdge(ForestNode < E >.Edge.Trailing);
        }

        private ForestNode<E> Next()
        {
            if (_current == null)
            {
                _current = _root.NewEdge(ForestNode < E >.Edge.Leading);
                return _current;
            }
            if (_current.CurrentEdge == ForestNode < E >.Edge.Leading)
            {
                if (_current.ChildCount == 0)
                {
                    _current = _current.NewEdge(ForestNode < E >.Edge.Trailing);
                    return _current;
                }
                _current = _current.Child(ForestNode < E >.Edge.Leading, 0);
                return _current;
            }
            ForestNode<E> parent = _current.Parent(ForestNode < E >.Edge.Trailing);
            if (parent == null)
                throw new Exception("No next node, never reached here");
            int curIndex = _current.ChildIndex;
            if (curIndex < parent.ChildCount - 1)
            {
                _current = parent.Child(ForestNode < E >.Edge.Leading, curIndex + 1);
                return _current;
            }
            // last sibling, go up ward.
            _current = parent;
            return _current;
        }

        public ForestNode<E> Current
        {
            get { return _current; }
        }

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _current; }
        }

        public bool MoveNext()
        {
            if (!HasNext)
                return false;
            Next();
            return true;
        }

        public void Reset()
        {
            _current = _root;
        }
    }
}
