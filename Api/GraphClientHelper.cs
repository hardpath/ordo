using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Ordo.Log;
using Ordo.Models;

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

                // Construct the endpoint URL using the UserId from AppSettings
                var url = $"{_baseUrl}{_appConfig.UserId}/todo/lists";

                // Use the HttpGetRequestAsync method to make the GET request
                var jsonResponse = await HttpGetRequestAsync(url);

                // Parse the JSON response
                var response = System.Text.Json.JsonSerializer.Deserialize<TodoListsResponse>(jsonResponse);

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

        public async Task<List<Ordo.Models.TodoTask>> GetTasksAsync(string todoListId)
        {
            try {
                var allTasks = new List<TodoTask>();

                // Construct the endpoint URL using the UserId and TodoListId from AppSettings
                var url = $"{_baseUrl}{_appConfig.UserId}/todo/lists/{todoListId}/tasks";

                // Use the HttpGetRequestAsync method to make the GET request
                var jsonResponse = await HttpGetRequestAsync(url);

                // Parse the JSON response
                var response = System.Text.Json.JsonSerializer.Deserialize<TodoTasksResponse>(jsonResponse);

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

                        todoTask.ListId = todoListId;

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

        private async Task<string> HttpGetRequestAsync(string url)
        {
            using var httpClient = new HttpClient();
            var accessToken = await GetAccessTokenAsync();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        #endregion
    }
}
