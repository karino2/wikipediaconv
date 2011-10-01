using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikipediaConv
{
    public class ForestNode<E> where E : class
    {
        Func<E, int, E> _childFunc;
        Func<E, E> _parentFunc;
        Func<E, int> _childCount;
        Func<E, int> _childIndex;
        
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
            Func<E, int> childIndex)
        {
                _edge = e;
                _node = elem;
                _childFunc = childFunc;
                _parentFunc = parentFunc;
                _childCount = childCount;
                _childIndex = childIndex;
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
                    _childIndex);
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
