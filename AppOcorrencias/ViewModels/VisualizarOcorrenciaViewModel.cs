using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using AppOcorrencias.Models;
using AppOcorrencias.Services;
using AppOcorrencias.Views;

namespace AppOcorrencias.ViewModels
{
    public class VisualizarOcorrenciaViewModel : BasePageViewModel
    {
        readonly WebDBService _db;

        public Ocorrencia Ocorrencia { get; private set; }
        public ObservableCollection<Adolescente> Adolescentes { get; } = new ObservableCollection<Adolescente>();
        public ObservableCollection<PessoaEnvolvida> Assinantes { get; } = new ObservableCollection<PessoaEnvolvida>();

        public string StatusTexto => Ocorrencia?.Sincronizado == true ? "Enviada" : "Não Enviada";
        public string StatusCor => Ocorrencia?.Sincronizado == true ? "#2E8B57" : "#C0392B";

        public bool TemMae => !string.IsNullOrWhiteSpace(Ocorrencia?.MaeNome);
        public bool TemPai => !string.IsNullOrWhiteSpace(Ocorrencia?.PaiNome);
        public bool TemResponsavel => !string.IsNullOrWhiteSpace(Ocorrencia?.ResponsavelNome);
        public bool TemEnderecoComercial => !string.IsNullOrWhiteSpace(Ocorrencia?.EnderecoComercial);

        // 🔹 campo editável do histórico
        public string HistoricoEditavel
        {
            get => Ocorrencia?.Historico;
            set
            {
                if (Ocorrencia == null) return;
                Ocorrencia.Historico = value;
                OnPropertyChanged();
            }
        }

        public ICommand SalvarHistoricoCommand { get; }
        public ICommand AssinarCommand { get; }

        public VisualizarOcorrenciaViewModel(string ocorrenciaUid)
        {
            _db = new WebDBService();

            SalvarHistoricoCommand = new Command(async () =>
            {
                if (Ocorrencia == null) return;

                await _db.SalvarOcorrenciaAsync(Ocorrencia);
                await Shell.Current.DisplayAlert("Sucesso", "Histórico atualizado.", "OK");
            });

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

                    // 🔹 verifica se já existe assinatura pra esse envolvido — se sim, ATUALIZA em vez de duplicar
                    var assinaturaExistente = await _db.GetAssinaturaPorEnvolvidoAsync(pessoa.Id);

                    var assinatura = assinaturaExistente ?? new Assinatura { PessoaEnvolvidaId = pessoa.Id };
                    assinatura.ImagemBase64 = base64;
                    assinatura.Hash = hash;
                    assinatura.Latitude = location?.Latitude;
                    assinatura.Longitude = location?.Longitude;
                    assinatura.CapturadoEm = DateTime.Now;
                    assinatura.Sincronizado = false; // 🔹 marca pra reenviar, já que mudou

                    await _db.SalvarAssinaturaAsync(assinatura);

                    var bytes = Convert.FromBase64String(base64);
                    pessoa.AssinaturaPreview = ImageSource.FromStream(() => new MemoryStream(bytes));
                    pessoa.Assinado = true;

                    await _db.SalvarEnvolvidoAsync(pessoa);
                };

                await Shell.Current.Navigation.PushModalAsync(pagina);
            });

            _ = CarregarAsync(ocorrenciaUid);
        }

        async Task CarregarAsync(string ocorrenciaUid)
        {
            Ocorrencia = await _db.GetOcorrenciaPorUidAsync(ocorrenciaUid);
            OnPropertyChanged(nameof(Ocorrencia));
            OnPropertyChanged(nameof(HistoricoEditavel));
            OnPropertyChanged(nameof(StatusTexto));
            OnPropertyChanged(nameof(StatusCor));
            OnPropertyChanged(nameof(TemMae));
            OnPropertyChanged(nameof(TemPai));
            OnPropertyChanged(nameof(TemResponsavel));
            OnPropertyChanged(nameof(TemEnderecoComercial));

            var adolescentes = await _db.GetAdolescentesAsync(ocorrenciaUid);
            foreach (var adolescente in adolescentes)
                Adolescentes.Add(adolescente);

            var envolvidos = await _db.GetEnvolvidosAsync(ocorrenciaUid);
            foreach (var pessoa in envolvidos)
            {
                if (pessoa.Assinado)
                {
                    var assinatura = await _db.GetAssinaturaPorEnvolvidoAsync(pessoa.Id);
                    if (assinatura != null)
                    {
                        var bytes = Convert.FromBase64String(assinatura.ImagemBase64);
                        pessoa.AssinaturaPreview = ImageSource.FromStream(() => new MemoryStream(bytes));
                    }
                }

                Assinantes.Add(pessoa);
            }
        }

        static string CalcularHash(string base64)
        {
            var bytes = Encoding.UTF8.GetBytes(base64);
            var hashBytes = SHA256.HashData(bytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}