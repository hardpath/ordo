using Microsoft.Graph.Models;
using ordo.Api;
using ordo.Models;

namespace ordo.Core
{
    internal static class PeriodicTasks
    {
        internal static async Task SynchroniseProjectsWithToDo(AppSettings appSettings)
        {
            var graphClientHelper = GraphClientHelper.GetInstance(appSettings);

            Console.WriteLine("[INFO] Starting task synchronization with Microsoft ToDo...");

            // Fetch task lists (projects)
            List<TodoTaskList> todoTaskLists = await graphClientHelper.GetTaskListsAsync();

            foreach (TodoTaskList todoList in todoTaskLists) {
                if (todoList.Id == null) {
                    Console.WriteLine($"[WARNING] Task list {todoList.DisplayName} has a null ID and will be skipped.");
                    continue;
                }

                // Skip lists with [IGNORE] in their name
                if (todoList.DisplayName != null && todoList.DisplayName.Contains("[IGNORE]", StringComparison.OrdinalIgnoreCase)) {
                    Console.WriteLine($"[INFO] Skipping ignored task list: {todoList.DisplayName}");
                    continue;
                }

                Console.WriteLine($"[INFO] Fetching tasks for project: {todoList.DisplayName}");

                // Fetch tasks for each project
                List<TodoTask> todoTasks = await graphClientHelper.GetTasksAsync(todoList.Id);

                // Filter tasks without a due date using a traditional loop
                List<TodoTask> tasksWithDueDate = new List<TodoTask>();
                foreach (var task in todoTasks) {
                    if (task.DueDateTime != null && task.Status != null && task.Status != Microsoft.Graph.Models.TaskStatus.Completed) {
                        tasksWithDueDate.Add(task);
                    }
                }

                Console.WriteLine($"[INFO] Retrieved {tasksWithDueDate.Count} tasks with a due date for project: {todoList.DisplayName}");

                // Continue processing tasksWithDueDate
            }

            Console.WriteLine("[INFO] Synchronization with Microsoft ToDo completed.");
        }
    }
}
