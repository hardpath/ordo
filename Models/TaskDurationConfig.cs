
namespace ordo.Models
{
    internal class TaskDurationConfig
    {
        internal List<Project> Projects { get; set; } = new List<Project>();
    }

    internal class Project
    {
        internal string Name { get; set; } = string.Empty;
        internal List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }

    internal class ProjectTask
    {
        public string Name { get; set; } = string.Empty;
        public int Duration { get; set; } = 0;
        public bool IsMissing { get; set; } = false;
    }
}
