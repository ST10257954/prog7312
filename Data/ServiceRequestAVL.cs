using System;
using System.Collections.Generic;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    /*
    Implements a self-balancing AVL Tree to manage service requests
    by their update time or urgency. Ensures requests stay ordered and
    retrievals remain fast even as data grows (GeeksforGeeks, 2025).
    */


    // Represents a single node in the AVL tree (GeeksforGeeks, 2025)
    public class AVLNode
    {
        public Issue Data;
        public AVLNode Left, Right;
        public int Height;

        public AVLNode(Issue data)
        {
            Data = data;
            Height = 1; // Every new node starts at height 1
        }
    }

    public class ServiceRequestAVL
    {
        private AVLNode root; // The root node of the AVL tree (GeeksforGeeks, 2025)


        // Get the height of a node; returns 0 if null for simplicity
        private int Height(AVLNode node) => node?.Height ?? 0;

        // Compute balance factor (difference in height) to detect imbalance
        private int BalanceFactor(AVLNode node) => node == null ? 0 : Height(node.Left) - Height(node.Right);


        /* Rotate tree to the right to fix left-heavy imbalance.
          Used when many recent updates are inserted on the left side. (GeeksforGeeks, 2025) */
        private AVLNode RotateRight(AVLNode y)
        {
            var x = y.Left;
            var T2 = x.Right;

            // Reassign branches to restore order 
            x.Right = y;
            y.Left = T2;

            // Update heights after rotation
            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;


            return x; // x becomes the new root of this subtree
        }


        /* Rotate tree to the left to fix right-heavy imbalance. (GeeksforGeeks, 2025)
           Keeps the tree balanced when too many later updates appear on the right. */
        private AVLNode RotateLeft(AVLNode x)
        {
            var y = x.Right;
            var T2 = y.Left;
            y.Left = x;
            x.Right = T2;

            // Update heights to reflect new structure
            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;


            return y; // y becomes the new root of this subtree
        }

        /* Inserts a new issue into the tree based on LastUpdated date.
           AVL balancing ensures fast search, insertion, and retrieval. */
        public void Insert(Issue issue)
        {
            root = InsertNode(root, issue);
        }


        // Recursive helper to insert and rebalance nodes  (GeeksforGeeks, 2025)
        private AVLNode InsertNode(AVLNode node, Issue issue)
        {
            if (node == null)
                return new AVLNode(issue);

            // Compare by LastUpdated date to maintain order
            if (issue.LastUpdated < node.Data.LastUpdated)
                node.Left = InsertNode(node.Left, issue);
            else if (issue.LastUpdated > node.Data.LastUpdated)
                node.Right = InsertNode(node.Right, issue);
            else
                return node; // duplicates timestamps not allowed


            // Update height to reflect changes in subtrees
            node.Height = Math.Max(Height(node.Left), Height(node.Right)) + 1;

            int balance = BalanceFactor(node); // Check balance state

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

            // Return node unchanged if already balanced
            return node;
        }

        /* Returns all service requests in sorted order.
           Used to list issues by LastUpdated date for reporting. */
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

        /* Confirms whether the AVL tree is balanced.
           Helpful for diagnostics to ensure rotations worked correctly (GeeksforGeeks, 2025). */
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

/*references:
 * GeeksforGeeks, 2025. AVL Tree Data Structure. [Online] 
Available at: https://www.geeksforgeeks.org/dsa/introduction-to-avl-tree/
[Accessed 04 November 2025].
*/
