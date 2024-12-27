
namespace Ordo.Models
{
    public class ProjectsData
    {
        public List<Project> Projects { get; set; } = new List<Project>();

        public void Add(Project project)
        {
            if (project != null && !ProjectExists(project.Id)) {
                Projects.Add(project);
            }
        }

        public int GetTasksWithoutDurationCount()
        {
            int count = 0;

            foreach (var project in Projects) {
                if (project.ToDelete) {
                    continue;
                }

                foreach (var task in project.Tasks) {
                    if ((task.Duration == 0) && (!task.ToDelete)) {
                        count++;
                    }
                }
            }

            return count;
        }

        public DateTime GetLatestDueDate()
        {
            DateTime latestDueDate = DateTime.MinValue;

            foreach (var project in Projects) {
                if (project.ToDelete)
                    continue;
                foreach (var task in project.Tasks) {
                    if ( (task.DueDate > latestDueDate) && (!task.ToDelete) ) {
                        latestDueDate = task.DueDate;
                    }
                }
            }

            return latestDueDate;
        }

        private bool ProjectExists(string projectId)
        {
            foreach (var existingProject in Projects) {
                if (existingProject.Id == projectId) {
                    return true;
                }
            }
            return false;
        }
    }

    public class Project
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool ToDelete { get; set; } = false;
        public List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();

        public void Add(ProjectTask ptask)
        {
            if (ptask != null && !ProjectExists(ptask.Id)) {
                Tasks.Add(ptask);
            }
        }

        private bool ProjectExists(string taskId)
        {
            foreach (var existingTask in Tasks) {
                if (existingTask.Id == taskId) {
                    return true;
                }
            }
            return false;
        }
    }

    public class ProjectTask
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public DateTime DueDate { get; set; } = DateTime.MinValue;
        public int Duration { get; set; } = 0;
        public bool ToDelete { get; set; } = false;
    }
}
