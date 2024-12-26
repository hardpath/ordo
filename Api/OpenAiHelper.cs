using OpenAI.Chat;

namespace ordo.Api
{
    public static class OpenAiHelper
    {
        public static void TestOpenAiApi(string apiKey)
        {
            try {
                // Initialize the ChatClient with the API key and desired model
                ChatClient client = new ChatClient(model: "gpt-4", apiKey: apiKey);

                // Send a simple prompt to the assistant
                ChatCompletion completion = client.CompleteChat("Say 'this is a test.'");

                // Display the response
                Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
            }
            catch (Exception ex) {
                Console.WriteLine($"An error occurred while testing the OpenAI API: {ex.Message}");
            }
        }
    }
}
