# Windows Service Periodic Tasks

This document outlines the tasks performed periodically by the Ordo Windows Service.

---

## **Overview**
The Windows Service is designed to perform periodic synchronisation tasks to ensure that Microsoft ToDo and the Microsoft 365 Calendar remain in sync with Ordo's internal system.

---

## **Periodic Tasks**
### 1. Synchronise Tasks with Microsoft ToDo
- **Purpose**:
  - Ensure tasks in Microsoft ToDo are reflected in Ordo's internal durations JSON file.
- **Steps**:
  1. Fetch tasks from Microsoft ToDo.
  2. Compare the fetched tasks with entries in the durations JSON file.
  3. Handle changes:
     - **New Tasks**: Add new entries with a default duration.
     - **Removed Tasks**: Remove tasks that no longer exist in ToDo.
     - **Updated Tasks**: Update task metadata (e.g., title, due date) if changes are detected.

---

### 2. Validate Task Durations
- **Purpose**:
  - Ensure all tasks in the durations JSON file have valid durations.
- **Steps**:
  1. Check for tasks without a specified duration.
  2. Assign the **default duration** to those tasks.

---

### 3. Sync with Microsoft 365 Calendar
- **Purpose**:
  - Ensure Ordo-created events in the calendar reflect scheduled tasks.
- **Steps**:
  1. Fetch existing events from the Microsoft 365 Calendar.
  2. Compare tasks in the durations JSON file with Ordo-created events in the calendar:
     - **Missing Events**: Add events for unscheduled tasks.
     - **Updated Tasks**: Reschedule or update events if task details (e.g., duration or deadline) change.

---

## **Excluded Tasks**
- **Cleanup for Completed Tasks**:
  - Cleanup is only triggered explicitly by the user and is not part of periodic execution.

---

## **Execution Schedule**
- **Startup**:
  - Perform an initial synchronisation with Microsoft ToDo and the calendar.
- **Periodic Execution**:
  - Run the periodic tasks at a regular interval (e.g., every 15 minutes).

---

This document ensures clarity around the functionality of Ordo's Windows Service and its periodic responsibilities.
