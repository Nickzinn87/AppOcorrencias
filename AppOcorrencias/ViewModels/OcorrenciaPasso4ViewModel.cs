using System.Windows.Input;
using AppOcorrencias.Services;
using AppOcorrencias.Views;

namespace AppOcorrencias.ViewModels
{
    public class OcorrenciaPasso4ViewModel : BasePageViewModel
    {
        public string EnderecoResidencial
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.EnderecoResidencial;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.EnderecoResidencial = value; OnPropertyChanged(); }
        }

        public string EnderecoResidencialNumero
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.EnderecoResidencialNumero;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.EnderecoResidencialNumero = value; OnPropertyChanged(); }
        }

        public string BairroResidencial
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.BairroResidencial;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.BairroResidencial = value; OnPropertyChanged(); }
        }

        public string ComplementoResidencial
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.ComplementoResidencial;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.ComplementoResidencial = value; OnPropertyChanged(); }
        }

        public string FoneResidencial
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.FoneResidencial;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.FoneResidencial = value; OnPropertyChanged(); }
        }

        public string EnderecoComercial
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.EnderecoComercial;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.EnderecoComercial = value; OnPropertyChanged(); }
        }

        public string EnderecoComercialNumero
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.EnderecoComercialNumero;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.EnderecoComercialNumero = value; OnPropertyChanged(); }
        }

        public string BairroComercial
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.BairroComercial;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.BairroComercial = value; OnPropertyChanged(); }
        }

        public string ComplementoComercial
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.ComplementoComercial;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.ComplementoComercial = value; OnPropertyChanged(); }
        }

        public string FoneComercial
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.FoneComercial;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.FoneComercial = value; OnPropertyChanged(); }
        }

        public ICommand VoltarCommand { get; }
        public ICommand ProximoCommand { get; }

        public OcorrenciaPasso4ViewModel()
        {
            VoltarCommand = new Command(async () =>
            {
                await Shell.Current.Navigation.PopAsync();
            });

            ProximoCommand = new Command(async () =>
            {
                var o = NovaOcorrenciaSingleton.Instance.OcorrenciaAtual;
                if (await OcorrenciaValidator.BloquearSeInvalidoAsync(OcorrenciaValidator.ValidarPasso4(o))) return;

                await Shell.Current.Navigation.PushAsync(new pgOcorrenciaPasso5());
            });
        }
    }
}