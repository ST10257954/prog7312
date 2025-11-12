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
        private readonly Queue<string> searchHistory = new();     // last 5 searches
        private readonly Stack<Event> recentlyViewed = new();     // recently viewed events
        private readonly HashSet<string> uniqueCategories = new();

        // Persistent search history file
        private readonly string historyFile = "searchHistory.txt";

        public EventsForm()
        {
            InitializeComponent();
            Load += EventsForm_Load;
        }

        private void EventsForm_Load(object sender, EventArgs e)
        {
            // Load previous user behaviour
            LoadSearchHistory();

            // ------------------------------------------------------------------
            // Demo data (representing events retrieved from backend)
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
            // ------------------------------------------------------------------

            // DataGridView setup
            dgvEvents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEvents.CellClick += DgvEvents_CellClick;

            // Enter key triggers Search
            this.AcceptButton = btnSearch;

            // Display initial view
            DisplayEvents();
            btnShowAll.Visible = false;

            // Label styling polish
            lblRecommendations.AutoSize = true;
            lblRecommendations.MaximumSize = new Size(820, 0);
            lblRecommendations.TextAlign = ContentAlignment.TopLeft;
            lblRecommendations.Visible = false;

            txtSearch.Enter += TxtSearch_Enter;
        }

        // ----------------------------------------------------------------------
        // Event creation helper
        private void AddEvent(string title, DateTime date, string category, string description)
        {
            if (!eventsByDate.ContainsKey(date))
                eventsByDate[date] = new List<Event>();

            eventsByDate[date].Add(new Event(title, date, category, description));
            uniqueCategories.Add(category);
        }

        // ----------------------------------------------------------------------
        // Display all or filtered events
        private void DisplayEvents(List<Event>? list = null)
        {
            dgvEvents.Rows.Clear();

            var displayList = list ?? eventsByDate.SelectMany(kv => kv.Value);
            foreach (var ev in displayList)
                dgvEvents.Rows.Add(ev.Title, ev.Date.ToShortDateString(), ev.Category, ev.Description);

            lblRecommendations.Visible = list != null;
            lblRecommendations.Text = list == null ? "Showing all events." : "Filtered results displayed below.";
        }

        // ----------------------------------------------------------------------
        // SEARCH FUNCTION
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                MessageBox.Show("Please enter a search term.", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Maintain only 5 most recent searches
            if (searchHistory.Count >= 5)
                searchHistory.Dequeue();
            searchHistory.Enqueue(keyword);
            SaveSearchHistory();

            // Update AutoComplete source
            var src = new AutoCompleteStringCollection();
            src.AddRange(searchHistory.Reverse().ToArray());
            txtSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtSearch.AutoCompleteCustomSource = src;

            // Perform the search
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

            // Track user behaviour
            foreach (var ev in results)
                recentlyViewed.Push(ev);

            DisplayEvents(results);
            GenerateRecommendations(keyword, results);
            btnShowAll.Visible = true;

            MessageBox.Show($"{results.Count} event(s) found.", "Search Results",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ----------------------------------------------------------------------
        // SHOW ALL EVENTS (Reset)
        private void btnShowAll_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            recentlyViewed.Clear();
            lblRecommendations.Visible = false;
            DisplayEvents();
            btnShowAll.Visible = false;
        }

        // ----------------------------------------------------------------------
        // RECOMMENDATION ENGINE
        private void GenerateRecommendations(string keyword, List<Event> currentResults)
        {
            var allEvents = eventsByDate.SelectMany(kv => kv.Value).ToList();

            // Find main category among current results
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

            // Add keyword matches if needed
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

            // Add trending searches
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

            // Build recommendation message
            string recText = related.Count == 0
                ? "You might also like:\nNo other related events found."
                : "You might also like:\n" +
                  string.Join("\n", related.Select(r => $"- {r.Title} ({r.Category}) on {r.Date:d}"));

            // Add recently viewed items
            if (recentlyViewed.Count > 0)
            {
                var recent = recentlyViewed.Take(3)
                    .Select(r => $"- {r.Title} ({r.Category}) on {r.Date:d}");
                recText += "\n\nRecently viewed:\n" + string.Join("\n", recent);
            }

            lblRecommendations.Text = recText.Replace("\n", Environment.NewLine);
            lblRecommendations.Visible = true;
        }

        // ----------------------------------------------------------------------
        // Event click handler
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

        // ----------------------------------------------------------------------
        // Search bar autocomplete
        private void TxtSearch_Enter(object sender, EventArgs e)
        {
            if (searchHistory.Count == 0) return;

            var autoComplete = new AutoCompleteStringCollection();
            autoComplete.AddRange(searchHistory.Reverse().ToArray());
            txtSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtSearch.AutoCompleteCustomSource = autoComplete;
        }

        // ----------------------------------------------------------------------
        // Save & load user search behaviour
        private void SaveSearchHistory()
        {
            try { File.WriteAllLines(historyFile, searchHistory); } catch { }
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

        // Navigation back to main menu
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            var main = new MainMenuForm();
            main.ShowDialog();
            this.Close();
        }
    }
}
