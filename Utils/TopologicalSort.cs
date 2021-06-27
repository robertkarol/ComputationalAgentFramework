using System;
using System.Collections.Generic;

namespace ComputationalAgentFramework.Utils
{
    public static class TopologicalSort
    {
        public static IList<T> Sort<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies, IEqualityComparer<T> comparer = null)
        {
            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>(comparer);

            foreach (var item in source)
            {
                Visit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        public static void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
        {
            bool inProcess;
            var alreadyVisited = visited.TryGetValue(item, out inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    throw new ArgumentException("Cyclic dependency found.");
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        Visit(dependency, getDependencies, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }

        public class Item
        {
            public string Name { get; }

            public Item[] Dependencies { get; set; }

            public Item(string name, params Item[] dependencies)
            {
                Name = name;
                Dependencies = dependencies;
            }

        }

        public class ItemEqualityComparer : EqualityComparer<Item>
        {
            public override bool Equals(Item x, Item y)
            {
                return x?.Name == y?.Name;
            }

            public override int GetHashCode(Item obj)
            {
                return obj?.Name.GetHashCode() ?? 0;
            }
        }
    }
}
