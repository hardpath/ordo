namespace Ordo.Commands
{
    internal static class HelpCommand
    {
        public static void Execute()
        {
            Console.WriteLine("Available Commands:");
            Console.WriteLine("- get tasks        Retrieve tasks from Microsoft ToDo.");
            Console.WriteLine("- exit             Quit the application.");
        }
    }
}
