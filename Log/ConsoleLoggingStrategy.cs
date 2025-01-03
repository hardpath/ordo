
namespace Ordo.Log
{
    public class ConsoleLoggingStrategy : ILoggingStrategy
    {
        public void Log(string level, string message)
        {
            Console.WriteLine($"[{level.ToUpper()}] {message}");
        }
    }
}
