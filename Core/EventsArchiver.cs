using Ordo.Models;
using System.Text.Json;

namespace Ordo.Core
{
    internal static class EventsArchiver
    {
        private const string FilePath = "events.json";

        internal static EventsData LoadData()
        {
            EventsData? eventsData;

            try {
                if (!File.Exists(FilePath)) {
                    Console.WriteLine($"[WARNING] File ${FilePath} not found; Returning an empty list.");
                    return new EventsData();
                }

                string json = File.ReadAllText(FilePath);

                eventsData = JsonSerializer.Deserialize<EventsData>(json);
                if (eventsData == null) {
                    Console.WriteLine("[WARNING] Deserialization returned null; creating a new empty EventsData object.");
                    eventsData = new EventsData();
                }
            }
            catch (JsonException ex) {
                Console.WriteLine($"[ERROR] Failed to deserialize events.json: {ex.Message}");
                eventsData = new EventsData();
            }

            return eventsData;
        }

        internal static void SaveData(EventsData eventsData)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            string json = JsonSerializer.Serialize(eventsData, options);
            File.WriteAllText(FilePath, json);
        }
    }
}
