
namespace Ordo.Log
{
    public sealed class Logger
    {
        private static readonly object _lock = new object();
        private static Logger? _instance;

        private ILoggingStrategy _loggingStrategy;

        public static bool DebugMode { get; set; } = false;

        private Logger()
        {
            _loggingStrategy = new ConsoleLoggingStrategy(); // Default logging strategy
        }

        public static Logger Instance
        {
            get {
                if (_instance == null) {
                    lock (_lock) {
                        if (_instance == null) {
                            _instance = new Logger();
                        }
                    }
                }
                return _instance;
            }
        }

        public void SetLoggingStrategy(ILoggingStrategy strategy)
        {
            _loggingStrategy = strategy;
        }

        public void Log(LogLevel level, string message)
        {
            if ( (DebugMode == false) && (level == LogLevel.DEBUG) ) return;

            _loggingStrategy.Log(level.ToString(), message);
        }
    }

    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARNING,
        ERROR
    }
}
