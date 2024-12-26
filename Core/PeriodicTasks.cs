using Microsoft.Graph.Models;
using ordo.Api;
using ordo.Models;
using Ordo.Core;

namespace ordo.Core
{
    internal static class PeriodicTasks
    {
        internal static async Task SynchroniseProjectsWithToDo(AppSettings appSettings)
        {
            Console.WriteLine("[INFO] Starting task synchronization with Microsoft ToDo...");

            // Orchestrate the tasks
            var todoData = await FetchDataFromToDo(appSettings);

            var localData = LoadProjectsFromJson();

            var updatedProjectData = UpdateProjectsData(todoData, localData);

            SaveProjectsToJson(updatedProjectData);

            Console.WriteLine("[INFO] Synchronization with Microsoft ToDo completed.");
        }

        #region Private Methods
        private static async Task<List<Project>> FetchDataFromToDo(AppSettings appSettings)
        {
            var graphClientHelper = GraphClientHelper.GetInstance(appSettings);

            Console.WriteLine("[INFO] Fetching data from Microsoft ToDo...");

            // Fetch task lists (projects)
            List<TodoTaskList> todoTaskLists = await graphClientHelper.GetTaskListsAsync();
            var projects = new List<Project>();

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

                Console.WriteLine($"[INFO] Retrieved {project.Tasks.Count} tasks with a due date for project: {todoList.DisplayName}");
                projects.Add(project);
            }

            return projects;
        }

        private static List<Project> LoadProjectsFromJson()
        {
            Console.WriteLine("[INFO] Loading projects from projects.json...");

            // Use the ProjectsManager to load the data
            var projectsData = ProjectsManager.LoadData();

            if (projectsData == null || projectsData.Projects == null) {
                Console.WriteLine("[WARNING] No projects found in projects.json. Returning an empty list.");
                return new List<Project>();
            }

            Console.WriteLine($"[INFO] Loaded {projectsData.Projects.Count} projects from projects.json.");
            return projectsData.Projects;
        }

        private static List<Project> UpdateProjectsData(List<Project> todoProjects, List<Project> localProjects)
        {
            Console.WriteLine("[INFO] Updating project data with ToDo data...");

            // Replicate ToDo data in local data
            foreach (var todoProject in todoProjects) {
                // Find matching project in existing data
                Project? existingProject = null;
                foreach (var project in localProjects) {
                    if (project.Id == todoProject.Id) {
                        existingProject = project;
                        break;
                    }
                }

                if (existingProject == null) {
                    // Add new project if it doesn't exist
                    localProjects.Add(todoProject);
                    //Console.WriteLine($"[INFO] Added new project: {todoProject.Name}");
                }
                else {
                    // Update existing project
                    foreach (var todoTask in todoProject.Tasks) {
                        var existingTask = FindTaskById(existingProject.Tasks, todoTask.Id);
                        if (existingTask == null) {
                            // Add new task if it doesn't exist
                            existingProject.Tasks.Add(todoTask);
                            //Console.WriteLine($"[INFO] Added new task: {todoTask.Name} to project: {existingProject.Name}");
                        }
                        else {
                            // Update task details if needed
                            existingTask.Name = todoTask.Name;
                            existingTask.DueDate = todoTask.DueDate;
                            //Console.WriteLine($"[INFO] Updated task: {existingTask.Name} in project: {existingProject.Name}");
                        }
                    }
                }
            }

            // Check if something was deleted in ToDo data
            foreach (var localproject in localProjects) {
                Project? existingProject = null;
                Project? todoProject = null;
                foreach (var todoproject in todoProjects) {
                    if (localproject.Id == todoproject.Id) {
                        existingProject = localproject;
                        todoProject = todoproject;
                        break;
                    }
                }

                if (existingProject == null) {
                    localproject.IsMissing = true;
                }
                else if (todoProject != null) {
                    foreach (var localTask in existingProject.Tasks) {
                        var existingTask = FindTaskById(todoProject.Tasks, localTask.Id);
                        if (existingTask == null) {
                            localTask.IsMissing = true;
                        }
                    }
                }
            }

            Console.WriteLine("[INFO] Project data updated successfully.");
            return localProjects;
        }

        private static ProjectTask? FindTaskById(List<ProjectTask> tasks, string taskId)
        {
            foreach (var task in tasks) {
                if (task.Id == taskId) {
                    return task;
                }
            }
            return null;
        }

        private static void SaveProjectsToJson(List<Project> updatedProjects)
        {
            Console.WriteLine("[INFO] Saving updated projects to projects.json...");

            // Create a ProjectsData object to hold the list of projects
            var projectsData = new ProjectsData {
                Projects = updatedProjects
            };

            // Use ProjectsManager to save the data
            ProjectsManager.SaveData(projectsData);

            Console.WriteLine("[INFO] Updated projects successfully saved to projects.json.");
        }
        #endregion
    }
}
