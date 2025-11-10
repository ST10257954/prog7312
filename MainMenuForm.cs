using System;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            InitializeComponent();
        }

        // Open Report Issue form
        private void btnReportIssues_Click(object sender, EventArgs e)
        {
            using (var form = new ReportIssueForm())
                form.ShowDialog();
        }

        // Open Local Events form
        private void btnLocalEvents_Click(object sender, EventArgs e)
        {
            using (var form = new EventsForm())
                form.ShowDialog();
        }

        // Open Service Request Status form
        private void btnStatus_Click(object sender, EventArgs e)
        {
            using (var form = new ServiceRequestStatusForm())
                form.ShowDialog();
        }
    }
}
