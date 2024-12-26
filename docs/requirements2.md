## 2. Calendar Integration

- **Primary Source**: 
  - Events are retrieved from Microsoft 365 Clendar, which serves as the central repository.
  - Only the events within the time range defined between now and the 23:59 on the lastest due date of all tasks should be retrieved.
  
- **Event Attributes**:
  - **Id**: The event unique identifier.
  - **Title**: The subject of the event.
  - **Start DateTime**: The event’s start time.
  - **End DateTime**: The event’s end time.

- **Distinguish between**:
  - **Ordo-Created Events**: Events with the suffix **"[ORDO]"** in their title, indicating they were added or managed by Ordo.
  - **User-Created Events**: Events without the **"[ORDO]"** suffix, representing meetings or personal appointments created manually by the user.

- **Adding New Events**:
  - Enable Ordo to add new events to the calendar based on scheduled tasks.
  - Each event will include:
    - **Title**: Derived from the task name, with the suffix **"[ORDO]"**.
    - **Start DateTime**: Based on the allocated time slot.
    - **End DateTime**: Calculated using the task’s estimated duration and the scheduling.
    - **Task Id**: From the task Id.
  - Ordo-created events must avoid conflicts with user-created events.

- **Editing Existing Events**:
  - Ordo can modify **only its own events** (those with the **"[ORDO]"** suffix).
  - Changes may include:
    - Adjusting the start or end time.
    - Updating the title to reflect task changes.
  - Ensure there are no scheduling conflicts when adding events.
  - If a user creates an event that causes conflict, Ordo will reschedule its event automatically to avoid overlap.
  - User-created events (without the **"[ORDO]"** suffix) remain untouched.

