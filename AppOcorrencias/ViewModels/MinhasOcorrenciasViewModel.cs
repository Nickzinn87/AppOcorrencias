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
                if (item == null) return;

                if (item.OcorrenciaUidLocal == null)
                {
                    await Shell.Current.DisplayAlert("Aviso", "Detalhes de ocorrências já enviadas ainda não estão disponíveis para visualização no app.", "OK");
                    return;
                }

                await Shell.Current.Navigation.PushAsync(new pgVisualizarOcorrencia(item.OcorrenciaUidLocal));
            });

            _ = CarregarOcorrenciasAsync();
        }

        bool _carregandoInterno = false;

        async Task CarregarOcorrenciasAsync()
        {
            if (_carregandoInterno) return;
            _carregandoInterno = true;
            Carregando = true;

            try
            {
                Ocorrencias.Clear();

                var conselheiroId = UserSingleton.Instance.UsuarioLogado?.IdServidor ?? 0;

                // 🔹 LOCAIS (só as que ainda não foram enviadas — as já enviadas vêm da API)
                var locais = await _db.GetOcorrenciasAsync();
                var locaisDoConselheiro = locais
                    .Where(o => o.ConselheiroId == conselheiroId && !o.Sincronizado)
                    .OrderByDescending(o => o.CriadoEm)
                    .ToList();

                foreach (var ocorrencia in locaisDoConselheiro)
                {
                    Ocorrencias.Add(new OcorrenciaListItem
                    {
                        Natureza = ocorrencia.Natureza,
                        Local = ocorrencia.Local,
                        Data = ocorrencia.Data,
                        Status = ocorrencia.Sincronizado ? "Enviada" : "Não Enviada",
                        StatusCor = ocorrencia.Sincronizado ? "#2E8B57" : "#C0392B",
                        OcorrenciaUidLocal = ocorrencia.Uid
                    });
                }

                // 🔹 DA API (já enviadas)
                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    var api = new ApiService();

                    // 1) busca a lista leve (id, data, local, conselheiro)
                    var listaLeve = await api.GetOcorrenciasAsync();

                    // 2) pra cada uma, busca os detalhes completos (só assim vem o usuario_id)
                    foreach (var item in listaLeve)
                    {
                        var detalhe = await api.GetOcorrenciaPorIdAsync(item.Id);
                        if (detalhe == null) continue;

                        if (detalhe.UsuarioId != conselheiroId) continue; // 🔹 filtro real acontece aqui

                        DateTime.TryParse(detalhe.Data, out var dataConvertida);

                        Ocorrencias.Add(new OcorrenciaListItem
                        {
                            Natureza = detalhe.Natureza,
                            Local = detalhe.Local,
                            Data = dataConvertida,
                            Status = "Enviada",
                            StatusCor = "#2E8B57",
                            OcorrenciaUidLocal = null
                        });
                    }
                }

                // 🔹 reordena tudo junto por data, mais recente primeiro
                var ordenadas = Ocorrencias.OrderByDescending(o => o.Data).ToList();
                Ocorrencias.Clear();
                foreach (var item in ordenadas)
                    Ocorrencias.Add(item);

                SemOcorrencias = Ocorrencias.Count == 0;
            }
            finally
            {
                Carregando = false;
                _carregandoInterno = false;
            }
        }

        public override void OnAppearingRefresh()
        {
            base.OnAppearingRefresh();
            _ = CarregarOcorrenciasAsync();
        }
    }

    // 🔹 classe auxiliar só pra exibição na lista (não é salva no banco)
    public class OcorrenciaListItem
    {
        public string Natureza { get; set; }
        public string Local { get; set; }
        public DateTime Data { get; set; }
        public string Status { get; set; }
        public string StatusCor { get; set; }

        // 🔹 só preenchido quando vem do banco local — usado pra abrir a tela de detalhes editável
        public string OcorrenciaUidLocal { get; set; }
    }
}