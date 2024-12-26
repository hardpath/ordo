using Microsoft.Graph;
using Ordo.Models;

namespace Ordo.Core
{
    internal static class PeriodicTasks
    {
        public static async Task SynchroniseTasksWithToDo(GraphServiceClient graphClient, AppSettings appSettings)
        {
            Console.WriteLine("[INFO] Starting task synchronisation with Microsoft ToDo...");
            // TODO: Implement logic to fetch and synchronise tasks with ToDo
        }

        public static async Task ValidateTaskDurations(AppSettings appSettings)
        {
            Console.WriteLine("[INFO] Validating task durations...");
            // TODO: Implement logic to validate and assign default durations
        }

        public static async Task CheckPastCalendarEvents(GraphServiceClient graphClient, AppSettings appSettings)
        {
            Console.WriteLine("[INFO] Checking past calendar events...");
            // TODO: Implement logic to identify past events and mark tasks for review
        }
    }
}
