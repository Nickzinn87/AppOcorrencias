using AppOcorrencias.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AppOcorrencias.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string cpf;
        public string CPF
        {
            get => cpf;
            set
            {
                cpf = value;
                OnPropertyChanged();
            }
        }

        private string senha;
        public string Senha
        {
            get => senha;
            set
            {
                senha = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(RealizarLogin);
        }

        private void RealizarLogin()
        {
            // Aqui vai sua lógica de login
            if (string.IsNullOrEmpty(CPF) || string.IsNullOrEmpty(Senha))
            {
                App.Current.MainPage.DisplayAlert("Erro", "Preencha todos os campos", "OK");
                return;
            }

            // Exemplo:
            if (CPF == "123" && Senha == "123")
            {
                Application.Current.MainPage = new NavigationPage(new pgNovaOcorrencia());
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Erro", "CPF ou senha inválidos", "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string nome = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
        }
    }
}