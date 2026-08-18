using AppOcorrencias.Models;
using AppOcorrencias.Services;
using AppOcorrencias.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AppOcorrencias.ViewModels
{
    public class MinhasOcorrenciasViewModel : BasePageViewModel
    {
        readonly WebDBService _db;

        public ObservableCollection<OcorrenciaListItem> Ocorrencias { get; } = new ObservableCollection<OcorrenciaListItem>();

        bool _carregando;
        public bool Carregando
        {
            get => _carregando;
            set { _carregando = value; OnPropertyChanged(); }
        }

        bool _semOcorrencias;
        public bool SemOcorrencias
        {
            get => _semOcorrencias;
            set { _semOcorrencias = value; OnPropertyChanged(); }
        }

        public ICommand AtualizarCommand { get; }
        public ICommand AbrirDetalhesCommand { get; }

        public MinhasOcorrenciasViewModel()
        {
            _db = new WebDBService();

            AtualizarCommand = new Command(async () => await CarregarOcorrenciasAsync());

            AbrirDetalhesCommand = new Command<OcorrenciaListItem>(async (item) =>
            {
                if (item?.Ocorrencia == null) return;
                await Shell.Current.Navigation.PushAsync(new pgVisualizarOcorrencia(item.Ocorrencia.Uid));
            });

            _ = CarregarOcorrenciasAsync();
        }

        bool _carregandoInterno = false; // 🔹 trava contra chamadas concorrentes

        async Task CarregarOcorrenciasAsync()
        {
            if (_carregandoInterno) return; // 🔹 se já está carregando, ignora chamada duplicada
            _carregandoInterno = true;
            Carregando = true;

            try
            {
                Ocorrencias.Clear();

                var conselheiroId = UserSingleton.Instance.UsuarioLogado?.IdServidor ?? 0;

                var locais = await _db.GetOcorrenciasAsync();

                var doConselheiro = locais
                    .Where(o => o.ConselheiroId == conselheiroId)
                    .OrderByDescending(o => o.CriadoEm)
                    .ToList();

                foreach (var ocorrencia in doConselheiro)
                {
                    Ocorrencias.Add(new OcorrenciaListItem
                    {
                        Ocorrencia = ocorrencia,
                        Status = ocorrencia.Sincronizado ? "Enviada" : "Não Enviada",
                        StatusCor = ocorrencia.Sincronizado ? "#2E8B57" : "#C0392B"
                    });
                }

                SemOcorrencias = Ocorrencias.Count == 0;
            }
            finally
            {
                Carregando = false;
                _carregandoInterno = false;
            }
        }
    }

    // 🔹 classe auxiliar só pra exibição na lista (não é salva no banco)
    public class OcorrenciaListItem
    {
        public Ocorrencia Ocorrencia { get; set; }
        public string Status { get; set; }
        public string StatusCor { get; set; }

        public string Numero => Ocorrencia.Numero;
        public string Natureza => Ocorrencia.Natureza;
        public string Local => Ocorrencia.Local;
        public DateTime Data => Ocorrencia.Data;
    }
}