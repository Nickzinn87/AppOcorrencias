using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using AppOcorrencias.Models;
using AppOcorrencias.Services;
using AppOcorrencias.Views;

namespace AppOcorrencias.ViewModels
{
    public class OcorrenciaPasso5ViewModel : BasePageViewModel
    {
        readonly WebDBService _db;

        public string Historico
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.Historico;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.Historico = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PessoaEnvolvida> Assinantes { get; }

        public ICommand AssinarCommand { get; }
        public ICommand VoltarCommand { get; }
        public ICommand ConcluirCommand { get; }

        public OcorrenciaPasso5ViewModel()
        {
            _db = new WebDBService();

            // 🔹 cria os 3 papéis fixos de assinatura, só na primeira vez que entra nessa tela
            if (NovaOcorrenciaSingleton.Instance.Envolvidos.Count == 0)
            {
                var ocorrenciaUid = NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.Uid;

                NovaOcorrenciaSingleton.Instance.Envolvidos.Add(new PessoaEnvolvida
                {
                    OcorrenciaUid = ocorrenciaUid,
                    Nome = "Conselheiro Responsável",
                    Papel = "Conselheiro Responsável"
                });

                NovaOcorrenciaSingleton.Instance.Envolvidos.Add(new PessoaEnvolvida
                {
                    OcorrenciaUid = ocorrenciaUid,
                    Nome = "Pai/Mãe ou Responsável",
                    Papel = "Pai/Mãe ou Responsável"
                });

                NovaOcorrenciaSingleton.Instance.Envolvidos.Add(new PessoaEnvolvida
                {
                    OcorrenciaUid = ocorrenciaUid,
                    Nome = "Solicitante",
                    Papel = "Solicitante"
                });
            }

            Assinantes = new ObservableCollection<PessoaEnvolvida>(NovaOcorrenciaSingleton.Instance.Envolvidos);

            AssinarCommand = new Command<PessoaEnvolvida>(async (pessoa) =>
            {
                if (pessoa == null) return;

                var pagina = new pgAssinatura(pessoa.Nome);

                pagina.AssinaturaConfirmada += async (base64) =>
                {
                    var hash = CalcularHash(base64);
                    Microsoft.Maui.Devices.Sensors.Location location = null;
                    try
                    {
                        location = await Microsoft.Maui.Devices.Sensors.Geolocation.GetLastKnownLocationAsync();
                    }
                    catch { }

                    // 🔹 salva o envolvido já aqui pra garantir Id antes da assinatura referenciar
                    await _db.SalvarEnvolvidoAsync(pessoa);

                    await _db.SalvarAssinaturaAsync(new Assinatura
                    {
                        PessoaEnvolvidaId = pessoa.Id,
                        ImagemBase64 = base64,
                        Hash = hash,
                        Latitude = location?.Latitude,
                        Longitude = location?.Longitude
                    });

                    var bytes = Convert.FromBase64String(base64);
                    pessoa.AssinaturaPreview = ImageSource.FromStream(() => new MemoryStream(bytes));
                    pessoa.Assinado = true;

                    await _db.SalvarEnvolvidoAsync(pessoa);
                };

                await Shell.Current.Navigation.PushModalAsync(pagina);
            });

            VoltarCommand = new Command(async () =>
            {
                await Shell.Current.Navigation.PopAsync();
            });

            ConcluirCommand = new Command(async () =>
            {
                var ocorrencia = NovaOcorrenciaSingleton.Instance.OcorrenciaAtual;
                var adolescentes = NovaOcorrenciaSingleton.Instance.Adolescentes;

                // validacao completa ANTES de salvar qualquer coisa
                var erros = OcorrenciaValidator.ValidarCompleta(ocorrencia, adolescentes, Assinantes.Count(a => a.Assinado));
                if (await OcorrenciaValidator.BloquearSeInvalidoAsync(erros, "Corrija antes de concluir")) return;

                await _db.SalvarOcorrenciaAsync(ocorrencia);

                foreach (var adolescente in adolescentes)
                {
                    adolescente.OcorrenciaUid = ocorrencia.Uid;
                    await _db.SalvarAdolescenteAsync(adolescente);
                }

                foreach (var envolvido in Assinantes)
                {
                    envolvido.OcorrenciaUid = ocorrencia.Uid;
                    await _db.SalvarEnvolvidoAsync(envolvido);
                }

                string mensagem;

                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    var assinaturaConselheiro = await ObterBase64AssinaturaAsync("Conselheiro Responsável");
                    var assinaturaResponsavel = await ObterBase64AssinaturaAsync("Pai/Mãe ou Responsável");
                    var assinaturaSolicitante = await ObterBase64AssinaturaAsync("Solicitante");

                    var api = new ApiService();
                    var (sucesso, resposta) = await api.SalvarOcorrenciaAsync(
                        ocorrencia, adolescentes,
                        assinaturaConselheiro, assinaturaResponsavel, assinaturaSolicitante);

                    if (sucesso)
                    {
                        ocorrencia.Sincronizado = true;
                        ocorrencia.UltimoErro = null;
                        await _db.SalvarOcorrenciaAsync(ocorrencia);
                        mensagem = "Ocorrência registrada e enviada com sucesso!";
                    }
                    else
                    {
                        ocorrencia.UltimoErro = ApiService.ExtrairMensagem(resposta);
                        await _db.SalvarOcorrenciaAsync(ocorrencia);
                        mensagem = $"Salva no aparelho, mas o servidor recusou o envio:\n{ocorrencia.UltimoErro}";
                    }
                }
                else
                {
                    mensagem = "Ocorrência salva localmente. Será enviada quando houver conexão.";
                }

                await Shell.Current.DisplayAlert("Resultado", mensagem, "OK");

                NovaOcorrenciaSingleton.Instance.Limpar();
                await Shell.Current.Navigation.PopToRootAsync();
            });
        }

        async Task<string> ObterBase64AssinaturaAsync(string papel)
        {
            var pessoa = Assinantes.FirstOrDefault(a => a.Papel == papel);
            if (pessoa == null || !pessoa.Assinado) return null;

            var assinatura = await _db.GetAssinaturaPorEnvolvidoAsync(pessoa.Id);
            return assinatura?.ImagemBase64;
        }

        static string CalcularHash(string base64)
        {
            var bytes = Encoding.UTF8.GetBytes(base64);
            var hashBytes = SHA256.HashData(bytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}