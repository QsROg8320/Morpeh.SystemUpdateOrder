using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scellecs.Morpeh.Utils
{
    internal static class TopologicalSorter
    {
        public static IEnumerable<T> Sort<T>(Graph<T> graph) 
        {            
            Stack<Node<T>> nodes = new Stack<Node<T>>(graph.GetAllNodes());
            Stack<T> sortedItems = new Stack<T>();
            while (nodes.Count > 0)
            {
                var node = nodes.Pop();
                if (node.State == State.Visited)
                    continue;

                node.State = State.Processed;

                var notVisitedChildren = graph.GetConnectedNodes(node)
                   .Where(x => x.State == State.NotVisited)
                   .ToList();


                if (graph.GetConnectedNodes(node).Any(x => x.State == State.Processed))
                      throw new Exception("Cyclic dependencies have been found.");

                if(notVisitedChildren.Any())
                    nodes.Push(node);
               
                foreach (var child in notVisitedChildren)                 
                    nodes.Push(child);                   
                

                if (!notVisitedChildren.Any())
                {
                    node.State = State.Visited;
                    sortedItems.Push(node.Value);
                }
                
            }
            while (sortedItems.Count > 0)
            {
                var item = sortedItems.Pop();
                yield return item;
            }
        }
    }
}
