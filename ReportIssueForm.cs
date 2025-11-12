using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Data;

namespace MunicipalServicesApp
{

    /*
    ReportIssueForm — allows residents to submit new municipal service requests.
    This form collects details such as location, category, area, and attachments,
    and supports offline or data-saving modes for accessibility (Microsoft, 2025).
    */

    public partial class ReportIssueForm : Form
    {

        /*These static Color fields define a consistent visual identity for the form.
           Using constants ensures maintainable styling and consistent contrast ratios.*/
        private static readonly Color Accent = Color.FromArgb(46, 125, 50);
        private static readonly Color AccentDark = Color.FromArgb(27, 94, 32);
        private static readonly Color Surface = Color.White;
        private static readonly Color SurfaceAlt = Color.FromArgb(246, 248, 250);
        private static readonly Color BorderColor = Color.FromArgb(220, 223, 226);
        private static readonly Color Muted = Color.FromArgb(108, 117, 125);


        /*Declare all UI components used within the report submission form.
           Grouping them here simplifies initialization and later styling changes.*/
        private TextBox txtLocation;
        private ComboBox cmbCategory;
        private ComboBox cmbArea;
        private RichTextBox rtbDescription;
        private Button btnAddAttachment, btnSubmit, btnCopySms, btnExport, btnBack;
        private ListBox lstAttachments;
        private CheckBox chkDataSaver, chkOffline;
        private Label lblEstimate, lblEngagement;
        private ProgressBar progress;
        private StatusStrip status;
        private ToolStripStatusLabel statusMode, statusDataSaver;

        /*Constructor
         Initializes the form and triggers the UI builder method.
         Separating layout construction into BuildUi() improves readability and aligns with the Single-Responsibility Principle (Microsoft, 2025).*/
        public ReportIssueForm()
        {
            BuildUi(); // dynamically builds and styles all controls when the form is created
        }

        private void InitializeComponent() { }


        /*This method programmatically creates the interface for reporting a municipal issue.
          It uses layout containers (TableLayoutPanel, FlowLayoutPanel) to maintain consistent spacing and alignment.
         */

        private void BuildUi()
        {
            /*
             BASIC FORM SETTINGS
             Defines window size, title, style, and general behavior.
             Using fixed sizing prevents layout shifting and enforces a
             consistent user experience across display resolutions.
             */

            Text = "Report an Issue";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(1060, 640);
            BackColor = SurfaceAlt;
            Font = new Font("Segoe UI", 10.5f);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // -----------------------------------------------------------
            // MAIN LAYOUT CONTAINER
            // -----------------------------------------------------------
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(20, 16, 20, 16)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));     // Header
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));     // Content
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));     // Footer buttons
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));     // Status bar
            Controls.Add(root);


            /*Visually identifies the form and app brand. The line below the
              header provides a subtle separation from the main content.*/
            var header = new Panel { BackColor = AccentDark, Dock = DockStyle.Fill };
            header.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(200, 200, 200));
                e.Graphics.DrawLine(pen, 0, header.Height - 1, header.Width, header.Height - 1);
            };

            var headerLbl = new Label
            {
                Text = "Municipal Services — Report an Issue",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(18, 0, 18, 0)
            };
            header.Controls.Add(headerLbl);
            root.Controls.Add(header, 0, 0);

            /*
            The modular structure improves code reuse and visual consistency.
            */
            var content = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 60));  // Issue details + engagement
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 40));  // Attachments
            root.Controls.Add(content, 0, 1);

            // Top Card (Issue Details + Engagement)
            var cardTop = MakeCard();
            content.Controls.Add(cardTop, 0, 0);

            var topGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2
            };
            topGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            topGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            cardTop.Controls.Add(topGrid);

            /*
             Contains textboxes and dropdowns for structured issue reporting.
             Events are linked to UpdateEngagement() and UpdateEstimate() so feedback adjusts dynamically based on user input.
             */
            var details = MakeCard(innerPadding: 16);
            topGrid.Controls.Add(details, 0, 0);

            var detailsGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(6)
            };
            detailsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            detailsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            details.Controls.Add(detailsGrid);

            detailsGrid.Controls.Add(MakeSectionTitle("Issue details"), 0, 0);
            detailsGrid.SetColumnSpan(detailsGrid.GetControlFromPosition(0, 0), 2);

            // Address field — essential for location-based routing
            detailsGrid.Controls.Add(MakeLabel("Address:"), 0, 1);
            txtLocation = new TextBox { Dock = DockStyle.Fill };
            txtLocation.TextChanged += (_, __) => { UpdateEngagement(); UpdateEstimate(); };
            detailsGrid.Controls.Add(txtLocation, 1, 1);

            // Ward/area selection
            detailsGrid.Controls.Add(MakeLabel("Service Area:"), 0, 2);
            cmbArea = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbArea.Items.AddRange(new[] { "Ward A", "Ward B", "Ward C", "Ward D" });
            cmbArea.SelectedIndex = 0;
            detailsGrid.Controls.Add(cmbArea, 1, 2);

            // Service category selection
            detailsGrid.Controls.Add(MakeLabel("Category:"), 0, 3);
            cmbCategory = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategory.DataSource = Enum.GetValues(typeof(IssueCategory));
            cmbCategory.SelectedIndexChanged += (_, __) => { UpdateEngagement(); UpdateEstimate(); };
            detailsGrid.Controls.Add(cmbCategory, 1, 3);

            // Description input — allows contextual detail for technicians
            detailsGrid.Controls.Add(MakeLabel("Description:"), 0, 4);
            rtbDescription = new RichTextBox { Dock = DockStyle.Fill };
            rtbDescription.TextChanged += (_, __) => { UpdateEngagement(); UpdateEstimate(); };
            detailsGrid.Controls.Add(rtbDescription, 1, 4);

            /*
             Includes offline and data-saver checkboxes, progress bar, and guidance text to keep the user informed (Microsoft, 2025).
             */
            var modes = MakeCard(innerPadding: 16);
            topGrid.Controls.Add(modes, 1, 0);

            var modesGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                Padding = new Padding(6)
            };
            modes.Controls.Add(modesGrid);

            modesGrid.Controls.Add(MakeSectionTitle("Modes & Engagement"), 0, 0);

            // Toggles for data-saver and offline queueing
            var toggleRow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };
            chkDataSaver = new CheckBox { Text = "Data saver", Checked = true, AutoSize = true };
            chkOffline = new CheckBox { Text = "Work offline", AutoSize = true };
            chkDataSaver.CheckedChanged += (_, __) => { ToggleForDataSaver(); UpdateEstimate(); UpdateStatusBar(); };
            chkOffline.CheckedChanged += (_, __) => { UpdateStatusBar(); };
            toggleRow.Controls.Add(chkDataSaver);
            toggleRow.Controls.Add(new Label { Text = "   " });
            toggleRow.Controls.Add(chkOffline);
            modesGrid.Controls.Add(toggleRow, 0, 1);

            lblEstimate = new Label { Text = "Estimated upload size: ~1 KB", Dock = DockStyle.Fill, ForeColor = Muted };
            modesGrid.Controls.Add(lblEstimate, 0, 2);

            lblEngagement = new Label
            {
                Text = "Step 1: Add your location to help teams find the issue.",
                Dock = DockStyle.Fill
            };
            modesGrid.Controls.Add(lblEngagement, 0, 4);

            progress = new ProgressBar
            {
                Dock = DockStyle.Fill,
                Minimum = 0,
                Maximum = 100,
                Style = ProgressBarStyle.Continuous
            };
            modesGrid.Controls.Add(progress, 0, 5);

            //users can upload supporting images or documents, organized with a button and list to improve usability.
                 var cardBottom = MakeCard();
            content.Controls.Add(cardBottom, 0, 1);

            var attachGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(16)
            };
            attachGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            attachGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            cardBottom.Controls.Add(attachGrid);

            attachGrid.Controls.Add(MakeSectionTitle("Attachments"), 0, 0);
            attachGrid.SetColumnSpan(attachGrid.GetControlFromPosition(0, 0), 2);

            btnAddAttachment = new Button
            {
                Text = "Add attachment(s)",
                Dock = DockStyle.Fill,
                BackColor = Surface,
                FlatStyle = FlatStyle.Flat
            };
            btnAddAttachment.FlatAppearance.BorderColor = BorderColor;
            btnAddAttachment.FlatAppearance.BorderSize = 1;
            btnAddAttachment.Click += BtnAddAttachment_Click;
            attachGrid.Controls.Add(btnAddAttachment, 0, 1);

            lstAttachments = new ListBox { Dock = DockStyle.Fill };
            attachGrid.Controls.Add(lstAttachments, 1, 1);
            attachGrid.SetRowSpan(lstAttachments, 2);

            //Provides main navigation and submission controls, positioned right-aligned for ergonomic access.
            var footer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 8, 0, 0)
            };
            root.Controls.Add(footer, 0, 2);

            btnBack = Ghost("Back to Main Menu");
            btnBack.Click += (_, __) =>
            {
                this.Hide();
                var main = new MainMenuForm();
                main.ShowDialog();
                this.Close();
            };

            btnExport = Ghost("Export Pending");
            btnCopySms = Ghost("Copy SMS Text");

            btnSubmit = Primary("Submit");
            btnSubmit.Click += BtnSubmit_Click;

            footer.Controls.AddRange(new Control[] { btnBack, btnExport, btnCopySms, btnSubmit });

            //Displays current operational mode and data-saving status, helps users confirm whether they are working online/offline .
            status = new StatusStrip { SizingGrip = false, BackColor = Surface };
            statusMode = new ToolStripStatusLabel("Online");
            statusDataSaver = new ToolStripStatusLabel("Data saver: On") { ForeColor = Muted };
            status.Items.Add(statusMode);
            status.Items.Add(new ToolStripStatusLabel(" | ") { ForeColor = BorderColor });
            status.Items.Add(statusDataSaver);
            root.Controls.Add(status, 0, 3);

            //These handlers reset background colors when a field is corrected, helping users clearly see validation feedback (Polgár, 2024).
            txtLocation.TextChanged += (_, __) =>
            {
                if (!string.IsNullOrWhiteSpace(txtLocation.Text))
                    txtLocation.BackColor = Color.White;
            };

            cmbCategory.SelectedIndexChanged += (_, __) =>
            {
                if (cmbCategory.SelectedIndex != -1)
                    cmbCategory.BackColor = Color.White;
            };

            rtbDescription.TextChanged += (_, __) =>
            {
                if (!string.IsNullOrWhiteSpace(rtbDescription.Text))
                    rtbDescription.BackColor = Color.White;
            };

            // Defines form-wide defaults and triggers initial UI refresh (Polgár, 2024). 

            AcceptButton = btnSubmit;  // Pressing Enter triggers Submit
            ToggleForDataSaver();
            UpdateEngagement();
            UpdateEstimate();
            UpdateStatusBar();
        }

        // Creates a card-style container to group related form sections
        private Panel MakeCard(int innerPadding = 0) =>
            new Panel
            {
                BackColor = Surface,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                Padding = new Padding(innerPadding)
            };

        // Creates bold section headers for form clarity
        private static Label MakeSectionTitle(string text) =>
            new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.Black,
                TextAlign = ContentAlignment.MiddleLeft
            };

        // Creates standard labels for input fields
        private static Label MakeLabel(string text) =>
            new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Black
            };

        // Primary button – used for main actions like “Submit”
        private Button Primary(string text) =>
            new Button
            {
                Text = text,
                AutoSize = true,
                BackColor = Accent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold)
            };

        // Ghost button – used for secondary actions (e.g., “Back” or “Export”)
        private Button Ghost(string text) =>
            new Button
            {
                Text = text,
                AutoSize = true,
                BackColor = Surface,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10.5f)
            };


        // Toggles Data Saver mode to disable or clear attachments
        private void ToggleForDataSaver()
        {
            btnAddAttachment.Enabled = !chkDataSaver.Checked;
            if (chkDataSaver.Checked)
            {
                lstAttachments.Items.Clear(); // removes files to conserve data
                lblEngagement.Text = "Attachments disabled in Data Saver Mode.";
            }
        }

        // Updates the status bar to reflect current mode (online/offline, data saver)
        private void UpdateStatusBar()
        {
            statusMode.Text = chkOffline.Checked ? "Offline (queued)" : "Online";
            statusDataSaver.Text = $"Data saver: {(chkDataSaver.Checked ? "On" : "Off")}";
        }

        // Tracks form completion and gives user step-by-step feedback (Polgár, 2024)
        private void UpdateEngagement()
        {
            int p = 0;

            // Calculate completion percentage
            if (!string.IsNullOrWhiteSpace(txtLocation.Text)) p += 25;
            if (cmbCategory.SelectedItem != null) p += 25;
            if (!string.IsNullOrWhiteSpace(rtbDescription.Text)) p += 25;
            if (lstAttachments.Items.Count > 0 && btnAddAttachment.Enabled) p += 25;

            progress.Value = p;

            // Display guidance message based on what’s missing
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                lblEngagement.Text = "Step 1: Add your location to help teams find the issue.";
            }
            else if (cmbCategory.SelectedItem == null)
            {
                lblEngagement.Text = "Step 2: Choose a category that best describes the issue.";
            }
            else if (string.IsNullOrWhiteSpace(rtbDescription.Text))
            {
                lblEngagement.Text = "Step 3: Add a short description of the issue.";
            }
            else if (lstAttachments.Items.Count == 0 && btnAddAttachment.Enabled)
            {
                lblEngagement.Text = "Step 4 (optional): Add photo attachments for better detail.";
            }
            else
            {
                lblEngagement.Text = "All set! Press Submit to generate your ticket.";
            }
        }

        // Updates the estimated data usage for uploading attachments
        private void UpdateEstimate()
        {
            int approxKb = 1 + lstAttachments.Items.Count * 2;
            if (chkDataSaver.Checked) approxKb /= 2;
            lblEstimate.Text = $"Estimated upload size: ~{approxKb} KB";
        }


        // Handles adding file attachments when the user clicks “Add Attachment(s)”
        private void BtnAddAttachment_Click(object sender, EventArgs e)
        {

            // Prevent adding files if Data Saver mode is active
            if (chkDataSaver.Checked)
            {
                MessageBox.Show("Attachments are disabled in Data Saver mode.", "Data Saver Active",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            // Allow user to select multiple files from local storage
            using var ofd = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Select attachment(s)",
                Filter = "All files|*.*"
            };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;


            // Display selected file names in the attachments list
            foreach (var path in ofd.FileNames)
                lstAttachments.Items.Add(Path.GetFileName(path));


            // Recalculate engagement progress and upload estimate
            UpdateEngagement();
            UpdateEstimate();
        }


        // Handles the Submit button click — validates input, saves issue, and resets the form (Polgár, 2024)
        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            // Highlight missing fields to ensure complete issue reports
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                txtLocation.BackColor = Color.MistyRose;
                MessageBox.Show("Please enter the location.", "Missing information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLocation.Focus();
                return;
            }

            if (cmbCategory.SelectedIndex == -1)
            {
                cmbCategory.BackColor = Color.MistyRose;
                MessageBox.Show("Please select a category for the issue.",
                    "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCategory.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(rtbDescription.Text))
            {
                rtbDescription.BackColor = Color.MistyRose;
                MessageBox.Show("Please enter a short description of the issue.",
                    "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rtbDescription.Focus();
                return;
            }

            // Build a new Issue record with all user inputs (Polgár, 2024)
            var issue = new Issue
            {
                TicketNumber = $"MS-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}",
                Location = txtLocation.Text.Trim(),
                Area = cmbArea.SelectedItem.ToString(),
                Category = (IssueCategory)cmbCategory.SelectedItem,
                Description = rtbDescription.Text.Trim(),
                Channel = chkOffline.Checked ? "Offline-Queued" : (chkDataSaver.Checked ? "LowData" : "DesktopApp"),
                CreatedAt = DateTime.Now
            };

            // Save issue to repository for persistence
            IssueRepository.AddIssue(issue);

            // Confirm submission and show ticket details to the user
            MessageBox.Show(
                $"Submitted!\nTicket: {issue.TicketNumber}\nCategory: {issue.Category}\nLocation: {issue.Location}",
                "Report Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Reset all fields to prepare for a new entry
            ClearForm();
        }



        // Clears form fields after successful submission to avoid duplicate entries
        private void ClearForm()
        {
            txtLocation.Clear();
            cmbArea.SelectedIndex = 0;
            cmbCategory.SelectedIndex = -1;
            rtbDescription.Clear();
            lstAttachments.Items.Clear();
            progress.Value = 0;
            lblEngagement.Text = "Step 1: Add your location to help teams find the issue.";
            lblEstimate.Text = "Estimated upload size: ~1 KB";
        }
    }
}

/*
 References:
Polgár, T., 2024. Form validation in Windows Forms with C#. [Online] 
Available at: https://medium.com/developer-rants/form-validation-in-windows-forms-with-c-b0b07284d962
[Accessed 28 October 2025].
 */