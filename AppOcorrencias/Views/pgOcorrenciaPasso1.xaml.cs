using AppOcorrencias.ViewModels;

namespace AppOcorrencias.Views
{
    public partial class pgOcorrenciaPasso1 : BasePage
    {
        public pgOcorrenciaPasso1()
        {
            InitializeComponent();
            BindingContext = new OcorrenciaPasso1ViewModel();
        }
    }
}