## 1. ToDo Integration

### Task Retrieval
- Fetch tasks and their attributes from Microsoft ToDo, grouped by Lists (projects).
- Ignore ToDo List with [IGNORE] in the name.
- Ignore ToDo Tasks with no due date.
- Ignore completed ToDo Tasks.

### Attributes
#### Lists
- **Id**: Unique identifier for the project/list.
- **Name**: Title of the project/list.

#### Tasks
- **Id**: Unique identifier for the task.
- **Title**: Title of the task.
- **DueDate**: Deadline for task completion.
- **Status**: Status of the tasks (completed or not).

### 4. Exclusions
- Lists with `[IGNORE]` in the name.
- Tasks:
  - Without due dates.
  - Completed.
