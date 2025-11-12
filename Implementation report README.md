

# **Implementation Report – PROG7312 Part 3 POE Submission**

**Group Member:** Krisha Pareshkumar Gandhi (ST10257954)

**Module:** Programming 3B – PROG7312

**Repository:** [https://github.com/VCPTA/bca3-prog7312-poe-submission-ST10257954.git](https://github.com/VCPTA/bca3-prog7312-poe-submission-ST10257954.git)

**IDE:** Visual Studio 2022

**Language:** C# (.NET 6 WinForms)

---

## **Project Overview**

The **Municipal Services Application** helps local governments by allowing residents to report, monitor, and evaluate service requests (such as water, electricity, and road repairs).
The **Service Request Status Module** focuses on smart status administration via:

* **Effective retrieval and organization of requests** (Binary Search Tree)
* **Prioritizing urgency** (Min-Heap)
* **Visual optimization of routes and workload** (Graph)

These algorithms improve the system's **speed**, **organization**, and **usability** for municipal operations.

---

## **Improvements from Part 1 and Part 2**

1. UI Responsiveness
Before: Static panels with no live status updates.
Now: Added a status dropdown and color-coded labels for instant visual feedback on request progress.
2. Data Handling & Search Efficiency
Before: Linear search lists slowed performance as data grew.
Now: Replaced lists with a Binary Search Tree (BST) for efficient sorting and retrieval of requests.
3. Urgent Task Prioritisation
Before: Urgent issues had to be located manually.
Now: Implemented a Min-Heap priority queue to automatically identify and display the most critical request first.
4. Visual Feedback & Analytics
Before: No clear summary or statistics of municipal requests.
Now: Added a bar-chart view to show service requests per category, supporting data-driven decisions.
5. Interface Design & Structure
Before: Forms lacked consistency and code was difficult to maintain.
Now: Introduced a ThemeManager for uniform styling and reorganised logic into Models, Data, and UI layers for easier maintenance.


---

## **How to Compile and Run**

1. **Clone Repository**

   ```bash
   git clone https://github.com/ST10257954/PROG7312-POE.git
   ```
2. Open in Visual Studio 2022 → `MunicipalServicesApp.sln`
3. Build → Press `Ctrl + Shift + B` → Ensure “Build Succeeded”
4. Run → Press `Ctrl + F5` (Start Without Debugging)
5. From the Main Menu, select **Service Request Status**

---

## **Using the Application**

| **Component**    | **Description**                                                     | **Data Structure Used**   |
| ---------------- | ------------------------------------------------------------------- | ------------------------- |
| Search Bar       | Filter requests by category, location, or description in real time. | Binary Search Tree        |
| TreeView List    | Displays requests in hierarchical order with color-coded status.    | Binary Search Tree        |
| Show Next Urgent | Fetches the highest-priority request instantly.                     | Min-Heap (Priority Queue) |
| Route Optimiser  | Visualises requests per area/category.                              | Graph (Adjacency Mapping) |
| Status Dropdown  | Update each ticket’s status and refresh the TreeView.               | Event Handler + Binding   |
| Attachments      | Shows linked files/photos for each request.                         | File I/O Stream           |

---

## **Data Structures and Their Role**

### Binary Search Tree (BST)

**Purpose:** Store and display service requests in sorted order.
**Contribution:** Reduces search time and automates sorted display.

```csharp
bst = new ServiceRequestBST();
foreach (var issue in allIssues) bst.Insert(issue);
var sorted = bst.GetInOrderList();
RebuildTreeView(sorted, true);
```

**Complexity:** The tree supports logarithmic-time insertions and linear-time traversal.

**Benefit:** Ensures optimal performance when managing or displaying numerous service requests.

---

### Min-Heap (Priority Queue)

**Purpose:** Identify and display the most urgent tasks.
**Contribution:** Ensures critical services (e.g., water or electricity) are handled first.

```csharp
priorityHeap.BuildHeap(allIssues);
var urgent = priorityHeap.ExtractMin();
lblNextUrgent.Text = $"Next urgent: {urgent.TicketNumber}";
```

**Complexity:** Extraction and reordering operations occur in logarithmic time
**Benefit:** Replicates real-world service prioritisation and reduces response delays.

---

### Graph (Analytics and Route Optimisation)

**Purpose:** Represent request categories as nodes and their relationships as edges.
**Contribution:** Provides a visual breakdown of service distribution and workload.

```csharp
DrawCategoryBarGraph(ev.Graphics, categoryCounts, selectedArea);
```

**Complexity:** Traversal per category occurs in linear time
**Benefit:** Helps municipal staff plan routes and allocate resources efficiently.

---

### ComboBox (Status Control)

**Purpose:** Allow technicians to update request statuses instantly.
**Contribution:** Synchronises user input with live UI data.
**Benefit:** Improves communication between users and municipal staff.

---

### TreeView (Display Layer)

**Purpose:** Visually organise issues hierarchically.
**Contribution:** Converts data structure outputs into human-readable format.
**Benefit:** Makes the interface intuitive and professional.

---

## **Efficiency Summary**

| **Structure**      | **Role**                       | **Time Complexity** | **Efficiency Gain**                    |
| ------------------ | ------------------------------ | ------------------- | -------------------------------------- |
| Binary Search Tree | Sorted storage & quick search  | Logarithmic Time    | Fast retrieval of service requests     |
| Min-Heap           | Prioritisation of urgent tasks | Logarithmic Time    | Quick access to high-priority requests |
| Graph              | Analytical visualisation       | Linear Time         | Route and workload optimisation        |
| ComboBox           | Live status updates            | Constant Time       | Instant feedback to users              |
| TreeView           | Hierarchical data display      | Linear Time         | Clear visual representation            |

---

## **Impact on Users and the Municipality**

* **Citizens:** Can log and track requests with live status updates.
* **Municipal Staff:** Quickly identify and prioritise urgent issues.
* **Administrators:** Analyse trends and service categories for informed decisions.
* **System Performance:** Optimised for speed, reliability, and scalability.

---


