using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        public async Task<Usuario> LoginAsync(string cpf, string senha)
        {
            try
            {
                cpf = cpf?.Trim();
                senha = senha?.Trim();

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("cpf", cpf),
                    new KeyValuePair<string, string>("senha", senha)
                });

                var response = await _http.PostAsync("login.php", content);

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<LoginResponse>(json);

                // 🔥 agora só verifica se veio usuário
                if (result != null && result.usuario != null)
                {
                    return new Usuario
                    {
                        CPF = cpf,
                        Senha = senha,
                        Nome = result.usuario.nome
                    };
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }

    // 🔥 ajustado para bater com o JSON real
    public class LoginResponse
    {
        [JsonPropertyName("mensagem")]
        public string Mensagem { get; set; }

        [JsonPropertyName("usuario")]
        public UsuarioApi usuario { get; set; }
    }

    public class UsuarioApi
    {
        [JsonPropertyName("id")]
        public string id { get; set; }

        [JsonPropertyName("nome")]
        public string nome { get; set; }
    }
}