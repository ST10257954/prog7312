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

        // ================================================================
        // BASIC EDGE MANAGEMENT
        // ================================================================
        public void AddEdge(string src, string dest, int weight = 1)
        {
            if (!adjacencyList.ContainsKey(src)) adjacencyList[src] = new List<(string, int)>();
            if (!adjacencyList.ContainsKey(dest)) adjacencyList[dest] = new List<(string, int)>();
            adjacencyList[src].Add((dest, weight));
            adjacencyList[dest].Add((src, weight));
        }

        // ================================================================
        // BFS TRAVERSAL
        // ================================================================
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

        // ================================================================
        // PRIM'S ALGORITHM – MINIMUM SPANNING TREE (REAL DATA)
        // ================================================================
        public (List<(string From, string To, int Weight)> Edges, int TotalCost)
            ComputeMST_Prim(string start)
        {
            // ✅ Named tuple fields defined correctly here
            var mstEdges = new List<(string From, string To, int Weight)>();
            var visited = new HashSet<string>();

            // SortedSet comparer using tuple positions (Item1, Item2, Item3)
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
            foreach (var (dest, weight) in adjacencyList[start])
                pq.Add((weight, start, dest));

            int totalCost = 0;

            while (pq.Count > 0 && visited.Count < adjacencyList.Count)
            {
                var (weight, from, to) = pq.Min;
                pq.Remove(pq.Min);

                if (visited.Contains(to)) continue;

                visited.Add(to);
                mstEdges.Add((from, to, weight));
                totalCost += weight;

                foreach (var (dest, w) in adjacencyList[to])
                {
                    if (!visited.Contains(dest))
                        pq.Add((w, to, dest));
                }
            }

            return (mstEdges, totalCost);
        }

        // ================================================================
        // DEMO GRAPH TRAVERSAL (OPTIONAL)
        // ================================================================
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

        // ================================================================
        // DRAWING VISUAL GRAPH (STATIC DEMO)
        // ================================================================
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
