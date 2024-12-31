using Microsoft.Extensions.Configuration;
using Ordo.Models;
using System.Text;

namespace Ordo.Api
{
    public class MotionHelper
    {
        private static MotionHelper? _instance;
        private static readonly object _lock = new object();
        private readonly string _apiKey;
        private const string BaseUrl = "https://api.usemotion.com/v1";

        public static MotionHelper GetInstance()
        {
            if (_instance == null) {
                lock (_lock) {
                    if (_instance == null) {
                        _instance = new MotionHelper();
                    }
                }
            }
            return _instance;
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
                var apiKey = configuration["MotionSettings:ApiKey"];

                if (string.IsNullOrEmpty(apiKey)) {
                    throw new Exception("API key is missing or empty in config.json.");
                }

                return apiKey;
            }
            catch (Exception ex) {
                throw new Exception($"Error retrieving API key: {ex.Message}");
            }
        }

        private async Task<(List<MotionWorkspace> Workspaces, string? Cursor)> FetchWorkspacesPageAsync(string? cursor = null)
        {
            var queryParams = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(cursor)) {
                queryParams["cursor"] = cursor;
            }

            var (isSuccess, responseContent) = await MakeRequestAsync("workspaces", HttpMethod.Get, queryParams);

            if (!isSuccess) {
                try {
                    // Parse the error response
                    var errorResponse = System.Text.Json.JsonSerializer.Deserialize<MotionErrorResponse>(responseContent);

                    if (errorResponse == null || string.IsNullOrEmpty(errorResponse.Message) ||
                        string.IsNullOrEmpty(errorResponse.Error) || errorResponse.StatusCode == 0) {
                        throw new Exception("Response JSON does not match the expected format.");
                    }

                    throw new Exception($"{errorResponse.StatusCode} {errorResponse.Error} ({errorResponse.Message})");
                }
                catch (System.Text.Json.JsonException ex) {
                    throw new Exception($"Failed to parse error response JSON: {ex.Message}", ex);
                }
            }

            try {
                // Parse the workspaces response
                var workspacesResponse = System.Text.Json.JsonSerializer.Deserialize<MotionWorkspacesResponse>(responseContent);

                if (workspacesResponse == null) {
                    throw new Exception("Response JSON does not match the expected format.");
                }

                return (workspacesResponse.Workspaces, workspacesResponse.Cursor);
            }
            catch (System.Text.Json.JsonException ex) {
                throw new Exception($"Failed to parse workspaces response JSON: {ex.Message}", ex);
            }
        }

        private async Task<(List<MotionProject> Projects, string? Cursor)> FetchProjectsPageAsync(string workspaceId, string? cursor = null)
        {
            var queryParams = new Dictionary<string, string>();

           queryParams["workspaceId"] = workspaceId;

            if (!string.IsNullOrEmpty(cursor)) {
                queryParams["cursor"] = cursor;
            }

            var (isSuccess, responseContent) = await MakeRequestAsync("projects", HttpMethod.Get, queryParams);

            if (!isSuccess) {
                try {
                    // Parse the error response
                    var errorResponse = System.Text.Json.JsonSerializer.Deserialize<MotionErrorResponse>(responseContent);

                    if (errorResponse == null || string.IsNullOrEmpty(errorResponse.Message) ||
                        string.IsNullOrEmpty(errorResponse.Error) || errorResponse.StatusCode == 0) {
                        throw new Exception("Response JSON does not match the expected format.");
                    }

                    throw new Exception($"{errorResponse.StatusCode} {errorResponse.Error} ({errorResponse.Message})");
                }
                catch (System.Text.Json.JsonException ex) {
                    throw new Exception($"Failed to parse error response JSON: {ex.Message}", ex);
                }
            }

            try {
                // Parse the projects response
                var projectsResponse = System.Text.Json.JsonSerializer.Deserialize<MotionProjectsResponse>(responseContent);

                if (projectsResponse == null) {
                    throw new Exception("Response JSON does not match the expected format.");
                }

                return (projectsResponse.Projects, projectsResponse.Cursor);
            }
            catch (System.Text.Json.JsonException ex) {
                throw new Exception($"Failed to parse projects response JSON: {ex.Message}", ex);
            }
        }

        private async Task<(List<MotionTask> Tasks, string? Cursor)> FetchTasksPageAsync(string? cursor = null)
        {
            var queryParams = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(cursor)) {
                queryParams["cursor"] = cursor;
            }

            var (isSuccess, responseContent) = await MakeRequestAsync("tasks", HttpMethod.Get, queryParams);

            if (!isSuccess) {
                try {
                    // Parse the error response
                    var errorResponse = System.Text.Json.JsonSerializer.Deserialize<MotionErrorResponse>(responseContent);

                    if (errorResponse == null || string.IsNullOrEmpty(errorResponse.Message) ||
                        string.IsNullOrEmpty(errorResponse.Error) || errorResponse.StatusCode == 0) {
                        throw new Exception("Response JSON does not match the expected format.");
                    }

                    throw new Exception($"{errorResponse.StatusCode} {errorResponse.Error} ({errorResponse.Message})");
                }
                catch (System.Text.Json.JsonException ex) {
                    throw new Exception($"Failed to parse error response JSON: {ex.Message}", ex);
                }
            }

            try {
                // Parse the tasks response
                var tasksResponse = System.Text.Json.JsonSerializer.Deserialize<MotionTasksResponse>(responseContent);

                if (tasksResponse == null) {
                    throw new Exception("Response JSON does not match the expected format.");
                }

                return (tasksResponse.Tasks, tasksResponse.Cursor);
            }
            catch (System.Text.Json.JsonException ex) {
                throw new Exception($"Failed to parse tasks response JSON: {ex.Message}", ex);
            }
        }

        /*private string? ExtractCursorFromResponse(string jsonResponse)
        {
            var jsonDocument = System.Text.Json.JsonDocument.Parse(jsonResponse);
            if (jsonDocument.RootElement.TryGetProperty("cursor", out var cursorElement)) {
                return cursorElement.GetString();
            }

            return null;
        }*/

        private async Task<(bool IsSuccess, string Content)> MakeRequestAsync(string endpoint, HttpMethod httpMethod, Dictionary<string, string> queryParams)
        {
            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var url = new StringBuilder($"{BaseUrl}/{endpoint}");
            if (queryParams.Count > 0) {
                url.Append("?");
                foreach (var param in queryParams) {
                    url.AppendFormat("{0}={1}&", param.Key, Uri.EscapeDataString(param.Value));
                }
                url.Length--;
            }

            var request = new HttpRequestMessage(httpMethod, url.ToString());

            try {
                var response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                return (response.IsSuccessStatusCode, content);
            }
            catch (HttpRequestException ex) {
                Console.WriteLine($"[ERROR] HTTP Request failed: {ex.Message}");
                throw;
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] An unexpected error occurred: {ex.Message}");
                throw;
            }
        }
        #endregion
    }
}
