
namespace Ordo.Log
{
    public interface ILoggingStrategy
    {
        void Log(string level, string message);
    }
}
