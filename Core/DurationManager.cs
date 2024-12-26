using System.Text.Json;
using ordo.Models;

namespace Ordo.Core
{
    internal static class DurationManager
    {
        private const string FilePath = "durations.json";

        internal static TaskDurationConfig LoadDurations()
        {
            if (!File.Exists(FilePath)) {
                return new TaskDurationConfig();
            }

            string json = File.ReadAllText(FilePath);
            TaskDurationConfig config = JsonSerializer.Deserialize<TaskDurationConfig>(json) ?? new TaskDurationConfig();

            return config;
        }

        internal static void SaveDurations(TaskDurationConfig config)
        {
            string json = JsonSerializer.Serialize(
                config,
                new JsonSerializerOptions { WriteIndented = true }
            );
            File.WriteAllText(FilePath, json);
        }

        internal static void UpdateTaskDuration(string projectName, string taskName, int duration)
        {
            TaskDurationConfig config = LoadDurations();

            Project? project = null;
            foreach (Project proj in config.Projects) {
                if (proj.Name == projectName) {
                    project = proj;
                    break;
                }
            }

            if (project == null) {
                project = new Project { Name = projectName };
                config.Projects.Add(project);
            }

            ProjectTask? task = null;
            foreach (ProjectTask t in project.Tasks) {
                if (t.Name == taskName) {
                    task = t;
                    break;
                }
            }

            if (task == null) {
                task = new ProjectTask { Name = taskName, Duration = duration };
                project.Tasks.Add(task);
            }
            else {
                task.Duration = duration;
            }

            SaveDurations(config);
        }

        internal static int? GetTaskDuration(string projectName, string taskName)
        {
            TaskDurationConfig config = LoadDurations();

            foreach (Project project in config.Projects) {
                if (project.Name == projectName) {
                    foreach (ProjectTask task in project.Tasks) {
                        if (task.Name == taskName) {
                            return task.Duration;
                        }
                    }
                }
            }

            return null;
        }
    }
}
