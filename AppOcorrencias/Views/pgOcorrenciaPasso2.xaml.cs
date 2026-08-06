using AppOcorrencias.ViewModels;

namespace AppOcorrencias.Views
{
    public partial class pgOcorrenciaPasso2 : BasePage
    {
        public pgOcorrenciaPasso2()
        {
            InitializeComponent();
            BindingContext = new OcorrenciaPasso2ViewModel();
        }
    }
}