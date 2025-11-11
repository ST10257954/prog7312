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
        private System.Windows.Forms.Panel pnlRecommendations;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // Initialize components
            this.header = new System.Windows.Forms.Panel();
            this.lblHeader = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnShowAll = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.dgvEvents = new System.Windows.Forms.DataGridView();
            this.pnlRecommendations = new System.Windows.Forms.Panel();
            this.lblRecommendations = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.dgvEvents)).BeginInit();
            this.SuspendLayout();

            // --- Form properties ---
            this.Text = "Local Events & Announcements";
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(920, 640);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // --- Header ---
            this.header.Dock = System.Windows.Forms.DockStyle.Top;
            this.header.Height = 70;
            this.header.BackColor = System.Drawing.Color.FromArgb(27, 94, 32);
            this.lblHeader.Text = "Municipal Services — Local Events & Announcements";
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.header.Controls.Add(this.lblHeader);

            // --- Search box ---
            this.txtSearch.Location = new System.Drawing.Point(40, 90);
            this.txtSearch.Size = new System.Drawing.Size(420, 32);
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 11);
            this.txtSearch.PlaceholderText = "Search by keyword, category, or date...";
            this.txtSearch.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtSearch.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;

            // --- Search button ---
            this.btnSearch.Text = "Search";
            this.btnSearch.Font = new System.Drawing.Font("Segoe UI Semibold", 11);
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(46, 125, 50);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Size = new System.Drawing.Size(100, 32);
            this.btnSearch.Location = new System.Drawing.Point(470, 90);
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);

            // --- Show All button ---
            this.btnShowAll.Text = "Show All Events";
            this.btnShowAll.Font = new System.Drawing.Font("Segoe UI Semibold", 11);
            this.btnShowAll.BackColor = System.Drawing.Color.FromArgb(27, 94, 32);
            this.btnShowAll.ForeColor = System.Drawing.Color.White;
            this.btnShowAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowAll.Size = new System.Drawing.Size(150, 32);
            this.btnShowAll.Location = new System.Drawing.Point(580, 90);
            this.btnShowAll.Visible = false;
            this.btnShowAll.Click += new System.EventHandler(this.btnShowAll_Click);

            // --- Back button ---
            this.btnBack.Text = "Back to Main Menu";
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(27, 94, 32);
            this.btnBack.ForeColor = System.Drawing.Color.White;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Size = new System.Drawing.Size(160, 32);
            this.btnBack.Location = new System.Drawing.Point(740, 90);
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);

            // --- Events table ---
            this.dgvEvents.Location = new System.Drawing.Point(40, 140);
            this.dgvEvents.Size = new System.Drawing.Size(860, 310);
            this.dgvEvents.AllowUserToAddRows = false;
            this.dgvEvents.ReadOnly = true;
            this.dgvEvents.RowHeadersVisible = false;
            this.dgvEvents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEvents.Columns.Add("Title", "Title");
            this.dgvEvents.Columns.Add("Date", "Date");
            this.dgvEvents.Columns.Add("Category", "Category");
            this.dgvEvents.Columns.Add("Description", "Description");

            // --- Recommendations Panel ---
            this.pnlRecommendations.Location = new System.Drawing.Point(40, 470);
            this.pnlRecommendations.Size = new System.Drawing.Size(860, 120);
            this.pnlRecommendations.BackColor = System.Drawing.Color.FromArgb(240, 248, 240);
            this.pnlRecommendations.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlRecommendations.Padding = new System.Windows.Forms.Padding(15);
            this.pnlRecommendations.AutoScroll = true;

            this.pnlRecommendations.Paint += (s, e) =>
            {
                using (var shadowBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(70, 0, 0, 0)))
                    e.Graphics.FillRectangle(shadowBrush, pnlRecommendations.Width - 6, 4, 6, pnlRecommendations.Height - 8);
            };

            // --- Recommendations Label ---
            this.lblRecommendations.Font = new System.Drawing.Font("Segoe UI", 10.5f, System.Drawing.FontStyle.Italic);
            this.lblRecommendations.ForeColor = System.Drawing.Color.FromArgb(40, 60, 40);
            this.lblRecommendations.AutoSize = true;
            this.lblRecommendations.MaximumSize = new System.Drawing.Size(820, 0);
            this.lblRecommendations.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.lblRecommendations.Text = "Recommendations will appear here...";
            this.lblRecommendations.Location = new System.Drawing.Point(10, 10);
            this.lblRecommendations.Visible = false;
            this.pnlRecommendations.Controls.Add(this.lblRecommendations);

            // --- Add all controls to the form ---
            this.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.header,
                this.txtSearch,
                this.btnSearch,
                this.btnShowAll,
                this.btnBack,
                this.dgvEvents,
                this.pnlRecommendations
            });

            ((System.ComponentModel.ISupportInitialize)(this.dgvEvents)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
