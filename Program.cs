using Microsoft.Extensions.Configuration;
using Ordo.Models;

namespace Ordo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) // Sets the base path for config.json
                .AddJsonFile("config.json", optional: true, reloadOnChange: true) // Adds config.json
                .Build();

            // Bind the configuration to AppSettings
            var appSettings = new AppSettings();
            configuration.GetSection("AppSettings").Bind(appSettings);

            // Test: Display loaded values
            Console.WriteLine("Configuration Loaded:");
            Console.WriteLine($"ClientId: {appSettings.ClientId}");
            Console.WriteLine($"TenantId: {appSettings.TenantId}");
            Console.WriteLine($"ClientSecret: {appSettings.ClientSecret}");
        }
    }
}
