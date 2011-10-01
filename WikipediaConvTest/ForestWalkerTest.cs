using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WikipediaConv;

namespace WikipediaConvTest
{
    [TestFixture]
    public class ForestWalkerTest
    {
        public class TreeForTest
        {
            public TreeForTest Parent;
            public List<TreeForTest> Children = new List<TreeForTest>();
        }

        [Test]
        public void TestForest()
        {
            /*
             * a--b-+-d
             *    | |
             *    | +-e
             *    c
             */
            TreeForTest a = new TreeForTest();
            TreeForTest b = new TreeForTest();
            TreeForTest c = new TreeForTest();
            TreeForTest d = new TreeForTest();
            TreeForTest e= new TreeForTest();
            a.Children.Add(b);
            a.Children.Add(c);
            b.Parent = a;
            c.Parent = a;
            b.Children.Add(d);
            b.Children.Add(e);
            d.Parent = b;
            e.Parent = b;

            ForestNode<TreeForTest> root = new ForestNode<TreeForTest>(
                 ForestNode<TreeForTest>.Edge.Leading,
                a,
                (node, i) => node.Children[i],
                (node) => node.Parent,
                (node) => node.Children.Count,
                (node) => node.Parent == null ? 1 : node.Parent.Children.IndexOf(node));

             ForestWalker<TreeForTest> walker = new ForestWalker<TreeForTest>(root);

            Assert.IsTrue(walker.MoveNext());
            AssertNode(ForestNode<TreeForTest>.Edge.Leading, a, walker.Current);
                
            Assert.IsTrue(walker.MoveNext());
            AssertNode(ForestNode<TreeForTest>.Edge.Leading, b, walker.Current);
                
            Assert.IsTrue(walker.MoveNext());
            AssertNode(ForestNode<TreeForTest>.Edge.Leading, d, walker.Current);
                
            Assert.IsTrue(walker.MoveNext());
            AssertNode(ForestNode<TreeForTest>.Edge.Trailing, d, walker.Current);
                
            Assert.IsTrue(walker.MoveNext());
            AssertNode(ForestNode<TreeForTest>.Edge.Leading, e, walker.Current);
                
            Assert.IsTrue(walker.MoveNext());
            AssertNode(ForestNode<TreeForTest>.Edge.Trailing, e, walker.Current);
                
            Assert.IsTrue(walker.MoveNext());
            AssertNode(ForestNode<TreeForTest>.Edge.Trailing, b, walker.Current);
                
            Assert.IsTrue(walker.MoveNext());
            AssertNode(ForestNode<TreeForTest>.Edge.Leading, c, walker.Current);
                
            Assert.IsTrue(walker.MoveNext());
            AssertNode(ForestNode<TreeForTest>.Edge.Trailing, c, walker.Current);
                
            Assert.IsTrue(walker.MoveNext());
            AssertNode(ForestNode<TreeForTest>.Edge.Trailing, a, walker.Current);
                
            Assert.IsFalse(walker.MoveNext());
        }

        void AssertNode(ForestNode<TreeForTest>.Edge expectE, TreeForTest expectNode, ForestNode<TreeForTest> actual)
        {
                Assert.AreEqual(expectE, actual.CurrentEdge);
                Assert.AreEqual(expectNode, actual.Element);
        }   
    }
}
