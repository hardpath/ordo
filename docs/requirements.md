# Requirements for Ordo

## 1. Task Handling
- **Primary Source**: 
  - Tasks are retrieved from Microsoft ToDo, which serves as the central repository.
- **Project Organisation**:
  - Tasks are grouped into **Lists** in ToDo, corresponding to **projects** in Ordo.
- **Task Attributes**:
  - **Name**: The title of the task.
  - **Due Date**: The deadline for completion.
- **Estimated Duration**:
  - Ordo will manage **estimated task durations**, which are critical for scheduling.
  - Duration values can be:
    - Defaulted (e.g., 1 hour for tasks without explicit estimates).
    - Customised via:
      - A configuration file (e.g., `durations.json`).
      - ChatGPT interaction (e.g., "Set task X to 2 hours").
- **Completed Tasks**:
  - **Exclusion**:
    - Completed tasks will be excluded from scheduling and further handling in Ordo.
  - **Cleanup**:
    - Ordo will provide a **clean functionality** to remove completed tasks from:
      1. **Microsoft ToDo**.
      2. The **JSON file** storing task durations.
    - **User-Triggered**: Cleanup will occur only when explicitly triggered by the user (e.g., via a ChatGPT command like "Clean completed tasks").
    - **Confirmation**: Ordo will confirm before performing the cleanup to prevent accidental deletions.

---

## 2. Calendar Integration
- **Existing Events**:
  - Retrieve existing events (appointments) from the Microsoft 365 Calendar.
  - Extract the following details for each event:
    - **Title**: The subject of the event.
    - **Start DateTime**: The event’s start time.
    - **End DateTime**: The event’s end time.
  - Distinguish between:
    - **Ordo-Created Events**: Events with the suffix **"[Ordo]"** in their title, indicating they were added or managed by Ordo.
    - **User-Created Events**: Events without the **"[Ordo]"** suffix, representing meetings or personal appointments created manually by the user.

- **Adding New Events**:
  - Enable Ordo to add new events to the calendar based on scheduled tasks.
  - Each event will include:
    - **Title**: Derived from the task name, with the suffix **"[Ordo]"**.
    - **Start DateTime**: Based on the allocated time slot.
    - **End DateTime**: Calculated using the task’s estimated duration.
  - Ensure there are no scheduling conflicts when adding events.

- **Editing Existing Events**:
  - Ordo can modify **only its own events** (those with the **"[Ordo]"** suffix).
  - Changes may include:
    - Adjusting the start or end time.
    - Updating the title to reflect task changes.
  - User-created events (without the **"[Ordo]"** suffix) remain untouched.

---

## 3. Task Scheduling

- **Objective**:
  - Allocate tasks into calendar slots based on their due dates, durations, and available time.

- **Key Features**:
  1. **Deadline-Oriented Scheduling**:
     - Tasks must be scheduled in such a way that they can be completed by their **due date**.
     - Ordo will prioritise tasks based on:
       - **Due Date**: Tasks with closer deadlines are prioritised.
       - **Duration**: Longer tasks may require splitting into multiple events.

  2. **Duration Management**:
     - Task durations are critical for accurate scheduling.
     - If a task’s duration exceeds the available time in a single slot, Ordo will:
       - **Split the Task**: Create multiple events to accommodate the task.
       - **Spread the Events**: Distribute the parts of the task over multiple time slots or days, ensuring completion before the due date.

  3. **Conflict Avoidance**:
     - Ordo will dynamically check for scheduling conflicts with existing calendar events.
     - Tasks will only be scheduled in available slots.

  4. **Optimisation**:
     - Ordo will optimise task placement to minimise fragmentation of tasks and ensure efficient use of available time.

  5. **Re-Scheduling**:
     - If priorities change, Ordo can adjust the schedule dynamically to meet new requirements.

---

## 4. User Interaction

- **Purpose**:
  - Users will interact with Ordo to give instructions about task scheduling.

- **Modes of Interaction**:
  - Ordo will be controlled via:
    - **ChatGPT Integration**: The primary interface for scheduling commands.

- **Key Features**:
  1. **Schedule Tasks**:
     - Instruct Ordo to generate a schedule for tasks based on available calendar time.
     - Example command:
       - "Schedule all tasks for tomorrow."
  2. **Adjust Scheduling**:
     - Modify the schedule dynamically by:
       - Rescheduling tasks to a different time or date.
       - Updating task durations.
     - Example commands:
       - "Reschedule Task X to next Monday."
       - "Update the duration of Task X to 2 hours."
  3. **Trigger Cleanup**:
     - Instruct Ordo to clean completed tasks from Microsoft ToDo and the JSON file.
     - Example command:
       - "Clean completed tasks."

- **Workflow Transparency**:
  - Users will rely on:
    - **Microsoft ToDo** to view and manage tasks.
    - **Outlook Calendar** to view scheduled events created by Ordo.
