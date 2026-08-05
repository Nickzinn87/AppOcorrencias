using System;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using AppOcorrencias.Models;
using AppOcorrencias.Services;
using AppOcorrencias.Views;

namespace AppOcorrencias.ViewModels
{
    public class NovaOcorrenciaViewModel : BasePageViewModel
    {
        readonly WebDBService _db;
        Ocorrencia _ocorrencia = new Ocorrencia();

        public string Tipo
        {
            get => _ocorrencia.Tipo;
            set { _ocorrencia.Tipo = value; OnPropertyChanged(); }
        }

        public string Local
        {
            get => _ocorrencia.Local;
            set { _ocorrencia.Local = value; OnPropertyChanged(); }
        }

        public string Descricao
        {
            get => _ocorrencia.Descricao;
            set { _ocorrencia.Descricao = value; OnPropertyChanged(); }
        }

        public DateTime DataHora
        {
            get => _ocorrencia.DataHora;
            set { _ocorrencia.DataHora = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PessoaEnvolvida> Envolvidos { get; } = new ObservableCollection<PessoaEnvolvida>();

        string _novoEnvolvidoNome;
        public string NovoEnvolvidoNome
        {
            get => _novoEnvolvidoNome;
            set { _novoEnvolvidoNome = value; OnPropertyChanged(); }
        }

        string _novoEnvolvidoDocumento;
        public string NovoEnvolvidoDocumento
        {
            get => _novoEnvolvidoDocumento;
            set { _novoEnvolvidoDocumento = value; OnPropertyChanged(); }
        }

        string _novoEnvolvidoPapel;
        public string NovoEnvolvidoPapel
        {
            get => _novoEnvolvidoPapel;
            set { _novoEnvolvidoPapel = value; OnPropertyChanged(); }
        }

        public ICommand AdicionarEnvolvidoCommand { get; }
        public ICommand AssinarCommand { get; }
        public ICommand SalvarOcorrenciaCommand { get; }
        public ICommand VerAssinaturaCommand { get; }

        public NovaOcorrenciaViewModel()
        {
            _db = new WebDBService();

            // 🔹 Vincula quem está registrando, puxando do usuário logado
            _ocorrencia.RegistradoPorCPF = UserSingleton.Instance.UsuarioLogado?.CPF;

            AdicionarEnvolvidoCommand = new Command(() =>
            {
                if (string.IsNullOrWhiteSpace(NovoEnvolvidoNome))
                    return;

                Envolvidos.Add(new PessoaEnvolvida
                {
                    OcorrenciaUid = _ocorrencia.Uid,
                    Nome = NovoEnvolvidoNome,
                    Documento = NovoEnvolvidoDocumento,
                    Papel = NovoEnvolvidoPapel
                });

                NovoEnvolvidoNome = string.Empty;
                NovoEnvolvidoDocumento = string.Empty;
                NovoEnvolvidoPapel = string.Empty;
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

                    // 🔹 agora que Assinado e AssinaturaPreview notificam mudança,
                    // não precisa mais remover/reinserir o item na lista
                    pessoa.AssinaturaPreview = ImageSource.FromStream(() => new MemoryStream(bytes));
                    pessoa.Assinado = true;

                    await _db.SalvarEnvolvidoAsync(pessoa);
                };

                await Shell.Current.Navigation.PushModalAsync(pagina);
            });

            SalvarOcorrenciaCommand = new Command(async () =>
            {
                if (string.IsNullOrWhiteSpace(Tipo))
                {
                    await Shell.Current.DisplayAlert("Atenção", "Informe o tipo da ocorrência.", "OK");
                    return;
                }

                await _db.SalvarOcorrenciaAsync(_ocorrencia);

                foreach (var envolvido in Envolvidos)
                    await _db.SalvarEnvolvidoAsync(envolvido);

                await Shell.Current.DisplayAlert("Sucesso", "Ocorrência salva localmente. Será enviada quando houver conexão.", "OK");

                LimparFormulario(); // 🔹 reseta tudo antes de voltar

                await Shell.Current.GoToAsync("..");
            });

            VerAssinaturaCommand = new Command<PessoaEnvolvida>(async (pessoa) =>
            {
                if (pessoa?.AssinaturaPreview == null) return;

                await Shell.Current.Navigation.PushModalAsync(
                    new pgVisualizarAssinatura(pessoa.AssinaturaPreview));
            });
        }

        void LimparFormulario()
        {
            _ocorrencia = new Ocorrencia
            {
                RegistradoPorCPF = UserSingleton.Instance.UsuarioLogado?.CPF
            };

            OnPropertyChanged(nameof(Tipo));
            OnPropertyChanged(nameof(Local));
            OnPropertyChanged(nameof(Descricao));
            OnPropertyChanged(nameof(DataHora));

            Envolvidos.Clear();

            NovoEnvolvidoNome = string.Empty;
            NovoEnvolvidoDocumento = string.Empty;
            NovoEnvolvidoPapel = string.Empty;
        }

        static string CalcularHash(string base64)
        {
            var bytes = Encoding.UTF8.GetBytes(base64);
            var hashBytes = SHA256.HashData(bytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}