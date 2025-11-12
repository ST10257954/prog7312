using System;
using System.Collections.Generic;
using System.Text;

namespace MunicipalServicesApp.Data
{
    /*
    Represents a simple undirected graph showing collaboration links between departments.
    Used to model how different service units (e.g. Water, Sanitation, Roads) are interconnected for coordination and communication within the municipality.
     */
    public class DepartmentGraph
    {

        // Stores graph edges: each department maps to its connected departments
        private Dictionary<string, List<string>> adj = new();


        // Adds a two-way (undirected) connection between two departments.
        public void AddEdge(string from, string to)
        {
            if (!adj.ContainsKey(from))
                adj[from] = new List<string>();
            if (!adj.ContainsKey(to))
                adj[to] = new List<string>();

            adj[from].Add(to);
            adj[to].Add(from);
        }

        /// Performs a Breadth-First Search (BFS) to show connected departments (GeeksforGeeks, 2025).
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

/*References
GeeksforGeeks, 2025. Breadth First Search or BFS for a Graph. [Online] 
Available at: https://www.geeksforgeeks.org/dsa/breadth-first-search-or-bfs-for-a-graph/
[Accessed 12 November 2025].
*/