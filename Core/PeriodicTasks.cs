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

            Console.WriteLine("[INFO] Starting task synchronisation with Microsoft ToDo...");

            // Fetch task lists (projects)
            List<TodoTaskList> todoTaskLists = await graphClientHelper.GetTaskListsAsync();

            foreach (TodoTaskList todoList in todoTaskLists) {
                if (todoList.Id == null) {
                    Console.WriteLine($"[WARNING] Task list {todoList.DisplayName} has a null ID and will be skipped.");
                    continue;
                }

                Console.WriteLine($"[INFO] Fetching tasks for project: {todoList.DisplayName}");

                // Fetch tasks for each project
                List<TodoTask> todoTasks = await graphClientHelper.GetTasksAsync(todoList.Id);

                // Filter tasks without a due date
                List<TodoTask> tasksWithDueDate = new List<TodoTask>();
                foreach (var task in todoTasks) {
                    if (task.DueDateTime != null) {
                        tasksWithDueDate.Add(task);
                    }
                }

                Console.WriteLine($"[INFO] Retrieved {tasksWithDueDate.Count} tasks with a due date for project: {todoList.DisplayName}");

                // Next step: Compare filtered tasks with `durations.json`
            }

            Console.WriteLine("[INFO] Synchronization with Microsoft ToDo completed.");
        }
    }
}
