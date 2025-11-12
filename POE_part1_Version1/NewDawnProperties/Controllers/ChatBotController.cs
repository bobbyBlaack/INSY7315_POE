using Microsoft.AspNetCore.Mvc;
using NewDawnProperties.Services;
using System;
using System.Threading.Tasks;

namespace NewDawnProperties.Controllers
{
    public class ChatBotController : Controller
    {
        private readonly OllamaService _ollama;

        public ChatBotController(OllamaService ollama)
        {
            _ollama = ollama;
        }

        public IActionResult MyAiAssistant()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string message)
        {
            try
            {
                var aiResponse = await _ollama.AskOllamaAsync(message);
                return Json(new { response = aiResponse });
            }
            catch (Exception ex)
            {
                return Json(new { response = $"Error: {ex.Message}" });
            }
        }
    }
}
