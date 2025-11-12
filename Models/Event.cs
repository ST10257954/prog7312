using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServicesApp.Models
{

    /*
    Represents a local municipal or community event record.
    Stores event details such as title, date, category, and description
    so that users can view upcoming activities or announcements.
    */
    public class Event
    {

        // The main name of the event (e.g., “Health Awareness Drive”)
        public string Title { get; set; }

        // The scheduled date of the event — used for sorting and reminders
        public DateTime Date { get; set; }

        // Category helps group similar events (e.g., “Health”, “Education”, “Community”)
        public string Category { get; set; }

        // A short explanation or summary to display in the UI
        public string Description { get; set; }


        /* Constructor to initialise an event record with all required details.
           Ensures every event created has consistent key information. */
        public Event(string title, DateTime date, string category, string description)
        {
            Title = title;
            Date = date;
            Category = category;
            Description = description;
        }


        /* Returns a readable summary format for display in lists or logs.
           Example: “Health Awareness Drive (Health) on 12/11/2025”. */
        public override string ToString() => $"{Title} ({Category}) on {Date:d}";
    }
}

/*References
Microsoft, 2025. Classes and Objects in C#. [Online]
Available at: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/classes
[Accessed 02 November 2025].
*/