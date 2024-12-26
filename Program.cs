using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Ordo.Api;
using Ordo.Models;
using Ordo.Core;
using ordo.Models;

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

                // Get an authenticated Graph client
                var graphClient = GraphClientHelper.GetAuthenticatedGraphClient(appSettings);

                // Execute periodic tasks
                // await RunTasks(graphClient, appSettings);

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

        static async Task RunTasks(GraphServiceClient graphClient, AppSettings appSettings)
        {
            Console.WriteLine("Starting periodic tasks...");

            // Synchronise tasks with Microsoft ToDo
            await PeriodicTasks.SynchroniseTasksWithToDo(graphClient, appSettings);

            // Validate task durations
            await PeriodicTasks.ValidateTaskDurations(appSettings);

            // Sync with Microsoft 365 Calendar
            await PeriodicTasks.CheckPastCalendarEvents(graphClient, appSettings);

            Console.WriteLine("Periodic tasks completed successfully.");
        }
    }
}
