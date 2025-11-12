using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.Models
{

    /*
    Represents a municipal service issue or complaint record.
    Used throughout the application to capture, track, and update service requests.
    Each issue is tagged with a category, location, and status for efficient management.
    */

    // Enumeration to classify the type of issue reported (Microsoft, 2025)
    public enum IssueCategory { Water, Sanitation, Electricity, Roads, SolidWaste, Other }

    // Core issue record stored in-memory (Part 1)
    public class Issue
    {
        public string TicketNumber { get; set; }      // Unique identifier for each issue, used for tracking and retrieval
        public string Location { get; set; }         // nearest address / landmark  (Microsoft, 2025)
        public IssueCategory Category { get; set; }  // type of service problem
        public string Description { get; set; }     // user-entered details

        public List<string> AttachmentPaths { get; set; } = new List<string>(); // local copies of files

        public DateTime SubmittedAt { get; set; } = DateTime.Now; // when it was actually submitted/saved  (Microsoft, 2025)
        public string Channel { get; set; } = "DesktopApp";        // source channel (App/LowData/Offline-Queued)

        public DateTime CreatedAt { get; set; }       // when the user created the draft (set on submit)

        public int Priority { get; set; } = 3; // default medium priority

        public string AttachmentPath { get; set; }  // File path or URL of uploaded attachment

        public string Area { get; set; } = "Ward A";      // The service area or ward linked to the report for route mapping

        public string Status { get; set; } = "Pending";   // Current issue status: Pending, In Progress, or Resolved

        public DateTime LastUpdated { get; set; } = DateTime.Now.AddMinutes(new Random().Next(-60, 60)); // Last updated timestamp to track modifications and monitoring recency





    }
}
/*References
Microsoft, 2025. Best Practices for the TableLayoutPanel Control. [Online]
Available at: https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/best-practices-for-the-tablelayoutpanel-control
[Accessed 07 September 2025].
microsoft, 2025.Tutorial: Create a Windows Forms app in Visual Studio with C#. [Online] 
Available at: https://learn.microsoft.com/en-us/visualstudio/ide/create-csharp-winform-visual-studio?view=vs-2022
[Accessed 05 September 2025]. */