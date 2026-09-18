using AppOcorrencias.ViewModels;

namespace AppOcorrencias.Views
{
    public partial class pgSincronizar : BasePage
    {
        public pgSincronizar()
        {
            InitializeComponent();
            BindingContext = new SincronizarViewModel();
        }
    }
}