using AppOcorrencias.ViewModels;

namespace AppOcorrencias.Views
{
    public partial class pgOcorrenciaPasso5 : BasePage
    {
        public pgOcorrenciaPasso5()
        {
            InitializeComponent();
            BindingContext = new OcorrenciaPasso5ViewModel();
        }
    }
}