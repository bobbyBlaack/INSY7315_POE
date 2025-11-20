using NewDawnProperties.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace NewDawnProperties.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;
        private readonly string _firebaseKey = "AIzaSyAPvCRpwZR9m59ZSTbl8WTXtIVGLOY5evg";

        public ApiService(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://insy7315-api-v1.onrender.com/");
        }

        // Login using Firebase
        public async Task<UserModel?> LoginAsync(string email, string password)
        {
            var loginPayload = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginPayload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _client.PostAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_firebaseKey}",
                content
            );

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            var firebase = JsonSerializer.Deserialize<FirebaseLoginResponse>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (firebase == null || string.IsNullOrEmpty(firebase.LocalId))
                return null;

            return await GetUserProfile(firebase.LocalId);
        }

        // ------------------------------------------------------
        // FETCH USER PROFILE
        // ------------------------------------------------------
        public async Task<UserModel?> GetUserProfile(string uid)
        {
            var response = await _client.GetAsync($"api/Profile/user/{uid}");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            var user = JsonSerializer.Deserialize<UserModel>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (user == null)
                return null;

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("name", out var nameProp))
                user.FullName = nameProp.GetString();

            if (root.TryGetProperty("phoneNumber", out var phoneProp))
                user.Phone = phoneProp.GetString();

            if (root.TryGetProperty("userType", out var roleProp))
                user.Role = roleProp.GetString();

            if (root.TryGetProperty("located", out var blockProp))
                user.Block = blockProp.GetString();

            if (root.TryGetProperty("unit", out var unitProp))
                user.Unit = unitProp.GetString();

            return user;
        }

        // ------------------------------------------------------
        // GET CARETAKER TASKS
        // ------------------------------------------------------
        public async Task<List<MaintenanceTaskModel>> GetCaretakerTasksAsync()
        {
            var response = await _client.GetAsync("api/all/MaintenanceRequests");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<MaintenanceTaskModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<MaintenanceTaskModel>();
        }

        // ------------------------------------------------------
        // GET ALL MESSAGES FOR USER
        // ------------------------------------------------------
        public async Task<List<CommunicationMessage>> GetMessagesAsync(string uid)
        {
            var response = await _client.GetAsync($"api/Messages/get/all/users/messages/{uid}");

            if (!response.IsSuccessStatusCode)
                return new List<CommunicationMessage>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<CommunicationMessage>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<CommunicationMessage>();
        }

        // ------------------------------------------------------
        // SEND MESSAGE (PUT)
        // ------------------------------------------------------
        public async Task<bool> SendMessageAsync(CommunicationMessage message)
        {
            var payload = new
            {
                senderId = message.SenderId,
                senderName = message.SenderName,
                role = message.Role,
                message = message.Message,
                recipientId = message.RecipientId,
                recipientName = message.RecipientName
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _client.PutAsync("api/Messages/add", content);

            return response.IsSuccessStatusCode;
        }

        public class FirebaseLoginResponse
        {
            public string IdToken { get; set; }
            public string Email { get; set; }
            public string LocalId { get; set; }
            public bool Registered { get; set; }
        }
    }
}