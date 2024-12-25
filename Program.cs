using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Ordo.Api;
using Ordo.Models;
using Ordo.Core;

namespace Ordo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try {
                // Load configuration
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("config.json", optional: true, reloadOnChange: true)
                    .Build();

                // Bind the configuration to AppSettings
                var appSettings = new AppSettings();
                configuration.GetSection("AppSettings").Bind(appSettings);

                // Get an authenticated Graph client
                var graphClient = GraphClientHelper.GetAuthenticatedGraphClient(appSettings);

                // Execute periodic tasks
                await RunTasks(graphClient, appSettings);
            }
            catch (Exception ex) {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static async Task RunTasks(GraphServiceClient graphClient, AppSettings appSettings)
        {
            Console.WriteLine("Starting periodic tasks...");

            // Synchronise tasks with Microsoft ToDo
            await PeriodicTasks.SynchroniseTasksWithToDo(graphClient, appSettings);

            // Validate task durations
            await PeriodicTasks.ValidateTaskDurations(appSettings);

            // Sync with Microsoft 365 Calendar
            await PeriodicTasks.SyncWithCalendar(graphClient, appSettings);

            Console.WriteLine("Periodic tasks completed successfully.");
        }
    }
}
