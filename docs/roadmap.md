## Roadmap for Ordo

## Feature 1: Basic User Interaction
Lay the foundation for user interaction with Ordo, enabling the user to issue commands and receive feedback.

### Tasks
- [x] Implement a command-line interface (CLI) for user interaction:
  - [x] Display a prompt (`Ordo>`) to accept user commands.
  - [x] Parse and validate user commands.
  - [x] Display appropriate feedback for valid and invalid commands.
- [x] Implement a "help" command to display available commands and their descriptions.
- [x] Implement an "exit" command to quit the application with a goodbye message.
- [ ] Provide stubs (placeholders) for key user commands:
  - [x] `get tasks`
  - [ ] `get events`
- [x] Display meaningful feedback for unimplemented commands.

## Feature 2: Basic Task Management
Ordo can manage tasks, synchronize with ToDo, and maintain a local JSON file with additional task attributes.

### Tasks
- [x] Fetch tasks from Microsoft ToDo grouped by Lists (projects).
- [ ] Assign an alias to each task.
- [x] Save tasks to `projects.json` for local storage and processing.
- [x] Synchronize `projects.json` with Microsoft ToDo:
  - [x] Add new tasks from ToDo to `projects.json`.
  - [x] Update tasks in `projects.json` when changes occur in ToDo (e.g., due date or status changes).
  - [x] Remove tasks from `projects.json` if they are deleted or completed in ToDo.
- [ ] Remove tasks with no due dates:
  - [ ] Delete tasks without due dates from `projects.json`.
- [ ] Implement functionality to set task durations via:
  - [ ] User input in the command line.
  - [ ] Updates through ChatGPT commands.
- [x] Exclude tasks based on criteria:
  - [x] `[IGNORE]` suffix in List names.
  - [x] Tasks marked as completed.
- [ ] Allow the user to manually trigger the retrieval of tasks from Microsoft ToDo.

---

## Feature 3: Calendar Integration
Ordo integrates with the calendar, manages events, and ensures conflict-free scheduling.

### Tasks
- [ ] Fetch events from Microsoft 365 Calendar from now until the latest due date from the tasks.
- [ ] Save all events (Ordo-created and user-created) to `events.json`.
- [ ] Distinguish between Ordo-created and user-created events:
  - [ ] Add `[ORDO]` suffix to the title of Ordo-created events.
  - [ ] Set the `Task Link` attribute in `events.json`:
    - [ ] Ordo-created events link to the relevant task(s).
    - [ ] User-created events have a `null` value for the `Task Link`.
- [ ] Add new Ordo-created events to the calendar:
  - [ ] Create events based on tasks and their durations.
  - [ ] Assign calculated start and end times.
  - [ ] Ensure no conflicts with user-created events.
- [ ] Update Ordo-created events in the calendar to reflect task changes:
  - [ ] Adjust event start or end times.
  - [ ] Update titles to reflect changes in associated tasks.
- [ ] Delete Ordo-created events:
  - [ ] If the related task is deleted or completed.
  - [ ] If the user prompts for deletion.
- [ ] Ensure `events.json` is updated:
  - [ ] When data is fetched from the calendar.
  - [ ] After Ordo-created events are added, updated, or deleted.
- [ ] Allow the user to manually trigger the retrieval of events from Microsoft 365 Calendar.

---

## Feature 4: Task Scheduling with ChatGPT
Ordo uses ChatGPT to generate schedules for tasks based on due dates, durations, and available calendar time. Ordo does not implement any scheduling logic.

### Tasks
- [ ] Highlight tasks with overdue due dates in the ChatGPT response.
- [ ] Prepare data for ChatGPT:
  - [ ] Include task details such as `Id`, `Name`, `DueDate`, and `Duration`.
  - [ ] Include calendar events with details like `Id`, `Title`, `Start DateTime`, `End DateTime`, and `Task Link`.
- [ ] Send prepared data to ChatGPT for scheduling.
- [ ] Process ChatGPT scheduling responses:
  - [ ] Apply the proposed schedule by updating `projects.json` and `events.json`.
  - [ ] If no feasible schedule exists, highlight the conflict to the user.
- [ ] Prompt the user to resolve scheduling conflicts:
  - [ ] Suggest adjusting task due dates.
  - [ ] Allow updates to task durations if required.

