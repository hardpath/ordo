## 4. User Interaction

### 1. Purpose
- Users interact with Ordo to give instructions about task scheduling.

### 2. Modes of Interaction
- Controlled via **ChatGPT Integration** as the primary interface for scheduling commands.

### 3. Tasks Triggered by the User
- **Update Schedule**: Generate or update the schedule for tasks based on available calendar time.
- **Remove Completed Tasks in ToDo**: Clean completed tasks from Microsoft ToDo and the JSON file.
- **Remove Ordo-Created Events from Calendar**: Clear all Ordo-created events from the calendar to enable a new scheduling process.
- **Set Task Durations**: Update task durations in `projects.json`.

### 4. Handling Scheduling Conflicts
- If there is no time available to complete the tasks by their due date:
  - ChatGPT will highlight the conflict.
  - The user will be prompted to adjust the due dates or make changes to task durations.

### 5. Workflow Transparency
- Users rely on Microsoft ToDo for task visibility and Outlook Calendar for scheduled events.

> **Note:** Additional commands and use cases may be introduced later as the application evolves.

