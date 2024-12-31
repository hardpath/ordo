
using Ordo.Api;
using Ordo.Core;
using Ordo.Models;

namespace Ordo.Commands
{
    internal class GetMotionDataCommand
    {
        private static string _filename = "motion.json";

        public static async Task ExecuteAsync()
        {
            Console.WriteLine("[INFO] Retrieving Motion data...");

            MotionData motionData = new MotionData();
            try {
                var helper = MotionHelper.GetInstance();

                // Get WORKSPACES
                Console.WriteLine("[DEBUG] ... Workspaces...");
                motionData.Workspaces = await helper.GetWorkspacesAsync();

                // Get PROJECTS
                Console.WriteLine("[DEBUG] ... Projects...");
                foreach (var workspace in motionData.Workspaces) {
                    List<MotionProject> projects = await helper.GetProjectsAsync(workspace.Id);
                    foreach (var project in projects) {
                        motionData.Projects.Add(project);
                    }
                }

                // Get TASKS
                Console.WriteLine("[DEBUG] ... Tasks...");
                motionData.Tasks = await helper.GetTasksAsync();

                new Archiver(_filename).Save(motionData);
                Console.WriteLine($"[INFO] Motion data saved in {_filename}.");
            }
            catch (Exception ex) {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }
    }
}
