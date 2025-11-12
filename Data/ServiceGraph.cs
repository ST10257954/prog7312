using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{

    /*
    Represents a weighted, undirected graph showing how service areas (wards)
    are connected for route planning. Used to visualise travel paths, perform
    traversals, and compute minimum-cost inspection routes.
    */
    public class ServiceGraph
    {

        // Stores each node and its connected edges with distances (weights)
        private readonly Dictionary<string, List<(string dest, int weight)>> adjacencyList = new();

        // Records the most recent traversal path for visual highlighting
        private List<string> traversalOrder = new();


        /* Add a connection between two service areas with a travel cost.
        This models how wards or depots are physically linked.*/
        public void AddEdge(string src, string dest, int weight = 1)
        {
            if (!adjacencyList.ContainsKey(src)) adjacencyList[src] = new List<(string, int)>();
            if (!adjacencyList.ContainsKey(dest)) adjacencyList[dest] = new List<(string, int)>();
            adjacencyList[src].Add((dest, weight));
            adjacencyList[dest].Add((src, weight));
        }

        /* Traverse all connected areas starting from one ward to identify
           reachable routes. BFS is used because it finds the shortest path
           (fewest hops) from the starting node to others (geeksforgeeks, 2025).*/
        public List<string> BFS(string start)
        {
            var visited = new HashSet<string>();
            var queue = new Queue<string>();
            var order = new List<string>();

            if (!adjacencyList.ContainsKey(start)) return order;

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                order.Add(current);

                // Visit each neighbouring ward to explore reachable routes
                foreach (var (neighbor, _) in adjacencyList[current])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
            return order;
        }

        /* PRIM'S ALGORITHM – MINIMUM SPANNING TREE
           Compute the cheapest set of connections that links all wards.
           This helps the municipality plan inspection routes efficiently.*/

        public (List<(string From, string To, int Weight)> Edges, int TotalCost)
            ComputeMST_Prim(string start)
        {
            var mstEdges = new List<(string From, string To, int Weight)>();
            var visited = new HashSet<string>();

            // Priority queue holds edges sorted by weight (smallest first)
            var pq = new SortedSet<(int, string, string)>(
                Comparer<(int, string, string)>.Create((a, b) =>
                {
                    int cmp = a.Item1.CompareTo(b.Item1);
                    if (cmp == 0) cmp = (a.Item2 + a.Item3).CompareTo(b.Item2 + b.Item3);
                    return cmp;
                }));

            if (!adjacencyList.ContainsKey(start))
                return (new List<(string From, string To, int Weight)>(), 0);

            visited.Add(start);

            // Begin with all edges from the starting ward
            foreach (var (dest, weight) in adjacencyList[start])
                pq.Add((weight, start, dest));

            int totalCost = 0;

            // Select the smallest edge that connects to a new ward
            while (pq.Count > 0 && visited.Count < adjacencyList.Count)
            {
                var (weight, from, to) = pq.Min;
                pq.Remove(pq.Min);

                if (visited.Contains(to)) continue;

                visited.Add(to);
                mstEdges.Add((from, to, weight));
                totalCost += weight;

                // Add new edges from the newly connected ward
                foreach (var (dest, w) in adjacencyList[to])
                {
                    if (!visited.Contains(dest))
                        pq.Add((w, to, dest));
                }
            }

            return (mstEdges, totalCost);
        }

        
        
        
        /* Demonstrates how graph traversal and MST logic operate
           using sample ward connections combined with real issue data.
           This hybrid setup allows testing and visualisation of route planning
           even without live map coordinates. */
        public void DemoGraphTraversal(List<Issue> issuesInArea, string areaName)
        {
            AddEdge("Ward A", "Ward B", 5);
            AddEdge("Ward B", "Ward C", 3);
            AddEdge("Ward A", "Ward D", 2);
            AddEdge("Ward C", "Ward D", 4);

            var urgentIssues = issuesInArea.OrderBy(i => i.Priority).ToList();
            traversalOrder = BFS(areaName);
            string route = traversalOrder.Count > 0 ? string.Join(" → ", traversalOrder) : "(no connected route)";

            var (edges, totalCost) = ComputeMST_Prim(areaName);

            string mstText = edges.Count == 0
                ? "(No MST edges found)"
                : string.Join("\n", edges.Select(e => $"{e.From} → {e.To} ({e.Weight})"));

            MessageBox.Show(
                $"Optimised Service Route for {areaName}:\n\n" +
                $"Pending requests: {urgentIssues.Count}\n" +
                $"BFS Route: {route}\n\n" +
                $"MST Connections:\n{mstText}\n\n" +
                $"Total Optimised Cost: {totalCost}",
                "Optimised Service Route",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        // Used to help users understand connectivity at a glance.
        public void DrawGraph(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var pen = new Pen(Color.SeaGreen, 2);
            var font = new Font("Segoe UI", 10);
            var brush = Brushes.Black;


            // Predefined layout positions for visual clarity
            var positions = new Dictionary<string, Point>
            {
                ["Ward A"] = new Point(80, 100),
                ["Ward B"] = new Point(250, 60),
                ["Ward C"] = new Point(420, 100),
                ["Ward D"] = new Point(250, 200)
            };

            // Draw connections and their travel costs
            DrawLine(g, pen, positions["Ward A"], positions["Ward B"], "5");
            DrawLine(g, pen, positions["Ward B"], positions["Ward C"], "3");
            DrawLine(g, pen, positions["Ward A"], positions["Ward D"], "2");
            DrawLine(g, pen, positions["Ward C"], positions["Ward D"], "4");


            // Draw each ward as a circle; highlight those visited by BFS
            foreach (var kvp in positions)
            {
                var isVisited = traversalOrder.Contains(kvp.Key);
                var fill = isVisited ? Brushes.LightGreen : Brushes.LightGray;
                g.FillEllipse(fill, kvp.Value.X - 20, kvp.Value.Y - 20, 40, 40);
                g.DrawEllipse(Pens.DarkGreen, kvp.Value.X - 20, kvp.Value.Y - 20, 40, 40);
                g.DrawString(kvp.Key, font, brush, kvp.Value.X - 25, kvp.Value.Y + 25);
            }
        }

        // Draw a connection line and label it with its distance (weight)
        private void DrawLine(Graphics g, Pen pen, Point a, Point b, string weight)
        {
            g.DrawLine(pen, a, b);
            var mid = new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
            g.DrawString(weight, new Font("Segoe UI", 9, FontStyle.Bold), Brushes.DarkGreen, mid);
        }
    }
}

/*References
GeeksforGeeks, 2025. Prim’s Minimum Spanning Tree (MST) Algorithm – Greedy Approach. [Online]
Available at: https://www.geeksforgeeks.org/prims-minimum-spanning-tree-mst-greedy-algo-5/
[Accessed 12 November 2025].

Microsoft, 2025. Graph Data Structures in C#. [Online]
Available at: https://learn.microsoft.com/en-us/dotnet/standard/collections/when-to-use-generic-collections
[Accessed 12 November 2025].

geeksforgeeks, 2025. Breadth First Search or BFS for a Graph. [Online] 
Available at: https://www.geeksforgeeks.org/dsa/breadth-first-search-or-bfs-for-a-graph/
[Accessed 12 November 2025].
*/

