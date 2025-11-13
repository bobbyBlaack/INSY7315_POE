using NewDawnProperties.Models;
using System.Net.Http;
using System.Text.Json;

namespace NewDawnProperties.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;

        public ApiService(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://insy7315-api-v1.onrender.com/");
        }

        public async Task<List<PropertyModel>> GetPropertiesAsync()
        {
            var response = await _client.GetAsync("api/Properties");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<PropertyModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<MaintenanceTaskModel>> GetCaretakerTasksAsync()
        {
            var response = await _client.GetAsync("api/all/MaintenanceRequests");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<MaintenanceTaskModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}