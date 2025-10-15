namespace MunicipalServicesApp
{
    partial class EventsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel header;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnShowAll;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.DataGridView dgvEvents;
        private System.Windows.Forms.Label lblRecommendations;
        private System.Windows.Forms.Panel pnlRecommendations; // scrollable panel for recommendations

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // UI components  (Uizard, 2023)
            header = new Panel();
            lblHeader = new Label();
            txtSearch = new TextBox();
            btnSearch = new Button();
            btnShowAll = new Button();
            btnBack = new Button();
            dgvEvents = new DataGridView();
            lblRecommendations = new Label();
            pnlRecommendations = new Panel();

            // --- Form setup (Iannace, 2025) ---
            this.Text = "Local Events & Announcements";
            this.BackColor = Color.WhiteSmoke;
            this.ClientSize = new Size(920, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // --- Header ---
            header.Dock = DockStyle.Top;
            header.Height = 70;
            header.BackColor = Color.FromArgb(27, 94, 32);
            lblHeader.Text = "Municipal Services — Local Events & Announcements";
            lblHeader.ForeColor = Color.White;
            lblHeader.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            lblHeader.Dock = DockStyle.Fill;
            header.Controls.Add(lblHeader);

            // --- Search box ---
            txtSearch.Location = new Point(40, 90);
            txtSearch.Size = new Size(420, 32);
            txtSearch.Font = new Font("Segoe UI", 11);
            txtSearch.PlaceholderText = "Search by keyword, category, or date...";
            txtSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;

            // --- Search button ---
            btnSearch.Text = "Search";
            btnSearch.Font = new Font("Segoe UI Semibold", 11);
            btnSearch.BackColor = Color.FromArgb(46, 125, 50);
            btnSearch.ForeColor = Color.White;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.Size = new Size(100, 32);
            btnSearch.Location = new Point(470, 90);
            btnSearch.Click += btnSearch_Click;

            // --- Show All button (Iannace, 2025) ---
            btnShowAll.Text = "Show All Events";
            btnShowAll.Font = new Font("Segoe UI Semibold", 11);
            btnShowAll.BackColor = Color.FromArgb(27, 94, 32);
            btnShowAll.ForeColor = Color.White;
            btnShowAll.FlatStyle = FlatStyle.Flat;
            btnShowAll.Size = new Size(150, 32);
            btnShowAll.Location = new Point(580, 90);
            btnShowAll.Visible = false;
            btnShowAll.Click += btnShowAll_Click;

            // --- Back button moved next to Show All ---
            btnBack.Text = "Back to Main Menu";
            btnBack.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnBack.BackColor = Color.FromArgb(27, 94, 32);
            btnBack.ForeColor = Color.White;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Size = new Size(160, 32);
            btnBack.Location = new Point(740, 90);
            btnBack.Click += btnBack_Click;

            // --- Events table (Uizard, 2023) ---
            dgvEvents.Location = new Point(40, 140);
            dgvEvents.Size = new Size(860, 340);
            dgvEvents.AllowUserToAddRows = false;
            dgvEvents.ReadOnly = true;
            dgvEvents.RowHeadersVisible = false;
            dgvEvents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEvents.Columns.Add("Title", "Title");
            dgvEvents.Columns.Add("Date", "Date");
            dgvEvents.Columns.Add("Category", "Category");
            dgvEvents.Columns.Add("Description", "Description");

            // --- Recommendations Panel (scrollable with padding + shadow) ---
            pnlRecommendations.Location = new Point(40, 500);
            pnlRecommendations.Size = new Size(860, 120);
            pnlRecommendations.BackColor = Color.FromArgb(240, 248, 240); // soft green tint
            pnlRecommendations.BorderStyle = BorderStyle.FixedSingle;
            pnlRecommendations.Padding = new Padding(15);
            pnlRecommendations.AutoScroll = true;

            // Add subtle shadow using Paint event
            pnlRecommendations.Paint += (s, e) =>
            {
                using (var shadowBrush = new SolidBrush(Color.FromArgb(70, 0, 0, 0)))
                    e.Graphics.FillRectangle(shadowBrush, pnlRecommendations.Width - 6, 4, 6, pnlRecommendations.Height - 8);
            };

            // --- Recommendations Label (inside panel) ---
            lblRecommendations.Font = new Font("Segoe UI", 10.5f, FontStyle.Italic);
            lblRecommendations.ForeColor = Color.FromArgb(40, 60, 40);
            lblRecommendations.AutoSize = true;
            lblRecommendations.MaximumSize = new Size(820, 0); // wraps neatly
            lblRecommendations.TextAlign = ContentAlignment.TopLeft;
            lblRecommendations.Text = "Recommendations will appear here...";
            lblRecommendations.Location = new Point(10, 10);
            pnlRecommendations.Controls.Add(lblRecommendations);

            // --- Add controls ---
            Controls.AddRange(new Control[]
            {
                header, txtSearch, btnSearch, btnShowAll, btnBack, dgvEvents, pnlRecommendations
            });
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
