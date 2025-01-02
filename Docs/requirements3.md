## 3. Synchronisation

### Synchronisation Method
- Update Motion Tasks as follows:
| ToDo  | Motion | Action                                                   |
|-------|--------|----------------------------------------------------------|
|  YES  |   NO   | If task was in Motion before, mark as Completed in Todo  |
|       |        | If task was not in Motion before, create in Motion       |
|  NO   |   YES  | Warn user                                                |
|  YES  |   YES  | Keep the Name and Due Date updated in Motion             |
|  NO   |   NO   | (none)                                                   |

### Sequence
1. Fetch data from ToDo and save in `todo.json`.
2. Fetch data from Motion and save in `motion.json`.
3. For each task from `todo.json`,
   a. If in `ids.json`, update **Name** and **Due Date** in Motion
   b. If not in `ids.json`, create in Motion and add to `ids.json`.
4. For each task in `motion.json`, if not in `ids.json`, warn the user
5. For each entry in `ids.json`,
   a. If not in `motion.json`, mark as **Completed** in ToDo.
   b. If not in `motion.json` and not in `todo.json`, delete from `ids.json`.

### Generic functionalities
- Associate ToDo Lists names' prefixes to Motion Workspaces:
  - Define a json format to hold this association.
  - Use regular expression on tasks names' prefix to distiguish between different workspaces.
  - Ensure that associations defined in json are unique.
- Associate ToDo Lists to Motion Projects:
  - Define a json format to hold this association.
  - Ensure that assoications define in json are unique.
- Associate ToDO Tasks to Motion Tasks.

### Notes
