# ordo
AI-powered assistant for seamlessly organising tasks and scheduling them into your calendar

## Roadmap

### Phase 1: Planning and Requirements
1. **Key Features**:
   - Fetch tasks from Microsoft ToDo.
   - Integrate with Microsoft 365 Calendar.
   - Implement task prioritization and scheduling logic.
   - Enable ChatGPT interaction for user commands and feedback.

2. **Technical Feasibility**:
   - Leverage Microsoft Graph API for data access.
   - Integrate ChatGPT API for conversational interaction.

3. **Constraints**:
   - Consider task durations, working hours, and schedule conflicts.

4. **Finalise Scope**:
   - Start with basic scheduling functionality and add advanced features later.

---

### Phase 2: Architecture Design
1. **System Components**:
   - **Data Layer**: Fetch tasks and calendar data via APIs.
   - **Logic Layer**: Prioritize tasks and allocate time slots.
   - **Interaction Layer**: Handle communication through ChatGPT.

2. **Flow Design**:
   - **Input**: Fetch data from ToDo and Calendar.
   - **Processing**: Prioritize tasks and match them to free time slots.
   - **Output**: Update the calendar and provide feedback to the user.

3. **Technology Stack**:
   - Backend: .NET Core for API and logic implementation.
   - ChatGPT: OpenAI API for conversational capabilities.

---

### Phase 3: Development
#### Step 1: Core Integrations
- Implement API connections to Microsoft ToDo and Calendar using Microsoft Graph API.
- Fetch and display tasks and events.

#### Step 2: Scheduling Logic
- Develop logic to prioritize tasks based on urgency and deadlines.
- Create an algorithm for time slot allocation.

#### Step 3: ChatGPT Interaction
- Integrate ChatGPT API for user commands like:
  - "What does my schedule look like?"
  - "Move Task X to tomorrow."

#### Step 4: Calendar Updates
- Automate adding tasks to the calendar.
- Handle conflicts like overlapping events or unavailable time slots.

---

### Phase 4: Testing
1. **Unit Testing**: Test individual components (e.g., task fetching, scheduling).
2. **Integration Testing**: Ensure API interactions and scheduling logic work seamlessly.
3. **User Simulation**: Simulate real-world use cases and adjust functionality.

---

### Phase 5: Deployment
1. **Prototype Launch**:
   - Deploy locally or on a private server for testing.
   - Gather feedback on usability and functionality.

2. **Iterate**:
   - Address bugs and refine based on feedback.
   - Improve scheduling logic and overall user experience.

3. **Production Release**:
   - Deploy to a cloud platform (e.g., Azure) for scalability.
   - Ensure secure API access and permissions.

---

### Phase 6: Continuous Improvements
- **AI Enhancements**: Add machine learning for adapting to user preferences.
- **Notifications**: Implement reminders and updates via email or apps.
- **Multi-Device Support**: Ensure seamless use across desktop and mobile platforms.

---

## Documentation
- [Requirements](docs/requirements.md): Detailed feature definitions and planning for Ordo.
