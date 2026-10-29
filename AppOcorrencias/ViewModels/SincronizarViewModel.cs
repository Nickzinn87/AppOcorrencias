using System.Collections.ObjectModel;
using System.Windows.Input;
using AppOcorrencias.Models;
using AppOcorrencias.Services;
using AppOcorrencias.Views;

namespace AppOcorrencias.ViewModels
{
    public class SincronizarViewModel : BasePageViewModel
    {
        readonly SincronizacaoService _sync;

        public ObservableCollection<ItemSincronizacaoView> Pendentes { get; } = new ObservableCollection<ItemSincronizacaoView>();

        bool _carregando;
        public bool Carregando
        {
            get => _carregando;
            set { _carregando = value; OnPropertyChanged(); }
        }

        bool _sincronizando;
        public bool Sincronizando
        {
            get => _sincronizando;
            set { _sincronizando = value; OnPropertyChanged(); }
        }

        bool _semPendentes;
        public bool SemPendentes
        {
            get => _semPendentes;
            set { _semPendentes = value; OnPropertyChanged(); }
        }

        bool _semInternet;
        public bool SemInternet
        {
            get => _semInternet;
            set { _semInternet = value; OnPropertyChanged(); }
        }


        readonly WebDBService _db = new WebDBService();

        public ICommand AbrirCommand { get; }
        public ICommand DescartarCommand { get; }
        public ICommand SincronizarCommand { get; }
        public ICommand AtualizarCommand { get; }

        public SincronizarViewModel()
        {
            _sync = new SincronizacaoService();

            AtualizarCommand = new Command(async () => await CarregarPendentesAsync());

            SincronizarCommand = new Command(async () => await SincronizarAsync());

            AbrirCommand = new Command<ItemSincronizacaoView>(async (item) =>
            {
                if (item == null) return;
                await Shell.Current.Navigation.PushAsync(new pgVisualizarOcorrencia(item.Ocorrencia.Uid));
            });

            DescartarCommand = new Command<ItemSincronizacaoView>(async (item) =>
            {
                if (item == null) return;

                bool ok = await Shell.Current.DisplayAlert(
                    "Descartar ocorrência",
                    "Isso apaga a ocorrência deste aparelho e não pode ser desfeito. Continuar?",
                    "Descartar", "Cancelar");

                if (!ok) return;

                await _db.DeletarOcorrenciaCompletaAsync(item.Ocorrencia.Uid);
                await CarregarPendentesAsync();
            });

            _ = CarregarPendentesAsync();
        }

        public override void OnAppearingRefresh()
        {
            base.OnAppearingRefresh();
            _ = CarregarPendentesAsync();
        }

        bool _carregandoInterno = false;

        async Task CarregarPendentesAsync()
        {
            if (_carregandoInterno) return;
            _carregandoInterno = true;
            Carregando = true;

            try
            {
                SemInternet = Connectivity.Current.NetworkAccess != NetworkAccess.Internet;

                Pendentes.Clear();
                var pendentes = await _sync.GetPendentesAsync();

                foreach (var ocorrencia in pendentes)
                {
                    bool comErro = !string.IsNullOrWhiteSpace(ocorrencia.UltimoErro);

                    Pendentes.Add(new ItemSincronizacaoView
                    {
                        Ocorrencia = ocorrencia,
                        Status = comErro ? "Com erro" : "Pendente",
                        StatusCor = comErro ? "#C0392B" : "#5C639C"
                    });
                }

                SemPendentes = Pendentes.Count == 0;
            }
            finally
            {
                Carregando = false;
                _carregandoInterno = false;
            }
        }

        async Task SincronizarAsync()
        {
            if (Sincronizando) return;

            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Sem conexão", "Conecte-se à internet para sincronizar.", "OK");
                return;
            }

            Sincronizando = true;

            try
            {
                var (enviadas, falhas) = await _sync.SincronizarTodasAsync((ocorrencia, status) =>
                {
                    var item = Pendentes.FirstOrDefault(p => p.Ocorrencia.Uid == ocorrencia.Uid);
                    if (item == null) return;

                    item.Status = status;
                    item.StatusCor = status switch
                    {
                        "Enviada" => "#2E8B57",
                        "Falhou" => "#C0392B",
                        "Enviando..." => "#E5A742",
                        _ => "#5C639C"
                    };

                    // 🔹 força atualização visual do item (classe não é INotifyPropertyChanged)
                    var index = Pendentes.IndexOf(item);
                    if (index >= 0)
                    {
                        Pendentes.RemoveAt(index);
                        Pendentes.Insert(index, item);
                    }
                });

                await Shell.Current.DisplayAlert(
                    "Sincronização concluída",
                    $"{enviadas} ocorrência(s) enviada(s) com sucesso." + (falhas > 0 ? $"\n{falhas} falharam e continuam pendentes." : ""),
                    "OK");

                await CarregarPendentesAsync();
            }
            finally
            {
                Sincronizando = false;
            }
        }
    }

    public class ItemSincronizacaoView
    {
        public Ocorrencia Ocorrencia { get; set; }
        public string Status { get; set; }
        public string StatusCor { get; set; }

        public string Natureza => Ocorrencia.Natureza;
        public string Local => Ocorrencia.Local;

        public string UltimoErro => Ocorrencia.UltimoErro;
        public bool TemErro => !string.IsNullOrWhiteSpace(Ocorrencia.UltimoErro);
    }
}