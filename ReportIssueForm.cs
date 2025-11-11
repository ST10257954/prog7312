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
    public partial class ReportIssueForm : Form
    {
        // Theme
        private static readonly Color Accent = Color.FromArgb(46, 125, 50);
        private static readonly Color AccentDark = Color.FromArgb(27, 94, 32);
        private static readonly Color Surface = Color.White;
        private static readonly Color SurfaceAlt = Color.FromArgb(246, 248, 250);
        private static readonly Color BorderColor = Color.FromArgb(220, 223, 226);
        private static readonly Color Muted = Color.FromArgb(108, 117, 125);

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

        public ReportIssueForm()
        {
            BuildUi();
        }

        private void InitializeComponent() { }

        private void BuildUi()
        {
            Text = "Report an Issue";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(1060, 640);
            BackColor = SurfaceAlt;
            Font = new Font("Segoe UI", 10.5f);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(20, 16, 20, 16)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            Controls.Add(root);

            // Header
            var header = new Panel { BackColor = AccentDark, Dock = DockStyle.Fill };
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

            // Main content
            var content = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            root.Controls.Add(content, 0, 1);

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

            // LEFT — Issue details
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

            detailsGrid.Controls.Add(MakeLabel("Address:"), 0, 1);
            txtLocation = new TextBox { Dock = DockStyle.Fill };
            txtLocation.TextChanged += (_, __) => { UpdateEngagement(); UpdateEstimate(); };
            detailsGrid.Controls.Add(txtLocation, 1, 1);

            detailsGrid.Controls.Add(MakeLabel("Service Area:"), 0, 2);
            cmbArea = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbArea.Items.AddRange(new[] { "Ward A", "Ward B", "Ward C", "Ward D" });
            cmbArea.SelectedIndex = 0;
            detailsGrid.Controls.Add(cmbArea, 1, 2);

            detailsGrid.Controls.Add(MakeLabel("Category:"), 0, 3);
            cmbCategory = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategory.DataSource = Enum.GetValues(typeof(IssueCategory));
            cmbCategory.SelectedIndexChanged += (_, __) => { UpdateEngagement(); UpdateEstimate(); };
            detailsGrid.Controls.Add(cmbCategory, 1, 3);

            detailsGrid.Controls.Add(MakeLabel("Description:"), 0, 4);
            rtbDescription = new RichTextBox { Dock = DockStyle.Fill };
            rtbDescription.TextChanged += (_, __) => { UpdateEngagement(); UpdateEstimate(); };
            detailsGrid.Controls.Add(rtbDescription, 1, 4);

            // RIGHT — Modes & Engagement
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

            // BOTTOM — Attachments
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

            // Footer
            var footer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 8, 0, 0)
            };
            root.Controls.Add(footer, 0, 2);

            btnBack = Ghost("Back");
            btnBack.Click += (_, __) => Close();

            btnExport = Ghost("Export Pending");
            btnCopySms = Ghost("Copy SMS Text");

            btnSubmit = Primary("Submit");
            btnSubmit.Click += BtnSubmit_Click;

            footer.Controls.AddRange(new Control[] { btnBack, btnExport, btnCopySms, btnSubmit });

            // Status bar
            status = new StatusStrip { SizingGrip = false, BackColor = Surface };
            statusMode = new ToolStripStatusLabel("Online");
            statusDataSaver = new ToolStripStatusLabel("Data saver: On") { ForeColor = Muted };
            status.Items.Add(statusMode);
            status.Items.Add(new ToolStripStatusLabel(" | ") { ForeColor = BorderColor });
            status.Items.Add(statusDataSaver);
            root.Controls.Add(status, 0, 3);

            // Initial setup
            ToggleForDataSaver();
            UpdateEngagement();
            UpdateEstimate();
            UpdateStatusBar();
        }

        // Helpers -------------------------------------------------------------
        private Panel MakeCard(int innerPadding = 0) =>
            new Panel { BackColor = Surface, BorderStyle = BorderStyle.FixedSingle, Dock = DockStyle.Fill, Padding = new Padding(innerPadding) };

        private static Label MakeSectionTitle(string text) =>
            new Label { Text = text, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10.5f, FontStyle.Bold), ForeColor = Color.Black, TextAlign = ContentAlignment.MiddleLeft };

        private static Label MakeLabel(string text) =>
            new Label { Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.Black };

        private Button Primary(string text) =>
            new Button { Text = text, AutoSize = true, BackColor = Accent, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10.5f, FontStyle.Bold) };

        private Button Ghost(string text) =>
            new Button { Text = text, AutoSize = true, BackColor = Surface, ForeColor = Color.Black, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10.5f) };

        // Behaviour -----------------------------------------------------------
        private void ToggleForDataSaver()
        {
            btnAddAttachment.Enabled = !chkDataSaver.Checked;
            if (chkDataSaver.Checked)
            {
                lstAttachments.Items.Clear();
                lblEngagement.Text = "Attachments disabled in Data Saver Mode.";
            }
        }

        private void UpdateStatusBar()
        {
            statusMode.Text = chkOffline.Checked ? "Offline (queued)" : "Online";
            statusDataSaver.Text = $"Data saver: {(chkDataSaver.Checked ? "On" : "Off")}";
        }

        private void UpdateEngagement()
        {
            int p = 0;
            if (!string.IsNullOrWhiteSpace(txtLocation.Text)) p += 25;
            if (cmbCategory.SelectedItem != null) p += 25;
            if (!string.IsNullOrWhiteSpace(rtbDescription.Text)) p += 25;
            if (lstAttachments.Items.Count > 0 && btnAddAttachment.Enabled) p += 25;

            progress.Value = p;
            lblEngagement.Text =
                p < 25 ? "Step 1: Add your location to help teams find the issue." :
                p < 50 ? "Good! Now choose a category." :
                p < 75 ? "Almost done. Add a short description." :
                p < 100 ? "Optional: Add photos or documents to help staff." :
                          "All set! Press Submit to generate your ticket.";
        }

        private void UpdateEstimate()
        {
            int approxKb = 1 + lstAttachments.Items.Count * 2;
            if (chkDataSaver.Checked) approxKb /= 2;
            lblEstimate.Text = $"Estimated upload size: ~{approxKb} KB";
        }

        // Event Handlers ------------------------------------------------------
        private void BtnAddAttachment_Click(object sender, EventArgs e)
        {
            if (chkDataSaver.Checked)
            {
                MessageBox.Show("Attachments are disabled in Data Saver mode.", "Data Saver Active",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var ofd = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Select attachment(s)",
                Filter = "All files|*.*"
            };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            foreach (var path in ofd.FileNames)
                lstAttachments.Items.Add(Path.GetFileName(path));

            UpdateEngagement();
            UpdateEstimate();
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                MessageBox.Show("Please enter the location.", "Missing information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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

            IssueRepository.Issues.Add(issue);

            MessageBox.Show($"Submitted!\nTicket: {issue.TicketNumber}\nCategory: {issue.Category}\nLocation: {issue.Location}",
                "Report Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ClearForm();
        }

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
