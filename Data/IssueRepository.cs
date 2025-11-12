using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    public static class IssueRepository
    {
        private static readonly string FilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "issues.json");

        public static List<Issue> Issues { get; private set; } = new();

        // Load issues from JSON file when the app starts
        static IssueRepository()
        {
            LoadIssues();
        }

        // Add a new issue and immediately save
        public static void AddIssue(Issue issue)
        {
            Issues.Add(issue);
            SaveIssues();
        }

        // Save the list of issues to a JSON file
        public static void SaveIssues()
        {
            try
            {
                var json = JsonSerializer.Serialize(Issues, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving issues: {ex.Message}");
            }
        }

        // Load issues from file if it exists
        public static void LoadIssues()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var json = File.ReadAllText(FilePath);
                    var loaded = JsonSerializer.Deserialize<List<Issue>>(json);
                    if (loaded != null)
                        Issues = loaded;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading issues: {ex.Message}");
                Issues = new List<Issue>();
            }
        }
    }
}

/*References
microsoft, 2025.Tutorial: Create a Windows Forms app in Visual Studio with C#. [Online] 
Available at: https://learn.microsoft.com/en-us/visualstudio/ide/create-csharp-winform-visual-studio?view=vs-2022
[Accessed 05 September 2025]. */