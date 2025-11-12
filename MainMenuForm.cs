using System;
using System.Windows.Forms;

namespace MunicipalServicesApp
{

    /*
    Main navigation form for the Municipal Services Application.
    Uses modal Windows Forms (ShowDialog) to open other modules such as:
    - ReportIssueForm
    - EventsForm
    - ServiceRequestStatusForm
    This structure ensures users can focus on one task at a time before returning to the main menu (Microsoft, 2025).
    */

    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            InitializeComponent();
        }

        // Opens the Report Issue form so users can log a new service request.
        private void btnReportIssues_Click(object sender, EventArgs e)
        {
            using (var form = new ReportIssueForm())
                form.ShowDialog();
        }

        // Opens the Local Events form to view community event information.
        private void btnLocalEvents_Click(object sender, EventArgs e)
        {
            using (var form = new EventsForm())
                form.ShowDialog();
        }

        // Opens the Service Request Status form to check progress on logged issues (Microsoft, 2025).
        private void btnStatus_Click(object sender, EventArgs e)
        {
            using (var form = new ServiceRequestStatusForm())
                form.ShowDialog();
        }
    }
}

/* References:
Microsoft, 2025. How to: Navigate Data with the Windows Forms BindingNavigator Control. [Online] 
Available at: https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-navigate-data-with-the-windows-forms-bindingnavigator-control
[Accessed 05 November 2025].
 */