﻿
using Microsoft.Extensions.Configuration;
using Microsoft.Graph.Models;
using ordo.Core;
using Ordo.Core;
using Ordo.Log;
using Ordo.Models;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ordo
{
    class Program
    {
        private static AppSettings _appSettings = new AppSettings();
        private static TodoData _todoData = new TodoData();
        private static MotionData _motionData = new MotionData();
        private static IdsData _idsData = new IdsData();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Ordo (v1.0 rc2)");

            Logger.Instance.SetLoggingStrategy(new ConsoleLoggingStrategy());

            LoadConfiguration();

            Logger.DebugMode = _appSettings.Debug ?? false;

            LoadIdsData();

            if (await GetToDoData()) return;

            if (await GetMotionData()) return;

            if (CheckMotionZombies()) return;

            if (await Todo2Motion()) return;

            if (await DeleteTasks()) return;

            Logger.Instance.Log(LogLevel.INFO, "Synchronisation completed.");
        }

        private static void LoadConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                .Build();

            bool failLoadSettings = false;
            List<string> missingFields = new List<string>();
            AppSettings? loadedSettings = configuration.Get<AppSettings>();

            if (loadedSettings == null) {
                failLoadSettings = true;
            }
            else {
                missingFields = loadedSettings.Validate();
                _appSettings = loadedSettings;
            }

            // Apply defaults
            _appSettings.AbortTodoOverdue ??= true;
            _appSettings.AbortMotionZombie ??= true;
            _appSettings.Debug ??= false;
            _appSettings.Refresh ??= 30;
            _appSettings.Log ??= string.Empty;

            if (failLoadSettings) {
                Logger.Instance.Log(LogLevel.WARNING, "Failed to load the app settings; defaults will be applied.");
            }
            else if (missingFields.Any()) {
                Logger.Instance.Log(LogLevel.WARNING, $"App settings missing (defaults will be applied): {string.Join(", ", missingFields)}.");
            }
        }

        private static void LoadIdsData()
        {
            try {
                if (!File.Exists("ids.json")) {
                    Logger.Instance.Log(LogLevel.WARNING, "IDs file does not exist; a new one will be created.");
                    _idsData = JSONArchiver.Load<IdsData>("ids.json", true);
                }
                else {
                    _idsData = JSONArchiver.Load<IdsData>("ids.json", false);
                }
            }
            catch (Exception ex) {
                Logger.Instance.Log(LogLevel.ERROR, ex.Message);
            }
        }

        private static async Task<bool> GetToDoData()
        {
            bool fail;
            (fail, _todoData) = await ToDoCommands.GetDataAsync(_appSettings.AbortTodoOverdue ?? true);

            if (fail) return true;

            try {
                JSONArchiver.Save("todo.json", _todoData);
                return false;
            }
            catch (Exception ex) {
                Logger.Instance.Log(LogLevel.ERROR, ex.Message);
                return true;
            }

        }

        private static async Task<bool> GetMotionData()
        {
            bool fail;
            (fail, _motionData) = await MotionCommands.GetDataAsync();

            if (fail) return true;

            try {
                JSONArchiver.Save("motion.json", _motionData);
                return false;
            }
            catch (Exception ex) {
                Logger.Instance.Log(LogLevel.ERROR, ex.Message);
                return true;
            }
        }

        private static bool CheckMotionZombies()
        {
            Logger.Instance.Log(LogLevel.INFO, "Check Motion data for zombies...");

            List<string> missingProjects = new List<string>();
            foreach (var project in _motionData.Projects) {
                if (!_idsData.ListExistsInMotion(project.Id)) {
                    missingProjects.Add(project.Name);
                }
            }

            List<string> missingTasks = new List<string>();
            foreach (var task in _motionData.Tasks) {
                if (!_idsData.TaskExistsInMotion(task.Id)) {
                    missingTasks.Add(task.Name);
                }
            }

            // Display results
            if (missingProjects.Count > 0) {
                string message = $"Zombie Motion projects: {string.Join(", ", missingProjects)}";

                if (_appSettings.AbortMotionZombie ?? true) {
                    Logger.Instance.Log(LogLevel.ERROR, message);
                }
                else {
                    Logger.Instance.Log(LogLevel.WARNING, message);
                }
            }

            if (missingTasks.Count > 0) {
                string message = $"Zombie Motion tasks: {string.Join(", ", missingTasks)}";

                if (_appSettings.AbortMotionZombie ?? true) {
                    Logger.Instance.Log(LogLevel.ERROR, message);
                }
                else {
                    Logger.Instance.Log(LogLevel.WARNING, message);
                }
            }

            if ( (_appSettings.AbortMotionZombie ?? true) && ( (missingProjects.Count > 0) || (missingTasks.Count > 0) ) )
                return true;

            return false;
        }

        private static async Task<bool> Todo2Motion()
        {
            bool motion_changed = false;

            #region LISTS / PROJECTS
            Logger.Instance.Log(LogLevel.INFO, "Sync Lists...");
            foreach (var list in _todoData.Lists) {
                if (!_idsData.ListExistsInTodo(list.Id)) { // If the list does not exist in IdsData
                    // ADD LIST TO MOTION AS PROJECT

                    // Get Workspace
                    string workspace = GetWorkspaceName(list.DisplayName);
                    if (workspace == string.Empty) {
                        Logger.Instance.Log(LogLevel.ERROR, $"Failed to identify workspace for list '{list.DisplayName}'.");
                        return true;
                    }

                    // Get Workspace ID
                    string workspaceId = _motionData.GetWorkspaceId(workspace);
                    if (workspaceId == string.Empty) {
                        Logger.Instance.Log(LogLevel.ERROR, $"Failed to get ID for workspace '{workspace}'.");
                        return true;
                    }

                    // Add Project
                    string project_id = await MotionCommands.AddProjectAsync(list.DisplayName, workspaceId);
                    if (project_id == string.Empty) { return true; }

                    IdPair ids = new IdPair();
                    ids.ToDoId = list.Id;
                    ids.MotionId = project_id;
                    _idsData.Lists.Add(ids);

                    motion_changed = true;
                }
            }
            #endregion

            #region TASKS
            Logger.Instance.Log(LogLevel.INFO, "Sync Tasks...");

            foreach (var task in _todoData.Tasks) {
                if (!_idsData.TaskExistsInTodo(task.Id)) { // If the task does not exist in IdsData
                    // ADD TASK TO MOTION

                    // Get List Name
                    string listName = _todoData.GetListName(task.ListId);
                    if (listName == string.Empty) {
                        Logger.Instance.Log(LogLevel.ERROR, $"List name not found for list ID '{task.ListId}'.");
                        return true;
                    }

                    // Get Workspace
                    string workspace = GetWorkspaceName(listName);
                    if (workspace == string.Empty) {
                        Logger.Instance.Log(LogLevel.ERROR, $"Failed to identify workspace for list '{listName}'.");
                        return true;
                    }

                    // Get Workspace ID
                    string workspaceId = _motionData.GetWorkspaceId(workspace);
                    if (workspaceId == string.Empty) {
                        Logger.Instance.Log(LogLevel.ERROR, $"Failed to get ID for workspace '{workspace}'.");
                        return true;
                    }

                    // Get Project ID
                    string projectID = _idsData.GetProjectID(task.ListId);
                    if (projectID == string.Empty) {
                        Logger.Instance.Log(LogLevel.ERROR, $"Failed to get Project ID for List '{listName}'.");
                        return true;
                    }

                    // Add Task
                    Logger.Instance.Log(LogLevel.DEBUG, $"Add task '{listName} - {task.Title}' to Motion.");
                    if (task.DueDateTime == null) {
                        Logger.Instance.Log(LogLevel.ERROR, "DueDateTime cannot be null.");
                        return true;
                    }
                    string task_id = await MotionCommands.AddTaskAsync(task.Title, workspaceId, projectID, task.DueDateTime.GetUtcTime().ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                    if (task_id == string.Empty) { return true; }

                    IdPair ids = new IdPair();
                    ids.ToDoId = task.Id;
                    ids.MotionId = task_id;
                    _idsData.Tasks.Add(ids);

                    motion_changed = true;
                }
                else { // If the task exists in IdsData
                    // EDIT TASK IN MOTION

                    // Get Project ID
                    string task_id = _idsData.GetTaskID(task.Id);

                    if (task.DueDateTime == null) {
                        Logger.Instance.Log(LogLevel.ERROR, "DueDateTime cannot be null.");
                        return true;
                    }
                    if (!_motionData.TaskHasChanged(task_id, task.Title, task.DueDateTime.GetUtcTime()))
                        continue;

                    // Edit task
                    Logger.Instance.Log(LogLevel.DEBUG, $"Edit task in Motion > {task.Title}");
                    if (await MotionCommands.EditTaskAsync(task_id, task.Title, task.DueDateTime.GetUtcTime().ToString("yyyy-MM-ddTHH:mm:ss.fff"))) return true;
                }
            }
            #endregion

            JSONArchiver.Save("ids.json", _idsData);

            if (motion_changed) {
                await GetMotionData();
            }

            return false;
        }

        private static async Task<bool> DeleteTasks()
        {
            List<string> tasks_to_del_motion = new List<string>();
            List<string> tasks_to_del_todo = new List<string>();

            //TODO 1 - Try-catch missing

            // For each entry in `ids.json` if not in `todo.json`, delete from Motion.
            foreach (var task in _idsData.Tasks) {
                if (!_todoData.TaskExists(task.ToDoId)) {
                    tasks_to_del_motion.Add(task.MotionId);
                }
            }

            // For each entry in `ids.json` if not in `motion.json`, mark as Completed in ToDo.
            foreach (var task in _idsData.Tasks) {
                if (!_motionData.TaskExists(task.MotionId)) {
                    tasks_to_del_todo.Add(task.ToDoId);
                }
            }

            // Delete tasks in Motion
            foreach (string task_id in tasks_to_del_motion) {
                string task_name = _motionData.GetTasKName(task_id);
                Logger.Instance.Log(LogLevel.DEBUG, $"Delete Motion task '{task_name}'.");

                await MotionCommands.DeleteTask(task_id);

                if (_idsData.DeleteTaskPair(null, task_id)) {
                    Logger.Instance.Log(LogLevel.ERROR, "Failed to removed entry from ids.json.");
                    return true;
                }
            }

            // Mark tasks as completed in ToDo
            foreach (string task_id in tasks_to_del_todo) {
                string task_name = _todoData.GetTaskName(task_id);
                Logger.Instance.Log(LogLevel.DEBUG, $"Mark task '{task_name}' as completed in ToDo.");

                string list_id = _todoData.GetListId(task_id);

                await ToDoCommands.MarkAsCompletedAsync(list_id, task_id);

                if (_idsData.DeleteTaskPair(task_id, null)) {
                    Logger.Instance.Log(LogLevel.ERROR, "Failed to removed entry from ids.json.");
                    return true;
                }
            }

            JSONArchiver.Save("ids.json", _idsData);

            return false;
        }

        private static string GetWorkspaceName(string listName)
        {
            if (listName == "Tasks") {
                return "My Tasks (Private)";
            }

            string pattern;

            pattern = @"^[A-Za-z]{3}-[A-Za-z]-\d{3}";
            if (Regex.IsMatch(listName, pattern)) {
                return "JDR";
            }

            pattern = @"^HPC\d{5}";
            if (Regex.IsMatch(listName, pattern)) {
                return "HARDPATH";
            }

            return string.Empty;
        }
    }
}
