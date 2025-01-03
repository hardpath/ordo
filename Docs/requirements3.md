## 3. Synchronisation

### Sequence
- [x] Fetch data from ToDo and save in `todo.json`.
- [x] Fetch data from Motion and save in `motion.json`.
- [ ] For each task in `motion.json`, if not in `ids.json`, warn the user
- [ ] For each task from `todo.json`,
      - [ ] If in `ids.json`, update **Name** and **Due Date** in Motion
      - [ ] If not in `ids.json`, create in Motion and add to `ids.json`.
- [ ] For each entry in `ids.json`,
      - [ ] If not in `motion.json`, mark as **Completed** in ToDo.
      - [ ] If not in `motion.json` and not in `todo.json`, delete from `ids.json`.

### Generic functionalities
- [ ] Associate ToDo Lists names' prefixes to Motion Workspaces:
      - [ ] Define a json format to hold this association.
      - [ ] Use regular expression on tasks names' prefix to distiguish between different workspaces.
      - [ ] Ensure that associations defined in json are unique.
- [ ] Associate ToDo Lists to Motion Projects:
      - [ ] Define a json format to hold this association.
      - [ ] Ensure that assoications define in json are unique.
- [ ] Associate ToDO Tasks to Motion Tasks.

