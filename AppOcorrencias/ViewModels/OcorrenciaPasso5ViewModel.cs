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

                if (string.IsNullOrWhiteSpace(ocorrencia.Natureza))
                {
                    await Shell.Current.DisplayAlert("Atenção", "A ocorrência ficou sem Natureza preenchida. Volte ao Passo 1 e corrija.", "OK");
                    return;
                }

                // 🔹 salva a ocorrência principal
                await _db.SalvarOcorrenciaAsync(ocorrencia);

                // 🔹 salva os adolescentes
                foreach (var adolescente in NovaOcorrenciaSingleton.Instance.Adolescentes)
                {
                    adolescente.OcorrenciaUid = ocorrencia.Uid;
                    await _db.SalvarAdolescenteAsync(adolescente);
                }

                // 🔹 garante que todos os envolvidos (assinados ou não) estão salvos
                foreach (var envolvido in Assinantes)
                {
                    envolvido.OcorrenciaUid = ocorrencia.Uid;
                    await _db.SalvarEnvolvidoAsync(envolvido);
                }

                await Shell.Current.DisplayAlert("Sucesso", "Ocorrência registrada com sucesso! Será enviada quando houver conexão.", "OK");

                NovaOcorrenciaSingleton.Instance.Limpar();

                await Shell.Current.Navigation.PopToRootAsync();
            });
        }

        static string CalcularHash(string base64)
        {
            var bytes = Encoding.UTF8.GetBytes(base64);
            var hashBytes = SHA256.HashData(bytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}