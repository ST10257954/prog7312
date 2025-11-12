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
        private ComboBox cmbStatus;
        private Label lblStatusColor;


        private Panel rightCard;
        private Panel graphPanel;

        private List<Issue> allIssues = new();
        private ServiceRequestBST bst = new();
        private MinHeap priorityHeap = new();

        // Keep a single paint handler bound to the graph panel
        private PaintEventHandler graphPaintHandler;

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

            /* ---------------------------- Toolbar Section ---------------------------- */
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
            {
        btnRefresh, btnBack, btnUrgent, btnGraphDemo, btnBackToDetails
            });

            /* -------------------------- Main Split Layout --------------------------- */
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

            /* --------------------------- Left Side Panel ---------------------------- */
            var leftCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeManager.CardWhite,
                Padding = new Padding(10),
                Margin = new Padding(0, 0, 8, 0)
            };
            leftCard.Paint += (s, e) => ThemeManager.DrawCardShadow(e.Graphics, leftCard.ClientRectangle);

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

            /* -------------------------- Right Side Panel ---------------------------- */
            rightCard = new Panel
            {
                Name = "rightCard",
                Dock = DockStyle.Fill,
                BackColor = ThemeManager.CardWhite,
                Margin = new Padding(8, 0, 0, 0),
                Padding = new Padding(0)
            };
            rightCard.Paint += (s, e) => ThemeManager.DrawCardShadow(e.Graphics, rightCard.ClientRectangle);
            mainSplit.Controls.Add(rightCard, 1, 0);

            /* ------------------------ Content + Details Card ------------------------ */
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20, 18, 20, 18)
            };

            var detailsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 1,
                BackColor = Color.White
            };
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45)); // Title
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 70));  // Body
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // Status label
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Dropdown
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

            // --- Status label ---
            lblStatusColor = new Label
            {
                Text = "Status: Pending",
                ForeColor = Color.DarkRed,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(6, 4, 6, 4)
            };
            detailsLayout.Controls.Add(lblStatusColor, 0, 2);

            // --- Status dropdown ---
            cmbStatus = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.White
            };
            cmbStatus.Items.AddRange(new[] { "Pending", "In Progress", "Completed", "On Hold" });
            cmbStatus.SelectedIndex = 0;
            cmbStatus.SelectedIndexChanged += CmbStatus_SelectedIndexChanged;
            detailsLayout.Controls.Add(cmbStatus, 0, 3);

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

            /* ------------------------------ Graph Panel ----------------------------- */
            graphPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Visible = false
            };
            mainSplit.Controls.Add(graphPanel, 1, 0);

            // ✅ Anti-flicker enhancement for graphs
            typeof(Panel).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(graphPanel, true, null);

            lblNextUrgent = new Label
            {
                Text = "Next urgent request: (none)",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Italic),
                ForeColor = ThemeManager.TextDark,
                TextAlign = ContentAlignment.MiddleLeft
            };
            root.Controls.Add(lblNextUrgent, 0, 2);

            /* ------------------------- Final Data Preparation ----------------------- */
            ReloadDataAndBuildTree();
            BuildPriorityHeap();
            UpdateNextUrgentLabel();
        }
        // ===========================================================
        // STATUS DROPDOWN HANDLER
        // ===========================================================
        private void CmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tvRequests.SelectedNode?.Tag is not Issue issue)
                return;

            issue.Status = cmbStatus.SelectedItem?.ToString() ?? "Pending";

            // 🔹 Update the color label
            lblStatusColor.Text = $"Status: {issue.Status}";
            lblStatusColor.ForeColor = issue.Status switch
            {
                "Completed" => Color.ForestGreen,
                "In Progress" => Color.DarkOrange,
                "On Hold" => Color.MediumVioletRed,
                _ => Color.DarkRed
            };

            // 🔹 Update the title text for feedback
            lblDetailsTitle.Text = $"Ticket: {issue.TicketNumber} ({issue.Status})";

            // 🔹 Rebuild TreeView colors dynamically
            ReloadDataAndBuildTree();
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

            var availableAreas = allIssues
                .Where(i => !string.IsNullOrWhiteSpace(i.Area))
                .Select(i => i.Area)
                .Distinct()
                .ToArray();

            string selectedArea = ShowAreaSelectionDialog(availableAreas, includeAllOption: true);
if (string.IsNullOrEmpty(selectedArea)) return;

// If "All Areas" is selected, use allIssues; otherwise, filter
var filtered = selectedArea == "All Areas"
    ? allIssues.ToList()
    : allIssues.Where(i => i.Area == selectedArea).ToList();


            if (filtered.Count == 0)
            {
                MessageBox.Show($"No service requests found for {selectedArea}.",
                    "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Count by category
            var categoryCounts = filtered
                .GroupBy(i => i.Category)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());

            // Prepare graph panel
            rightCard.Visible = false;
            graphPanel.Visible = true;
            btnGraphDemo.Visible = false;
            btnBackToDetails.Visible = true;

            // Unhook any old Paint events
            if (graphPaintHandler != null)
                graphPanel.Paint -= graphPaintHandler;

            graphPaintHandler = (s, ev) =>
            {
                DrawCategoryBarGraph(ev.Graphics, categoryCounts, selectedArea);
            };
            graphPanel.Paint += graphPaintHandler;
            graphPanel.Invalidate();
        }

        private void DrawCategoryBarGraph(Graphics g, Dictionary<string, int> counts, string area)
        {
            g.Clear(Color.White);

            using var fontTitle = new Font("Segoe UI Semibold", 14);
            using var fontLabel = new Font("Segoe UI", 10);
            var textBrush = Brushes.Black;
            var barBrush = new SolidBrush(ThemeManager.EmeraldMid);

            // Layout
            int chartLeft = 70, chartTop = 80, chartHeight = 220, chartRightPadding = 30;
            int n = Math.Max(1, counts.Count);
            int plotWidth = graphPanel.ClientSize.Width - chartLeft - chartRightPadding;
            int barWidth = Math.Max(28, (int)(plotWidth / (n * 1.8)));
            int spacing = Math.Max(16, (int)(barWidth * 0.6));
            int startX = chartLeft;

            int maxVal = Math.Max(1, counts.Values.Max());

            // Title
            g.DrawString($"Service Requests by Category — {area}", fontTitle,
                new SolidBrush(ThemeManager.EmeraldDark), new PointF(30, 30));

            // Axis line (baseline)
            using var axisPen = new Pen(Color.Gray, 1);
            g.DrawLine(axisPen, chartLeft - 10, chartTop + chartHeight, chartLeft + plotWidth, chartTop + chartHeight);

            // Bars
            int x = startX;
            foreach (var kv in counts.OrderBy(k => k.Key))
            {
                float h = (float)kv.Value / maxVal * chartHeight;
                var bar = new Rectangle(x, (int)(chartTop + chartHeight - h), barWidth, (int)h);

                g.FillRectangle(barBrush, bar);
                g.DrawRectangle(Pens.DarkGray, bar);

                // Value label
                var valPt = new PointF(x + barWidth / 2f - 8, bar.Top - 18);
                g.DrawString(kv.Value.ToString(), fontLabel, textBrush, valPt);

                // Category label (vertical if needed)
                var catText = kv.Key;
                var labelPt = new PointF(x, chartTop + chartHeight + 6);
                if (barWidth < 60 || catText.Length > 10)
                {
                    // rotate -45° for long labels
                    var state = g.Save();
                    g.TranslateTransform(labelPt.X + barWidth / 2f, labelPt.Y + 16);
                    g.RotateTransform(-45);
                    g.DrawString(catText, fontLabel, textBrush, new PointF(-barWidth / 2f, 0));
                    g.Restore(state);
                }
                else
                {
                    g.DrawString(catText, fontLabel, textBrush, labelPt);
                }

                x += barWidth + spacing;
            }
        }

        private void BtnBackToDetails_Click(object sender, EventArgs e)
        {
            graphPanel.Visible = false;
            rightCard.Visible = true;
            btnBackToDetails.Visible = false;
            btnGraphDemo.Visible = true;

            // detach paint handler
            if (graphPaintHandler != null)
            {
                graphPanel.Paint -= graphPaintHandler;
                graphPaintHandler = null;
            }

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

        private void UpdateNextUrgentLabel()
        {
            var peek = priorityHeap?.Peek();
            lblNextUrgent.Text = peek == null
                ? "Next urgent request: (none)"
                : $"Next urgent request: {peek.TicketNumber} ({peek.Category})";
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
                attachmentPanel.Visible = false;
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
                attachmentPanel.Visible = false;
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

            DisplayAttachments(urgent.AttachmentPaths);
            UpdateNextUrgentLabel(); 

        }

        private void ReloadDataAndBuildTree()
        {
            allIssues = IssueRepository.Issues?.ToList() ?? new List<Issue>();

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

            Color ColorFor(string status) => status switch
            {
                "Completed" => Color.ForestGreen,
                "In Progress" => Color.DarkOrange,
                "On Hold" => Color.MediumVioletRed,
                _ => Color.DarkRed
            };

            TreeNode MakeIssueNode(Issue issue)
            {
                var node = new TreeNode($"{issue.TicketNumber} — {issue.Location} [{issue.Status}]") { Tag = issue };
                node.ForeColor = ColorFor(issue.Status ?? "Pending");
                return node;
            }

            if (groupByCategory)
            {
                foreach (var group in list.GroupBy(i => i.Category))
                {
                    var catNode = new TreeNode(group.Key.ToString()) { ForeColor = ThemeManager.EmeraldDark };
                    foreach (var issue in group) catNode.Nodes.Add(MakeIssueNode(issue));
                    tvRequests.Nodes.Add(catNode);
                }
            }
            else
            {
                foreach (var issue in list) tvRequests.Nodes.Add(MakeIssueNode(issue));
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
            UpdateNextUrgentLabel();
            MessageBox.Show("Service requests refreshed.", "Updated",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TvRequests_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is not Issue issue) return;

            // --- Display request details ---
            lblDetailsTitle.Text = $"Ticket: {issue.TicketNumber}";
            lblDetailsBody.Text =
                $"Category: {issue.Category}\n" +
                $"Location: {issue.Location}\n" +
                $"Description:\n{issue.Description}\n\n" +
                $"Channel: {issue.Channel}\n" +
                $"Created: {issue.CreatedAt:g}";

            // --- Display attachments safely ---
            DisplayAttachments(issue.AttachmentPaths ?? new List<string>());

            // --- Ensure issue.Status always has a value ---
            if (string.IsNullOrWhiteSpace(issue.Status))
                issue.Status = "Pending";

            // --- Sync dropdown selection safely ---
            if (cmbStatus.Items.Contains(issue.Status))
                cmbStatus.SelectedItem = issue.Status;
            else
                cmbStatus.SelectedIndex = 0;

            // --- Update color-coded status label ---
            lblStatusColor.Text = $"Status: {issue.Status}";
            lblStatusColor.ForeColor = issue.Status switch
            {
                "Completed" => Color.ForestGreen,
                "In Progress" => Color.DarkOrange,
                "On Hold" => Color.MediumVioletRed,
                _ => Color.DarkRed
            };
        }


        private void DisplayAttachments(List<string> paths)
        {
            attachmentPanel.Controls.Clear();

            var list = paths ?? new List<string>();
            if (list.Count == 0)
            {
                attachmentPanel.Visible = false;
                return;
            }

            foreach (var p in list)
            {
                var fileName = SafeFileName(p);
                var btn = new Button
                {
                    Text = fileName,
                    AutoSize = true,
                    Padding = new Padding(8, 4, 8, 4),
                    Margin = new Padding(6),
                    BackColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = Color.Gainsboro;

                btn.Click += (_, __) =>
                {
                    try
                    {
                        if (File.Exists(p))
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = p,
                                UseShellExecute = true
                            });
                        else
                            MessageBox.Show("File not found: " + p, "Open Attachment",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show("Unable to open: " + p, "Open Attachment",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                attachmentPanel.Controls.Add(btn);
            }

            attachmentPanel.Visible = true;
        }

        private static string SafeFileName(string path)
        {
            try { return Path.GetFileName(path); }
            catch { return path ?? ""; }
        }

private string ShowAreaSelectionDialog(string[] availableAreas, bool includeAllOption = false)
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
            Text = "Choose a service area to analyse:",
            Dock = DockStyle.Top,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(0, 15, 0, 10)
        };

        var combo = new ComboBox
        {
            Dock = DockStyle.Top,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        if (includeAllOption)
            combo.Items.Add("All Areas");

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
            ? combo.SelectedItem?.ToString() ?? string.Empty
            : string.Empty;
    }
}
    }
}
