## 3. Synchronisation

### Sequence
- [x] Fetch data from ToDo and save in `todo.json`.
- [x] Fetch data from Motion and save in `motion.json`.
- [x] For each task in `motion.json`, if not in `ids.json`, warn the user
- [x] For each list from `todo.json`, if not in `ids.json`, create project in Motion and add to `ids.json`.
- [x] For each task from `todo.json`,
      - [x] If not in `ids.json`, create in Motion and add to `ids.json`.
      - [x] If in `ids.json`, update **Name** and **Due Date** in Motion
- [ ] For each entry in `ids.json`,
      - [x] If not in `todo.json`, delete from Motion.
      - [ ] If not in `motion.json`, mark as **Completed** in ToDo.
      - [ ] If not in `motion.json` and not in `todo.json`, delete from `ids.json`.

### Generic functionalities
- [ ] Associate ToDo Lists names' prefixes to Motion Workspaces:
      - [ ] Define a json format to hold this association.
      - [x] Use regular expression on tasks names' prefix to distiguish between different workspaces.
      - [ ] Ensure that associations defined in json are unique.
- [x] Associate ToDo Lists to Motion Projects:
      - [x] Define a json format to hold this association.
      - [x] Ensure that assoications define in json are unique.
- [x] Associate ToDo Tasks to Motion Tasks.

