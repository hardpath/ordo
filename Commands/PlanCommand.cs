using Microsoft.Extensions.Configuration;
using ordo.Models;
using Ordo.Api;
using Ordo.Core;
using Ordo.Models;
using System.Text.Json;

namespace Ordo.Commands
{
    internal static class PlanCommand
    {
        public static async Task ExecuteAsync()
        {
            try {
                string json = Consolidate();

                //File.WriteAllText("consolidated_data.json", json);

                string jsonResponse;
                try { 
                    jsonResponse = await OpenAiHelper.GetScheduleAsync(json);
                }
                catch (Exception ex) {
                    Console.WriteLine($"[ERROR] Failed to retrieve schedule from OpenAI: {ex.Message}");
                    return;
                }

                string notes;
                List<EventData> scheduledEvents;
                List<ProblematicTask> problematicTasks;

                try {
                    ProcessResponse(jsonResponse, out notes, out scheduledEvents, out problematicTasks);
                }
                catch (Exception ex) {
                    Console.WriteLine($"[ERROR] Failed to process response: {ex.Message}");
                    return;
                }

                // Now you can use the variables
                Console.WriteLine("Scheduled Events: " + scheduledEvents.Count);
                Console.WriteLine("Problematic Tasks: " + problematicTasks.Count);
                Console.WriteLine("Notes: " + notes);

                if (!scheduledEvents.Any()) {
                    Console.WriteLine("[INFO] No events scheduled.");
                    return;
                }

                while (true) {
                    if (problematicTasks.Count > 0) {
                        Console.WriteLine("\nADD events to the calendar, SEE problematic tasks or CANCEL?");
                    }
                    else {
                        Console.WriteLine("\nADD events to the calendar or CANCEL?");
                    }

                    string? confirmation = Console.ReadLine();

                    if (string.Equals(confirmation, "see", StringComparison.OrdinalIgnoreCase)) {
                        if (problematicTasks.Count == 0) {
                            Console.WriteLine("[INFO] There are no problematic tasks.");
                            continue;
                        }

                        foreach (var problematicTask in problematicTasks) {
                            Console.WriteLine($"{problematicTask.Project} - {problematicTask.Task}");
                        }
                    }

                    if (string.Equals(confirmation, "cancel", StringComparison.OrdinalIgnoreCase)) {
                        Console.WriteLine("[INFO] Scheduling operation canceled.");
                        return;
                    }

                    if (string.Equals(confirmation, "add", StringComparison.OrdinalIgnoreCase)) {
                        break;
                    }
                }

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
                    var graphClientHelper = GraphClientHelper.GetInstance(appSettings);
                    await graphClientHelper.AddEventsToCalendarAsync(scheduledEvents);
                    Console.WriteLine("[INFO] All events successfully added to the calendar.");
                }
                catch (Exception ex) {
                    Console.WriteLine($"[ERROR] Failed to add events to the calendar: {ex.Message}");
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] Unexpected error occurred: {ex.Message}");
            }
        }

        #region Private
        private static string Consolidate()
        {
            string json = string.Empty;

            try {
                Console.WriteLine($"[DEBUG] Consolidating data...");

                //string ScheduleFilePath = "schedule.json";

                #region Load projects from projects.json
                var projectsData = ProjectsArchiver.LoadData();
                var projects = new List<ProjectData>();

                foreach (var project in projectsData.Projects) {
                    if (project.ToDelete) continue;

                    var projectTasks = new List<TaskData>();
                    foreach (var task in project.Tasks) {
                        if (task.ToDelete) continue;
                        //TODO: 1-Ignore overdue tasks

                        projectTasks.Add(new TaskData {
                            Id = task.Id,
                            Name = task.Name,
                            Duration = task.Duration,
                            DueDate = task.DueDate
                        });
                    }

                    projects.Add(new ProjectData {
                        Id = project.Id,
                        Name = project.Name,
                        Tasks = projectTasks
                    });
                }
                #endregion

                #region Load events from events.json
                var eventsData = EventsArchiver.LoadData();
                var events = new List<EventData>();

                foreach (var calendarEvent in eventsData.Events) {
                    events.Add(new EventData {
                        Id = calendarEvent.Id,
                        Subject = calendarEvent.Subject,
                        Start = calendarEvent.Start,
                        End = calendarEvent.End,
                        IsOrdoCreated = calendarEvent.IsOrdoCreated
                    });
                }
                #endregion

                // Combine into ScheduleRequest
                var scheduleRequest = new ScheduleRequest {
                    Projects = projects,
                    Events = events
                };

                // Serialize to JSON and save to file
                var options = new JsonSerializerOptions { WriteIndented = true };
                json = JsonSerializer.Serialize(scheduleRequest, options);
                //File.WriteAllText(ScheduleFilePath, json);

                Console.WriteLine($"[DEBUG] Schedule data consolidated.");
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] An error occurred while consolidating schedule data: {ex.Message}");
            }

            return json;
        }

        private static void ProcessResponse(
            string jsonResponse,
            out string notes,
            out List<EventData> scheduledEvents,
            out List<ProblematicTask> problematicTasks)
        {
            // Initialize output variables
            notes = string.Empty;
            scheduledEvents = new List<EventData>();
            problematicTasks = new List<ProblematicTask>();

            // Remove markdown delimiters if present
            jsonResponse = jsonResponse.Trim();
            if (jsonResponse.StartsWith("```json")) {
                jsonResponse = jsonResponse.Substring(7); // Remove "```json"
            }
            if (jsonResponse.EndsWith("```")) {
                jsonResponse = jsonResponse.Substring(0, jsonResponse.Length - 3); // Remove "```"
            }

            try {
                var scheduleResponse = JsonSerializer.Deserialize<ScheduleResponse>(jsonResponse, new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                });

                if (scheduleResponse != null) {
                    // Assign to output variables
                    notes = scheduleResponse.Notes;
                    scheduledEvents = scheduleResponse.ScheduledEvents;
                    problematicTasks = scheduleResponse.ProblematicTasks;
                }
                else {
                    Console.WriteLine("[ERROR] Failed to deserialize schedule response.");
                }
            }
            catch (JsonException ex) {
                Console.WriteLine($"[ERROR] JSON deserialization error: {ex.Message}");
            }
        }
        #endregion
    }
}
