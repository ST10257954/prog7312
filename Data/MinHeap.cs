using System;
using System.Collections.Generic;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    /// <summary>
    /// Min-Heap for prioritising high-urgency service requests.
    /// Lower Priority value = more urgent (1 = High, 5 = Low)
    /// </summary>
    public class MinHeap
    {
        private List<Issue> heap = new();

        private int Parent(int i) => (i - 1) / 2;
        private int Left(int i) => 2 * i + 1;
        private int Right(int i) => 2 * i + 2;

        // Insert a new Issue into the heap
        public void Insert(Issue issue)
        {
            heap.Add(issue);
            int i = heap.Count - 1;

            // Bubble-up
            while (i > 0 && heap[Parent(i)].Priority > heap[i].Priority)
            {
                Swap(i, Parent(i));
                i = Parent(i);
            }
        }

        // Return (but do not remove) the smallest priority item
        public Issue Peek()
        {
            return heap.Count > 0 ? heap[0] : null;
        }

        // Extract the minimum (highest priority) Issue
        public Issue ExtractMin()
        {
            if (heap.Count == 0)
                return null;

            Issue root = heap[0];
            heap[0] = heap[^1];
            heap.RemoveAt(heap.Count - 1);
            Heapify(0);
            return root;
        }

        // Heapify-down from given index
        private void Heapify(int i)
        {
            int smallest = i;
            int left = Left(i);
            int right = Right(i);

            if (left < heap.Count && heap[left].Priority < heap[smallest].Priority)
                smallest = left;
            if (right < heap.Count && heap[right].Priority < heap[smallest].Priority)
                smallest = right;

            if (smallest != i)
            {
                Swap(i, smallest);
                Heapify(smallest);
            }
        }

        private void Swap(int i, int j)
        {
            Issue temp = heap[i];
            heap[i] = heap[j];
            heap[j] = temp;
        }

        public int Count => heap.Count;

        // Rebuild heap from issue list
        public void BuildHeap(IEnumerable<Issue> issues)
        {
            heap = new List<Issue>(issues);
            for (int i = heap.Count / 2 - 1; i >= 0; i--)
                Heapify(i);
        }
    }
}
