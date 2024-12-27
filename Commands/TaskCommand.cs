using Microsoft.Extensions.Configuration;
using Ordo.Core;
using Ordo.Models;

namespace Ordo.Commands
{
    internal static class TaskCommand
    {
        public static async Task FetchTasks()
        {
            Console.WriteLine("Fetching tasks from Microsoft ToDo...");
            try {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("config.json", optional: true, reloadOnChange: true)
                    .Build();

                var appSettings = new AppSettings();
                configuration.GetSection("AppSettings").Bind(appSettings);

                if (!appSettings.Validate(out string errorMessage)) {
                    Console.WriteLine($"[ERROR] {errorMessage}");
                    return;
                }

                await SyncTasks.SynchroniseProjectsWithToDo(appSettings);
                Console.WriteLine("Tasks fetched and saved to projects.json successfully.");
            }
            catch (Exception ex) {
                Console.WriteLine($"An error occurred while fetching tasks: {ex.Message}");
            }
        }
    }
}
