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
            else if (input.Equals("get data", StringComparison.OrdinalIgnoreCase)) {
                await GetToDoDataCommand.ExecuteAsync();
                await GetMotionDataCommand.ExecuteAsync();
            }
            else if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) || input.Equals("quit", StringComparison.OrdinalIgnoreCase)) {
                Console.WriteLine("Goodbye! Thank you for using Ordo.");
                Environment.Exit(0); // Exit the application
            }
            #endregion

            #region ToDo
            else if (input.Equals("get todo data", StringComparison.OrdinalIgnoreCase)) {
                await GetToDoDataCommand.ExecuteAsync();
            }
            #endregion

            #region Motion
            else if (input.Equals("get motion data", StringComparison.OrdinalIgnoreCase)) {
                await GetMotionDataCommand.ExecuteAsync();
            }
            #endregion

            else {
                Console.WriteLine($"Unknown command: {input}");
            }
        }
    }
}
