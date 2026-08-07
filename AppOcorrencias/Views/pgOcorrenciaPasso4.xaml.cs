using AppOcorrencias.ViewModels;

namespace AppOcorrencias.Views
{
    public partial class pgOcorrenciaPasso4 : BasePage
    {
        public pgOcorrenciaPasso4()
        {
            InitializeComponent();
            BindingContext = new OcorrenciaPasso4ViewModel();
        }
    }
}