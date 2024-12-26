using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using ordo.Models;

namespace ordo.Api
{
    internal class GraphClientHelper
    {
        private static GraphClientHelper? _instance;
        private static readonly object _lock = new object();

        private GraphServiceClient _graphClient;
        private string? _userId;

        private GraphClientHelper(AppSettings appSettings)
        {
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

        internal async Task<List<Event>> GetCalendarEventsAsync()
        {
            try {
                var events = await _graphClient.Users[_userId].Calendar.Events.GetAsync();
                return events?.Value?.ToList() ?? new List<Event>();
            }
            catch (ServiceException ex) {
                Console.WriteLine($"[ERROR] Graph API error while fetching calendar events: {ex.Message}");
                return new List<Event>();
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] Unexpected error while fetching calendar events: {ex.Message}");
                return new List<Event>();
            }
        }
    }
}
