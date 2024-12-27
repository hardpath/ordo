using Ordo.Core;
using Ordo.Models;
using System.Threading.Tasks;

namespace Ordo.Commands
{
    internal static class SetDurationCommand
    {
        public static void Execute()
        {
            try {
                var jsonData = ProjectsArchiver.LoadData();

                var selectableProjects = new List<Project>();

                // SELECT PROJECT
                foreach (var project in jsonData.Projects) {
                    if (project.ToDelete) {
                        continue;
                    }

                    foreach (var task in project.Tasks) {
                        if (!task.ToDelete) {
                            selectableProjects.Add(project);
                            break;
                        }
                    }
                }

                if (selectableProjects.Count == 0) {
                    Console.WriteLine("[INFO] No projects available for selection.");
                    return;
                }

                Console.WriteLine("Select a project by entering its number:");
                for (int i = 0; i < selectableProjects.Count; i++) {
                    Console.WriteLine($"[{i + 1}] {selectableProjects[i].Name}");
                }
                Console.WriteLine("'cancel' to cancel operation");

                int projectNumber;
                while (true) {
                    // Prompt the user to select a project
                    Console.Write("Project: ");
                    string? input = Console.ReadLine();

                    if (string.Equals(input, "cancel", StringComparison.OrdinalIgnoreCase)) {
                        Console.WriteLine("[WARNING] Update cancelled; no changes were saved.");
                        return;
                    }

                    if (!int.TryParse(input, out projectNumber) || projectNumber < 1 || projectNumber > selectableProjects.Count) {
                        Console.WriteLine("[ERROR] Invalid project selection.");
                    }
                    else {
                        break;
                    }
                }

                var selectedProject = selectableProjects[projectNumber - 1];

                // SELECT TASK
                Console.WriteLine("Select a task by entering its alias:");
                foreach (var task in selectedProject.Tasks) {
                    if (!task.ToDelete) {
                        Console.WriteLine($"{task.Alias}: {task.Name}, Duration: {task.Duration} minutes");
                    }
                }
                Console.WriteLine("'cancel' to cancel operation");

                ProjectTask? selectedTask = null;
                while (true) {
                    // Prompt the user to select a project
                    Console.Write("Task: ");
                    string? input = Console.ReadLine();

                    if (string.Equals(input, "cancel", StringComparison.OrdinalIgnoreCase)) {
                        Console.WriteLine("[WARNING] Update cancelled; no changes were saved.");
                        return;
                    }

                    foreach (var task in selectedProject.Tasks) {
                        if (task.Alias.Equals(input, StringComparison.OrdinalIgnoreCase)) {
                            selectedTask = task;
                            break;
                        }
                    }

                    if (selectedTask == null) {
                        Console.WriteLine("[ERROR] Invalid alias.");
                        continue;
                    }

                    break;
                }
                Console.WriteLine($"Task selected: {selectedTask.Name}");

                // DURATION
                while (true) {
                    Console.Write("Duration: ");
                    string? input = Console.ReadLine();

                    if (string.Equals(input, "cancel", StringComparison.OrdinalIgnoreCase)) {
                        Console.WriteLine("[INFO] Operation canceled.");
                        return;
                    }

                    if (TryParseDuration(input, out int newDuration)) {
                        selectedTask.Duration = newDuration;
                        Console.WriteLine($"[INFO] Duration set to {newDuration} minutes for task: {selectedTask.Name}.");
                        break; // Exit the loop after successfully setting the duration
                    }

                    Console.WriteLine("[ERROR] Invalid input.");
                }

                // Save updated durations back to the file
                ProjectsArchiver.SaveData(jsonData);
                Console.WriteLine("[INFO] Durations have been updated successfully.");
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] An error occurred while setting durations: {ex.Message}");
            }
        }

        private static bool TryParseDuration(string? input, out int duration)
        {
            duration = 0;

            if (string.IsNullOrWhiteSpace(input)) {
                return false;
            }

            // Handle working days input
            if (input.EndsWith("d", StringComparison.OrdinalIgnoreCase)) {
                if (int.TryParse(input.TrimEnd('d', 'D'), out int days)) {
                    duration = days * 480; // Convert days to minutes
                    return true;
                }
            }

            if (input.EndsWith("h", StringComparison.OrdinalIgnoreCase)) {
                if (int.TryParse(input.TrimEnd('h', 'H'), out int hours)) {
                    duration = hours * 60; // Convert hours to minutes
                    return true;
                }
            }

            // Handle minutes input
            return int.TryParse(input, out duration) && duration > 0;
        }
    }
}
