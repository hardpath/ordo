using Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;
using OpenAI.Chat;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ordo.Api
{
    public static class OpenAiHelper
    {
        public static async Task<string> GetScheduleAsync(string jsonData)
        {
            try {
                // Load the prompt from the file
                string promptFilePath = Path.Combine(AppContext.BaseDirectory, "Prompts\\scheduling.txt");
                if (!File.Exists(promptFilePath)) {
                    Console.WriteLine($"[ERROR] Prompt file not found ({promptFilePath})");
                }

                string prompt = File.ReadAllText(promptFilePath);

                // Replace placeholder with actual JSON data
                DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour + 1, 0, 0);

                prompt = prompt.Replace("{{START}}", start.ToString("dd/MM/yyyy HH:mm:ss"));
                //Console.WriteLine(prompt); return null;
                prompt = prompt.Replace("{{JSON_DATA}}", jsonData);

                // Initialize the ChatClient with the API key and desired model
                string apiKey = GetApiKey();
                ChatClient client = new ChatClient(model: "gpt-4o", apiKey: apiKey);

                Console.WriteLine("[INFO] Scheduling in progress...");

                // Send the chat request
                ChatCompletion completion = await client.CompleteChatAsync(prompt);

                // Return the assistant's response
                return completion.Content[0].Text;
            }
            catch (Exception ex) {
                throw new Exception($"[ERROR] Error while interacting with OpenAI API: {ex.Message}");
            }
        }
        
        public static string? TestOpenAiApi()
        {
            try {
                string apiKey = GetApiKey();

                // Initialize the ChatClient with the API key and desired model
                ChatClient client = new ChatClient(model: "gpt-4", apiKey: apiKey);

                // Send a simple prompt to the assistant
                ChatCompletion completion = client.CompleteChat("Say 'this is a test.'");

                // Display the response
                return completion.Content[0].Text;
            }
            catch (Exception ex) {
                Console.WriteLine($"An error occurred while testing the OpenAI API: {ex.Message}");
                return null;
            }
        }

        #region Private
        private static string GetApiKey()
        {
            try {
                // Build configuration
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory) // Set the base path to the application's directory
                    .AddJsonFile("config.json", optional: false, reloadOnChange: true) // Load the JSON file
                    .Build();

                // Retrieve the API key from the OpenAiSettings section
                var apiKey = configuration["OpenAiSettings:ApiKey"];

                if (string.IsNullOrEmpty(apiKey)) {
                    throw new Exception("API key is missing or empty in config.json.");
                }

                return apiKey;
            }
            catch (Exception ex) {
                throw new Exception($"Error retrieving API key: {ex.Message}");
            }
        }
        #endregion
    }
}
