using AppOcorrencias.Models;
using AppOcorrencias.Services;
using AppOcorrencias.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AppOcorrencias.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _api;

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
            _api = new ApiService();

            LoginCommand = new Command(async () => await RealizarLogin());
        }

        private async Task RealizarLogin()
        {
            if (string.IsNullOrEmpty(CPF) || string.IsNullOrEmpty(Senha))
            {
                await App.Current.MainPage.DisplayAlert("Erro", "Preencha todos os campos", "OK");
                return;
            }

            // 🌐 LOGIN VIA API
            var usuario = await _api.LoginAsync(CPF, Senha);

            if (usuario != null)
            {
                // 🔐 salva no singleton
                UserSingleton.Instance.SetUsuario(usuario);

                // 🚀 navegação
                Application.Current.MainPage = new NavigationPage(new pgNovaOcorrencia());
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Erro", "CPF ou senha inválidos", "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string nome = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
        }
    }
}