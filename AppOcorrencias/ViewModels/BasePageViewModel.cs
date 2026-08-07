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

        // 🔹 puxa o nome direto do usuário logado no singleton
        public string NomeUsuario => UserSingleton.Instance.UsuarioLogado?.Nome ?? "Usuário";

        public ICommand ToggleMenuCommand { get; }
        public ICommand OpenNova { get; }
        public ICommand OpenSync { get; }

        public BasePageViewModel()
        {
            ToggleMenuCommand = new Command(() =>
            {
                MenuAberto = !MenuAberto;
            });
            OpenNova = new Command(async () =>
            {
                await Shell.Current.GoToAsync("//nova");
            });
            OpenSync = new Command(async () =>
            {
                await Shell.Current.GoToAsync("//sync");
            });
        }
    }
}