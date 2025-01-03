using System.Text.Json;

namespace Ordo.Core
{
    internal class JSONArchiver
    {
        public static T Load<T>(string filePath, bool createIfNotExists = false)
        {
            try {
                // Check if the file exists
                if (!File.Exists(filePath)) {
                    if (createIfNotExists) {
                        File.WriteAllText(filePath, "{}");    // Create an empty JSON file
                        return Activator.CreateInstance<T>(); // Return default instance of T
                    }
                    else {
                        throw new ArchiverException($"File not found: {filePath}");
                    }
                }

                // Read the JSON content from the file
                string json = File.ReadAllText(filePath);

                // Deserialize the JSON into the specified type
                T data = JsonSerializer.Deserialize<T>(json) ?? throw new ArchiverException("Deserialized object is null.");

                return data;
            }
            catch (Exception ex) {
                // Throw a custom exception with a meaningful message
                throw new ArchiverException($"Failed to load data of type {typeof(T).Name}: {ex.Message}");
            }
        }

        public static void Save<T>(string filePath, T data)
        {
            try {
                // Serialize the data object to JSON
                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions {
                    WriteIndented = true // Optional: Makes JSON output readable
                });

                // Write the serialized JSON to the file
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex) {
                // Log or handle exceptions appropriately
                throw new ArchiverException($"Failed to save data of type {typeof(T).Name}: {ex.Message}");
            }
        }
    }

    internal class ArchiverException : Exception
    {
        public ArchiverException(string message) : base(message) { }
    }
}
