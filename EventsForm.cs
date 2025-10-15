using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp
{
    public partial class EventsForm : Form
    {
        // Core data structures
        private readonly SortedDictionary<DateTime, List<Event>> eventsByDate = new(); //(geeksforgeeks, 2025)
        private readonly Queue<string> searchHistory = new();      // Stores last 5 searches (Alexandra, 2017)
        private readonly Stack<Event> recentlyViewed = new();      // Tracks recently viewed events (Mooney, 2022)
        private readonly HashSet<string> uniqueCategories = new(); // Stores unique event categories  (w3schools, n.d.)

        public EventsForm()
        {
            InitializeComponent();
            Load += EventsForm_Load;
        }

        private void EventsForm_Load(object sender, EventArgs e)
        {
            // Demo data
            AddEvent("Heritage Day Celebration", new DateTime(2025, 9, 24), "Cultural", "Parade and food market.");
            AddEvent("Heritage Business Expo", new DateTime(2025, 10, 25), "Business", "Support local businesses.");
            AddEvent("Job Fair", new DateTime(2025, 10, 28), "Employment", "Meet local companies hiring youth.");
            AddEvent("Community Health Day", new DateTime(2025, 10, 30), "Health", "Free screening & nutrition tips.");
            AddEvent("Community Clean-Up", new DateTime(2025, 11, 5), "Environment", "Join us to clean local parks.");
            AddEvent("Youth Coding Bootcamp", new DateTime(2025, 11, 15), "Education", "Intro to coding for beginners.");
            AddEvent("Recycling Awareness Week", new DateTime(2025, 11, 20), "Environment", "Learn to recycle smartly.");
            AddEvent("Sports Day at the Stadium", new DateTime(2025, 11, 22), "Sports", "Family-friendly sports day.");
            AddEvent("Holiday Food Drive", new DateTime(2025, 12, 5), "Charity", "Donate food & support families.");
            AddEvent("Art in the Park", new DateTime(2025, 12, 10), "Arts", "Outdoor art exhibition & workshops.");

            dgvEvents.CellClick += DgvEvents_CellClick;

            // Handle showing recent searches when clicking the search box
            txtSearch.Enter += TxtSearch_Enter;

            DisplayEvents();
            btnShowAll.Visible = false;
        }

        private void AddEvent(string title, DateTime date, string category, string description)
        {
            if (!eventsByDate.ContainsKey(date))
                eventsByDate[date] = new List<Event>();
            eventsByDate[date].Add(new Event(title, date, category, description));

            uniqueCategories.Add(category);
        }

        private void DisplayEvents(List<Event>? list = null)
        {
            dgvEvents.Rows.Clear();
            var display = list ?? eventsByDate.SelectMany(kv => kv.Value);

            foreach (var ev in display)
                dgvEvents.Rows.Add(ev.Title, ev.Date.ToShortDateString(), ev.Category, ev.Description);

            lblRecommendations.Text = list == null ? "Showing all events." : "Filtered results displayed below.";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                MessageBox.Show("Please enter a search term.", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Maintain search history (max 5) (Nicholas, 2012)
            if (searchHistory.Count >= 5)
                searchHistory.Dequeue();
            searchHistory.Enqueue(keyword);

            // Update AutoComplete list for next search
            txtSearch.AutoCompleteCustomSource = new AutoCompleteStringCollection();
            txtSearch.AutoCompleteCustomSource.AddRange(searchHistory.Reverse().ToArray());

            // Search events
            var results = eventsByDate
                .SelectMany(kv => kv.Value)
                .Where(ev => ev.Title.ToLower().Contains(keyword)
                          || ev.Category.ToLower().Contains(keyword)
                          || ev.Description.ToLower().Contains(keyword))
                .ToList();

            if (results.Count == 0)
            {
                MessageBox.Show("No matching events found.", "Search",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblRecommendations.Text = "";
                return;
            }

            foreach (var ev in results)
                recentlyViewed.Push(ev);

            DisplayEvents(results);
            GenerateRecommendations(keyword, results);
            btnShowAll.Visible = true;

            MessageBox.Show($"{results.Count} event(s) found.", "Search Results",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            DisplayEvents();
            lblRecommendations.Text = "Showing all events.";
            btnShowAll.Visible = false;
        }

        private void GenerateRecommendations(string keyword, List<Event> currentResults)
        {
            var allEvents = eventsByDate.SelectMany(kv => kv.Value).ToList();

            string? mainCategory = currentResults
                .GroupBy(e => e.Category)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            List<Event> related = new();

            if (!string.IsNullOrEmpty(mainCategory))
            {
                related = allEvents
                    .Where(e => !currentResults.Contains(e)
                             && e.Category.Equals(mainCategory, StringComparison.OrdinalIgnoreCase))
                    .Take(3)
                    .ToList();
            }

            if (related.Count < 3)
            {
                var keywordMatches = allEvents
                    .Where(e => !currentResults.Contains(e)
                             && (e.Title.ToLower().Contains(keyword)
                              || e.Description.ToLower().Contains(keyword)
                              || e.Category.ToLower().Contains(keyword)))
                    .Except(related)
                    .Take(3 - related.Count)
                    .ToList();
                related.AddRange(keywordMatches);
            }

            var frequentTerm = searchHistory.GroupBy(s => s)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(frequentTerm) && related.Count < 3)
            {
                var trending = allEvents
                    .Where(e => !currentResults.Contains(e)
                             && (e.Title.ToLower().Contains(frequentTerm)
                              || e.Category.ToLower().Contains(frequentTerm)))
                    .Except(related)
                    .Take(3 - related.Count);
                related.AddRange(trending);
            }

            // --- Build Recommendations and Recently Viewed Section ---
            var recText = related.Count == 0
                ? "You might also like:\nNo other related events found."
                : "You might also like:\n" + string.Join("\n", related.Select(r => $"- {r.Title} ({r.Category}) on {r.Date:d}"));

            // Include recently viewed events (if available)
            if (recentlyViewed.Count > 0)
            {
                var recent = recentlyViewed.Take(3)
                    .Select(r => $"- {r.Title} ({r.Category}) on {r.Date:d}");
                recText += "\n\nRecently viewed:\n" + string.Join("\n", recent);
            }

            lblRecommendations.Text = recText;
        }

        private void DgvEvents_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvEvents.Rows.Count)
            {
                string title = dgvEvents.Rows[e.RowIndex].Cells["Title"].Value?.ToString();
                var ev = eventsByDate.Values.SelectMany(v => v)
                    .FirstOrDefault(x => x.Title == title);
                if (ev != null)
                {
                    recentlyViewed.Push(ev);
                    MessageBox.Show($"Viewed: {ev.Title}", "Event Viewed",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // Show recent search suggestions when clicking inside the search bar
        private void TxtSearch_Enter(object sender, EventArgs e)
        {
            if (searchHistory.Count == 0) return;

            txtSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;

            var autoComplete = new AutoCompleteStringCollection();
            autoComplete.AddRange(searchHistory.Reverse().ToArray());
            txtSearch.AutoCompleteCustomSource = autoComplete;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            var main = new MainMenuForm();
            main.ShowDialog();
            this.Close();
        }
    }
}
/* References 
Alexandra, 2017. What Is a C# Queue? How It Works, and the Benefits and Challenges of Working with C# Queues. [Online] 
Available at: https://stackify.com/what-is-csharp-queue/
[Accessed 15 October 2025].
geeksforgeeks, 2025. SortedDictionary Implementation in C#. [Online] 
Available at: https://www.geeksforgeeks.org/c-sharp/sorteddictionary-implementation-in-c-sharp/
[Accessed 13 October 2025].
Mooney, L., 2022. Understanding the Stack and Heap in C#. [Online] 
Available at: https://endjin.com/blog/2022/07/understanding-the-stack-and-heap-in-csharp-dotnet
[Accessed 03 October 2025].
Nicholas, 2012. C# Web Browser History Help. [Online] 
Available at: https://www.c-sharpcorner.com/forums/c-sharp-web-browser-history-help
[Accessed 03 October 2025].
w3schools, n.d.. DSA Hash Sets. [Online] 
Available at: https://www.w3schools.com/dsa/dsa_data_hashsets.php#:~:text=A%20Hash%20Set%20is%20a,is%20part%20of%20a%20set.
[Accessed 03 October 2025].
*/
