## 1. ToDo Integration

- **Primary Source**: 
  - Tasks are retrieved from Microsoft ToDo, which serves as the central repository.
  
- **Project Organisation**:
  - Tasks are grouped into **Lists** in ToDo, corresponding to **projects** in Ordo.

- **Project Attributes**:
  - **Id**: The project unique identifier.
  - **Name**: The title of the project

- - **Task Attributes**:
  - **Id**: The task unique identifier.
  - **Name**: The title of the task.
  - **Due Date**: The deadline for completion.

- **Estimated Duration**:
  - Ordo will manage **estimated task durations**, which are critical for scheduling.
  - Duration values:
    - Tasks without specified durations will default to zero; Ordo will not attempt any estimation.
    - Can be customised via:
      - A configuration file (e.g., `durations.json`).
      - ChatGPT interaction (e.g., "Set task X to 2 hours").

- **Exclusion**:
  - Tasks lists with [IGNORE] suffix in the name will be excluded from scheduling and further handling in Ordo.
  - Tasks without a due date will be ignored by Ordo and excluded from scheduling and duration management.
  - Completed tasks will be excluded from scheduling and further handling in Ordo.

- **Cleanup**:
  - Ordo will provide a **clean functionality** to:
    - remove completed tasks from:
      1. **Microsoft ToDo**.
      2. The **JSON file** storing task durations.
    - remove ignored projects from the **JSON file**.
    - remove tasks without due date from the **JSON file**.
  - **User-Triggered**: Cleanup will occur only when explicitly triggered by the user (e.g., via a ChatGPT command like "Clean completed tasks").
  - **Confirmation**: Ordo will confirm before performing the cleanup to prevent accidental deletions.
