using Microsoft.Extensions.Configuration;
using Microsoft.Graph.Models;
using Ordo.Api;
using Ordo.Core;
using Ordo.Models;


namespace Ordo.Commands
{
    internal static class GetEventsCommand
    {
        public static async Task Execute()
        {
            Console.Write("[INFO] Fetching events from Microsoft Calendar ");

            try {
                #region AppSettings
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
                #endregion

                //Set start date
                var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                //Set end date (from tasks in the json file)
                var tasks = ProjectsArchiver.LoadData();
                var endDate = tasks.GetLatestDueDate();

                Console.WriteLine($"from {startDate.ToString("dd-MM-yyyy")} to {endDate.AddMonths(2).ToString("dd-MM-yyyy")}...");

                var calendarData = await GetEventsFromCalendar(appSettings, startDate, endDate.AddMonths(2));

                //var jsonData = EventsArchiver.LoadData();

                EventsArchiver.SaveData(calendarData);

                Console.WriteLine("[INFO] Events successfully fetched and saved to .json file.");
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] An error occurred while fetching events: {ex.Message}");
            }
        }

        #region Private Methods
        private static async Task<EventsData> GetEventsFromCalendar(AppSettings appSettings, DateTime startDate, DateTime endDate)
        {
            var graphClientHelper = GraphClientHelper.GetInstance(appSettings);

            // Fetch events from Microsoft Calendar
            List<RawEvent> calendarEvents = await graphClientHelper.GetCalendarEventsAsync(startDate, endDate);

            // Map data to EventsData
            var events = new EventsData();
            foreach (var calendarEvent in calendarEvents) {
                if (calendarEvent.id == null) {
                    Console.WriteLine("[WARNING] Null ID in calendar event.");
                    continue;
                }

                if (calendarEvent.start == null) {
                    Console.WriteLine($"[WARNING] Event '{calendarEvent.subject}' with null start date; event ignored.");
                    continue;
                }

                if (calendarEvent.end == null) {
                    Console.WriteLine($"[WARNING] Event '{calendarEvent.subject}' with null end date; event ignored.");
                    continue;
                }
                events.Add(new CalendarEvent {
                    Id = calendarEvent.id,
                    Subject = calendarEvent.subject ?? string.Empty,
                    Start = calendarEvent.start.ToUTCDateTime(),
                    End = calendarEvent.end.ToUTCDateTime(),
                    IsOrdoCreated = calendarEvent.subject?.Contains("[ORDO]") ?? false
                });
            }

            return events;
        }
        #endregion
    }
}
