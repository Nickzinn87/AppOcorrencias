using System.Text;
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

                var content = new StringContent(
                    JsonSerializer.Serialize(new
                    {
                        cpf = cpf,
                        senha = senha
                    }),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _http.PostAsync("login.php", content);

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<LoginResponse>(json);

                if (result != null && result.usuario != null)
                {
                    await App.Current.MainPage.DisplayAlert("RESPOSTA", json, "OK");

                    return new Usuario
                    {
                        CPF = cpf,
                        Senha = senha,
                        Nome = result.usuario.nome
                    };
                }

                // debug temporário
                await App.Current.MainPage.DisplayAlert("RESPOSTA", json, "OK");

                return null;
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    App.Current.MainPage.DisplayAlert("ERRO", ex.Message, "OK")
                );

                return null;
            }
        }
    }

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
        public int id { get; set; }

        [JsonPropertyName("nome")]
        public string nome { get; set; }
    }
}