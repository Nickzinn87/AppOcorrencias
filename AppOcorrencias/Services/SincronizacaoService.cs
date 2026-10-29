using AppOcorrencias.Models;

namespace AppOcorrencias.Services
{
    public class ItemSincronizacao
    {
        public Ocorrencia Ocorrencia { get; set; }
        public string Status { get; set; } // "Pendente", "Enviando...", "Enviada", "Falhou"
    }

    public class SincronizacaoService
    {
        readonly WebDBService _db;
        readonly ApiService _api;

        public SincronizacaoService()
        {
            _db = new WebDBService();
            _api = new ApiService();
        }

        // 🔹 retorna as ocorrências locais ainda não enviadas, do conselheiro logado
        public async Task<List<Ocorrencia>> GetPendentesAsync()
        {
            var conselheiroId = UserSingleton.Instance.UsuarioLogado?.IdServidor ?? 0;
            var locais = await _db.GetOcorrenciasAsync();

            return locais
                .Where(o => o.ConselheiroId == conselheiroId && !o.Sincronizado)
                .OrderByDescending(o => o.CriadoEm)
                .ToList();
        }

        // 🔹 sincroniza todas as pendentes, chamando onStatusAtualizado a cada mudança
        // (para a UI poder refletir progresso item a item)
        public async Task<(int enviadas, int falhas)> SincronizarTodasAsync(Action<Ocorrencia, string> onStatusAtualizado = null)
        {
            int enviadas = 0;
            int falhas = 0;

            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return (0, 0);

            var pendentes = await GetPendentesAsync();

            foreach (var ocorrencia in pendentes)
            {
                onStatusAtualizado?.Invoke(ocorrencia, "Enviando...");

                try
                {
                    var adolescentes = await _db.GetAdolescentesAsync(ocorrencia.Uid);
                    var envolvidos = await _db.GetEnvolvidosAsync(ocorrencia.Uid);

                    // nao adianta nem chamar a API se os dados ja estao incompletos
                    var erros = OcorrenciaValidator.ValidarCompleta(ocorrencia, adolescentes, envolvidos.Count(p => p.Assinado));
                    if (erros.Count > 0)
                    {
                        ocorrencia.UltimoErro = "Dados incompletos: " + string.Join(" ", erros);
                        await _db.SalvarOcorrenciaAsync(ocorrencia);

                        onStatusAtualizado?.Invoke(ocorrencia, "Falhou");
                        falhas++;
                        continue;
                    }

                    var assinaturaConselheiro = await ObterBase64Async(envolvidos, "Conselheiro Responsável");
                    var assinaturaResponsavel = await ObterBase64Async(envolvidos, "Pai/Mãe ou Responsável");
                    var assinaturaSolicitante = await ObterBase64Async(envolvidos, "Solicitante");

                    var (sucesso, resposta) = await _api.SalvarOcorrenciaAsync(
                        ocorrencia, adolescentes,
                        assinaturaConselheiro, assinaturaResponsavel, assinaturaSolicitante);

                    if (sucesso)
                    {
                        ocorrencia.Sincronizado = true;
                        ocorrencia.UltimoErro = null;
                        await _db.SalvarOcorrenciaAsync(ocorrencia);

                        onStatusAtualizado?.Invoke(ocorrencia, "Enviada");
                        enviadas++;
                    }
                    else
                    {
                        ocorrencia.UltimoErro = ApiService.ExtrairMensagem(resposta);
                        await _db.SalvarOcorrenciaAsync(ocorrencia);

                        onStatusAtualizado?.Invoke(ocorrencia, "Falhou");
                        falhas++;
                    }
                }
                catch (Exception ex)
                {
                    ocorrencia.UltimoErro = ex.Message;
                    await _db.SalvarOcorrenciaAsync(ocorrencia);

                    onStatusAtualizado?.Invoke(ocorrencia, "Falhou");
                    falhas++;
                }
            }

            return (enviadas, falhas);
        }

        async Task<string> ObterBase64Async(List<PessoaEnvolvida> envolvidos, string papel)
        {
            var pessoa = envolvidos.FirstOrDefault(p => p.Papel == papel);
            if (pessoa == null || !pessoa.Assinado) return null;

            var assinatura = await _db.GetAssinaturaPorEnvolvidoAsync(pessoa.Id);
            return assinatura?.ImagemBase64;
        }
    }
}