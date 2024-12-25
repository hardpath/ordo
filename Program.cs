using Microsoft.Extensions.Configuration;
using Ordo.Api;
using Ordo.Models;

namespace Ordo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try {
                // Load configuration
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory) // Sets the base path for config.json
                    .AddJsonFile("config.json", optional: true, reloadOnChange: true) // Adds config.json
                    .Build();

                // Bind the configuration to AppSettings
                var appSettings = new AppSettings();
                configuration.GetSection("AppSettings").Bind(appSettings);

                // Get an authenticated Graph client
                var graphClient = GraphClientHelper.GetAuthenticatedGraphClient(appSettings);

                // Test: Verify the Graph client was created successfully
                Console.WriteLine("Authenticated Graph Client Created!");

                // Fetch tasks from Microsoft ToDo
                await GraphClientHelper.FetchTasksAsync(graphClient, "eoliver@hardpath.co.uk"); // Replace with a valid user ID or email

            }
            catch (Exception ex) {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
