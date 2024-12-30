using Ordo.Core;

namespace Ordo.Commands
{
    internal static class ShowOverdueCommand
    {
        public static void Execute()
        {
            var projectsData = ProjectsArchiver.LoadData();

            if (projectsData == null ) {
                Console.WriteLine("[ERROR] No data found.");
                return;
            }
            if (projectsData.GetOverDueTasksCount() == 0 ) {
                Console.WriteLine("[INFO] No overdue tasks.");
                return;
            }
            projectsData.PrintOverDueTasks();
        }
    }
}
