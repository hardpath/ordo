using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Ordo.Models;

namespace Ordo.Api
{
    public static class GraphClientHelper
    {
        internal static GraphServiceClient GetAuthenticatedGraphClient(AppSettings appSettings)
        {
            var clientSecretCredential = new ClientSecretCredential(
                appSettings.TenantId,
                appSettings.ClientId,
                appSettings.ClientSecret
            );

            return new GraphServiceClient(clientSecretCredential);
        }

        public static async Task<List<TodoTaskList>> GetTaskListsAsync(GraphServiceClient graphClient, string userId)
        {
            var taskLists = await graphClient.Users[userId].Todo.Lists.GetAsync();
            return taskLists?.Value?.ToList() ?? new List<TodoTaskList>();
        }

        public static async Task<List<TodoTask>> GetTasksAsync(GraphServiceClient graphClient, string userId, string listId)
        {
            var tasks = await graphClient.Users[userId].Todo.Lists[listId].Tasks.GetAsync();
            return tasks?.Value?.ToList() ?? new List<TodoTask>();
        }

        public static async Task<List<Event>> GetCalendarEventsAsync(GraphServiceClient graphClient, string userId)
        {
            var events = await graphClient.Users[userId].Calendar.Events.GetAsync();
            return events?.Value?.ToList() ?? new List<Event>();
        }
    }
}
