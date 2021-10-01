using Morpeh.Attributes;
using Morpeh.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morpeh
{
    public static  class SystemHelper
    {
        private struct SystemObject
        {
            public SystemObject(Type type, object @object)
            {
                Type = type;
                Object = @object;
            }

            public Type Type {  get; private set; }
            public Object Object { get; private set; }

        }
        private struct SystemNode
        {
            public SystemNode(Type type, Node<Object> node)
            {
                Type = type;
                Node = node;
            }
            public Type Type { get; private set; }
            public Node<Object> Node { get; private set; }

        }


        public static IEnumerable<Object> GetSortedUpdateSystems(string scriptableObjectPath, bool includeAllByDefault)
        {
            var classes = GetSystems<UpdateSystem>(includeAllByDefault);
            var systems = GenerateSystems(scriptableObjectPath, classes);
            return Sort(systems);
        }
        public static IEnumerable<Object> GetSortedLateUpdateSystems(string scriptableObjectPath, bool includeAllByDefault)
        {
            var classes = GetSystems<LateUpdateSystem>(includeAllByDefault);
            var systems = GenerateSystems(scriptableObjectPath, classes);
            return Sort(systems);
        }
        public static IEnumerable<Object> GetSortedFixedUpdateSystems(string scriptableObjectPath, bool includeAllByDefault)
        {
            var classes = GetSystems<FixedUpdateSystem>(includeAllByDefault);
            var systems = GenerateSystems(scriptableObjectPath, classes);
            return Sort(systems);
        }
        private static IEnumerable<SystemObject> GenerateSystems(string path, IEnumerable<Type> systems)
        {
            List<SystemObject> list = new List<SystemObject>();
            foreach (var item in systems)
            {
                list.Add(new SystemObject(item,AssetHelper.CreateAsset(item, path)));
            }
            return list;
        }
        private static IEnumerable<Type> GetSystems<T>(bool includeAllByDefault)
        {
            var systems = AssetHelper.FindClasses<T>();
            if(!includeAllByDefault)
            {
                systems =systems
                    .Where(x=> x.IsDefined(typeof(IncludeAttribute), false))
                    .ToList();
            }
            return systems;
        }
        private static IEnumerable<Object> Sort(IEnumerable<SystemObject> systems)
        {
            Graph<Object> graph = new Graph<Object>();
            var nodes = systems
                .Select(x=>new SystemNode(x.Type,new Node<Object>(x.Object,State.NotVisited)))
                .ToList();
            var nodesDic = nodes.ToDictionary(x => x.Type, x => x.Node);

            foreach(var node in nodes)
            {
                graph.AddNode(node.Node);
            }

            foreach(var node in nodes)
            {
                var updateAfters = node.Type.GetCustomAttributes(
                    typeof(UpdateAfterAttribute), true
                ).OfType<UpdateAfterAttribute>();
                foreach(var updateAfter in updateAfters)
                {
                    if (!nodesDic.ContainsKey(updateAfter.Type))
                        throw new Exception($"System {updateAfter.Type.Name} not found");
                    graph.AddEdge(nodesDic[updateAfter.Type], node.Node);
                }
            }
            foreach (var node in nodes)
            {
                var updateBefors = node.Type.GetCustomAttributes(
                    typeof(UpdateBeforeAttribute), true
                ).OfType < UpdateAfterAttribute>();
                foreach(var updateBefore in updateBefors)
                {
                    if (!nodesDic.ContainsKey(updateBefore.Type))
                        throw new Exception($"System {updateBefore.Type.Name} not found");
                    graph.AddEdge( node.Node, nodesDic[updateBefore.Type]);
                }
            }
            return TopologicalSorter.Sort(graph);

        }
    }
}
