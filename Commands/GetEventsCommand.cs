using Ordo.Core;

namespace Ordo.Commands
{
    internal static class GetEventsCommand
    {
        //public static async Task Execute()
        public static void Execute()
        {
            Console.WriteLine("Fetching events from Microsoft Calendar...");
            try {
                // Add call here
                Console.WriteLine("Not implemented yet.");
            }
            catch (Exception ex) {
                Console.WriteLine($"An error occurred while fetching tasks: {ex.Message}");
            }
        }
    }
}
