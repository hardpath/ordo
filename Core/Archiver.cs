using System.Text.Json;

namespace Ordo.Core
{
    internal class Archiver
    {
        private readonly string _filePath;

        public Archiver(string filePath)
        {
            _filePath = filePath;
        }

        public void Save<T>(T data)
        {
            try {
                // Serialize the data object to JSON
                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions {
                    WriteIndented = true // Optional: Makes JSON output readable
                });

                // Write the serialized JSON to the file
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex) {
                // Log or handle exceptions appropriately
                throw new ArchiverException($"Failed to save data of type {typeof(T).Name}: {ex.Message}");
            }
        }

        public T Load<T>()
        {
            try {
                // Ensure the file exists
                if (!File.Exists(_filePath)) {
                    throw new ArchiverException($"File not found: {_filePath}");
                }

                // Read the JSON content from the file
                string json = File.ReadAllText(_filePath);

                // Deserialize the JSON into the specified type
                T data = JsonSerializer.Deserialize<T>(json) ?? throw new ArchiverException("Deserialized object is null.");

                return data;
            }
            catch (Exception ex) {
                // Throw a custom exception with a meaningful message
                throw new ArchiverException($"Failed to load data of type {typeof(T).Name}: {ex.Message}");
            }
        }
    }

    internal class ArchiverException : Exception
    {
        public ArchiverException(string message) : base(message) { }
    }
}
