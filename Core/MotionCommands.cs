using Ordo.Api;
using Ordo.Core;
using Ordo.Log;
using Ordo.Models;

namespace ordo.Core
{
    internal class MotionCommands
    {
        public static async Task<(bool, MotionData)> GetDataAsync()
        {
            Logger.Instance.Log(LogLevel.INFO, "Retrieving Motion data...");

            MotionData motionData = new MotionData();
            try
            {
                // Get WORKSPACES
                Logger.Instance.Log(LogLevel.DEBUG, "... Workspaces...");
                motionData.Workspaces = await MotionHelper.Instance.GetWorkspacesAsync();

                // Get PROJECTS
                Logger.Instance.Log(LogLevel.DEBUG, "... Projects...");
                foreach (var workspace in motionData.Workspaces)
                {
                    List<MotionProject> projects = await MotionHelper.Instance.GetProjectsAsync(workspace.Id);
                    foreach (var project in projects)
                    {
                        motionData.Projects.Add(project);
                    }
                }

                // Get TASKS
                Logger.Instance.Log(LogLevel.DEBUG, "... Tasks...");
                motionData.Tasks = await MotionHelper.Instance.GetTasksAsync();

                return (false, motionData);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(LogLevel.ERROR, $"{ex.Message}");
                return (true, motionData);
            }
        }
    }
}
