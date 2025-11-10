using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    public class ServiceGraph
    {
        private readonly Dictionary<string, List<(string dest, int weight)>> adjacencyList = new();
        private List<string> traversalOrder = new();

        public void AddEdge(string src, string dest, int weight = 1)
        {
            if (!adjacencyList.ContainsKey(src)) adjacencyList[src] = new List<(string, int)>();
            if (!adjacencyList.ContainsKey(dest)) adjacencyList[dest] = new List<(string, int)>();
            adjacencyList[src].Add((dest, weight));
            adjacencyList[dest].Add((src, weight));
        }

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

        public int MinimumSpanningTreeCost()
        {
            if (adjacencyList.Count == 0) return 0;
            var nodes = adjacencyList.Keys.ToList();
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
                if (minEdge.weight == int.MaxValue) break;
                visited.Add(minEdge.dest);
                totalCost += minEdge.weight;
            }

            return totalCost;
        }

        public void DemoGraphTraversal(List<Issue> issuesInArea, string areaName)
        {
            AddEdge("Ward A", "Ward B", 5);
            AddEdge("Ward B", "Ward C", 3);
            AddEdge("Ward A", "Ward D", 2);
            AddEdge("Ward C", "Ward D", 4);

            var urgentIssues = issuesInArea.OrderBy(i => i.Priority).ToList();
            traversalOrder = BFS(areaName);
            string route = traversalOrder.Count > 0 ? string.Join(" → ", traversalOrder) : "(no connected route)";
            int cost = MinimumSpanningTreeCost();

            MessageBox.Show(
                $"Optimised Service Route for {areaName}:\n\n" +
                $"Pending requests: {urgentIssues.Count}\n" +
                $"Route: {route}\n" +
                $"Estimated total travel time: {cost} minutes",
                "Optimised Service Route",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        public void DrawGraph(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var pen = new Pen(Color.SeaGreen, 2);
            var font = new Font("Segoe UI", 10);
            var brush = Brushes.Black;

            var positions = new Dictionary<string, Point>
            {
                ["Ward A"] = new Point(80, 100),
                ["Ward B"] = new Point(250, 60),
                ["Ward C"] = new Point(420, 100),
                ["Ward D"] = new Point(250, 200)
            };

            DrawLine(g, pen, positions["Ward A"], positions["Ward B"], "5");
            DrawLine(g, pen, positions["Ward B"], positions["Ward C"], "3");
            DrawLine(g, pen, positions["Ward A"], positions["Ward D"], "2");
            DrawLine(g, pen, positions["Ward C"], positions["Ward D"], "4");

            foreach (var kvp in positions)
            {
                var isVisited = traversalOrder.Contains(kvp.Key);
                var fill = isVisited ? Brushes.LightGreen : Brushes.LightGray;
                g.FillEllipse(fill, kvp.Value.X - 20, kvp.Value.Y - 20, 40, 40);
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
