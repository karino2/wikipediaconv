using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikipediaConv
{
    public class ForestNode<E> where E : class 
    {
        internal Func<E, int, E> _childFunc;
        internal Func<E, E> _parentFunc;
        internal Func<E, int> _childCount;
        internal Func<E, int> _childIndex;
        internal Func<E, E, bool> _comparer;

        public bool ElementEquals(E target)
        {
            if (_comparer != null)
                return _comparer(Element, target);
            return Element == target;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            ForestNode<E> nobj = obj as ForestNode<E>;
            if(nobj == null)
                return this == null;
            if (_edge != nobj.CurrentEdge)
                return false;
            return ElementEquals(nobj.Element);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            if (_comparer != null)
                throw new Exception("NYI custom GetHashCode for ForestNode");
            return base.GetHashCode();
        }

        public ForestWalker<E> Walker
        {
            get
            {
                return new ForestWalker<E>(this);
            }
        }
        
        public enum Edge
        {
                Leading,
                Trailing
        }
        private Edge _edge;
        private E _node;

        public ForestNode(Edge e, E elem,
            Func<E, int, E> childFunc,
            Func<E, E> parentFunc,
            Func<E, int> childCount,
            Func<E, int> childIndex,
            Func<E, E, bool> comparer)
        {
            _edge = e;
            _node = elem;
            _childFunc = childFunc;
            _parentFunc = parentFunc;
            _childCount = childCount;
            _childIndex = childIndex;
            _comparer = comparer;
        }

        public ForestNode(Edge e, E elem,
            Func<E, int, E> childFunc,
            Func<E, E> parentFunc,
            Func<E, int> childCount,
            Func<E, int> childIndex) : this(e, elem, childFunc, parentFunc, childCount, childIndex, null)
        {
        }
        
        public ForestNode(Edge edge, E node) : this(edge, node, null, null, null, null)
        {
        }
        public E Element
        {
            get
            {
                return _node;
            }
        }
        public Edge CurrentEdge
        {
            get
            {
                return _edge;
            }
        }
        private ForestNode<E> createNode(Edge e, E elem)
        {
                return new ForestNode<E>(e, elem,
                    _childFunc,
                    _parentFunc,
                    _childCount,
                    _childIndex,
                    _comparer);
        }
        public ForestNode<E> Child(Edge e, int i) {
                return createNode(e, _childFunc(_node, i));
        }
        public ForestNode<E> Parent(Edge e) {
                E parent = _parentFunc(_node);
                if(parent == null)
                        return null;
                return createNode(e, parent);
        }
        public ForestNode<E> NewEdge(Edge newE)
        {
                return createNode(newE, _node);
        }
        public int ChildCount { get { return _childCount(_node); } }
        public int ChildIndex { get { return _childIndex(_node); } }
    }
}
