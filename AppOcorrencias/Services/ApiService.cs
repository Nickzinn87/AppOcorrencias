using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AppOcorrencias.Models;

namespace AppOcorrencias.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "https://guardsystem.hro.dev.br/api/";

        public ApiService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        // 🔹 adiciona o token JWT no cabeçalho, se existir usuário logado com token salvo
        private void AplicarTokenAuth()
        {
            var token = UserSingleton.Instance.UsuarioLogado?.Token;

            if (!string.IsNullOrWhiteSpace(token))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            else
                _http.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<Usuario> LoginAsync(string cpf, string senha)
        {
            try
            {
                cpf = cpf?.Trim();
                senha = senha?.Trim();
                var content = new StringContent(
                    JsonSerializer.Serialize(new { cpf = cpf, senha = senha }),
                    Encoding.UTF8,
                    "application/json"
                );
                var response = await _http.PostAsync("login.php", content);
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[LOGIN] Resposta: {json}");

                var result = JsonSerializer.Deserialize<LoginResponse>(json);
                if (result != null && result.usuario != null)
                {
                    // 🔹 token pode vir solto no topo do JSON OU dentro de "usuario" — testa os dois
                    var token = result.token ?? result.usuario.token;

                    return new Usuario
                    {
                        IdServidor = result.usuario.id,
                        CPF = cpf,
                        Senha = senha,
                        Nome = result.usuario.nome,
                        Token = token
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LOGIN] ERRO: {ex}");
                return null;
            }
        }

        // ================== OCORRÊNCIAS ==================

        public async Task<List<OcorrenciaApiListItem>> GetOcorrenciasAsync()
        {
            try
            {
                AplicarTokenAuth();

                var response = await _http.GetAsync("ocorrencias.php");
                System.Diagnostics.Debug.WriteLine($"[API OCORRENCIAS] Status (lista): {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                    return new List<OcorrenciaApiListItem>();

                var json = await response.Content.ReadAsStringAsync();
                var lista = JsonSerializer.Deserialize<List<OcorrenciaApiListItem>>(json);
                return lista ?? new List<OcorrenciaApiListItem>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[API OCORRENCIAS] ERRO (lista): {ex}");
                return new List<OcorrenciaApiListItem>();
            }
        }

        public async Task<OcorrenciaApiItem> GetOcorrenciaPorIdAsync(int id)
        {
            try
            {
                AplicarTokenAuth();

                var response = await _http.GetAsync($"ocorrencias.php?id={id}");
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<OcorrenciaApiItem>(json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[API OCORRENCIAS] ERRO (detalhe id={id}): {ex}");
                return null;
            }
        }

        public async Task<(bool sucesso, string respostaBruta)> SalvarOcorrenciaAsync(
            Ocorrencia ocorrencia,
            List<Adolescente> adolescentes,
            string assinaturaConselheiro,
            string assinaturaResponsavel,
            string assinaturaSolicitante)
        {
            try
            {
                AplicarTokenAuth();

                var payload = new
                {
                    numero_registro = ocorrencia.Numero,
                    natureza = ocorrencia.Natureza,
                    local = ocorrencia.Local,
                    data = ocorrencia.Data.ToString("yyyy-MM-dd"),
                    hora = ocorrencia.Hora,
                    solicitante = ocorrencia.Solicitante,
                    usuario_id = ocorrencia.ConselheiroId,

                    nome_mae = ocorrencia.MaeNome,
                    rg_mae = ocorrencia.MaeRG,
                    cpf_mae = ocorrencia.MaeCPF,
                    profissao_mae = ocorrencia.MaeProfissao,

                    nome_pai = ocorrencia.PaiNome,
                    rg_pai = ocorrencia.PaiRG,
                    cpf_pai = ocorrencia.PaiCPF,
                    profissao_pai = ocorrencia.PaiProfissao,

                    nome_responsavel = ocorrencia.ResponsavelNome,
                    rg_responsavel = ocorrencia.ResponsavelRG,
                    profissao_responsavel = ocorrencia.ResponsavelProfissao,

                    endereco_residencial = ocorrencia.EnderecoResidencial,
                    numero_residencial = ocorrencia.EnderecoResidencialNumero,
                    bairro_residencial = ocorrencia.BairroResidencial,
                    complemento_residencial = ocorrencia.ComplementoResidencial,
                    telefone_residencial = ocorrencia.FoneResidencial,

                    endereco_comercial = ocorrencia.EnderecoComercial,
                    numero_comercial = ocorrencia.EnderecoComercialNumero,
                    bairro_comercial = ocorrencia.BairroComercial,
                    complemento_comercial = ocorrencia.ComplementoComercial,
                    telefone_comercial = ocorrencia.FoneComercial,

                    historico = ocorrencia.Historico,

                    assinatura_conselheiro = assinaturaConselheiro,
                    assinatura_responsavel = assinaturaResponsavel,
                    assinatura_solicitante = assinaturaSolicitante,

                    adolescentes = adolescentes.Select(a => new
                    {
                        nome = a.Nome,
                        data_nasc = a.DataNascimento.ToString("yyyy-MM-dd"),
                        escola = a.Escola
                    }).ToList()
                };

                var json = JsonSerializer.Serialize(payload);
                System.Diagnostics.Debug.WriteLine($"[API SALVAR OCORRENCIA] Enviando: {json}");
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _http.PostAsync("ocorrencias.php", content);

                var respostaTexto = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[API SALVAR OCORRENCIA] Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[API SALVAR OCORRENCIA] Resposta: {respostaTexto}");

                return (response.IsSuccessStatusCode, respostaTexto);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[API SALVAR OCORRENCIA] ERRO: {ex}");
                return (false, ex.Message);
            }
        }

        // Pega o campo "mensagem" do JSON de erro do servidor (ou devolve o texto bruto)
        public static string ExtrairMensagem(string respostaBruta)
        {
            try
            {
                using var doc = JsonDocument.Parse(respostaBruta);
                if (doc.RootElement.TryGetProperty("mensagem", out var m))
                    return m.GetString();
            }
            catch { }

            return respostaBruta;
        }
    }

    public class LoginResponse
    {
        [JsonPropertyName("mensagem")]
        public string Mensagem { get; set; }
        [JsonPropertyName("usuario")]
        public UsuarioApi usuario { get; set; }
        [JsonPropertyName("token")]
        public string token { get; set; } // 🔹 caso venha solto no topo do JSON
    }

    public class UsuarioApi
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("nome")]
        public string nome { get; set; }
        [JsonPropertyName("token")]
        public string token { get; set; } // 🔹 caso venha dentro do objeto "usuario"
    }

    public class OcorrenciaApiListItem
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("data")] public string Data { get; set; }
        [JsonPropertyName("local")] public string Local { get; set; }
        [JsonPropertyName("conselheiro")] public string ConselheiroNome { get; set; }
    }

    public class OcorrenciaApiItem
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("data")] public string Data { get; set; }
        [JsonPropertyName("natureza")] public string Natureza { get; set; }
        [JsonPropertyName("local")] public string Local { get; set; }
        [JsonPropertyName("hora")] public string Hora { get; set; }
        [JsonPropertyName("solicitante")] public string Solicitante { get; set; }
        [JsonPropertyName("usuario_id")] public int UsuarioId { get; set; }
        [JsonPropertyName("conselheiro")] public string ConselheiroNome { get; set; }
        [JsonPropertyName("numero_registro")] public int? NumeroRegistro { get; set; }

        [JsonPropertyName("nome_mae")] public string NomeMae { get; set; }
        [JsonPropertyName("rg_mae")] public string RgMae { get; set; }
        [JsonPropertyName("cpf_mae")] public string CpfMae { get; set; }
        [JsonPropertyName("profissao_mae")] public string ProfissaoMae { get; set; }

        [JsonPropertyName("nome_pai")] public string NomePai { get; set; }
        [JsonPropertyName("rg_pai")] public string RgPai { get; set; }
        [JsonPropertyName("cpf_pai")] public string CpfPai { get; set; }
        [JsonPropertyName("profissao_pai")] public string ProfissaoPai { get; set; }

        [JsonPropertyName("nome_responsavel")] public string NomeResponsavel { get; set; }
        [JsonPropertyName("rg_responsavel")] public string RgResponsavel { get; set; }
        [JsonPropertyName("profissao_responsavel")] public string ProfissaoResponsavel { get; set; }

        [JsonPropertyName("endereco_residencial")] public string EnderecoResidencial { get; set; }
        [JsonPropertyName("numero_residencial")] public string NumeroResidencial { get; set; }
        [JsonPropertyName("bairro_residencial")] public string BairroResidencial { get; set; }
        [JsonPropertyName("complemento_residencial")] public string ComplementoResidencial { get; set; }
        [JsonPropertyName("telefone_residencial")] public string TelefoneResidencial { get; set; }

        [JsonPropertyName("endereco_comercial")] public string EnderecoComercial { get; set; }
        [JsonPropertyName("numero_comercial")] public string NumeroComercial { get; set; }
        [JsonPropertyName("bairro_comercial")] public string BairroComercial { get; set; }
        [JsonPropertyName("complemento_comercial")] public string ComplementoComercial { get; set; }
        [JsonPropertyName("telefone_comercial")] public string TelefoneComercial { get; set; }

        [JsonPropertyName("historico")] public string Historico { get; set; }

        [JsonPropertyName("assinatura_conselheiro")] public string AssinaturaConselheiro { get; set; }
        [JsonPropertyName("assinatura_responsavel")] public string AssinaturaResponsavel { get; set; }
        [JsonPropertyName("assinatura_solicitante")] public string AssinaturaSolicitante { get; set; }
    }
}