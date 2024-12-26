
namespace ordo.Models
{
    internal class ProjectsData
    {
        internal List<Project> Projects { get; set; } = new List<Project>();
    }

    internal class Project
    {
        internal string Id { get; set; } = string.Empty;
        internal string Name { get; set; } = string.Empty;
        internal List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }

    internal class ProjectTask
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Duration { get; set; } = 0;
        public bool IsMissing { get; set; } = false;
    }
}
