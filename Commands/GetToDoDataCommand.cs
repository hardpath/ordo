using Microsoft.Extensions.Configuration;
using Microsoft.Graph.Models;
using Ordo.Api;
using Ordo.Core;
using Ordo.Models;

namespace Ordo.Commands
{
    internal static class GetToDoDataCommand
    {
        private static string _filename = "todo.json";

        public static async Task ExecuteAsync()
        {
            Console.WriteLine("[INFO] Fetching tasks from Microsoft ToDo...");

            TodoData todoData = new TodoData();
            try {
                var helper = GraphClientHelper.GetInstance();

                // Get LISTS
                Console.WriteLine("[DEBUG] ... Lists...");
                todoData.Lists = await helper.GetListsAsync();

                // Get TASKS
                Console.WriteLine("[DEBUG] ... Tasks...");
                foreach (var list in todoData.Lists) {
                    List<Ordo.Models.TodoTask> tasks = await helper.GetTasksAsync(list.Id);

                    foreach (var task in tasks) {
                        if (task == null) { continue; }
                        if (task.Status != "completed") {
                            todoData.Tasks.Add(task);
                        }
                    }
                }

                new Archiver(_filename).Save(todoData);
                Console.WriteLine($"[INFO] ToDo data saved in {_filename}.");
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] An error occurred while fetching tasks: {ex.Message}");
            }
        }
    }
}
