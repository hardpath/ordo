## 2. Motion Integration

### Task Retrieval
- [x] Fetch tasks and their attributes from Motion.

### Attributes

#### Workspaces
- **id** (*string*): A unique identifier for the workspace.
- **name** (*string*): The name of the workspace.

#### Projects
- **id** (*string*): A unique identifier for the project.
- **name** (*string*): The name of the project.
- **workspaceId** (*string*): A reference to the id of the workspace the project belongs to.
- **description** (*string*): A description of the project (can be empty).

#### Tasks
- **id** (*string*): A unique identifier for the task.
- **name** (*string*): The name of the task.
- **dueDate** (*string*): The due date and time in ISO 8601 format.
- **project (object):
  - **id** (*string*): The id of the project the task belongs to.
  - **name** (*string*): The name of the project.
  - **workspaceId** (*string*): The id of the workspace the project belongs to.
  - **description** (*string*): A description of the project (empty in this case).
