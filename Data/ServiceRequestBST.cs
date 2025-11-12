using System;
using System.Collections.Generic;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    /*
    Implements a Binary Search Tree (BST) to organise municipal service requests (Microsoft, 2022)
    by TicketNumber for quick searching, sorting, and efficient retrieval.
    Each node represents a service request (Issue)(GeeksforGeeks, 2025).
    */
    public class ServiceRequestBST
    {
        // Represents a single node within the BST storing one Issue record (GeeksforGeeks, 2025).
        private class TreeNode
        {
            public Issue Data;
            public TreeNode Left;
            public TreeNode Right;

            public TreeNode(Issue data)
            {
                Data = data;
            }
        }

        private TreeNode root;  // The root node of the BST (GeeksforGeeks, 2025).

        /* Inserts a new Issue into the BST, maintaining alphabetical order
           by TicketNumber. This ensures requests remain searchable by ID. */
        public void Insert(Issue issue)
        {
            root = InsertRec(root, issue);
        }


        // Recursive helper that determines correct insertion position (Microsoft, 2022).
        private TreeNode InsertRec(TreeNode node, Issue issue)
        {
            if (node == null)
                return new TreeNode(issue);

            // Compare ticket numbers alphabetically (A–Z)
            int compare = string.Compare(issue.TicketNumber, node.Data.TicketNumber, StringComparison.OrdinalIgnoreCase);

            if (compare < 0)
                node.Left = InsertRec(node.Left, issue);
            else if (compare > 0)
                node.Right = InsertRec(node.Right, issue);


            // Duplicate TicketNumbers are ignored since each request is unique (GeeksforGeeks, 2025)
            return node;
        }

        /* Searches for a service request by TicketNumber.
           Returns the Issue if found, otherwise null. */
        public Issue Search(string ticketNo)
        {
            return SearchRec(root, ticketNo);

        }


        // Recursive search that follows BST order for fast lookups.
        private Issue SearchRec(TreeNode node, string ticketNo)
        {
            if (string.IsNullOrWhiteSpace(ticketNo))
                return null;
            if (node == null)
                return null;


            int compare = string.Compare(ticketNo, node.Data.TicketNumber, StringComparison.OrdinalIgnoreCase);
            if (compare == 0)
                return node.Data;  // Match found
            else if (compare < 0)
                return SearchRec(node.Left, ticketNo);  // Search left subtree
            else
                return SearchRec(node.Right, ticketNo);  // Search right subtree
        }

        /* Retrieves all issues in sorted (alphabetical) order of TicketNumber.
           Used to display structured issue lists in the UI. */
        public List<Issue> GetInOrderList()
        {
            var list = new List<Issue>();
            InOrder(root, list);
            return list;
        }


        // Recursive in-order traversal for natural alphabetical sorting.
        private void InOrder(TreeNode node, List<Issue> list)
        {
            if (node == null) return;

            InOrder(node.Left, list);
            list.Add(node.Data);
            InOrder(node.Right, list);
        }

        /* Removes all nodes from the tree.
           Useful when refreshing or rebuilding the data structure. */
        public void Clear()
        {
            root = null;
        }


        /* Returns total number of stored requests.
         Helps track dataset size and performance diagnostics. */
        public int Count()
        {
            return CountRec(root);
        }

        // Recursively count nodes in the tree.
        private int CountRec(TreeNode node)
        {
            if (node == null) return 0;
            return 1 + CountRec(node.Left) + CountRec(node.Right);
        }

    }
}

/*References
GeeksforGeeks, 2025. Binary Search Tree (BST) – Search and Insertion. [Online]
Available at: https://www.geeksforgeeks.org/binary-search-tree-data-structure/
[Accessed 01 November 2025].

Microsoft, 2022. Collections and Data Structures in C#. [Online]
Available at: https://learn.microsoft.com/en-us/dotnet/standard/collections/
[Accessed 01 November 2025].
*/