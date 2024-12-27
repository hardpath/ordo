namespace Ordo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to Ordo - AI Task Scheduler!");
            Console.WriteLine("Type 'help' to see the list of available commands.");

            while (true) {
                Console.Write("Ordo> ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;

                await CommandHandler.HandleCommand(input);
            }
        }
    }
}
