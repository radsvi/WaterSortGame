using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.Models
{
    internal class NodeTree<T>
    {
        private T data;
        private LinkedList<NodeTree<T>> child;
        public NodeTree(T data)
        {
            this.data = data;
            child = new LinkedList<NodeTree<T>>();
        }
        public void AddChild(T data)
        {
            child.AddFirst(new NodeTree<T>(data));
        }
        public NodeTree<T> GetChild(int i)
        {
            foreach (NodeTree<T> node in child)
                if (--i == 0)
                    return node;
            return null;
        }
    }
}
