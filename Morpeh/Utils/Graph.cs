using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scellecs.Morpeh.Utils
{

    internal sealed class Graph<T>
    {
       
        private HashSet<Node<T>> _nodes = new HashSet<Node<T>>();
        private Dictionary<Node<T>, HashSet<Node<T>>> _edges = new Dictionary<Node<T>, HashSet<Node<T>>>();
        public void AddNode(Node<T> node)
        {
            _nodes.Add(node);
        }
        public void AddEdge(Node<T> nodeA, Node<T> nodeB)
        {
            _nodes.Add(nodeA);
            _nodes.Add(nodeB);
            if (!_edges.ContainsKey(nodeA))
                _edges[nodeA] = new HashSet<Node<T>>();
            _edges[nodeA].Add(nodeB);
        }
        public void RemoveEdge(Node<T> nodeA, Node<T> nodeB)
        {
            if (_edges.ContainsKey(nodeA) && _edges[nodeA].Contains(nodeB))
            {
                _edges[nodeA].Remove(nodeB);
            }
        }
        public IEnumerable<Node<T>> GetAllNodes() => _nodes;
        public void RemoveNode(Node<T> node)
        {
            _nodes.Remove(node);
            _edges.Remove(node);
        }
        public IEnumerable<Node<T>> GetConnectedNodes(Node<T> node)
        {
            if (_edges.TryGetValue(node, out var connected))
                return connected;
            else
                return Enumerable.Empty<Node<T>>();
        }
    }
}
