using Ordo.Commands;

namespace Ordo
{
    internal static class CommandHandler
    {
        public static async Task HandleCommand(string input)
        {
            input = input.Trim();

            #region General
            if (input.Equals("help", StringComparison.OrdinalIgnoreCase)) {
                HelpCommand.Execute();
            }
            if (input.Equals("cleanup", StringComparison.OrdinalIgnoreCase)) {
                CleanupCommand.Execute();
            }
            else if (input.Equals("get data", StringComparison.OrdinalIgnoreCase)) {
                await GetTasksCommand.ExecuteAsync();
                await GetEventsCommand.ExecuteAsync();
            }
            else if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) || input.Equals("quit", StringComparison.OrdinalIgnoreCase)) {
                Console.WriteLine("Goodbye! Thank you for using Ordo.");
                Environment.Exit(0); // Exit the application
            }
            #endregion

            #region Tasks
            else if (input.Equals("get tasks", StringComparison.OrdinalIgnoreCase)) {
                await GetTasksCommand.ExecuteAsync();
            }
            else if (input.Equals("set durations", StringComparison.OrdinalIgnoreCase)) {
                SetDurationsCommand.Execute();
            }
            else if (input.Equals("set duration", StringComparison.OrdinalIgnoreCase)) {
                SetDurationCommand.Execute();
            }
            else if (input.Equals("reset durations", StringComparison.OrdinalIgnoreCase)) {
                ResetDurationsCommand.Execute();
            }
            #endregion

            #region Events
            else if (input.Equals("get events", StringComparison.OrdinalIgnoreCase)) {
                await GetEventsCommand.ExecuteAsync();
            }
            else if (input.Equals("delete events", StringComparison.OrdinalIgnoreCase)) {
                await DeleteEventsCommand.ExecuteAsync();
            }
            #endregion

            #region Scheduling
            else if (input.Equals("plan", StringComparison.OrdinalIgnoreCase)) {
                await PlanCommand.ExecuteAsync();
            }
            #endregion

            else {
                Console.WriteLine($"Unknown command: {input}");
            }
        }
    }
}
