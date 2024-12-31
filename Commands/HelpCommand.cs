namespace Ordo.Commands
{
    internal static class HelpCommand
    {
        public static void Execute()
        {
            Console.WriteLine("Available Commands:");
            Console.WriteLine("  - get data               Retrieve tasks from ToDo and events from Calendar.");
            Console.WriteLine("  - exit/quit              Quit the application.");
            Console.WriteLine("ToDo:");
            Console.WriteLine("  - get todo data          Retrieve tasks from ToDo.");
            Console.WriteLine("Motion:");
            Console.WriteLine("  - get motion data        Retrieve data from Motion");
        }
    }
}
