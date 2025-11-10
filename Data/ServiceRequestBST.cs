using System;
using System.Collections.Generic;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    /// <summary>
    /// Implements a Binary Search Tree (BST) for Service Requests.
    /// Each node stores an Issue object, sorted by its TicketNumber.
    /// </summary>
    public class ServiceRequestBST
    {
        // Nested TreeNode class
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

        private TreeNode root;

        // Insert a new Issue into the BST (sorted by TicketNumber)
        public void Insert(Issue issue)
        {
            root = InsertRec(root, issue);
        }

        private TreeNode InsertRec(TreeNode node, Issue issue)
        {
            if (node == null)
                return new TreeNode(issue);

            // Compare TicketNumber alphabetically
            int compare = string.Compare(issue.TicketNumber, node.Data.TicketNumber, StringComparison.OrdinalIgnoreCase);

            if (compare < 0)
                node.Left = InsertRec(node.Left, issue);
            else if (compare > 0)
                node.Right = InsertRec(node.Right, issue);
            // If duplicate, ignore (tickets are unique)
            return node;
        }

        // Search by TicketNumber
        public Issue Search(string ticketNo)
        {
            return SearchRec(root, ticketNo);

        }

        private Issue SearchRec(TreeNode node, string ticketNo)
        {
            if (string.IsNullOrWhiteSpace(ticketNo))
                return null;
            if (node == null)
                return null;


            int compare = string.Compare(ticketNo, node.Data.TicketNumber, StringComparison.OrdinalIgnoreCase);
            if (compare == 0)
                return node.Data;
            else if (compare < 0)
                return SearchRec(node.Left, ticketNo);
            else
                return SearchRec(node.Right, ticketNo);
        }

        // In-order traversal: alphabetical order of TicketNumber
        public List<Issue> GetInOrderList()
        {
            var list = new List<Issue>();
            InOrder(root, list);
            return list;
        }

        private void InOrder(TreeNode node, List<Issue> list)
        {
            if (node == null) return;

            InOrder(node.Left, list);
            list.Add(node.Data);
            InOrder(node.Right, list);
        }

        // Optional: Clear the tree
        public void Clear()
        {
            root = null;
        }
        public int Count()
        {
            return CountRec(root);
        }

        private int CountRec(TreeNode node)
        {
            if (node == null) return 0;
            return 1 + CountRec(node.Left) + CountRec(node.Right);
        }

    }
}
