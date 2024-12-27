using Ordo.Core;

namespace Ordo.Commands
{
    internal static class ResetDurationsCommand
    {
        public static void Execute()
        {
            try {
                Console.WriteLine("[WARNING] This will reset all task durations to 0.");
                Console.Write("Are you sure you want to continue? Type 'yes' to confirm or 'no' to cancel: ");
                string? confirmation = Console.ReadLine();

                if (!string.Equals(confirmation, "yes", StringComparison.OrdinalIgnoreCase)) {
                    Console.WriteLine("[INFO] Reset operation canceled.");
                    return; // Exit the command
                }

                var jsonData = ProjectsArchiver.LoadData();

                foreach (var project in jsonData.Projects) {
                    if (project.ToDelete) {
                        continue;
                    }

                    foreach (var task in project.Tasks) {
                        if (task.ToDelete) {
                            continue;
                        }

                        task.Duration = 0;
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
    }
}
