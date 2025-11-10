using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Data;

namespace MunicipalServicesApp
{
    public partial class ServiceRequestStatusForm : Form
    {
        private TextBox txtSearch;
        private Button btnRefresh;
        private Button btnBack;
        private Button btnUrgent;
        private Button btnGraphDemo;
        private Button btnBackToDetails;
        private TreeView tvRequests;
        private Label lblNextUrgent;
        private Label lblHeaderTitle;
        private Label lblDetailsTitle;
        private Label lblDetailsBody;
        private FlowLayoutPanel attachmentPanel;

        private Panel rightCard;
        private Panel graphPanel;

        private List<Issue> allIssues = new();
        private ServiceRequestBST bst = new();
        private MinHeap priorityHeap = new();

        public ServiceRequestStatusForm()
        {
            InitializeComponent();
            BuildUI();
        }

        private void InitializeComponent() { }

        private void BuildUI()
        {
            Text = "Service Request Status";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1080, 630);
            BackColor = ThemeManager.BackgroundLight;
            Font = new Font("Segoe UI", 10f);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                Padding = new Padding(15, 10, 15, 10),
                BackColor = ThemeManager.BackgroundLight
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            Controls.Add(root);

            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = ThemeManager.BackgroundLight,
                Padding = new Padding(5),
                AutoScroll = false,
                WrapContents = false
            };
            root.Controls.Add(toolbar, 0, 0);

            txtSearch = new TextBox
            {
                PlaceholderText = "Search by ticket, category, location or description...",
                Width = 400,
                Margin = new Padding(6, 10, 10, 10)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            toolbar.Controls.Add(txtSearch);

            btnRefresh = CreateToolbarButton("Refresh", ThemeManager.EmeraldMid);
            btnBack = CreateToolbarButton("Back", ThemeManager.MutedGrey);
            btnUrgent = CreateToolbarButton("Show Next Urgent", ThemeManager.EmeraldDark);
            btnGraphDemo = CreateToolbarButton("Route Optimiser", ThemeManager.EmeraldMid);
            btnBackToDetails = CreateToolbarButton("Back to Details", ThemeManager.MutedGrey);
            btnBackToDetails.Visible = false;

            btnRefresh.Click += BtnRefresh_Click;
            btnBack.Click += (_, __) => Close();
            btnUrgent.Click += BtnUrgent_Click;
            btnGraphDemo.Click += BtnGraphDemo_Click;
            btnBackToDetails.Click += BtnBackToDetails_Click;

            toolbar.Controls.AddRange(new Control[]
                { btnRefresh, btnBack, btnUrgent, btnGraphDemo, btnBackToDetails });

            var mainSplit = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = ThemeManager.BackgroundLight,
                Margin = new Padding(0)
            };
            mainSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            mainSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            root.Controls.Add(mainSplit, 0, 1);

            var leftCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeManager.CardWhite,
                Padding = new Padding(10),
                Margin = new Padding(0, 0, 8, 0)
            };
            leftCard.Paint += (s, e) =>
                ThemeManager.DrawCardShadow(e.Graphics, leftCard.ClientRectangle);

            tvRequests = new TreeView
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            tvRequests.AfterSelect += TvRequests_AfterSelect;
            leftCard.Controls.Add(tvRequests);
            mainSplit.Controls.Add(leftCard, 0, 0);

            rightCard = new Panel
            {
                Name = "rightCard",
                Dock = DockStyle.Fill,
                BackColor = ThemeManager.CardWhite,
                Margin = new Padding(8, 0, 0, 0),
                Padding = new Padding(0)
            };
            rightCard.Paint += (s, e) =>
                ThemeManager.DrawCardShadow(e.Graphics, rightCard.ClientRectangle);
            mainSplit.Controls.Add(rightCard, 1, 0);

            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20, 18, 20, 18)
            };

            var detailsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                BackColor = Color.White
            };
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            contentPanel.Controls.Add(detailsLayout);

            lblDetailsTitle = new Label
            {
                Text = "Select a service request to view details",
                Font = new Font("Segoe UI Semibold", 13f, FontStyle.Bold),
                ForeColor = ThemeManager.EmeraldDark,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            detailsLayout.Controls.Add(lblDetailsTitle, 0, 0);

            lblDetailsBody = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10.5f),
                ForeColor = ThemeManager.TextDark,
                TextAlign = ContentAlignment.TopLeft,
                Padding = new Padding(6, 4, 6, 4),
                BackColor = Color.White
            };
            detailsLayout.Controls.Add(lblDetailsBody, 0, 1);

            attachmentPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 100,
                AutoScroll = true,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10),
                Visible = false
            };
            rightCard.Controls.Add(attachmentPanel);

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 45,
                BackColor = ThemeManager.EmeraldMid,
                Padding = new Padding(15, 0, 0, 0)
            };
            lblHeaderTitle = new Label
            {
                Text = "Request Details",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 11f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            headerPanel.Controls.Add(lblHeaderTitle);

            rightCard.Controls.Add(contentPanel);
            rightCard.Controls.Add(headerPanel);

            graphPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Visible = false
            };
            mainSplit.Controls.Add(graphPanel, 1, 0);

            lblNextUrgent = new Label
            {
                Text = "Next urgent request: (none)",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Italic),
                ForeColor = ThemeManager.TextDark,
                TextAlign = ContentAlignment.MiddleLeft
            };
            root.Controls.Add(lblNextUrgent, 0, 2);

            ReloadDataAndBuildTree();
            BuildPriorityHeap();
        }

        private Button CreateToolbarButton(string text, Color color)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(150, 35),
                BackColor = color,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Margin = new Padding(5, 8, 5, 8)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(
                Math.Min(color.R + 20, 255),
                Math.Min(color.G + 20, 255),
                Math.Min(color.B + 20, 255));
            return btn;
        }

        private void BtnGraphDemo_Click(object sender, EventArgs e)
        {
            if (allIssues == null || allIssues.Count == 0)
            {
                MessageBox.Show("No service requests found. Please refresh the list first.",
                    "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get distinct areas from the current issues
            var availableAreas = allIssues.Select(i => i.Area).Distinct().ToArray();
            string selectedArea = ShowAreaSelectionDialog(availableAreas);
            if (string.IsNullOrEmpty(selectedArea)) return;

            // Filter issues for that area, sorted by urgency
            var filteredIssues = allIssues
                .Where(i => i.Area == selectedArea)
                .OrderBy(i => i.Priority)
                .ToList();

            if (filteredIssues.Count == 0)
            {
                MessageBox.Show($"No service requests found for {selectedArea}.",
                    "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Display the graph panel
            rightCard.Visible = false;
            graphPanel.Visible = true;
            btnGraphDemo.Visible = false;
            btnBackToDetails.Visible = true;

            // Pass the filtered issues and area into the ServiceGraph
            var logic = new ServiceGraph();
            logic.DemoGraphTraversal(filteredIssues, selectedArea);

            graphPanel.Paint += (s, ev) =>
            {
                logic.DrawGraph(ev);
            };
            graphPanel.Refresh();
        }

        private void BtnBackToDetails_Click(object sender, EventArgs e)
        {
            graphPanel.Visible = false;
            rightCard.Visible = true;
            btnBackToDetails.Visible = false;
            btnGraphDemo.Visible = true;
        }

        private void BuildPriorityHeap()
        {
            foreach (var issue in allIssues)
            {
                switch (issue.Category)
                {
                    case IssueCategory.Water:
                        issue.Priority = 1;
                        break;
                    case IssueCategory.Electricity:
                        issue.Priority = 2;
                        break;
                    case IssueCategory.Sanitation:
                    case IssueCategory.Roads:
                        issue.Priority = 3;
                        break;
                    case IssueCategory.SolidWaste:
                        issue.Priority = 4;
                        break;
                    default:
                        issue.Priority = 5;
                        break;
                }
            }

            priorityHeap = new MinHeap();
            priorityHeap.BuildHeap(allIssues);
        }

        private void BtnUrgent_Click(object sender, EventArgs e)
        {
            if (priorityHeap == null)
            {
                BuildPriorityHeap();
            }

            if (priorityHeap == null || allIssues == null || allIssues.Count == 0)
            {
                MessageBox.Show("No service requests found. Please refresh the list first.",
                    "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);

                lblNextUrgent.Text = "Next urgent request: (none)";
                lblDetailsTitle.Text = "Request Details";
                lblDetailsBody.Text = "No urgent requests available.";
                if (attachmentPanel != null) attachmentPanel.Visible = false;
                return;
            }

            var urgent = priorityHeap.ExtractMin();

            if (urgent == null)
            {
                MessageBox.Show("No urgent requests remaining in the queue.",
                    "Queue Empty", MessageBoxButtons.OK, MessageBoxIcon.Information);

                lblNextUrgent.Text = "Next urgent request: (none)";
                lblDetailsTitle.Text = "Request Details";
                lblDetailsBody.Text = "No urgent requests available.";
                if (attachmentPanel != null) attachmentPanel.Visible = false;
                return;
            }

            lblNextUrgent.Text = $"Next urgent request: {urgent.TicketNumber} ({urgent.Category})";
            lblDetailsTitle.Text = $"Next Urgent Request — {urgent.TicketNumber}";
            lblDetailsBody.Text =
                $"Category: {urgent.Category}\n" +
                $"Location: {urgent.Location}\n" +
                $"Description:\n{urgent.Description}\n\n" +
                $"Channel: {urgent.Channel}\n" +
                $"Created: {urgent.CreatedAt:g}";

            if (attachmentPanel != null)
            {
                if (urgent.AttachmentPaths == null)
                    urgent.AttachmentPaths = new List<string>();

                attachmentPanel.Visible = urgent.AttachmentPaths.Count > 0;
            }
        }

        private void ReloadDataAndBuildTree()
        {
            allIssues = IssueRepository.Issues.ToList();
            bst = new ServiceRequestBST();
            foreach (var issue in allIssues)
                bst.Insert(issue);
            var sorted = bst.GetInOrderList();
            RebuildTreeView(sorted, true);
        }

        private void RebuildTreeView(IEnumerable<Issue> issues, bool groupByCategory)
        {
            tvRequests.BeginUpdate();
            tvRequests.Nodes.Clear();

            var list = issues?.ToList() ?? new List<Issue>();
            if (list.Count == 0)
            {
                tvRequests.Nodes.Add("No service requests found.");
                tvRequests.EndUpdate();
                return;
            }

            if (groupByCategory)
            {
                foreach (var group in list.GroupBy(i => i.Category))
                {
                    var catNode = new TreeNode(group.Key.ToString())
                    {
                        ForeColor = ThemeManager.EmeraldDark
                    };

                    foreach (var issue in group)
                        catNode.Nodes.Add(
                            new TreeNode($"{issue.TicketNumber} — {issue.Location}")
                            { Tag = issue });

                    tvRequests.Nodes.Add(catNode);
                }
            }
            else
            {
                foreach (var issue in list)
                    tvRequests.Nodes.Add(
                        new TreeNode($"{issue.TicketNumber} — {issue.Location}")
                        { Tag = issue });
            }

            tvRequests.ExpandAll();
            tvRequests.EndUpdate();
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            var q = (txtSearch.Text ?? string.Empty).Trim();
            if (q.Length == 0)
            {
                RebuildTreeView(bst.GetInOrderList(), true);
                return;
            }

            var tokens = q.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(t => t.ToLowerInvariant()).ToArray();

            bool Match(Issue i)
            {
                string ticket = i.TicketNumber?.ToLowerInvariant() ?? "";
                string cat = i.Category.ToString().ToLowerInvariant();
                string loc = i.Location?.ToLowerInvariant() ?? "";
                string desc = i.Description?.ToLowerInvariant() ?? "";
                string channel = i.Channel?.ToLowerInvariant() ?? "";

                return tokens.All(token =>
                    ticket.Contains(token) || cat.Contains(token) ||
                    loc.Contains(token) || desc.Contains(token) || channel.Contains(token));
            }

            var filtered = bst.GetInOrderList().Where(Match).ToList();
            bool group = filtered.Select(i => i.Category).Distinct().Count() > 1;
            RebuildTreeView(filtered, group);
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            ReloadDataAndBuildTree();
            BuildPriorityHeap();
            MessageBox.Show("Service requests refreshed.", "Updated",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TvRequests_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is not Issue issue) return;

            lblDetailsTitle.Text = $"Ticket: {issue.TicketNumber}";
            lblDetailsBody.Text =
                $"Category: {issue.Category}\n" +
                $"Location: {issue.Location}\n" +
                $"Description:\n{issue.Description}\n\n" +
                $"Channel: {issue.Channel}\n" +
                $"Created: {issue.CreatedAt:g}";
        }
        private string ShowAreaSelectionDialog(string[] availableAreas)
        {
            if (availableAreas == null || availableAreas.Length == 0)
            {
                MessageBox.Show("No areas found in the current issues.",
                    "No Areas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return string.Empty;
            }

            using (var form = new Form())
            {
                form.Text = "Select Service Area";
                form.StartPosition = FormStartPosition.CenterParent;
                form.ClientSize = new Size(300, 150);
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new Label
                {
                    Text = "Choose a service area to optimise route for:",
                    Dock = DockStyle.Top,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Padding = new Padding(0, 15, 0, 10)
                };

                var combo = new ComboBox
                {
                    Dock = DockStyle.Top,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                combo.Items.AddRange(availableAreas);
                combo.SelectedIndex = 0;

                var ok = new Button
                {
                    Text = "OK",
                    Dock = DockStyle.Bottom,
                    DialogResult = DialogResult.OK
                };

                form.Controls.Add(ok);
                form.Controls.Add(combo);
                form.Controls.Add(label);

                form.AcceptButton = ok;

                return form.ShowDialog() == DialogResult.OK
                    ? combo.SelectedItem.ToString()
                    : string.Empty;
            }
        }

    }

}
