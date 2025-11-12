using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NewDawnProperties.Controllers
{
    public class OllamaController : Controller
    {
        private readonly HttpClient _httpClient;

        public OllamaController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AskOllama(string message)
        {
            
            var modelName = " llama3.2";

            // Prepare request payload for Ollama
            var requestBody = new
            {
                model = modelName,
                prompt = message
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("http://localhost:11434/api/generate", content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();

                // Ollama returns partial JSON lines — get the last one with "response"
                var lastResponse = responseString.Split('\n')
                    .Where(line => line.Contains("\"response\""))
                    .LastOrDefault();

                if (lastResponse == null)
                    return Json(new { response = "⚠ No response from Ollama." });

                using var doc = JsonDocument.Parse(lastResponse);
                var aiText = doc.RootElement.GetProperty("response").GetString();

                return Json(new { response = aiText });
            }
            catch (Exception ex)
            {
                return Json(new { response = $" Error connecting to Ollama: {ex.Message}" });
            }
        }
    }
}
