using System;
using System.Collections.Generic;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    /*
    Implements a Min-Heap data structure to prioritise service requests (GeeksforGeeks, 2025)
    based on urgency. Lower Priority value = higher urgency (1 = most urgent, 5 = least urgent).
    Adapted from standard C# heap logic.
    */
    public class MinHeap
    {

        // The internal list is used to efficiently store all service requests in heap order.
        private List<Issue> heap = new();


        // Helper functions to compute parent and child indices within the heap tree.
        private int Parent(int i) => (i - 1) / 2;
        private int Left(int i) => 2 * i + 1;
        private int Right(int i) => 2 * i + 2;

        // Insert a new issue and restore heap order by moving it upward as needed.
        public void Insert(Issue issue)
        {
            heap.Add(issue); // New request always starts at the bottom of the heap.
            int i = heap.Count - 1;

            /* "Bubble up" the issue if it has a higher priority (smaller number)
            than its parent, ensuring the most urgent issue always reaches the top.*/

            while (i > 0 && heap[Parent(i)].Priority > heap[i].Priority)
            {
                Swap(i, Parent(i));
                i = Parent(i);
            }
        }

        /* Returns (but does not remove) the most urgent service request.
           Useful when the system just needs to check which issue is next to handle.
        */
        public Issue Peek()
        {
            return heap.Count > 0 ? heap[0] : null;
        }
        


        /* Removes and returns the highest-priority issue(smallest priority number).
           Used when the municipality processes or resolves a request. */
        public Issue ExtractMin()
        {
            if (heap.Count == 0)
                return null;

            Issue root = heap[0];
            heap[0] = heap[^1];
            heap.RemoveAt(heap.Count - 1);

            // Rebuild heap after extraction
            Heapify(0);
            return root;
        }

        // Maintain heap property from a given index downwards
        private void Heapify(int i)
        {
            int smallest = i;
            int left = Left(i);
            int right = Right(i);

            // Compare and find the smallest priority value
            if (left < heap.Count && heap[left].Priority < heap[smallest].Priority)
                smallest = left;
            if (right < heap.Count && heap[right].Priority < heap[smallest].Priority)
                smallest = right;

            // Swap and continue heapifying recursively if needed
            if (smallest != i)
            {
                Swap(i, smallest);
                Heapify(smallest);
            }
        }

        // Swap two elements within the heap
        private void Swap(int i, int j)
        {
            Issue temp = heap[i];
            heap[i] = heap[j];
            heap[j] = temp;
        }

        // Returns the total number of service requests currently tracked in the heap.
        public int Count => heap.Count;


        /* Rebuilds the heap from an existing list of issues.
           This is useful when reloading saved data or after multiple insertions,
           ensuring the heap maintains O(log n) performance on future operations. 
        */
        public void BuildHeap(IEnumerable<Issue> issues)
        {
            heap = new List<Issue>(issues);

            // Start from the lowest non-leaf node because leaf nodes already satisfy the heap rule.
            for (int i = heap.Count / 2 - 1; i >= 0; i--)
                Heapify(i);
        }
    }
}


/*References
 * GeeksforGeeks, 2025. Introduction to Min-Heap. [Online] 
Available at: https://www.geeksforgeeks.org/dsa/introduction-to-min-heap-data-
 */