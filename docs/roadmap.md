# Roadmap for Ordo

This roadmap outlines the development milestones for Ordo based on the latest requirements. Use this checklist to track progress.

---

## **Phase 1: Core Functionality**
### 1. Task Management
- [x] Fetch tasks from Microsoft ToDo grouped by Lists (projects).
- [x] Extract task details (Name, Due Date).
- [ ] Manage task durations:
  - [ ] Store estimated durations in a configuration file (e.g., `durations.json`).
  - [ ] Allow user-defined durations via ChatGPT.
  - [ ] Use default durations for tasks when the user explicitly asks ChatGPT to apply the default.

### 2. Calendar Integration
- [x] Fetch existing events from Microsoft 365 Calendar:
  - [x] Extract event details (Title, Start DateTime, End DateTime).
  - [ ] Distinguish between Ordo-Created Events (suffix "[Ordo]") and User-Created Events.
- [ ] Add events to the calendar based on scheduled tasks, avoiding conflicts.
- [ ] Edit existing events created by Ordo to reflect task changes.

---

## **Phase 2: Task Scheduling**
- [ ] Implement scheduling logic to:
  - [ ] Allocate tasks into calendar slots based on durations, due dates, and available time.
  - [ ] Split tasks into multiple events if required to meet deadlines.
  - [ ] Optimise task placement to minimise fragmentation and ensure efficient use of time.
- [ ] Enable dynamic rescheduling when priorities or durations change.

---

## **Phase 3: User Interaction**
- [ ] Integrate ChatGPT to:
  - [ ] Schedule tasks (e.g., "Schedule all tasks for tomorrow").
  - [ ] Adjust schedules (e.g., "Reschedule Task X to next Monday").
  - [ ] Trigger cleanup of completed tasks (e.g., "Clean completed tasks").
- [ ] Ensure workflow transparency:
  - [ ] Use Microsoft ToDo for task visibility.
  - [ ] Use Outlook Calendar for viewing scheduled events.

---

## **Phase 4: Advanced Features**
### 1. Workflow Optimisation (Optional)
- [ ] Suggest optimised workflows (task priorities, calendar availability).
- [ ] Highlight potential scheduling conflicts.

### 2. Windows Service Deployment
- [ ] Convert Ordo from a console app to a Windows Service:
  - [ ] Ensure seamless background operation.
  - [ ] Implement logging for visibility.

### 3. Extensibility
- [ ] Add support for advanced scheduling algorithms.
- [ ] Explore integration with additional Microsoft 365 tools.

---

This roadmap serves as a flexible guide for Ordo’s development. Use the checklist to track progress and adapt as needed.
