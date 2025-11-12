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
                    new { role = "system", content = "You are NewDawn Assistant, an AI trained to answer only questions about the NewDawnProperties app, property listings, troubleshooting, and app usage. Politely decline unrelated questions." },
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
