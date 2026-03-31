using System.Net.Http.Json;
using AppOcorrencias.Models;

namespace AppOcorrencias.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;

        private const string BaseUrl = "https://guardsystem.up.railway.app/api/";

        public ApiService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        // 🔹 Buscar usuários
        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Usuario>>("usuarios.php");
            }
            catch
            {
                return new List<Usuario>();
            }
        }

        // 🔹 Login via PHP
        public async Task<Usuario> LoginAsync(string cpf, string senha)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("cpf", cpf),
                    new KeyValuePair<string, string>("senha", senha)
                });

                var response = await _http.PostAsync("login.php", content);

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();

                // ⚠️ depende do retorno do seu PHP
                return System.Text.Json.JsonSerializer.Deserialize<Usuario>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}