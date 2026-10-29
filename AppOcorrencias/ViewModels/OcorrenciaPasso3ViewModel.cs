using System.Windows.Input;
using AppOcorrencias.Services;
using AppOcorrencias.Views;

namespace AppOcorrencias.ViewModels
{
    public class OcorrenciaPasso3ViewModel : BasePageViewModel
    {
        public string MaeNome
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.MaeNome;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.MaeNome = value; OnPropertyChanged(); }
        }

        public string MaeRG
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.MaeRG;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.MaeRG = value; OnPropertyChanged(); }
        }

        public string MaeCPF
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.MaeCPF;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.MaeCPF = value; OnPropertyChanged(); }
        }

        public string MaeProfissao
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.MaeProfissao;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.MaeProfissao = value; OnPropertyChanged(); }
        }

        public string PaiNome
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.PaiNome;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.PaiNome = value; OnPropertyChanged(); }
        }

        public string PaiRG
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.PaiRG;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.PaiRG = value; OnPropertyChanged(); }
        }

        public string PaiCPF
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.PaiCPF;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.PaiCPF = value; OnPropertyChanged(); }
        }

        public string PaiProfissao
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.PaiProfissao;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.PaiProfissao = value; OnPropertyChanged(); }
        }

        public string ResponsavelNome
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.ResponsavelNome;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.ResponsavelNome = value; OnPropertyChanged(); }
        }

        public string ResponsavelRG
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.ResponsavelRG;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.ResponsavelRG = value; OnPropertyChanged(); }
        }

        public string ResponsavelProfissao
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.ResponsavelProfissao;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.ResponsavelProfissao = value; OnPropertyChanged(); }
        }

        public ICommand VoltarCommand { get; }
        public ICommand ProximoCommand { get; }

        public OcorrenciaPasso3ViewModel()
        {
            VoltarCommand = new Command(async () =>
            {
                await Shell.Current.Navigation.PopAsync();
            });

            ProximoCommand = new Command(async () =>
            {
                var o = NovaOcorrenciaSingleton.Instance.OcorrenciaAtual;
                if (await OcorrenciaValidator.BloquearSeInvalidoAsync(OcorrenciaValidator.ValidarPasso3(o))) return;

                await Shell.Current.Navigation.PushAsync(new pgOcorrenciaPasso4());
            });
        }
    }
}