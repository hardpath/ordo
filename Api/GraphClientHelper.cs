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
            // Create a client credential using Azure.Identity
            var clientSecretCredential = new ClientSecretCredential(
                appSettings.TenantId,
                appSettings.ClientId,
                appSettings.ClientSecret
            );

            // Create and return a GraphServiceClient
            var graphClient = new GraphServiceClient(clientSecretCredential);
            return graphClient;
        }

        public static async Task FetchTasksAsync(GraphServiceClient graphClient, string userId)
        {
            try {
                // Retrieve all task lists for the specified user
                var taskLists = await graphClient.Users[userId].Todo.Lists.GetAsync();

                if (taskLists?.Value == null || taskLists.Value.Count == 0) {
                    Console.WriteLine("No task lists found.");
                    return;
                }

                Console.WriteLine("Task Lists Retrieved:");
                foreach (var taskList in taskLists.Value) {
                    Console.WriteLine($"- {taskList.DisplayName}");

                    // Retrieve tasks for each task list
                    var tasks = await graphClient.Users[userId].Todo.Lists[taskList.Id].Tasks.GetAsync();

                    if (tasks?.Value == null || tasks.Value.Count == 0) {
                        Console.WriteLine("  No tasks found in this list.");
                        continue;
                    }

                    Console.WriteLine("Tasks in List:");
                    foreach (var task in tasks.Value) {
                        Console.WriteLine($"  - {task.Title}");
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Error retrieving tasks: {ex.Message}");
            }
        }
    }
}
