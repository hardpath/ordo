
namespace Ordo.Models
{
    public class ProjectsData
    {
        public List<Project> Projects { get; set; } = new List<Project>();
    }

    public class Project
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsMissing { get; set; } = false;
        public List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }

    public class ProjectTask
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime DueDate { get; set; } = DateTime.MinValue;
        public int Duration { get; set; } = 0;
        public bool IsMissing { get; set; } = false;
    }
}
