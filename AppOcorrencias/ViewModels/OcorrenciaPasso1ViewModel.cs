using System.Windows.Input;
using AppOcorrencias.Services;
using AppOcorrencias.Views;

namespace AppOcorrencias.ViewModels
{
    public class OcorrenciaPasso1ViewModel : BasePageViewModel
    {
        public string Numero
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.Numero;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.Numero = value; OnPropertyChanged(); }
        }

        public string Natureza
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.Natureza;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.Natureza = value; OnPropertyChanged(); }
        }

        public string Local
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.Local;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.Local = value; OnPropertyChanged(); }
        }

        public DateTime Data
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.Data;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.Data = value; OnPropertyChanged(); }
        }

        public string Hora
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.Hora;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.Hora = value; OnPropertyChanged(); }
        }

        public string Solicitante
        {
            get => NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.Solicitante;
            set { NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.Solicitante = value; OnPropertyChanged(); }
        }

        public ICommand ProximoCommand { get; }

        public OcorrenciaPasso1ViewModel()
        {
            // 🔹 reinicia tudo sempre que essa tela (início do wizard) for aberta
            NovaOcorrenciaSingleton.Instance.IniciarNova();
            OnPropertyChanged(nameof(Numero));
            OnPropertyChanged(nameof(Natureza));
            OnPropertyChanged(nameof(Local));
            OnPropertyChanged(nameof(Data));
            OnPropertyChanged(nameof(Hora));
            OnPropertyChanged(nameof(Solicitante));

            ProximoCommand = new Command(async () =>
            {
                if (string.IsNullOrWhiteSpace(Natureza) ||
                    string.IsNullOrWhiteSpace(Local) ||
                    string.IsNullOrWhiteSpace(Hora) ||
                    string.IsNullOrWhiteSpace(Solicitante))
                {
                    await Shell.Current.DisplayAlert("Atenção", "Preencha Natureza, Local, Hora e Solicitante da ocorrência.", "OK");
                    return;
                }

                await Shell.Current.Navigation.PushAsync(new pgOcorrenciaPasso2());
            });
        }

        public override void OnAppearingRefresh()
        {
            base.OnAppearingRefresh();

            NovaOcorrenciaSingleton.Instance.IniciarNova();
            OnPropertyChanged(nameof(Numero));
            OnPropertyChanged(nameof(Natureza));
            OnPropertyChanged(nameof(Local));
            OnPropertyChanged(nameof(Data));
            OnPropertyChanged(nameof(Hora));
            OnPropertyChanged(nameof(Solicitante));
        }
    }
}