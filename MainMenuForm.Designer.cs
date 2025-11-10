using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    partial class MainMenuForm
    {
        private Panel pnlCard;
        private Label lblHeader;
        private Button btnReportIssues;
        private Button btnLocalEvents;
        private Button btnStatus;
        private Label lblFooter;

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // UI elements
            pnlCard = new Panel();
            lblHeader = new Label();
            btnReportIssues = new Button();
            btnLocalEvents = new Button();
            btnStatus = new Button();
            lblFooter = new Label();
            SuspendLayout();

            // Form setup
            this.ClientSize = new Size(720, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Municipal Services Portal";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.DoubleBuffered = true;
            this.Paint += MainMenuForm_Paint;
            this.BackColor = Color.White;

            // Header
            lblHeader.Text = "Municipal Services Portal";
            lblHeader.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
            lblHeader.ForeColor = Color.White;
            lblHeader.Size = new Size(480, 70);
            lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            lblHeader.BackColor = Color.Transparent;
            lblHeader.Location = new Point((720 - 480) / 2, 90);
            lblHeader.Paint += lblHeader_Paint;

            // Card panel
            pnlCard.Size = new Size(480, 250);
            pnlCard.Location = new Point((720 - 480) / 2, 170);
            pnlCard.Paint += pnlCard_Paint;
            pnlCard.BackColor = Color.Transparent;

            Font btnFont = new Font("Segoe UI", 12F);
            Color baseColor = Color.White;
            Color hoverColor = Color.FromArgb(240, 245, 240);
            Size btnSize = new Size(440, 55);

            // Report Issues button
            btnReportIssues.FlatStyle = FlatStyle.Flat;
            btnReportIssues.FlatAppearance.BorderSize = 0;
            btnReportIssues.Font = btnFont;
            btnReportIssues.ForeColor = Color.FromArgb(40, 40, 40);
            btnReportIssues.BackColor = baseColor;
            btnReportIssues.Size = btnSize;
            btnReportIssues.Location = new Point(20, 25);
            btnReportIssues.Text = "📋  Report Issues";
            btnReportIssues.TextAlign = ContentAlignment.MiddleLeft;
            btnReportIssues.Padding = new Padding(25, 0, 0, 0);
            btnReportIssues.MouseEnter += (s, e) => btnReportIssues.BackColor = hoverColor;
            btnReportIssues.MouseLeave += (s, e) => btnReportIssues.BackColor = baseColor;
            btnReportIssues.Click += btnReportIssues_Click;

            // Local Events button
            btnLocalEvents.FlatStyle = FlatStyle.Flat;
            btnLocalEvents.FlatAppearance.BorderSize = 0;
            btnLocalEvents.Font = btnFont;
            btnLocalEvents.ForeColor = Color.FromArgb(40, 40, 40);
            btnLocalEvents.BackColor = baseColor;
            btnLocalEvents.Size = btnSize;
            btnLocalEvents.Location = new Point(20, 95);
            btnLocalEvents.Text = "📅  Local Events  Announcements";
            btnLocalEvents.TextAlign = ContentAlignment.MiddleLeft;
            btnLocalEvents.Padding = new Padding(25, 0, 0, 0);
            btnLocalEvents.MouseEnter += (s, e) => btnLocalEvents.BackColor = hoverColor;
            btnLocalEvents.MouseLeave += (s, e) => btnLocalEvents.BackColor = baseColor;
            btnLocalEvents.Click += btnLocalEvents_Click;

            // Status button (now enabled and functional)
            btnStatus.FlatStyle = FlatStyle.Flat;
            btnStatus.FlatAppearance.BorderSize = 0;
            btnStatus.Font = btnFont;
            btnStatus.ForeColor = Color.FromArgb(40, 40, 40);
            btnStatus.BackColor = baseColor;
            btnStatus.Size = btnSize;
            btnStatus.Location = new Point(20, 165);
            btnStatus.Text = "🕓  View Service Requests";
            btnStatus.TextAlign = ContentAlignment.MiddleLeft;
            btnStatus.Padding = new Padding(25, 0, 0, 0);
            btnStatus.MouseEnter += (s, e) => btnStatus.BackColor = hoverColor;
            btnStatus.MouseLeave += (s, e) => btnStatus.BackColor = baseColor;
            btnStatus.Click += btnStatus_Click;

            // Footer
            lblFooter.Font = new Font("Segoe UI", 9F);
            lblFooter.ForeColor = Color.FromArgb(120, 120, 120);
            lblFooter.Text = "© 2025 City of Tshwane | Powered by Municipal Portal";
            lblFooter.Dock = DockStyle.Bottom;
            lblFooter.TextAlign = ContentAlignment.MiddleCenter;
            lblFooter.Height = 40;

            // Add controls
            pnlCard.Controls.Add(btnReportIssues);
            pnlCard.Controls.Add(btnLocalEvents);
            pnlCard.Controls.Add(btnStatus);
            Controls.Add(lblHeader);
            Controls.Add(pnlCard);
            Controls.Add(lblFooter);
            ResumeLayout(false);
        }

        // Header gradient
        private void lblHeader_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, lblHeader.Width, lblHeader.Height);
            using (GraphicsPath path = new GraphicsPath())
            {
                int radius = 20;
                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                path.AddLine(rect.Right, rect.Y + radius, rect.Right, rect.Bottom);
                path.AddLine(rect.Right, rect.Bottom, rect.X, rect.Bottom);
                path.CloseFigure();

                using (LinearGradientBrush brush = new LinearGradientBrush(
                    rect,
                    Color.FromArgb(25, 90, 35),
                    Color.FromArgb(60, 130, 60),
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillPath(brush, path);
                }
            }

            TextRenderer.DrawText(e.Graphics, lblHeader.Text, lblHeader.Font,
                rect, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        // Card panel design
        private void pnlCard_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, pnlCard.Width - 1, pnlCard.Height - 1);

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddEllipse(rect.X - 10, rect.Y - 10, rect.Width + 20, rect.Height + 30);
                using (PathGradientBrush pgb = new PathGradientBrush(gp))
                {
                    pgb.CenterColor = Color.FromArgb(60, 0, 0, 0);
                    pgb.SurroundColors = new Color[] { Color.Transparent };
                    e.Graphics.FillPath(pgb, gp);
                }
            }

            int radius = 18;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();

                using (SolidBrush brush = new SolidBrush(Color.White))
                    e.Graphics.FillPath(brush, path);
            }
        }

        // Background gradient
        private void MainMenuForm_Paint(object sender, PaintEventArgs e)
        {
            // Don’t draw until the form is properly created and sized
            if (!IsHandleCreated || !Visible)
                return;

            int width = ClientSize.Width;
            int height = ClientSize.Height;

            // Avoid invalid rectangles
            if (width < 2 || height < 2)
                return;

            Rectangle rect = new Rectangle(0, 0, width, height);

            try
            {
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    rect,
                    Color.FromArgb(245, 247, 240),   // top color
                    Color.FromArgb(228, 237, 227),   // bottom color
                    System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    e.Graphics.FillRectangle(brush, rect);
                }
            }
            catch (ArgumentException)
            {
                // Skip drawing if the rectangle is invalid during resizing
            }
        }


    }
}
