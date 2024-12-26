using System.Text.Json;
using ordo.Models;

namespace Ordo.Core
{
    internal static class ProjectsManager
    {
        private const string FilePath = "projects.json";

        internal static ProjectsData LoadDurations()
        {
            if (!File.Exists(FilePath)) {
                return new ProjectsData();
            }

            string json = File.ReadAllText(FilePath);
            ProjectsData config = JsonSerializer.Deserialize<ProjectsData>(json) ?? new ProjectsData();

            return config;
        }

        internal static void SaveDurations(ProjectsData config)
        {
            string json = JsonSerializer.Serialize(
                config,
                new JsonSerializerOptions { WriteIndented = true }
            );
            File.WriteAllText(FilePath, json);
        }

        internal static void UpdateTaskDuration(string projectId, string taskId, int duration)
        {
            // Load durations once
            ProjectsData config = LoadDurations();

            // Find or create the project
            var project = config.Projects.FirstOrDefault(p => p.Id == projectId);
            if (project == null) {
                project = new Project { Id = projectId, Name = projectId };
                config.Projects.Add(project);
            }

            // Find or create the task
            var task = project.Tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null) {
                task = new ProjectTask { Id = taskId, Duration = duration };
                project.Tasks.Add(task);
            }
            else {
                task.Duration = duration;
            }

            // Save updated durations back to the file
            SaveDurations(config);
        }

        internal static int? GetTaskDuration(string projectId, string taskId)
        {
            ProjectsData config = LoadDurations();

            foreach (Project project in config.Projects) {
                if (project.Id == projectId) {
                    foreach (ProjectTask task in project.Tasks) {
                        if (task.Id == taskId) {
                            return task.Duration;
                        }
                    }
                }
            }

            return null;
        }
    }
}
