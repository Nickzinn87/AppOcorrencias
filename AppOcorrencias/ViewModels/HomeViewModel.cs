using AppOcorrencias.Services;

namespace AppOcorrencias.ViewModels
{
    public class HomeViewModel : BasePageViewModel
    {
        readonly WebDBService _db;

        int _totalPendentes;
        public int TotalPendentes
        {
            get => _totalPendentes;
            set { _totalPendentes = value; OnPropertyChanged(); }
        }

        int _totalEnviadas;
        public int TotalEnviadas
        {
            get => _totalEnviadas;
            set { _totalEnviadas = value; OnPropertyChanged(); }
        }

        int _totalGeral;
        public int TotalGeral
        {
            get => _totalGeral;
            set { _totalGeral = value; OnPropertyChanged(); }
        }

        bool _carregando;
        public bool Carregando
        {
            get => _carregando;
            set { _carregando = value; OnPropertyChanged(); }
        }

        public HomeViewModel()
        {
            _db = new WebDBService();
            _ = CarregarResumoAsync();
        }

        public override void OnAppearingRefresh()
        {
            base.OnAppearingRefresh();
            _ = CarregarResumoAsync();
        }

        bool _carregandoInterno = false;

        async Task CarregarResumoAsync()
        {
            if (_carregandoInterno) return;
            _carregandoInterno = true;
            Carregando = true;

            try
            {
                var conselheiroId = UserSingleton.Instance.UsuarioLogado?.IdServidor ?? 0;

                // 🔹 pendentes: só locais, ainda não sincronizadas
                var locais = await _db.GetOcorrenciasAsync();
                var pendentes = locais.Where(o => o.ConselheiroId == conselheiroId && !o.Sincronizado).ToList();

                TotalPendentes = pendentes.Count;

                // 🔹 enviadas: busca na API, se tiver internet
                int enviadas = 0;

                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    var api = new ApiService();
                    var listaLeve = await api.GetOcorrenciasAsync();

                    foreach (var item in listaLeve)
                    {
                        var detalhe = await api.GetOcorrenciaPorIdAsync(item.Id);
                        if (detalhe != null && detalhe.UsuarioId == conselheiroId)
                            enviadas++;
                    }
                }

                TotalEnviadas = enviadas;
                TotalGeral = TotalPendentes + TotalEnviadas;
            }
            finally
            {
                Carregando = false;
                _carregandoInterno = false;
            }
        }
    }
}