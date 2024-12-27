
# User Interaction Design for Ordo

## Overview
Ordo uses a command-line interface (CLI) to interact with the user. The application is designed to be simple and user-friendly, allowing the user to issue commands and receive immediate feedback. Tasks are identified by **short aliases** to simplify user interaction.

---

## Interaction Flow

### 1. Start the Application
- Display a welcome message to introduce the user to Ordo:
  ```
  Welcome to Ordo - Task Scheduler!
  Type 'help' to see the list of available commands.
  ```

---

### 2. Continuous Interaction Loop
- The application runs in a loop until the user explicitly exits.
- At each iteration:
  1. Display a prompt to indicate readiness for commands:
     ```
     Ordo>
     ```
  2. Read user input and process the command.
  3. Validate the command and provide feedback.

---

### 3. User Commands
The following commands are available:

#### **get tasks**
- **Description:** get tasks from Microsoft ToDo and update `projects.json`.
- **Feedback Example:**
  ```
  Tasks geted successfully. Overdue tasks: [101] Task A, [102] Task B.
  ```

#### **get events**
- **Description:** get events from Microsoft 365 Calendar and update `events.json`.
- **Feedback Example:**
  ```
  Calendar events geted successfully and saved to 'events.json'.
  ```

#### **help**
- **Description:** Display the list of available commands and their descriptions.
- **Output Example:**
  ```
  Available Commands:
  - get tasks      Retrieve tasks from Microsoft ToDo.
  - get events     Retrieve events from Microsoft 365 Calendar.
  - exit           Quit the application.
  ```

#### **exit**
- **Description:** Exit the application.
- **Feedback Example:**
  ```
  Goodbye! Thank you for using Ordo.
  ```

---

## Handling Overdue Tasks
- After running the `get tasks` command:
  1. Scan for tasks in `projects.json` with due dates in the past.
  2. Highlight overdue tasks to the user with their aliases:
     ```
     Warning: The following tasks are overdue:
     - [101] Task A (Due: 2023-12-01)
     - [102] Task B (Due: 2023-11-28)
     ```

---

## Error Handling
- If a command fails (e.g., due to an API error), provide clear feedback:
  ```
  Error: Failed to get tasks from Microsoft ToDo. Check your configuration and try again.
  ```
- Allow the user to retry the operation without restarting the application.

---

## Example Interaction
Here’s an example of a typical session with Ordo:
```
Welcome to Ordo - AI Task Scheduler!
Type 'help' to see the list of available commands.

Ordo> help
Available Commands:
- get tasks      Retrieve tasks from Microsoft ToDo.
- get events     Retrieve events from Microsoft 365 Calendar.
- exit             Quit the application.

Ordo> get tasks
Tasks geted successfully. Overdue tasks: [101] Task A, [102] Task B.

Ordo> get events
Calendar events geted successfully and saved to 'events.json'.

Ordo> exit
Goodbye! Thank you for using Ordo.
```
