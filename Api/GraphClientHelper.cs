using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Ordo.Core;
using Ordo.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ordo.Api
{
    internal class GraphClientHelper
    {
        private static GraphClientHelper? _instance;
        private static readonly object _lock = new object();

        private GraphServiceClient _graphClient;
        private string? _userId;

        AppSettings _appSettings;

        //TODO: 2-Call GraphClientHelper only once
        private GraphClientHelper(AppSettings appSettings)
        {
            _appSettings = appSettings;

            _graphClient = new GraphServiceClient(
                new ClientSecretCredential(
                    appSettings.TenantId,
                    appSettings.ClientId,
                    appSettings.ClientSecret
                )
            );
            _userId = appSettings.UserId;
        }

        internal static GraphClientHelper GetInstance(AppSettings appSettings)
        {
            if (_instance == null) {
                lock (_lock) {
                    if (_instance == null) {
                        _instance = new GraphClientHelper(appSettings);
                    }
                }
            }
            return _instance;
        }

        internal static GraphClientHelper GetInstance()
        {
            if (_instance == null) {
                throw new InvalidOperationException("GraphClientHelper is not initialized. Call GetInstance with AppSettings first.");
            }
            return _instance;
        }

        #region ToDo
        internal async Task<List<TodoTaskList>> GetTaskListsAsync()
        {
            try {
                var taskLists = await _graphClient.Users[_userId].Todo.Lists.GetAsync();
                return taskLists?.Value?.ToList() ?? new List<TodoTaskList>();
            }
            catch (ServiceException ex) {
                Console.WriteLine($"[ERROR] Graph API error while fetching task lists: {ex.Message}");
                return new List<TodoTaskList>();
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] Unexpected error while fetching task lists: {ex.Message}");
                return new List<TodoTaskList>();
            }
        }

        internal async Task<List<TodoTask>> GetTasksAsync(string listId)
        {
            try {
                var tasks = await _graphClient.Users[_userId].Todo.Lists[listId].Tasks.GetAsync();
                return tasks?.Value?.ToList() ?? new List<TodoTask>();
            }
            catch (ServiceException ex) {
                Console.WriteLine($"[ERROR] Graph API error while fetching tasks for list {listId}: {ex.Message}");
                return new List<TodoTask>();
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] Unexpected error while fetching tasks for list {listId}: {ex.Message}");
                return new List<TodoTask>();
            }
        }
        #endregion

        #region Calendar
        internal async Task<List<RawEvent>> GetCalendarEventsAsync(DateTime startDate, DateTime endDate)
        {
            var events = new List<RawEvent>();
            var recurringvents = new List<RawEvent>();

            string requestEndpoint = $"https://graph.microsoft.com/v1.0/users/{_userId}/calendar/events";
            string requestUrl;

            var options = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true, // Allow case-insensitive matching
            };

            string strStart, strEnd;

            // Get all non-recurring events ('seriesMaster')
            try {
                strStart = startDate.ToString("yyyy-MM-dd");
                strEnd = endDate.ToString("yyyy-MM-dd");

                Console.WriteLine($"[DEBUG] Non-recurring from {strStart} to {strEnd}");

                requestUrl = requestEndpoint + $"?$filter=start/dateTime ge '{strStart}' and end/dateTime le '{strEnd}' and showAs eq 'busy' and type ne 'seriesMaster'";
                do {
                    var jsonResponse = await HttpGetRequest(requestUrl);

                    var graphResponse = JsonSerializer.Deserialize<GraphResponse<RawEvent>>(jsonResponse, options);

                    if (graphResponse?.Value == null) {
                        Console.WriteLine("[WARNING] Deserialisation returned null.");
                        break;
                    }

                    // Filter data
                    foreach (var rawEvent in graphResponse.Value) {
                        if (rawEvent == null) continue;

                        if (rawEvent.type == "seriesMaster") {
                            Console.WriteLine("[WARNING: Unexpected recurring event; event ignored.");
                            continue;
                        }

                        if ((rawEvent.start == null) || (rawEvent.end == null)) { continue; }
                        if (rawEvent.end.ToUTCDateTime() < startDate) { continue; }
                        if (rawEvent.start.ToUTCDateTime() > endDate) { continue; }

                        events.Add(rawEvent);
                    }


                    // Update the request URL with the next link
                    requestUrl = graphResponse.NextLink ?? string.Empty;
                } while (!string.IsNullOrEmpty(requestUrl));
            }
            catch (HttpRequestException ex) {
                Console.WriteLine($"[ERROR] HTTP Request failed: {ex.Message}");
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] An unexpected error occurred: {ex.Message}");
            }

            // Get all recurring events ('seriesMaster')
            try {
                strStart = startDate.AddYears(-2).ToString("yyyy-MM-dd");
                strEnd = endDate.ToString("yyyy-MM-dd");

                Console.WriteLine($"[DEBUG] Recurring from {strStart} to {strEnd}");

                requestUrl = requestEndpoint + $"?$filter=start/dateTime ge '{strStart}' and end/dateTime le '{strEnd}' and showAs eq 'busy' and type eq 'seriesMaster'";
                do {
                    var jsonResponse = await HttpGetRequest(requestUrl);

                    var graphResponse = JsonSerializer.Deserialize<GraphResponse<RawEvent>>(jsonResponse, options);

                    if (graphResponse?.Value == null) {
                        Console.WriteLine("[WARNING] Deserialisation returned null.");
                        break;
                    }

                    // Filter data
                    foreach (var rawEvent in graphResponse.Value) {
                        if (rawEvent == null) continue;

                        if (rawEvent.type != "seriesMaster") {
                            Console.WriteLine("[WARNING: Unexpected recurring event; event ignored.");
                            continue;
                        }

                        if ((rawEvent.start == null) || (rawEvent.end == null)) { continue; }

                        recurringvents.Add(rawEvent);
                    }


                    // Update the request URL with the next link
                    requestUrl = graphResponse.NextLink ?? string.Empty;
                } while (!string.IsNullOrEmpty(requestUrl));
            }
            catch (HttpRequestException ex) {
                Console.WriteLine($"[ERROR] HTTP Request failed: {ex.Message}");
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] An unexpected error occurred: {ex.Message}");
            }

            // Get instances for recurring events
            try {
                strStart = startDate.ToString("yyyy-MM-dd");
                strEnd = endDate.ToString("yyyy-MM-dd");

                Console.WriteLine($"[DEBUG] Instances for recurring events");

                foreach (var revent in recurringvents) {
                    //requestUrl = requestEndpoint + $"?{revent.id}/instances?startDateTime={strStart}&endDateTime={strEnd}$filter=showAs eq 'busy'";
                    requestUrl = requestEndpoint + $"/{revent.id}/instances?startDateTime={strStart}&endDateTime={strEnd}";
                    do {
                        var jsonResponse = await HttpGetRequest(requestUrl);

                        var graphResponse = JsonSerializer.Deserialize<GraphResponse<RawEvent>>(jsonResponse, options);

                        if (graphResponse?.Value == null) {
                            Console.WriteLine("[WARNING] Deserialisation returned null.");
                            break;
                        }

                        // Filter data
                        foreach (var rawEvent in graphResponse.Value) {
                            if (rawEvent == null) continue;

                            if ((rawEvent.start == null) || (rawEvent.end == null)) { continue; }

                            events.Add(rawEvent);
                        }


                        // Update the request URL with the next link
                        requestUrl = graphResponse.NextLink ?? string.Empty;
                    } while (!string.IsNullOrEmpty(requestUrl));
                }
            }
            catch (HttpRequestException ex) {
                Console.WriteLine($"[ERROR] HTTP Request failed: {ex.Message}");
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] An unexpected error occurred: {ex.Message}");
            }

            return events;
        }

        internal async Task AddEventsToCalendarAsync(List<EventData> scheduledEvents)
        {
            if (scheduledEvents == null || !scheduledEvents.Any()) {
                Console.WriteLine("[INFO] No events to add to the calendar.");
                return;
            }

            int totalEvents = scheduledEvents.Count;
            int createEvents = 0;

            try {
                Console.WriteLine("[INFO] Adding events to the calendar...");

                foreach (var eventData in scheduledEvents) {
                    if (!eventData.Subject.Contains("[ORDO]")) {
                        Console.WriteLine("[WARNING] Data from AI contain a non-ORDO event; event ignored");
                        continue;
                    }

                    // Prepare the request payload
                    var newEvent = new Event();
                    newEvent.Subject = eventData.Subject;

                    newEvent.Start = new DateTimeTimeZone();
                    newEvent.Start.DateTime = eventData.Start.ToString("yyyy-MM-ddTHH:mm:ss");
                    newEvent.Start.TimeZone = "UTC";

                    newEvent.End = new DateTimeTimeZone();
                    newEvent.End.DateTime = eventData.End.ToString("yyyy-MM-ddTHH:mm:ss");
                    newEvent.End.TimeZone = "UTC";

                    newEvent.IsReminderOn = false;

                    await CreateCalendarEvent(newEvent);
                    createEvents++;
                }
            }
            catch (Exception) {
                throw;
            }

        }
        
        internal async Task<bool> DeleteEventsFromCalendarAsync()
        {
            //TODO: 2-Configurable start and end dates (for retrieval)
            //TODO: 1-Remove retrieval of recurring events
            List<RawEvent> events = await GetCalendarEventsAsync(DateTime.Now.AddMonths(-6), DateTime.Now.AddMonths(3));

            bool eventsDeleted = false;
            foreach (var rawEvent in events) {
                if (!rawEvent.subject.Contains("[ORDO]")) { continue; }
                
                string id = rawEvent.id;
                try {
                    await _graphClient.Users[_userId].Events[id].DeleteAsync();
                    eventsDeleted = true;
                }
                catch (ServiceException ex) {
                    Console.WriteLine($"[ERROR] Graph API error while deleting event {rawEvent.subject}: {ex.Message}");
                    return eventsDeleted;
                }
                catch (Exception ex) {
                    Console.WriteLine($"[ERROR] Unexpected error while deleting event {rawEvent.subject}: {ex.Message}");
                    return eventsDeleted;
                }
            }
            return eventsDeleted;
        }
        #endregion

        #region Private
        // Helper method to retrieve the access token
        private async Task<string> GetAccessTokenAsync()
        {
            var clientCredential = new ClientSecretCredential(
                _appSettings.TenantId,
                _appSettings.ClientId,
                _appSettings.ClientSecret
            );

            var tokenRequestContext = new TokenRequestContext(new[] { "https://graph.microsoft.com/.default" });
            var accessToken = await clientCredential.GetTokenAsync(tokenRequestContext);
            return accessToken.Token;
        }

        private async Task<string> HttpGetRequest(string url)
        {
            using var httpClient = new HttpClient();

            // Set the Authorization header with the access token
            string accessToken = await GetAccessTokenAsync();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            //Console.WriteLine($"[DEBUG] {url}");

            // Send the GET request
            var response = await httpClient.GetAsync(url);

            // Ensure success status code
            response.EnsureSuccessStatusCode();

            // Read and parse the response content
            var jsonResponse = await response.Content.ReadAsStringAsync();

            //Console.WriteLine(jsonResponse);

            return jsonResponse;
        }

        private async Task<string> CreateCalendarEvent(Event requestBody)
        {
            try {
                // Send the request using the GraphServiceClient
                var result = await _graphClient.Users[_userId].Calendar.Events.PostAsync(requestBody);

                // Return the result ID or relevant data
                return result?.Id ?? "No ID returned";
            }
            catch (Exception) {
                throw;
            }
        }
        #endregion
    }

    // Helper class for deserialization
    public class GraphResponse<T>
    {
        public List<T>? Value { get; set; } // Holds the current page of data
        [JsonPropertyName("@odata.nextLink")]
        public string? NextLink { get; set; } // URL to the next page of results
    }

    public class GraphErrorResponse
    {
        public GraphError? Error { get; set; }
    }

    public class GraphError
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public InnerError? InnerError { get; set; }
    }

    public class InnerError
    {
        [JsonPropertyName("request-id")]
        public string? RequestId { get; set; }

        [JsonPropertyName("date")]
        public string? Date { get; set; }
    }
}
