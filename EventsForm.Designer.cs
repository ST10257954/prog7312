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
        private System.Windows.Forms.DataGridView dgvEvents;
        private System.Windows.Forms.Label lblRecommendations;
        private System.Windows.Forms.Panel pnlRecommendations; 
        private System.Windows.Forms.Button btnBack;

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
            dgvEvents = new DataGridView();
            lblRecommendations = new Label();
            pnlRecommendations = new Panel(); // scroll panel
            btnBack = new Button();

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
            txtSearch.Size = new Size(480, 32);
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
            btnSearch.Location = new Point(530, 90);
            btnSearch.Click += btnSearch_Click;

            // --- Show All button (Iannace, 2025) ---
            btnShowAll.Text = "Show All Events";
            btnShowAll.Font = new Font("Segoe UI Semibold", 11);
            btnShowAll.BackColor = Color.FromArgb(27, 94, 32);
            btnShowAll.ForeColor = Color.White;
            btnShowAll.FlatStyle = FlatStyle.Flat;
            btnShowAll.Size = new Size(150, 32);
            btnShowAll.Location = new Point(640, 90);
            btnShowAll.Visible = false;
            btnShowAll.Click += btnShowAll_Click;

            // --- Events table (Uizard, 2023) ---
            dgvEvents.Location = new Point(40, 140);
            dgvEvents.Size = new Size(800, 350);
            dgvEvents.AllowUserToAddRows = false;
            dgvEvents.ReadOnly = true;
            dgvEvents.RowHeadersVisible = false;
            dgvEvents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEvents.Columns.Add("Title", "Title");
            dgvEvents.Columns.Add("Date", "Date");
            dgvEvents.Columns.Add("Category", "Category");
            dgvEvents.Columns.Add("Description", "Description");

            // --- Recommendations Panel (scrollable for long text) ---
            pnlRecommendations.Location = new Point(40, 510);
            pnlRecommendations.Size = new Size(800, 100);
            pnlRecommendations.BorderStyle = BorderStyle.None;
            pnlRecommendations.AutoScroll = true;
            pnlRecommendations.BackColor = Color.White; // matches form theme

            // --- Recommendations Label (inside the scroll panel) ---
            lblRecommendations.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            lblRecommendations.ForeColor = Color.FromArgb(60, 60, 60);
            lblRecommendations.Location = new Point(0, 0);
            lblRecommendations.AutoSize = true;
            lblRecommendations.MaximumSize = new Size(760, 0); // wrap text nicely
            lblRecommendations.Text = "Recommendations will appear here...";
            pnlRecommendations.Controls.Add(lblRecommendations);

            // --- Back button (Iannace, 2025) ---
            btnBack.Text = "Back to Main Menu";
            btnBack.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnBack.BackColor = Color.FromArgb(46, 125, 50);
            btnBack.ForeColor = Color.White;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Size = new Size(160, 32);
            btnBack.Location = new Point(720, 580);
            btnBack.Click += btnBack_Click;

            // --- Add controls ---
            Controls.AddRange(new Control[]
            {
                header, txtSearch, btnSearch, btnShowAll, dgvEvents, pnlRecommendations, btnBack
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
