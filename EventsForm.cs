using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp
{

    /*
    Manages display, search, and recommendation of community or municipal events.
    Demonstrates the use of core data structures (Queue, Stack, HashSet, SortedDictionary)
    to enhance search experience, track user activity, and support event filtering (GeeksforGeeks, 2025).
    */

    public partial class EventsForm : Form
    {
        // Sorted by date to allow easy chronological browsing of events
        private readonly SortedDictionary<DateTime, List<Event>> eventsByDate = new();

        // Keeps track of the user’s last 5 searches for autocomplete suggestions
        private readonly Queue<string> searchHistory = new();

        // Stores the most recently viewed events to support “Recently Viewed” recommendations
        private readonly Stack<Event> recentlyViewed = new();

        // Prevents duplicate categories and helps identify popular event themes
        private readonly HashSet<string> uniqueCategories = new();

        // File used to persist user search history between sessions
        private readonly string historyFile = "searchHistory.txt";

        public EventsForm()
        {
            InitializeComponent();
            Load += EventsForm_Load;
        }


        // Called when the form loads to populate demo data and restore user preferences (GeeksforGeeks, 2025)
        private void EventsForm_Load(object sender, EventArgs e)
        {
            // reload saved searches from previous session
            LoadSearchHistory();



            // Demo data — simulates events that would normally come from a database
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



            // Configure table layout for better readability
            dgvEvents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEvents.CellClick += DgvEvents_CellClick;

            // Pressing Enter automatically triggers search
            this.AcceptButton = btnSearch;

            // Show default event list at startup
            DisplayEvents();
            btnShowAll.Visible = false;

            // Adjust label to allow dynamic recommendation text wrapping
            lblRecommendations.AutoSize = true;
            lblRecommendations.MaximumSize = new Size(820, 0);
            lblRecommendations.TextAlign = ContentAlignment.TopLeft;
            lblRecommendations.Visible = false;


            // Trigger autocomplete setup when user clicks the search box (GeeksforGeeks, 2025)
            txtSearch.Enter += TxtSearch_Enter;
        }

        // Adds a new event to the collection while grouping by date for structure
        private void AddEvent(string title, DateTime date, string category, string description)
        {
            if (!eventsByDate.ContainsKey(date))
                eventsByDate[date] = new List<Event>();

            eventsByDate[date].Add(new Event(title, date, category, description));
            uniqueCategories.Add(category);
        }

        // Displays all events or a filtered subset (search results)
        private void DisplayEvents(List<Event>? list = null)
        {
            dgvEvents.Rows.Clear();


            // Choose between full dataset or filtered results
            var displayList = list ?? eventsByDate.SelectMany(kv => kv.Value);
            foreach (var ev in displayList)
                dgvEvents.Rows.Add(ev.Title, ev.Date.ToShortDateString(), ev.Category, ev.Description);

            lblRecommendations.Visible = list != null;
            lblRecommendations.Text = list == null ? "Showing all events." : "Filtered results displayed below.";
        }

        // Handles search logic and triggers recommendation generation
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                MessageBox.Show("Please enter a search term.", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Maintain a short-term memory of only the 5 latest searches (GeeksforGeeks, 2025)
            if (searchHistory.Count >= 5)
                searchHistory.Dequeue();
            searchHistory.Enqueue(keyword);
            SaveSearchHistory();

            // Update autocomplete suggestions dynamically
            var src = new AutoCompleteStringCollection();
            src.AddRange(searchHistory.Reverse().ToArray());
            txtSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtSearch.AutoCompleteCustomSource = src;

            // Filter matching events based on keyword relevance
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

            // Push found events to “recently viewed” stack for user tracking
            foreach (var ev in results)
                recentlyViewed.Push(ev);


            // Show filtered results and generate related suggestions (GeeksforGeeks, 2025)
            DisplayEvents(results);
            GenerateRecommendations(keyword, results);
            btnShowAll.Visible = true;

            MessageBox.Show($"{results.Count} event(s) found.", "Search Results",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Resets the interface to show all available events again
        private void btnShowAll_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            recentlyViewed.Clear();
            lblRecommendations.Visible = false;
            DisplayEvents();
            btnShowAll.Visible = false;
        }

        // Generates event suggestions based on category, keyword, and user activity
        private void GenerateRecommendations(string keyword, List<Event> currentResults)
        {
            var allEvents = eventsByDate.SelectMany(kv => kv.Value).ToList();

            // Identify most common category in the current search results
            string? mainCategory = currentResults
                .GroupBy(e => e.Category)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            List<Event> related = new();


            // Recommend events from the same category
            if (!string.IsNullOrEmpty(mainCategory))
            {
                related = allEvents
                    .Where(e => !currentResults.Contains(e)
                             && e.Category.Equals(mainCategory, StringComparison.OrdinalIgnoreCase))
                    .Take(3)
                    .ToList();
            }

            // Include similar keyword matches to fill gaps
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

            // Add trending searches for a dynamic experience (GeeksforGeeks, 2025)
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

            // Build recommendation message for display
            string recText = related.Count == 0
                ? "You might also like:\nNo other related events found."
                : "You might also like:\n" +
                  string.Join("\n", related.Select(r => $"- {r.Title} ({r.Category}) on {r.Date:d}"));

            // Include up to 3 recently viewed events to personalise suggestions
            if (recentlyViewed.Count > 0)
            {
                var recent = recentlyViewed.Take(3)
                    .Select(r => $"- {r.Title} ({r.Category}) on {r.Date:d}");
                recText += "\n\nRecently viewed:\n" + string.Join("\n", recent);
            }

            lblRecommendations.Text = recText.Replace("\n", Environment.NewLine);
            lblRecommendations.Visible = true;
        }

        // When user clicks an event, mark it as recently viewed
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

        // Enables search suggestion when user focuses on the input box (GeeksforGeeks, 2025)
        private void TxtSearch_Enter(object sender, EventArgs e)
        {
            if (searchHistory.Count == 0) return;

            var autoComplete = new AutoCompleteStringCollection();
            autoComplete.AddRange(searchHistory.Reverse().ToArray());
            txtSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtSearch.AutoCompleteCustomSource = autoComplete;
        }

        // Save and reload user search behaviour to preserve continuity
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

        // Navigation button to return to main menu form (GeeksforGeeks, 2025)
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            var main = new MainMenuForm();
            main.ShowDialog();
            this.Close();
        }
    }
}

/*References
geeksforgeeks, 2025. SortedDictionary Implementation in C#. [Online] 
Available at: https://www.geeksforgeeks.org/c-sharp/sorteddictionary-implementation-in-c-sharp/
[Accessed 13 October 2025].
*/