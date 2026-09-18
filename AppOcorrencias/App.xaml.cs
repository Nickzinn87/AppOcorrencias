using AppOcorrencias.Services;

namespace AppOcorrencias
{
    public partial class App : Application
    {
        bool _sincronizandoAutomatico = false;

        public App()
        {
            InitializeComponent();

            Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
        }

        async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess != NetworkAccess.Internet) return;
            if (_sincronizandoAutomatico) return;
            if (!UserSingleton.Instance.IsLogado) return;

            _sincronizandoAutomatico = true;
            try
            {
                var sync = new SincronizacaoService();
                await sync.SincronizarTodasAsync();
            }
            finally
            {
                _sincronizandoAutomatico = false;
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}