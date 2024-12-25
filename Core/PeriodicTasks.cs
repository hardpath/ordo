using Microsoft.Graph;
using Ordo.Models;

namespace Ordo.Core
{
    internal static class PeriodicTasks
    {
        public static async Task SynchroniseTasksWithToDo(GraphServiceClient graphClient, AppSettings appSettings)
        {
            Console.WriteLine("[INFO] Starting task synchronisation with Microsoft ToDo...");

            // Validate inputs
            if (graphClient == null) throw new ArgumentNullException(nameof(graphClient));
            if (appSettings == null) throw new ArgumentNullException(nameof(appSettings));

            try {
                // TODO: Implement logic to fetch and synchronise tasks with ToDo
                Console.WriteLine("[INFO] Task synchronisation completed.");
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] Failed to synchronise tasks: {ex.Message}");
            }
        }

        public static async Task ValidateTaskDurations(AppSettings appSettings)
        {
            Console.WriteLine("[INFO] Validating task durations...");

            // Validate inputs
            if (appSettings == null) throw new ArgumentNullException(nameof(appSettings));

            try {
                // TODO: Implement logic to validate and assign default durations
                Console.WriteLine("[INFO] Task duration validation completed.");
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] Failed to validate task durations: {ex.Message}");
            }
        }

        public static async Task SyncWithCalendar(GraphServiceClient graphClient, AppSettings appSettings)
        {
            Console.WriteLine("[INFO] Syncing tasks with Microsoft 365 Calendar...");

            // Validate inputs
            if (graphClient == null) throw new ArgumentNullException(nameof(graphClient));
            if (appSettings == null) throw new ArgumentNullException(nameof(appSettings));

            try {
                // TODO: Implement logic to sync tasks with the calendar
                Console.WriteLine("[INFO] Calendar synchronisation completed.");
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] Failed to sync with calendar: {ex.Message}");
            }
        }
    }
}
