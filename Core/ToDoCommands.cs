using Ordo.Api;
using Ordo.Core;
using Ordo.Log;
using Ordo.Models;

namespace ordo.Core
{
    internal static class ToDoCommands
    {
        public static async Task<(bool, TodoData)> GetDataAsync(bool abortOverdue)
        {
            Logger.Instance.Log(LogLevel.INFO, "Fetching tasks from Microsoft ToDo...");

            TodoData todoData = new TodoData();
            try
            {
                // Get LISTS
                Logger.Instance.Log(LogLevel.DEBUG, "... Lists...");

                List<TodoList> lists = await GraphClientHelper.Instance.GetListsAsync();

                // Get TASKS
                Logger.Instance.Log(LogLevel.DEBUG, "... Tasks...");
                bool abort = false;
                foreach (var list in lists)
                {
                    if (list.DisplayName.Contains("[IGNORE]")) continue;

                    todoData.Lists.Add(list);

                    List<TodoTask> tasks = await GraphClientHelper.Instance.GetTasksAsync(list.Id);

                    foreach (var task in tasks)
                    {
                        if (task == null) { continue; }
                        if (task.DueDateTime == null) { continue; }
                        if (string.IsNullOrEmpty(task.DueDateTime.DateTime)) { continue; }
                        if (string.IsNullOrEmpty(task.DueDateTime.TimeZone)) { continue; }
                        if (task.Status == "completed") { continue; }

                        Logger.Instance.Log(LogLevel.DEBUG, $"Task '{list.DisplayName}': '{task.Title}'");

                        if (task.DueDateTime.GetUtcTime().Date < DateTime.Now.ToUniversalTime().Date)
                        {
                            if (abortOverdue) {
                                Logger.Instance.Log(LogLevel.ERROR, $"Task '{list.DisplayName}': '{task.Title}' overdue.");
                                abort = true;
                            }
                            else {
                                Logger.Instance.Log(LogLevel.WARNING, $"Task '{task.Title}' overdue.");
                            }
                        }

                        todoData.Tasks.Add(task);
                    }
                }

                if (abort) return (true, todoData);

                return (false, todoData);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(LogLevel.ERROR, $"An error occurred while fetching tasks; {ex.Message}");
                return (true, todoData);
            }
        }
    }
}
