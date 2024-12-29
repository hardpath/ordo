﻿using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Ordo.Models;
using System.Text;
using System.Text.Json;

namespace Ordo.Api
{
    internal class GraphClientHelper
    {
        private static GraphClientHelper? _instance;
        private static readonly object _lock = new object();

        private GraphServiceClient _graphClient;
        private string? _userId;

        AppSettings _appSettings;

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

        // ToDo
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

        // Calendar
        internal async Task<List<RawEvent>> GetCalendarEventsAsync(DateTime startDate, DateTime endDate)
        {
            var events = new List<RawEvent>();
            var recurringvents = new List<RawEvent>();

            try {
                //string id = "AQMkADNmNWIyZmExLTNlYjEtNDhjNC1hMDM3LThhNTQ4MTNkM2E5ZABGAAADSxwVx38gmUyWpDMCW_EKigcAfL1FcVpKxUOnXVZw6bVamwAAAgENAAAAfL1FcVpKxUOnXVZw6bVamwAD0vXRGAAAAA==";
                //string requestUrl = $"https://graph.microsoft.com/v1.0/users/{_userId}/calendar/events/{id}/instances?startDateTime=2024-01-01&endDateTime=2026-01-01";
                
                string requestUrl = $"https://graph.microsoft.com/v1.0/users/{_userId}/calendar/events/";
                string jsonPayload = string.Empty;

                var jsonResponse = await HttpGetRequest(requestUrl, jsonPayload);

                var options = new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true, // Allow case-insensitive matching
                };

                //Console.WriteLine($"... from {startDate.ToString()} to {endDate.ToString()}");

                var graphResponse = JsonSerializer.Deserialize<GraphResponse<RawEvent>>(jsonResponse, options);
                if (graphResponse?.Value == null) {
                    Console.WriteLine("[WARNING] Deserialisation returned null.");
                }
                else {
                    foreach (var rawEvent in graphResponse.Value) {
                        if (rawEvent == null) continue;
                        if (rawEvent.type == "seriesMaster")
                            recurringvents.Add(rawEvent);
                        else {
                            if ( (rawEvent.start == null) || (rawEvent.end == null) ) { continue; }
                            if (rawEvent.end.ToUTCDateTime()  < startDate) { continue; }
                            if (rawEvent.start.ToUTCDateTime() > endDate) { continue; }

                            events.Add(rawEvent);
                        }
                    }
                }

                if (recurringvents.Count > 0) {
                    foreach (var rawEvent in recurringvents) {
                        //requestUrl = $"https://graph.microsoft.com/v1.0/users/{_userId}/calendar/events/{rawEvent.id}/instances?startDateTime={startDate.ToString("yyyy-MM-dd")}&endDateTime={endDate.ToString("yyyy-MM-dd")}";
                        jsonPayload = $"{rawEvent.id}/instances?startDateTime={startDate.ToString("yyyy-MM-dd")}&endDateTime={endDate.ToString("yyyy-MM-dd")}";

                        jsonResponse = await HttpGetRequest(requestUrl, jsonPayload);

                        graphResponse = JsonSerializer.Deserialize<GraphResponse<RawEvent>>(jsonResponse, options);

                        if (graphResponse?.Value == null) {
                            Console.WriteLine("[WARNING] Deserialisation returned null for recurring events.");
                        }
                        else {
                            foreach (var recEvent in graphResponse.Value) {
                                events.Add(recEvent);
                            }
                        }

                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] Error while fetching calendar events: {ex.Message}");
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
                    // Prepare the request payload
                    var newEvent = new Event();
                    newEvent.Subject = eventData.Subject;

                    newEvent.Start = new DateTimeTimeZone();
                    newEvent.Start.DateTime = eventData.Start.ToString("yyyy-MM-ddTHH:mm:ss");
                    newEvent.Start.TimeZone = "UTC";

                    newEvent.End = new DateTimeTimeZone();
                    newEvent.End.DateTime = eventData.End.ToString("yyyy-MM-ddTHH:mm:ss");
                    newEvent.End.TimeZone = "UTC";

                    await CreateCalendarEvent(newEvent);
                    createEvents++;
                }
            }
            catch (Exception) {
                throw;
            }

        }

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

        private async Task<string> HttpGetRequest(string url, string jsonPayload)
        {
            using var httpClient = new HttpClient();

            // Set the Authorization header with the access token
            string accessToken = await GetAccessTokenAsync();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            // Send the GET request
            var response = await httpClient.GetAsync(url + jsonPayload);

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

        // Helper class for deserialization
        private class GraphResponse<T>
        {
            public List<T>? Value { get; set; }
        }
        #endregion
    }
}
