namespace Ordo.Models
{
    public class ScheduleRequest
    {
        public List<ProjectData> Projects { get; set; } = new List<ProjectData>();
        public List<EventData> Events { get; set; } = new List<EventData>();
    }

    public class ProjectData
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<TaskData> Tasks { get; set; } = new List<TaskData>();
    }

    public class TaskData
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Duration { get; set; } = 0;
        public DateTime DueDate { get; set; } = DateTime.MinValue;
    }

    public class EventData
    {
        public string Id { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsOrdoCreated { get; set; } = false;
    }
}
