using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Ordo.Log;
using Ordo.Models;
using System.Text;
using System.Text.Json;

namespace Ordo.Api
{
    public class GraphClientHelper
    {
        private static GraphClientHelper? _instance;
        private static readonly object _lock = new object();
        private readonly GraphConfig _appConfig;
        private const string _baseUrl = "https://graph.microsoft.com/v1.0/users/";

        public static GraphClientHelper Instance
        {
            get {
                if (_instance == null) {
                    lock (_lock) {
                        if (_instance == null) {
                            _instance = new GraphClientHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<List<TodoList>> GetListsAsync()
        {
            try {
                var allLists = new List<TodoList>();

                // Use the HttpGetRequestAsync method to make the GET request
                //var jsonResponse = await MakeRequestAsync(url, HttpMethod.Get);

                // Attempt to make the HTTP request
                var (isSuccess, responseContent) = await MakeRequestAsync("/todo/lists", HttpMethod.Get);

                if (string.IsNullOrEmpty(responseContent)) {
                    throw new Exception("Response content is null or empty.");
                }

                if (!isSuccess) {
                    HandleErrorResponse(responseContent);
                }

                // Parse the successful response
                var response = JsonSerializer.Deserialize<TodoListsResponse>(responseContent);

                if (response == null) {
                    throw new Exception("Response JSON does not match the expected format.");
                }

                // Parse the JSON response
                if (response != null && response.Value != null) {
                    foreach (var list in response.Value) {
                        var todoList = new TodoList();
                        if (list.Id != null) {
                            todoList.Id = list.Id;
                        }
                        else {
                            todoList.Id = string.Empty;
                        }

                        if (list.DisplayName != null) {
                            todoList.DisplayName = list.DisplayName;
                        }
                        else {
                            todoList.DisplayName = string.Empty;
                        }

                        allLists.Add(todoList);
                    }
                }

                return allLists;
            }
            catch (System.Text.Json.JsonException ex) {
                Logger.Instance.Log(LogLevel.ERROR, $"JSON parsing error: {ex.Message}");
                throw;
            }
            catch (Exception ex) {
                Logger.Instance.Log(LogLevel.ERROR, $"Error retrieving To-Do lists: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Ordo.Models.TodoTask>> GetTasksAsync(string listId)
        {
            try {
                var allTasks = new List<TodoTask>();

                // Use the HttpGetRequestAsync method to make the GET request
                var (isSuccess, responseContent) = await MakeRequestAsync($"/todo/lists/{listId}/tasks", HttpMethod.Get);

                if (string.IsNullOrEmpty(responseContent)) {
                    throw new Exception("Response content is null or empty.");
                }

                if (!isSuccess) {
                    HandleErrorResponse(responseContent);
                }

                // Parse the JSON response
                var response = JsonSerializer.Deserialize<TodoTasksResponse>(responseContent);

                if (response == null) {
                    throw new Exception("Response JSON does not match the expected format.");
                }

                if (response != null && response.Value != null) {
                    foreach (var task in response.Value) {
                        var todoTask = new TodoTask();
                        if (task.Id != null) {
                            todoTask.Id = task.Id;
                        }
                        else {
                            todoTask.Id = string.Empty;
                        }

                        if (task.Title != null) {
                            todoTask.Title = task.Title;
                        }
                        else {
                            todoTask.Title = string.Empty;
                        }

                        if (task.Status != null) {
                            todoTask.Status = task.Status;
                        }
                        else {
                            todoTask.Status = string.Empty;
                        }

                        todoTask.DueDateTime = new DateTimeZone();

                        if (task.DueDateTime != null) {
                            todoTask.DueDateTime.DateTime = task.DueDateTime.DateTime;
                            todoTask.DueDateTime.TimeZone = task.DueDateTime.TimeZone;
                        }

                        todoTask.ListId = listId;

                        allTasks.Add(todoTask);
                    }
                }

                return allTasks;
            }
            catch (System.Text.Json.JsonException ex) {
                Logger.Instance.Log(LogLevel.ERROR, $"JSON parsing error: {ex.Message}");
                throw;
            }
            catch (Exception ex) {
                Logger.Instance.Log(LogLevel.ERROR, $"Error retrieving To-Do tasks: {ex.Message}");
                throw;
            }
        }

        public async Task MarkAsCompletedAsync(string listdId, string taskId)
        {
            try {
                var payload = new Dictionary<string, object>();
                payload["status"] = "completed";

                // Use the HttpGetRequestAsync method to make the GET request
                var (isSuccess, responseContent) = await MakeRequestAsync($"/todo/lists/{listdId}/tasks/{taskId}", HttpMethod.Patch, null, payload);

                if (string.IsNullOrEmpty(responseContent)) {
                    throw new Exception("Response content is null or empty.");
                }

                if (!isSuccess) {
                    HandleErrorResponse(responseContent);
                }
            }
            catch (System.Text.Json.JsonException ex) {
                Logger.Instance.Log(LogLevel.ERROR, $"JSON parsing error: {ex.Message}");
                throw;
            }
            catch (Exception ex) {
                Logger.Instance.Log(LogLevel.ERROR, $"Error marking task as completed: {ex.Message}");
                throw;
            }
        }

        #region Private
        private GraphClientHelper()
        {
            // Load configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .Build();

            _appConfig = new GraphConfig();
            configuration.GetSection("GraphConfig").Bind(_appConfig);

            if (!_appConfig.Validate(out string errorMessage)) {
                throw new Exception($"Configuration validation failed: {errorMessage}");
            }
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var clientCredential = new ClientSecretCredential(
                _appConfig.TenantId,
                _appConfig.ClientId,
                _appConfig.ClientSecret
            );

            var tokenRequestContext = new TokenRequestContext(new[] { "https://graph.microsoft.com/.default" });
            var accessToken = await clientCredential.GetTokenAsync(tokenRequestContext);
            return accessToken.Token;
        }

        private async Task<(bool IsSuccess, string Content)> MakeRequestAsync(string endpoint, HttpMethod httpMethod, Dictionary<string, string>? queryParams = null, object? payload = null)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("Endpoint cannot be null or empty", nameof(endpoint));

            // Build the URL with query parameters if provided
            var url = new StringBuilder($"{_baseUrl}{_appConfig.UserId}/{endpoint}");
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

                using var httpClient = new HttpClient();

                var accessToken = await GetAccessTokenAsync();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

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
                var errorResponse = JsonSerializer.Deserialize<GraphErrorResponse>(responseContent);

                if (errorResponse == null || string.IsNullOrEmpty(errorResponse.Error.Message) || string.IsNullOrEmpty(errorResponse.Error.Code)) {
                    throw new Exception("Response JSON does not match the expected error format.");
                }

                throw new Exception($"{errorResponse.Error.Code} ({errorResponse.Error.Message})");
            }
            catch (JsonException ex) {
                throw new Exception($"Failed to parse error response JSON: {ex.Message}", ex);
            }
        }
        #endregion
    }
}
