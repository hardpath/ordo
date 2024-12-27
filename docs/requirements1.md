## 1. ToDo Integration

### 1. Task Retrieval
- Fetch tasks and their attributes from Microsoft ToDo, grouped by Lists (projects).

### 2. Attributes
#### Projects
- **Id**: Unique identifier for the project/list.
- **Name**: Title of the project/list.
- **IsMissing**: Indicates if the project was removed from Microsoft ToDo.
- **Tasks**: List of tasks under the project.

#### Tasks
- **Id**: Unique identifier for the task.
- **Name**: Title of the task.
- **Alias**: Short alias (e.g., an incremental numeric ID) for user-friendly interaction.
- **DueDate**: Deadline for task completion.
- **Duration**: Estimated time to complete the task (in minutes, default: 0).
- **IsMissing**: Indicates if the task was removed from Microsoft ToDo.

### 3. Estimated Duration
- Default to `0` if not specified.
- Customizable via:
  - `projects.json` configuration file.
  - ChatGPT instructions (e.g., "Set task X to 2 hours").

### 4. Exclusions
- Lists/Projects with `[IGNORE]` in the name.
- Tasks without due dates.
- Completed tasks in ToDo.

### 5. Cleanup Functionality
- Removes completed tasks from Microsoft ToDo.
- Updates `projects.json` by:
  - Removing tasks that were completed in Microsoft ToDo.
  - Removing tasks whose due dates were removed in Microsoft ToDo.
  - Removing projects/lists marked as `[IGNORE]` in Microsoft ToDo.
- **Confirmation Requirement**: The app will confirm with the user before performing each cleanup action.

## 6. User Triggered Data Collection
- Users must explicitly trigger the collection of tasks from Microsoft ToDo.
- This action does not involve any interaction with ChatGPT.
