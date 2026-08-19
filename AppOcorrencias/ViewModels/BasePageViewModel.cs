using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppOcorrencias.Services;

namespace AppOcorrencias.ViewModels
{
    public class BasePageViewModel : BindableObject
    {
        bool _menuAberto;
        public bool MenuAberto
        {
            get => _menuAberto;
            set
            {
                _menuAberto = value;
                OnPropertyChanged();
            }
        }

        public string NomeUsuario => UserSingleton.Instance.UsuarioLogado?.Nome ?? "Usuário";

        public ICommand ToggleMenuCommand { get; }
        public ICommand OpenNova { get; }
        public ICommand OpenSync { get; }
        public ICommand OpenMeus { get; }
        public ICommand LogoutCommand { get; }

        public BasePageViewModel()
        {
            ToggleMenuCommand = new Command(() =>
            {
                MenuAberto = !MenuAberto;
            });

            OpenNova = new Command(async () =>
            {
                MenuAberto = false;
                await Shell.Current.GoToAsync("//nova");
            });

            OpenSync = new Command(async () =>
            {
                MenuAberto = false;
                await Shell.Current.GoToAsync("//sync");
            });

            OpenMeus = new Command(async () =>
            {
                MenuAberto = false;
                await Shell.Current.GoToAsync("//minhas");
            });

            LogoutCommand = new Command(async () =>
            {
                MenuAberto = false;

                bool confirmar = await Shell.Current.DisplayAlert(
                    "Sair",
                    "Tem certeza que deseja sair da sua conta?",
                    "Sim",
                    "Cancelar");

                if (!confirmar) return;

                UserSingleton.Instance.Logout();

                // 🔹 troca a página raiz pro login, limpando toda a pilha de navegação
                await Shell.Current.GoToAsync("//login");
            });
        }
    }
}