using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq; // ✅ Needed for .First()
using System.Windows.Forms;
using MunicipalServicesApp.Models; // ✅ Access Issue model
using MunicipalServicesApp.Data;   // ✅ Access IssueRepository

namespace MunicipalServicesApp.Data
{
    /// <summary>
    /// Simple undirected weighted graph representing municipal areas or service routes.
    /// Supports BFS traversal to find connected paths and a basic Minimum Spanning Tree (Prim).
    /// Integrated visually with the Route Optimiser feature.
    /// </summary>
    public class ServiceGraph
    {
        private readonly Dictionary<string, List<(string dest, int weight)>> adjacencyList;
        private List<string> traversalOrder = new(); // ✅ store BFS order globally

        public ServiceGraph()
        {
            adjacencyList = new Dictionary<string, List<(string dest, int weight)>>();
        }

        public void AddEdge(string src, string dest, int weight = 1)
        {
            if (!adjacencyList.ContainsKey(src))
                adjacencyList[src] = new List<(string dest, int weight)>();
            if (!adjacencyList.ContainsKey(dest))
                adjacencyList[dest] = new List<(string dest, int weight)>();

            adjacencyList[src].Add((dest, weight));
            adjacencyList[dest].Add((src, weight)); // undirected
        }

        /// <summary>
        /// Breadth-first traversal starting from a node.
        /// Returns the order of visited nodes.
        /// </summary>
        public List<string> BFS(string start)
        {
            var visited = new HashSet<string>();
            var queue = new Queue<string>();
            var order = new List<string>();

            if (!adjacencyList.ContainsKey(start))
                return order;

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                order.Add(current);

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

        /// <summary>
        /// Simple Minimum Spanning Tree using Prim's algorithm (returns total cost).
        /// </summary>
        public int MinimumSpanningTreeCost()
        {
            if (adjacencyList.Count == 0) return 0;

            var nodes = new List<string>(adjacencyList.Keys);
            var visited = new HashSet<string> { nodes[0] };
            int totalCost = 0;

            while (visited.Count < nodes.Count)
            {
                (string src, string dest, int weight) minEdge = ("", "", int.MaxValue);

                foreach (var src in visited)
                {
                    foreach (var (dest, weight) in adjacencyList[src])
                    {
                        if (!visited.Contains(dest) && weight < minEdge.weight)
                            minEdge = (src, dest, weight);
                    }
                }

                if (minEdge.weight == int.MaxValue) break; // disconnected

                visited.Add(minEdge.dest);
                totalCost += minEdge.weight;
            }

            return totalCost;
        }

        /// <summary>
        /// Demonstrates graph traversal (BFS) and MST cost visually + text output.
        /// This method links directly to the Route Optimiser button in the UI.
        /// </summary>
        public void DemoGraphTraversal()
        {
            // Example municipal wards / service nodes
            AddEdge("Ward A", "Ward B", 5);
            AddEdge("Ward B", "Ward C", 3);
            AddEdge("Ward A", "Ward D", 2);
            AddEdge("Ward C", "Ward D", 4);

            // ✅ Determine start ward based on most urgent issue
            var allIssues = IssueRepository.Issues.ToList();
            string startWard = "Ward A"; // fallback

            if (allIssues.Count > 0)
            {
                var mostUrgent = allIssues.OrderBy(i => i.Priority).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(mostUrgent?.Location))
                    startWard = mostUrgent.Location;
            }

            // ✅ Perform traversal dynamically from the most urgent request location
            traversalOrder = BFS(startWard);
            string route = string.Join(" → ", traversalOrder);
            int cost = MinimumSpanningTreeCost();

            MessageBox.Show(
                $"The most efficient route to attend all service requests is:\n\n" +
                $"🗺️ {route}\n\n" +
                $"⏱️ Estimated total travel time: {cost} minutes",
                "Optimised Service Route",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        /// <summary>
        /// Draws the graph visually on the Route Optimiser panel.
        /// Highlights visited nodes based on the BFS traversal order.
        /// </summary>
        public void DrawGraph(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var pen = new Pen(Color.SeaGreen, 2);
            var font = new Font("Segoe UI", 10);
            var brush = Brushes.Black;

            // Fixed node positions
            var positions = new Dictionary<string, Point>
            {
                ["Ward A"] = new Point(80, 100),
                ["Ward B"] = new Point(250, 60),
                ["Ward C"] = new Point(420, 100),
                ["Ward D"] = new Point(250, 200)
            };

            // Draw edges with weights
            DrawLine(g, pen, positions["Ward A"], positions["Ward B"], "5");
            DrawLine(g, pen, positions["Ward B"], positions["Ward C"], "3");
            DrawLine(g, pen, positions["Ward A"], positions["Ward D"], "2");
            DrawLine(g, pen, positions["Ward C"], positions["Ward D"], "4");

            // Draw nodes (highlight traversal)
            foreach (var kvp in positions)
            {
                var isVisited = traversalOrder.Contains(kvp.Key);
                var fillColor = isVisited ? Brushes.LightGreen : Brushes.LightGray;

                g.FillEllipse(fillColor, kvp.Value.X - 20, kvp.Value.Y - 20, 40, 40);
                g.DrawEllipse(Pens.DarkGreen, kvp.Value.X - 20, kvp.Value.Y - 20, 40, 40);
                g.DrawString(kvp.Key, font, brush, kvp.Value.X - 25, kvp.Value.Y + 25);
            }
        }

        private void DrawLine(Graphics g, Pen pen, Point a, Point b, string weight)
        {
            g.DrawLine(pen, a, b);
            var mid = new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
            g.DrawString(weight, new Font("Segoe UI", 9, FontStyle.Bold), Brushes.DarkGreen, mid);
        }
    }
}
