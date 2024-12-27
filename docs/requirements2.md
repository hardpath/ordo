## 2. Calendar Integration

### 1. Event Retrieval
- Fetches events from Microsoft 365 Calendar for a specified date range:
  - From "now" to 23:59 on the latest due date of all tasks.

### 2. Event Attributes
- **Id**: Unique identifier for the event.
- **Title**: Event subject.
- **Start DateTime**: Event start time.
- **End DateTime**: Event end time.
- **Task Link**: A reference to the associated task(s) by `Id`.
  - For Ordo-created events: Links to the relevant task(s).
  - For user-created events: The value is `null`.

### 3. Event Distinction
- **Ordo-Created Events**: Events with the `[ORDO]` suffix in their title.
- **User-Created Events**: All other events, created manually by the user.

### 4. Adding New Events
- Ordo can add events to the calendar based on task schedules:
  - **Title**: Derived from the task name with `[ORDO]` suffix.
  - **Start/End DateTime**: Calculated based on task scheduling.
  - **Task Id**: Associated task.

### 5. Editing Events
- Ordo modifies only its own events (those with `[ORDO]` suffix).
- Updates include:
  - Adjusting start or end times.
  - Updating titles to reflect task changes.
  - Delete events

### 6. Conflict Management
- Ordo avoids conflicts with user-created events.
- If a user event causes a conflict, Ordo automatically reschedules its events.
- **Note**: User-created events must remain unchanged.

### 7. Visualization
- Ordo does not provide visualization or listing of calendar events.
- Users will rely on Microsoft Outlook to view calendar events.

### 8. Local Storage for Events
- All events (Ordo-created and user-created) will be saved in `events.json`.
- The file is updated:
  - When data is fetched from Microsoft ToDo or Calendar.
  - After rescheduling and immediately before updating the Calendar.


