using AppOcorrencias.ViewModels;

namespace AppOcorrencias.Views
{
    public partial class pgMinhasOcorrencias : BasePage
    {
        public pgMinhasOcorrencias()
        {
            InitializeComponent();
            BindingContext = new MinhasOcorrenciasViewModel();
        }
    }
}