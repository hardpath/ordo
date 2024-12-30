
using Microsoft.Extensions.Configuration;
using Ordo.Api;
using Ordo.Models;

namespace Ordo.Commands
{
    internal static class DeleteEventsCommand
    {
        public static async Task ExecuteAsync()
        {
            #region AppSettings
            AppSettings appSettings;
            try {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("config.json", optional: true, reloadOnChange: true)
                    .Build();

                appSettings = new AppSettings();
                configuration.GetSection("AppSettings").Bind(appSettings);

                if (!appSettings.Validate(out string errorMessage)) {
                    Console.WriteLine($"[ERROR] {errorMessage}");
                    return;
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] Failed to load application settings: {ex.Message}");
                return;
            }
            #endregion

            try {
                Console.WriteLine("[INFO] Deleting ORDO events...");

                var graphClientHelper = GraphClientHelper.GetInstance(appSettings);
                bool deltedEvents = await graphClientHelper.DeleteEventsFromCalendarAsync();

                if (deltedEvents) {
                    Console.WriteLine("[INFO] All ORDO events deleted.");
                }
                else {
                    Console.WriteLine("[INFO] No ORDO events to be deleted.");
                }

}
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] Failed to delete events: {ex.Message}");
            }

        }
    }
}
