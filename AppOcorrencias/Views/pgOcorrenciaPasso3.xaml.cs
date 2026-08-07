using AppOcorrencias.ViewModels;

namespace AppOcorrencias.Views
{
    public partial class pgOcorrenciaPasso3 : BasePage
    {
        public pgOcorrenciaPasso3()
        {
            InitializeComponent();
            BindingContext = new OcorrenciaPasso3ViewModel();
        }
    }
}