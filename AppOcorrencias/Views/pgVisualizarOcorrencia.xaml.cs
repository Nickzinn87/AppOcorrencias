using AppOcorrencias.ViewModels;

namespace AppOcorrencias.Views
{
    public partial class pgVisualizarOcorrencia : BasePage
    {
        public pgVisualizarOcorrencia(string ocorrenciaUid)
        {
            InitializeComponent();
            BindingContext = new VisualizarOcorrenciaViewModel(ocorrenciaUid);
        }
    }
}