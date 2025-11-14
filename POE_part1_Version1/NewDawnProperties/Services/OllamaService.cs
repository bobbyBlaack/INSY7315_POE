using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace NewDawnProperties.Services
{
    public class OllamaService
    {
        private readonly HttpClient _httpClient;

        public OllamaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:11434/");
        }

        public async Task<string> AskOllamaAsync(string userPrompt)
        {
            // Use the /api/chat endpoint for conversation
            var requestBody = new
            {
                model = "llama3.2",
                messages = new[]
                {
                    new { role = "system", content = "You are New Dawn Assistant, an AI trained to answer only questions about the New Dawn Properties app, property listings, troubleshooting, and app usage. Politely decline unrelated questions." +
                    "                                   If they want to manage leases tell them to navigate to the Manage Leases Button in the navigation bar." +
                    "                                   If they want help managing their property listings, direct them to the Update Listings Button in the navigation bar." +
                    "                                   If they want to view their escalations for thier properties, direct them to use the Escalations button in the navigation bar." +
                    "                                   If they want to view their profile or notifications , There are profile and Notification buttons in the navigation bar." +
                    "                                   You can also assist them with payment calculations and property management strategies" +
                    "                                   If they ask any questions not related to New Dawn Properties, real estate or the app, please temm them to speak to an admin or reword their prompt" },
                                                        
                                                        
                    new { role = "user", content = userPrompt }
                },
                stream = false
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("api/chat", content);
            response.EnsureSuccessStatusCode();

            var responseText = await response.Content.ReadAsStringAsync();

            // Parse the response
            using var doc = JsonDocument.Parse(responseText);
            var aiMessage = doc.RootElement
                               .GetProperty("message")
                               .GetProperty("content")
                               .GetString();

            return aiMessage ?? "Sorry, I didn’t get a response from the AI.";
        }
    }
}
