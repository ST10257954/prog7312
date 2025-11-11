using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp
{
    public partial class EventsForm : Form
    {
        // Core data structures
        private readonly SortedDictionary<DateTime, List<Event>> eventsByDate = new();
        private readonly Queue<string> searchHistory = new();
        private readonly Stack<Event> recentlyViewed = new();
        private readonly HashSet<string> uniqueCategories = new();

        // Save/Load search history file
        private readonly string historyFile = "searchHistory.txt";

        public EventsForm()
        {
            InitializeComponent();
            Load += EventsForm_Load;
        }

        private void EventsForm_Load(object sender, EventArgs e)
        {
            // Load previous search history
            LoadSearchHistory();

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

            // Set DataGridView column sizing and events
            dgvEvents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEvents.CellClick += DgvEvents_CellClick;
            txtSearch.Enter += TxtSearch_Enter;

            // Allow Enter key to trigger Search
            this.AcceptButton = btnSearch;

            DisplayEvents();
            btnShowAll.Visible = false;

            // Improve recommendation text appearance
            lblRecommendations.AutoEllipsis = false;
            lblRecommendations.MaximumSize = new Size(820, 0);
            lblRecommendations.AutoSize = true;
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
            lblRecommendations.Visible = list != null;
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

            // Maintain last 5 searches
            if (searchHistory.Count >= 5)
                searchHistory.Dequeue();
            searchHistory.Enqueue(keyword);
            SaveSearchHistory();

            txtSearch.AutoCompleteCustomSource = new AutoCompleteStringCollection();
            txtSearch.AutoCompleteCustomSource.AddRange(searchHistory.Reverse().ToArray());

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
                lblRecommendations.Visible = false;
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
            // Reset everything to default
            txtSearch.Clear();
            recentlyViewed.Clear();
            lblRecommendations.Visible = false;
            DisplayEvents();
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

            var recText = related.Count == 0
                ? "You might also like:\nNo other related events found."
                : "You might also like:\n" + string.Join("\n", related.Select(r => $"- {r.Title} ({r.Category}) on {r.Date:d}"));

            if (recentlyViewed.Count > 0)
            {
                var recent = recentlyViewed.Take(3)
                    .Select(r => $"- {r.Title} ({r.Category}) on {r.Date:d}");
                recText += "\n\nRecently viewed:\n" + string.Join("\n", recent);
            }

            lblRecommendations.Text = recText.Replace("\n", Environment.NewLine);
            lblRecommendations.Visible = true;
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

        private void TxtSearch_Enter(object sender, EventArgs e)
        {
            if (searchHistory.Count == 0) return;

            txtSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;

            var autoComplete = new AutoCompleteStringCollection();
            autoComplete.AddRange(searchHistory.Reverse().ToArray());
            txtSearch.AutoCompleteCustomSource = autoComplete;
        }

        // --- Save & Load search history ---
        private void SaveSearchHistory()
        {
            try
            {
                File.WriteAllLines(historyFile, searchHistory);
            }
            catch { }
        }

        private void LoadSearchHistory()
        {
            try
            {
                if (File.Exists(historyFile))
                {
                    var lines = File.ReadAllLines(historyFile);
                    foreach (var l in lines.TakeLast(5))
                        searchHistory.Enqueue(l);
                }
            }
            catch { }
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
