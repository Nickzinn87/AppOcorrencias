using AppOcorrencias.ViewModels;

namespace AppOcorrencias.Views
{
    public partial class pgHome : BasePage
    {
        public pgHome()
        {
            InitializeComponent();
            BindingContext = new HomeViewModel();
        }
    }
}