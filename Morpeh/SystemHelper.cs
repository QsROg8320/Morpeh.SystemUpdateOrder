using Scellecs.Morpeh.Attributes;
using Scellecs.Morpeh.Systems;
using Scellecs.Morpeh.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scellecs.Morpeh
{
    internal static class SystemHelper
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
            public SystemNode(Type type, Node<object> node)
            {
                Type = type;
                Node = node;
            }
            public Type Type { get; private set; }
            public Node<object> Node { get; private set; }
        }

        public static IEnumerable<object> GetSortedUpdateSystems(string scriptableObjectPath, bool includeAllByDefault)
        {
            var classes = GetSystems<UpdateSystem>(includeAllByDefault);
            var systems = GenerateSystems(scriptableObjectPath, classes);
            return Sort(systems);
        }
        public static IEnumerable<object> GetSortedLateUpdateSystems(string scriptableObjectPath, bool includeAllByDefault)
        {
            var classes = GetSystems<LateUpdateSystem>(includeAllByDefault);
            var systems = GenerateSystems(scriptableObjectPath, classes);
            return Sort(systems);
        }
        public static IEnumerable<object> GetSortedFixedUpdateSystems(string scriptableObjectPath, bool includeAllByDefault)
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
        private static IEnumerable<object> Sort(IEnumerable<SystemObject> systems)
        {
            Graph<object> graph = new Graph<object>();
            var nodes = systems
                .Select(x=>new SystemNode(x.Type,new Node<object>(x.Object,State.NotVisited)))
                .ToList();
            var nodesDic = nodes.ToDictionary(x => x.Type, x => x.Node);

            foreach(var node in nodes)
            {
                graph.AddNode(node.Node);
            }

            foreach(var node in nodes)
            {
                var updateAfterNodes = node.Type.GetCustomAttributes(typeof(UpdateAfterAttribute), true)
                    .OfType<UpdateAfterAttribute>();

                foreach(var updateAfterNode in updateAfterNodes)
                {
                    if (!nodesDic.ContainsKey(updateAfterNode.Type))
                        throw new Exception($"System {updateAfterNode.Type.Name} not found");
                    graph.AddEdge(nodesDic[updateAfterNode.Type], node.Node);
                }
            }
            foreach (var node in nodes)
            {
                var updateBeforeNodes = node.Type.GetCustomAttributes(typeof(UpdateBeforeAttribute), true)
                    .OfType<UpdateBeforeAttribute>();

                foreach(var updateBeforeNode in updateBeforeNodes)
                {
                    if (!nodesDic.ContainsKey(updateBeforeNode.Type))
                        throw new Exception($"System {updateBeforeNode.Type.Name} not found");
                    graph.AddEdge(node.Node, nodesDic[updateBeforeNode.Type]);
                }
            }
            return TopologicalSorter.Sort(graph);

        }
    }
}
