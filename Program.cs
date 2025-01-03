
using Microsoft.Extensions.Configuration;
using Microsoft.Graph.Models;
using ordo.Core;
using Ordo.Core;
using Ordo.Log;
using Ordo.Models;

namespace Ordo
{
    class Program
    {
        private static AppSettings _appSettings = new AppSettings();
        private static TodoData _todoData = new TodoData();
        private static MotionData _motionData = new MotionData();
        private static IdsData _idsData = new IdsData();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to Ordo, the ToDo-Motion Synchroniser");

            Logger.Instance.SetLoggingStrategy(new ConsoleLoggingStrategy());

            LoadConfiguration();

            Logger.DebugMode = _appSettings.Debug ?? false;
            Logger.Instance.Log(LogLevel.INFO, "Synchronising....");

            LoadIdsData();

            if (await SynchroniseToDoData()) return;

            if (await SynchroniseMotionData()) return;

            if (CheckMotionZombies()) return;

            Logger.Instance.Log(LogLevel.INFO, "Synchronisation completed.");
        }

        private static void LoadConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                .Build();

            bool failLoadSettings = false;
            List<string> missingFields = new List<string>();
            AppSettings? loadedSettings = configuration.Get<AppSettings>();

            if (loadedSettings == null) {
                failLoadSettings = true;
            }
            else {
                missingFields = loadedSettings.Validate();
                _appSettings = loadedSettings;
            }

            // Apply defaults
            _appSettings.AbortTodoOverdue ??= true;
            _appSettings.AbortMotionZombie ??= true;
            _appSettings.Debug ??= false;
            _appSettings.Refresh ??= 30;
            _appSettings.Log ??= string.Empty;

            if (failLoadSettings) {
                Logger.Instance.Log(LogLevel.WARNING, "Failed to load the app settings; defaults will be applied.");
            }
            else if (missingFields.Any()) {
                Logger.Instance.Log(LogLevel.WARNING, $"App settings missing (defaults will be applied): {string.Join(", ", missingFields)}.");
            }
        }

        private static void LoadIdsData()
        {
            try {
                if (!File.Exists("ids.json")) {
                    Logger.Instance.Log(LogLevel.WARNING, "IDs file does not exist; a new one will be created.");
                    _idsData = JSONArchiver.Load<IdsData>("ids.json", true);
                }
                else {
                    _idsData = JSONArchiver.Load<IdsData>("ids.json", false);
                }
            }
            catch (Exception ex) {
                Logger.Instance.Log(LogLevel.ERROR, ex.Message);
            }
        }

        private static async Task<bool> SynchroniseToDoData()
        {
            bool fail;
            (fail, _todoData) = await ToDoCommands.GetDataAsync(_appSettings.AbortTodoOverdue ?? true);

            if (fail) return true;

            try {
                JSONArchiver.Save("todo.json", _todoData);
                Logger.Instance.Log(LogLevel.DEBUG, $"ToDo data saved in 'todo.json'.");
                return false;
            }
            catch (Exception ex) {
                Logger.Instance.Log(LogLevel.ERROR, ex.Message);
                return true;
            }

        }

        private static async Task<bool> SynchroniseMotionData()
        {
            bool fail;
            (fail, _motionData) = await MotionCommands.GetDataAsync();

            if (fail) return true;

            try {
                JSONArchiver.Save("motion.json", _motionData);
                Logger.Instance.Log(LogLevel.DEBUG, $"Motion data saved in 'motion.json'.");
                return false;
            }
            catch (Exception ex) {
                Logger.Instance.Log(LogLevel.ERROR, ex.Message);
                return true;
            }
        }

        private static bool CheckMotionZombies()
        {
            List<string> missingTasks = new List<string>();

            foreach (var task in _motionData.Tasks) {
                bool found = false;
                foreach (var id in _idsData.Ids) {
                    if (task.Id == id.MotionId) {
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    missingTasks.Add(task.Name);
                }
            }

            // Display results
            if (missingTasks.Count > 0) {
                string message = $"Zombie motion tasks: {string.Join(", ", missingTasks)}";

                if (_appSettings.AbortMotionZombie ?? true) {
                    Logger.Instance.Log(LogLevel.ERROR, message);
                    return true;
                }

                Logger.Instance.Log(LogLevel.WARNING, message);
            }

            return false;
        }
    }
}
