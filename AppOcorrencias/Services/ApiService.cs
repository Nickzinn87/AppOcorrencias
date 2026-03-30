using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using AppOcorrencias.Models;

namespace AppOcorrencias.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;

        // 🔗 URL da sua API
        private const string BaseUrl = "https://guardsystem.infinityfree.me/";

        public ApiService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        // 🔹 Buscar usuários da web
        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Usuario>>("usuarios");
            }
            catch
            {
                return new List<Usuario>();
            }
        }

        // 🔹 Login via API
        public async Task<Usuario> LoginAsync(string cpf, string senha)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("login", new
                {
                    CPF = cpf,
                    Senha = senha
                });

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Usuario>();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
