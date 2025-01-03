## 1. ToDo Integration

### Task Retrieval
- [x] Fetch tasks and their attributes from Microsoft ToDo, grouped by Lists (projects).

### Exclusions
- [x] Ignore ToDo List with [IGNORE] in the name.
- [x] Ignore ToDo Tasks with no due date.
- [x] Ignore completed ToDo Tasks.

### Attributes

#### Lists
- **id** (*string*): A unique identifier for the list.
- **displayName** (*string*): The name of the task list.

#### Tasks
- **id** (*string*): A unique identifier for the task.
- **title** (*string*): The title or description of the task.
- **status** (*string*): The current status of the task (e.g., "notStarted").
- **dueDateTime** (object):
  - **dateTime** (*string*): The due date and time in ISO 8601 format.
  - **timeZone** (*string*): The time zone for the due date and time.
- **ListId** (*string*): A reference to the id of the list that the task belongs to.

