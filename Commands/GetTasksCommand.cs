using Microsoft.Extensions.Configuration;
using Microsoft.Graph.Models;
using Ordo.Api;
using Ordo.Core;
using Ordo.Models;

namespace Ordo.Commands
{
    internal static class GetTasksCommand
    {
        public static async Task Execute()
        {
            Console.WriteLine("[INFO] Fetching tasks from Microsoft ToDo...");
            try {
                #region AppSettings
                var configuration = new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("config.json", optional: true, reloadOnChange: true)
                        .Build();

                var appSettings = new AppSettings();
                configuration.GetSection("AppSettings").Bind(appSettings);

                if (!appSettings.Validate(out string errorMessage)) {
                    Console.WriteLine($"[ERROR] {errorMessage}");
                    return;
                }
                #endregion

                var todoData = await GetDataFromToDo(appSettings);

                var jsonData = ProjectsArchiver.LoadData();

                var finalData = Consolidate(todoData, jsonData);

                ProjectsArchiver.SaveData(finalData);

                Console.WriteLine("[INFO] Tasks fetched and saved to projects.json successfully.");

                int no_tasks_noduration = finalData.GetTasksWithoutDurationCount();
                if (no_tasks_noduration > 0) {
                    Console.WriteLine($"[WARNING] {no_tasks_noduration} tasks without duration.");
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] An error occurred while fetching tasks: {ex.Message}");
            }
        }

        #region Private Methods
        private static async Task<ProjectsData> GetDataFromToDo(AppSettings appSettings)
        {
            var graphClientHelper = GraphClientHelper.GetInstance(appSettings);

            //Console.WriteLine("[INFO] Fetching data from Microsoft ToDo...");

            // Fetch task lists (projects)
            List<TodoTaskList> todoTaskLists = await graphClientHelper.GetTaskListsAsync();
            var projects = new ProjectsData();

            foreach (TodoTaskList todoList in todoTaskLists) {
                if (todoList.Id == null || todoList.DisplayName == null) {
                    //Console.WriteLine($"[WARNING] Skipping task list with null ID or DisplayName.");
                    continue;
                }

                // Skip lists with [IGNORE] in their name
                if (todoList.DisplayName.Contains("[IGNORE]", StringComparison.OrdinalIgnoreCase)) {
                    //Console.WriteLine($"[INFO] Skipping ignored task list: {todoList.DisplayName}");
                    continue;
                }

                //Console.WriteLine($"[INFO] Fetching tasks for project: {todoList.DisplayName}");

                // Fetch tasks for each project
                List<TodoTask> todoTasks = await graphClientHelper.GetTasksAsync(todoList.Id);
                var project = new Project {
                    Id = todoList.Id,
                    Name = todoList.DisplayName,
                    Tasks = new List<ProjectTask>()
                };

                foreach (var task in todoTasks) {
                    if (task.DueDateTime != null && task.Status != Microsoft.Graph.Models.TaskStatus.Completed) {
                        project.Tasks.Add(new ProjectTask {
                            Id = task.Id ?? string.Empty,
                            Name = task.Title ?? "Unnamed Task",
                            DueDate = task.DueDateTime?.DateTime != null ? DateTime.Parse(task.DueDateTime.DateTime) : DateTime.MinValue,
                            Duration = 0 // Default duration; can be updated later
                        });
                    }
                }

                //Console.WriteLine($"[INFO] Retrieved {project.Tasks.Count} tasks with a due date for project: {todoList.DisplayName}");
                projects.Add(project);
            }

            return projects;
        }

        private static ProjectsData Consolidate(ProjectsData todoData, ProjectsData jsonData)
        {
            // Iterate through todoData projects
            foreach (var todoProject in todoData.Projects) {
                bool projectExists = false;

                // Check if the project exists in jsonData
                foreach (var jsonProject in jsonData.Projects) {
                    if (jsonProject.Id == todoProject.Id) {
                        projectExists = true;
                        if (jsonProject.Name != todoProject.Name) {
                            jsonProject.Name = todoProject.Name;
                            Console.WriteLine($"[INFO] Project name udpate: {jsonProject.Name}");
                        }
                        if (jsonProject.ToDelete) {
                            jsonProject.ToDelete = false;
                            Console.WriteLine($"[INFO] Project restored: {jsonProject.Name}");
                        }
                        UpdateProjectTasks(todoProject, jsonProject);
                        break;
                    }
                }

                if (!projectExists) {
                    /// Add the new project from todoData
                    jsonData.Add(todoProject);
                    Console.WriteLine($"[INFO] Added new project: {todoProject.Name}");
                }
            }

            // Mark projects in jsonData that are not in todoData as ToDelete
            foreach (var jsonProject in jsonData.Projects) {
                bool foundInTodo = false;

                foreach (var todoProject in todoData.Projects) {
                    if (todoProject.Id == jsonProject.Id) {
                        foundInTodo = true;
                        break;
                    }
                }

                if (!foundInTodo) {
                    if (!jsonProject.ToDelete) {
                        jsonProject.ToDelete = true;
                        Console.WriteLine($"[INFO] Project marked for deletion: {jsonProject.Name}");
                    }
                }
            }

            // TODO: Assign tasks aliases
            AssignAliases(jsonData);

            return jsonData;
        }

        private static void UpdateProjectTasks(Project todoProject, Project jsonProject)
        {
            //Console.WriteLine($"[INFO] Updating tasks for project: {todoProject.Name}");

            // Add or update tasks from todoProject
            foreach (var todoTask in todoProject.Tasks) {
                bool taskExists = false;

                foreach (var jsonTask in jsonProject.Tasks) {
                    if (jsonTask.Id == todoTask.Id) {
                        taskExists = true;

                        // Update task attributes
                        if (jsonTask.Name != todoTask.Name) {
                            jsonTask.Name = todoTask.Name;
                            Console.WriteLine($"[INFO] Task name updated: {todoTask.Name}");
                        }
                        if (jsonTask.DueDate != todoTask.DueDate) {
                            jsonTask.DueDate = todoTask.DueDate;
                            Console.WriteLine($"[INFO] Task due date updated: {todoTask.Name}");
                        }
                        if (jsonTask.ToDelete) {
                            jsonTask.ToDelete = false;
                            Console.WriteLine($"[INFO] Task restored: {todoTask.Name}");
                        }
                        break;
                    }
                }

                if (!taskExists) {
                    // Add new task from todoData
                    jsonProject.Tasks.Add(todoTask);
                    Console.WriteLine($"[INFO] Added new task {todoTask.Name} to project {jsonProject.Name}");
                }
            }

            // Mark tasks in jsonProject that are not in todoProject as ToDelete
            foreach (var jsonTask in jsonProject.Tasks) {
                bool foundInTodo = false;

                foreach (var todoTask in todoProject.Tasks) {
                    if (todoTask.Id == jsonTask.Id) {
                        foundInTodo = true;
                        break;
                    }
                }

                if (!foundInTodo) {
                    if (!jsonTask.ToDelete) {
                        jsonTask.ToDelete = true;
                        Console.WriteLine($"[INFO] Task marked for deletion: {jsonTask.Name}");
                    }
                }
            }
        }
        
        private static void AssignAliases(ProjectsData jsonData) {
            int projectIndex = 1;

            foreach (var project in jsonData.Projects) {
                int taskIndex = 1;

                foreach (var task in project.Tasks) {
                    // Ensure both project and task indices are two digits
                    string projectAlias = projectIndex.ToString("D2");
                    string taskAlias = taskIndex.ToString("D2");

                    // Combine to form PPTT alias
                    task.Alias = projectAlias + taskAlias;

                    taskIndex++;
                }

                projectIndex++;
            }
        }
        #endregion

    }
}
