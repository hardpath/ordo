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
            else if (input.Equals("get tasks", StringComparison.OrdinalIgnoreCase)) {
                await TaskCommand.FetchTasks();
            }
            else if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) {
                Console.WriteLine("Goodbye! Thank you for using Ordo.");
                Environment.Exit(0); // Exit the application
            }
            else {
                Console.WriteLine($"Unknown command: {input}");
            }
        }
    }
}
