using System;
using System.Collections.Generic;
using System.Text;

namespace MunicipalServicesApp.Data
{
    /// <summary>
    /// Represents a department collaboration graph.
    /// </summary>
    public class DepartmentGraph
    {
        private Dictionary<string, List<string>> adj = new();

        public void AddEdge(string from, string to)
        {
            if (!adj.ContainsKey(from))
                adj[from] = new List<string>();
            if (!adj.ContainsKey(to))
                adj[to] = new List<string>();

            adj[from].Add(to);
            adj[to].Add(from);
        }

        // Breadth-First Search traversal order
        public string BFS(string start)
        {
            if (!adj.ContainsKey(start))
                return "Start node not found.";

            var visited = new HashSet<string>();
            var queue = new Queue<string>();
            var sb = new StringBuilder();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                string dept = queue.Dequeue();
                sb.Append(dept);

                foreach (var neighbor in adj[dept])
                {
                    if (!visited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }

                if (queue.Count > 0) sb.Append(" → ");
            }

            return sb.ToString();
        }
    }
}
