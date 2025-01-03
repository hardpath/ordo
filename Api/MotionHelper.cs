using Microsoft.Extensions.Configuration;
using Ordo.Log;
using Ordo.Models;
using System.Text;
using System.Text.Json;

namespace Ordo.Api
{
    public class MotionHelper
    {
        private static MotionHelper? _instance;
        private static readonly object _lock = new object();
        private readonly string _apiKey;
        private const string BaseUrl = "https://api.usemotion.com/v1";
        private static int _requestCounter = 0; // Static to track across method calls

        public static MotionHelper Instance
        {
            get {
                if (_instance == null) {
                    lock (_lock) {
                        if (_instance == null) {
                            _instance = new MotionHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<List<MotionWorkspace>> GetWorkspacesAsync()
        {
            try {
                var allWorkspaces = new List<MotionWorkspace>();
                string? cursor = null;

                do {
                    var (workspaces, nextCursor) = await FetchWorkspacesPageAsync(cursor);

                    // Add the retrieved workspaces to the list
                    allWorkspaces.AddRange(workspaces);

                    // Update the cursor for the next request
                    cursor = nextCursor;

                } while (!string.IsNullOrEmpty(cursor));

                return allWorkspaces;
            }
            catch (Exception) {
                throw;
            }
        }

        public async Task<List<MotionProject>> GetProjectsAsync(string workspaceId)
        {
            try {
                var allProjects = new List<MotionProject>();
                string? cursor = null;

                do {
                    // Fetch a single page of projects
                    var (projects, nextCursor) = await FetchProjectsPageAsync(workspaceId, cursor);

                    // Add the retrieved projects to the list
                    allProjects.AddRange(projects);

                    // Update the cursor for the next request
                    cursor = nextCursor;

                } while (!string.IsNullOrEmpty(cursor));

                return allProjects;
            }
            catch (Exception) {
                throw;
            }
        }

        public async Task<List<MotionTask>> GetTasksAsync()
        {
            try {
                var allTasks = new List<MotionTask>();
                string? cursor = null;

                do {
                    // Fetch a single page of tasks
                    var (tasks, nextCursor) = await FetchTasksPageAsync(cursor);

                    // Add the retrieved tasks to the list
                    allTasks.AddRange(tasks);

                    // Update the cursor for the next request
                    cursor = nextCursor;

                } while (!string.IsNullOrEmpty(cursor));

                return allTasks;
            }
            catch (Exception) {
                throw;
            }
        }

        public async Task<string> AddProjectAsync(string projectName, string workspaceId)
        {
            var payload = new Dictionary<string, object>();
            payload["name"] = projectName;
            payload["workspaceId"] = workspaceId;
            payload["priority"] = "MEDIUM";

            try {
                var (isSuccess, responseContent) = await MakeRequestAsync("projects", HttpMethod.Post, null, payload);

                if (!isSuccess) {
                    HandleErrorResponse(responseContent);
                }

                // Parse respinseContent
                var createdProject = JsonSerializer.Deserialize<MotionProject>(responseContent);

                if (createdProject == null) {
                    throw new Exception($"Response JSON does not match the expected format [{responseContent}]");
                }

                if (string.IsNullOrEmpty(createdProject.Id)) {
                    throw new Exception("Project ID from Motion came null or empty.");
                }

                return createdProject.Id;
            }
            catch (JsonException ex) {
                throw new Exception($"Failed to parse response JSON: {ex.Message}", ex);
            }
            catch (Exception ex) {
                throw new Exception($"POST failed to create project: {ex.Message}", ex);
            }
        }

        public async Task<string> AddTaskAsync(string taskName, string workspaceId, string projectId, string dueDate)
        {
            var payload = new Dictionary<string, object>();
            payload["name"] = taskName;
            payload["workspaceId"] = workspaceId;
            payload["projectId"] = projectId;
            payload["dueDate"] = dueDate;

            try {
                var (isSuccess, responseContent) = await MakeRequestAsync("tasks", HttpMethod.Post, null, payload);

                if (!isSuccess) {
                    HandleErrorResponse(responseContent);
                }

                // Parse respinseContent
                var createdTask = JsonSerializer.Deserialize<MotionTask>(responseContent);

                if (createdTask == null) {
                    throw new Exception($"Response JSON does not match the expected format [{responseContent}]");
                }

                if (string.IsNullOrEmpty(createdTask.Id)) {
                    throw new Exception("Project ID from Motion came null or empty.");
                }

                return createdTask.Id;
            }
            catch (JsonException ex) {
                throw new Exception($"Failed to parse response JSON: {ex.Message}", ex);
            }
            catch (Exception ex) {
                throw new Exception($"POST failed to create task: {ex.Message}", ex);
            }
        }

        public async Task EditTaskAsync(string taskId, string taskName, string dueDate)
        {
            string endpoint = $"tasks/{taskId}";

            var payload = new Dictionary<string, object>();
            payload["name"] = taskName;
            payload["dueDate"] = dueDate;

            try {
                var (isSuccess, responseContent) = await MakeRequestAsync(endpoint, HttpMethod.Patch, null, payload);

                if (!isSuccess) {
                    HandleErrorResponse(responseContent);
                }

                // Parse respinseContent
                var editedTasks = JsonSerializer.Deserialize<MotionTask>(responseContent);

                if (editedTasks == null) {
                    throw new Exception($"Response JSON does not match the expected format [{responseContent}]");
                }

                if (string.IsNullOrEmpty(editedTasks.Id)) {
                    throw new Exception("Edited task ID from Motion came null or empty.");
                }
            }
            catch (JsonException ex) {
                throw new Exception($"Failed to parse response JSON: {ex.Message}", ex);
            }
            catch (Exception ex) {
                throw new Exception($"PATCH failed to edit task: {ex.Message}", ex);
            }
        }

        #region Private (for pagination)
        private async Task<(List<MotionWorkspace> Workspaces, string? Cursor)> FetchWorkspacesPageAsync(string? cursor = null)
        {
            var queryParams = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(cursor)) {
                queryParams["cursor"] = cursor;
            }

            try {
                // Attempt to make the HTTP request
                var (isSuccess, responseContent) = await MakeRequestAsync("workspaces", HttpMethod.Get, queryParams);

                if (string.IsNullOrEmpty(responseContent)) {
                    throw new Exception("Response content is null or empty.");
                }

                if (!isSuccess) {
                    HandleErrorResponse(responseContent);
                }

                // Parse the successful response
                var workspacesResponse = JsonSerializer.Deserialize<MotionWorkspacesResponse>(responseContent);

                if (workspacesResponse == null) {
                    throw new Exception("Response JSON does not match the expected format.");
                }

                return (workspacesResponse.Workspaces, workspacesResponse.Cursor);
            }
            catch (JsonException ex) {
                throw new Exception($"Failed to parse response JSON: {ex.Message}", ex);
            }
            catch (Exception ex) {
                throw new Exception($"Failed to fetch workspaces page: {ex.Message}", ex);
            }
        }

        private async Task<(List<MotionProject> Projects, string? Cursor)> FetchProjectsPageAsync(string workspaceId, string? cursor = null)
        {
            var queryParams = new Dictionary<string, string> {
                ["workspaceId"] = workspaceId
            };

            if (!string.IsNullOrEmpty(cursor)) {
                queryParams["cursor"] = cursor;
            }

            try {
                // Attempt to make the HTTP request
                var (isSuccess, responseContent) = await MakeRequestAsync("projects", HttpMethod.Get, queryParams);

                if (string.IsNullOrEmpty(responseContent)) {
                    throw new Exception("Response content is null or empty.");
                }

                if (!isSuccess) {
                    HandleErrorResponse(responseContent);
                }

                // Parse the successful response
                var projectsResponse = JsonSerializer.Deserialize<MotionProjectsResponse>(responseContent);

                if (projectsResponse == null) {
                    throw new Exception("Response JSON does not match the expected format.");
                }

                return (projectsResponse.Projects, projectsResponse.Cursor);
            }
            catch (JsonException ex) {
                throw new Exception($"Failed to parse response JSON: {ex.Message}", ex);
            }
            catch (Exception ex) {
                throw new Exception($"Failed to fetch projects page: {ex.Message}", ex);
            }
        }

        private async Task<(List<MotionTask> Tasks, string? Cursor)> FetchTasksPageAsync(string? cursor = null)
        {
            var queryParams = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(cursor)) {
                queryParams["cursor"] = cursor;
            }

            try {
                // Attempt to make the HTTP request
                var (isSuccess, responseContent) = await MakeRequestAsync("tasks", HttpMethod.Get, queryParams);

                if (string.IsNullOrEmpty(responseContent)) {
                    throw new Exception("Response content is null or empty.");
                }

                if (!isSuccess) {
                    HandleErrorResponse(responseContent);
                }

                // Parse the successful response
                var tasksResponse = JsonSerializer.Deserialize<MotionTasksResponse>(responseContent);

                if (tasksResponse == null) {
                    throw new Exception("Response JSON does not match the expected format.");
                }

                return (tasksResponse.Tasks, tasksResponse.Cursor);
            }
            catch (JsonException ex) {
                throw new Exception($"Failed to parse response JSON: {ex.Message}", ex);
            }
            catch (Exception ex) {
                throw new Exception($"Failed to fetch tasks page: {ex.Message}", ex);
            }
        }
        #endregion

        #region Private
        private MotionHelper()
        {
            _apiKey = GetApiKey();
        }

        private string GetApiKey()
        {
            try {
                // Build configuration
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory) // Set the base path to the application's directory
                    .AddJsonFile("config.json", optional: false, reloadOnChange: true) // Load the JSON file
                    .Build();

                // Retrieve the API key from the OpenAiSettings section
                var apiKey = configuration["MotionConfig:ApiKey"];

                if (string.IsNullOrEmpty(apiKey)) {
                    throw new Exception("API key is missing or empty in config.json.");
                }

                return apiKey;
            }
            catch (Exception ex) {
                throw new Exception($"Error retrieving API key: {ex.Message}");
            }
        }

        private async Task<(bool IsSuccess, string Content)> MakeRequestAsync(string endpoint, HttpMethod httpMethod, Dictionary<string, string>? queryParams = null, object? payload = null)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("Endpoint cannot be null or empty", nameof(endpoint));

            // Handle rate-limiting
            lock (_lock) {
                _requestCounter++;
                if (_requestCounter >= 10) {
                    _requestCounter = 0; // Reset the counter
                    Console.WriteLine("Rate limit reached. Pausing for 1 minute.");
                }
            }

            if (_requestCounter == 0) {
                await Task.Delay(TimeSpan.FromMinutes(1));
            }

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            // Build the URL with query parameters if provided
            var url = new StringBuilder($"{BaseUrl}/{endpoint}");
            if (queryParams != null && queryParams.Count > 0) {
                url.Append("?");
                foreach (var param in queryParams) {
                    url.AppendFormat("{0}={1}&", param.Key, Uri.EscapeDataString(param.Value));
                }
                url.Length--; // Remove trailing "&"
            }

            try {
                var request = new HttpRequestMessage(httpMethod, url.ToString());

                // Add payload for methods like POST or PUT
                if (payload != null && (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put || httpMethod == HttpMethod.Patch)) {
                    var jsonPayload = JsonSerializer.Serialize(payload);
                    request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                }

                var response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                return (response.IsSuccessStatusCode, content);
            }
            catch (TaskCanceledException ex) {
                throw new TimeoutException("The request timed out.", ex);
            }
            catch (HttpRequestException ex) {
                throw new HttpRequestException($"HTTP request failed: {ex.Message}", ex);
            }
            catch (Exception ex) {
                throw new Exception($"An unexpected error occurred: {ex.Message}", ex);
            }
        }

        private void HandleErrorResponse(string responseContent)
        {

            try {
                var errorResponse = JsonSerializer.Deserialize<MotionErrorResponse>(responseContent);

                if (errorResponse == null || string.IsNullOrEmpty(errorResponse.Message) || errorResponse.StatusCode == 0) {
                    throw new Exception("Response JSON does not match the expected error format.");
                }

                throw new Exception($"{errorResponse.StatusCode} {errorResponse.Error} ({errorResponse.Message})");
            }
            catch (JsonException) { }

            try {
                var errorResponse = JsonSerializer.Deserialize<MotionErrorResponse2>(responseContent);

                if (errorResponse == null || string.IsNullOrEmpty(errorResponse.Message[0]) || errorResponse.StatusCode == 0) {
                    throw new Exception("Response JSON does not match the expected error format.");
                }

                throw new Exception($"{errorResponse.StatusCode} {errorResponse.Error} ({errorResponse.Message[0]})");
            }
            catch (JsonException ex) {
                throw new Exception($"Failed to parse error response JSON: {ex.Message}", ex);
            }
        }
        #endregion
    }
}
