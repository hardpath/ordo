## 3. Task Scheduling

### 1. Objective
- Allocate tasks into calendar slots based on their due dates, durations, and available time.

### 2. Key Features
#### Inputs for Scheduling
- The following data is provided to ChatGPT for scheduling:
  - **Tasks List**: Tasks retrieved from Microsoft ToDo, including:
    - Task `Id`, `Name`, `DueDate`, and `Duration`.
  - **Existing Events**: Events fetched from the Calendar, including:
    - Event `Id`, `Title`, `Start DateTime`, `End DateTime`, and `Task Link` (if applicable).

#### Deadline-Oriented Scheduling
- Tasks will be scheduled to be completed by their **due date**.
- Parameters passed to ChatGPT for scheduling include:
  - **Maximum work time**: Duration without any breaks.
  - **Minimum event duration**: Smallest allowable block size.
  - **Break time**: Break duration (in multiples of 5 minutes).

#### Rescheduling
- Events must be rescheduled following the scheduling provided by ChatGPT.

#### Suggestions for Conflicts
- If ChatGPT cannot create a conflict-free schedule, ChatGPT will provide suggestions to the user for resolution.

### Notes
- Ordo is not expected to generate its own scheduling logic.
- Tasks retrieved from Microsoft ToDo with a due date in the past must be highlighted by ChatGPT when asked to schedule.
