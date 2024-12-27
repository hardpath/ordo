using System.Text.Json;
using Ordo.Models;

namespace Ordo.Core
{
    internal static class ProjectsArchiver
    {
        private const string FilePath = "projects.json";

        internal static ProjectsData LoadData()
        {
            ProjectsData? projectsData;

            try {
                if (!File.Exists(FilePath)) {
                    Console.WriteLine($"[WARNING] File ${FilePath} not found; Returning an empty list.");
                    return new ProjectsData();
                }

                string json = File.ReadAllText(FilePath);

                projectsData = JsonSerializer.Deserialize<ProjectsData>(json);
                if (projectsData == null) {
                    Console.WriteLine("[WARNING] Deserialization returned null; creating a new empty ProjectsData object.");
                    projectsData = new ProjectsData();
                }
            }
            catch (JsonException ex) {
                Console.WriteLine($"[ERROR] Failed to deserialize ${FilePath}: {ex.Message}");
                projectsData = new ProjectsData();
            }

            return projectsData;
        }

        internal static void SaveData(ProjectsData config)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            string json = JsonSerializer.Serialize(config, options);
            File.WriteAllText(FilePath, json);
        }

        internal static void SetTaskDuration(string projectId, string taskId, int duration)
        {
            // Load durations once
            ProjectsData config = LoadData();

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
            SaveData(config);
        }

        internal static int? GetTaskDuration(string projectId, string taskId)
        {
            ProjectsData config = LoadData();

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
