using System;
using System.Collections.Generic;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    // Node for the AVL Tree
    public class AVLNode
    {
        public Issue Data;
        public AVLNode Left, Right;
        public int Height;

        public AVLNode(Issue data)
        {
            Data = data;
            Height = 1;
        }
    }

    // Self-balancing AVL Tree for service requests
    public class ServiceRequestAVL
    {
        private AVLNode root;

        private int Height(AVLNode node) => node?.Height ?? 0;

        private int BalanceFactor(AVLNode node) => node == null ? 0 : Height(node.Left) - Height(node.Right);

        private AVLNode RotateRight(AVLNode y)
        {
            var x = y.Left;
            var T2 = x.Right;
            x.Right = y;
            y.Left = T2;
            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
            return x;
        }

        private AVLNode RotateLeft(AVLNode x)
        {
            var y = x.Right;
            var T2 = y.Left;
            y.Left = x;
            x.Right = T2;
            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
            return y;
        }

        // Insert issues by LastUpdated date for balanced updates
        public void Insert(Issue issue)
        {
            root = InsertNode(root, issue);
        }

        private AVLNode InsertNode(AVLNode node, Issue issue)
        {
            if (node == null)
                return new AVLNode(issue);

            // Compare using LastUpdated or Priority if you prefer
            if (issue.LastUpdated < node.Data.LastUpdated)
                node.Left = InsertNode(node.Left, issue);
            else if (issue.LastUpdated > node.Data.LastUpdated)
                node.Right = InsertNode(node.Right, issue);
            else
                return node; // duplicates not allowed

            node.Height = Math.Max(Height(node.Left), Height(node.Right)) + 1;

            int balance = BalanceFactor(node);

            // Four rotation cases
            if (balance > 1 && issue.LastUpdated < node.Left.Data.LastUpdated)
                return RotateRight(node);
            if (balance < -1 && issue.LastUpdated > node.Right.Data.LastUpdated)
                return RotateLeft(node);
            if (balance > 1 && issue.LastUpdated > node.Left.Data.LastUpdated)
            {
                node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }
            if (balance < -1 && issue.LastUpdated < node.Right.Data.LastUpdated)
            {
                node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }

            return node;
        }

        // In-order traversal for sorted retrieval
        public List<Issue> InOrder()
        {
            var list = new List<Issue>();
            void Traverse(AVLNode node)
            {
                if (node == null) return;
                Traverse(node.Left);
                list.Add(node.Data);
                Traverse(node.Right);
            }
            Traverse(root);
            return list;
        }

        // Quick diagnostic check for height and balance
        public bool IsBalanced()
        {
            bool Check(AVLNode node)
            {
                if (node == null) return true;
                int bf = Math.Abs(BalanceFactor(node));
                return bf <= 1 && Check(node.Left) && Check(node.Right);
            }
            return Check(root);
        }
    }
}
