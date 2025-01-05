using Microsoft.VisualBasic;
using Ordo.Api;
using Ordo.Log;
using Ordo.Models;

namespace ordo.Core
{
    internal class MotionCommands
    {
        public static async Task<(bool, MotionData)> GetDataAsync()
        {
            Logger.Instance.Log(LogLevel.INFO, "Get data from Motion...");

            MotionData motionData = new MotionData();
            try
            {
                // Get WORKSPACES
                motionData.Workspaces = await MotionHelper.Instance.GetWorkspacesAsync();

                // Get PROJECTS
                foreach (var workspace in motionData.Workspaces)
                {
                    List<MotionProject> projects = await MotionHelper.Instance.GetProjectsAsync(workspace.Id);
                    foreach (var project in projects)
                    {
                        motionData.Projects.Add(project);
                    }
                }

                // Get TASKS
                motionData.Tasks = await MotionHelper.Instance.GetTasksAsync();

                return (false, motionData);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(LogLevel.ERROR, $"{ex.Message}");
                return (true, motionData);
            }
        }

        public static async Task<string> AddProjectAsync(string projectName, string workspaceId)
        {
            try {
                string id = await MotionHelper.Instance.AddProjectAsync(projectName, workspaceId);

                if (id == string.Empty) {
                    Logger.Instance.Log(LogLevel.ERROR, $"Failed to add project '{projectName}' to Motion.");
                    return string.Empty;
                }

                return id;
            }
            catch (Exception ex) {
                Logger.Instance.Log(LogLevel.ERROR, $"{ex.Message}");
                return string.Empty;
            }
        }

        public static async Task<string> AddTaskAsync(string taskName, string workspaceId, string projectID, string dueDate)
        {
            try {
                string id = await MotionHelper.Instance.AddTaskAsync(taskName, workspaceId, projectID, dueDate);

                if (id == string.Empty) {
                    Logger.Instance.Log(LogLevel.ERROR, $"Failed to add task '{taskName}' to Motion.");
                    return string.Empty;
                }

                return id;
            }
            catch (Exception ex) {
                Logger.Instance.Log(LogLevel.ERROR, $"{ex.Message}");
                return string.Empty;
            }
        }

        public static async Task<bool> EditTaskAsync(string taskId, string taskName, string dueDate)
        {
            try {
                await MotionHelper.Instance.EditTaskAsync(taskId, taskName, dueDate);
                return false;
            }
            catch (Exception ex) {
                Logger.Instance.Log(LogLevel.ERROR, $"{ex.Message}");
                return true;
            }
        }

        public static async Task<bool> DeleteTask(string taskId)
        {
            try {
                await MotionHelper.Instance.DeleteTaskAsync(taskId);
                return false;
            }
            catch (Exception ex) {
                Logger.Instance.Log(LogLevel.ERROR, $"{ex.Message}");
                return true;
            }
        }
    }
}
