using Microsoft.Extensions.Configuration;
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

                // Bind the OpenAiSettings section
                var openAiSettings = new OpenAiSettings();
                configuration.GetSection("OpenAiSettings").Bind(openAiSettings);

                // Execute periodic tasks
                await RunTasks(appSettings);

                // Test the OpenAI API
                Console.WriteLine("Testing OpenAI integration...");
                if (string.IsNullOrEmpty(openAiSettings.ApiKey)) {
                    Console.WriteLine("OpenAI API Key is missing. Please check your configuration.");
                    return;
                }
                OpenAiHelper.TestOpenAiApi(openAiSettings.ApiKey);
            }
            catch (Exception ex) {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static async Task RunTasks(AppSettings appSettings)
        {
            Console.WriteLine("Starting periodic tasks...");

            // Synchronise tasks with Microsoft ToDo
            await PeriodicTasks.SynchroniseTasksWithToDo(appSettings);

            // Validate task durations
            await PeriodicTasks.ValidateTaskDurations();

            // Sync with Microsoft 365 Calendar
            await PeriodicTasks.CheckPastCalendarEvents(appSettings);

            Console.WriteLine("Periodic tasks completed successfully.");
        }
    }
}
