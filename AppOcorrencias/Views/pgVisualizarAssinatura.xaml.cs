namespace AppOcorrencias.Views
{
    public partial class pgVisualizarAssinatura : ContentPage
    {
        public pgVisualizarAssinatura(ImageSource assinatura)
        {
            InitializeComponent();
            imgAssinatura.Source = assinatura;
        }

        async void OnFechar(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}