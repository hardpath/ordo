
namespace Ordo.Log
{
    public class WindowsEventLoggingStrategy : ILoggingStrategy
    {
        public void Log(string level, string message)
        {
            // Implementation for logging to Windows Events will go here
            // For now, we'll simulate it with a console message
            Console.WriteLine($"[WINDOWS EVENT] [{level.ToUpper()}] {message}");
        }
    }
}
