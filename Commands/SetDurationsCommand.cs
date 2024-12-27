using Ordo.Core;

namespace Ordo.Commands
{
    internal static class SetDurationsCommand
    {
        public static void Execute()
        {
            try {
                var jsonData = ProjectsArchiver.LoadData();

                int taskCount = jsonData.GetTasksWithoutDurationCount();
                if (taskCount == 0) {
                    Console.WriteLine($"[INFO] All tasks have a duration set; operation cancelled.");
                    return;
                }

                ShowHelp();

                Console.WriteLine($"\n[INFO] {taskCount} tasks without duration.");

                foreach (var project in jsonData.Projects) {
                    if (project.ToDelete) {
                        continue;
                    }

                    foreach (var task in project.Tasks) {
                        if ((task.Duration > 0) || (task.ToDelete)) {
                            continue;
                        }

                        while (true) {
                            Console.WriteLine($"\nPROJECT: {project.Name}");
                            Console.WriteLine($"TASK: {task.Name}");
                            Console.Write("Duration: ");
                            string? input = Console.ReadLine();

                            if (string.Equals(input, "help", StringComparison.OrdinalIgnoreCase)) {
                                ShowHelp();
                            }

                            else if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase)) {
                                Console.WriteLine("[WARNING] This operation needs to be cancelled first.");
                            }

                            else if (string.Equals(input, "skip", StringComparison.OrdinalIgnoreCase)) {
                                Console.WriteLine("[INFO] Tasks skipped.");
                                break;
                            }

                            else if (string.Equals(input, "cancel", StringComparison.OrdinalIgnoreCase)) {
                                Console.WriteLine("[WARNING] Update cancelled; no changes were saved.");
                                return;
                            }

                            else if (string.Equals(input, "save", StringComparison.OrdinalIgnoreCase)) {
                                ProjectsArchiver.SaveData(jsonData);
                                Console.WriteLine("[INFO] Durations have been updated successfully.");
                            }

                            else if (TryParseDuration(input, out int duration)) {
                                task.Duration = duration;
                                Console.WriteLine($"[INFO] Duration set to {duration} minutes for task: {task.Name}.");
                                break;
                            }
                            else {
                                Console.WriteLine("[ERROR] Invalid input.");
                            }
                        }
                    }
                }

                // Save updated durations back to the file
                ProjectsArchiver.SaveData(jsonData);
                Console.WriteLine("[INFO] Durations have been updated successfully.");
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] An error occurred while setting durations: {ex.Message}");
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Duration:");
            Console.WriteLine("  X    For X minutes.");
            Console.WriteLine("  Xh   For X hours.");
            Console.WriteLine("  Xd   For X workign days (8 hours).");
            Console.WriteLine("");
            Console.WriteLine("Options:");
            Console.WriteLine("  - skip    Skip to the next task.");
            Console.WriteLine("  - cancel  Cancel update entirely; no data will be saved.");
            Console.WriteLine("  - save    Save durations entered so far.");
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
