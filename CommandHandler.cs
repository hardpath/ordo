using Ordo.Commands;

namespace Ordo
{
    internal static class CommandHandler
    {
        public static async Task HandleCommand(string input)
        {
            input = input.Trim();

            if (input.Equals("help", StringComparison.OrdinalIgnoreCase)) {
                HelpCommand.Execute();
            }
            else if (input.Equals("get data", StringComparison.OrdinalIgnoreCase)) {
                await GetTasksCommand.Execute();
                await GetEventsCommand.Execute();
            }
            else if (input.Equals("get tasks", StringComparison.OrdinalIgnoreCase)) {
                await GetTasksCommand.Execute();
            }
            else if (input.Equals("get events", StringComparison.OrdinalIgnoreCase)) {
                await GetEventsCommand.Execute();
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
            else if (input.Equals("plan", StringComparison.OrdinalIgnoreCase)) {
                await PlanCommand.ExecuteAsync();
            }
            else if (input.Equals("delete", StringComparison.OrdinalIgnoreCase)) {
                await PlanCommand.ExecuteAsync();
            }
            else if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) || input.Equals("quit", StringComparison.OrdinalIgnoreCase)) {
                Console.WriteLine("Goodbye! Thank you for using Ordo.");
                Environment.Exit(0); // Exit the application
            }
            else {
                Console.WriteLine($"Unknown command: {input}");
            }
        }
    }
}
