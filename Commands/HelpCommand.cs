namespace Ordo.Commands
{
    internal static class HelpCommand
    {
        public static void Execute()
        {
            Console.WriteLine("Available Commands:");
            Console.WriteLine("  - get data          Retrieve tasks from ToDo and events from Calendar.");
            Console.WriteLine("  - get events        Retrieve events from Calendar.");
            Console.WriteLine("  - get tasks         Retrieve tasks from ToDo.");
            Console.WriteLine("  - set durations     Set durations for all the tasks without duration.");
            Console.WriteLine("  - set duration      Set durations for a specific task.");
            Console.WriteLine("  - reset durations   Clear durations for all the tasks.");
            Console.WriteLine("  - exit/quit         Quit the application.");
        }
    }
}
