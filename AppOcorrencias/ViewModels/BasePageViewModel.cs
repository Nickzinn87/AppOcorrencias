using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        public ICommand ToggleMenuCommand { get; }

        public BasePageViewModel()
        {
            ToggleMenuCommand = new Command(() =>
            {
                MenuAberto = !MenuAberto;
            });
        }
    }
}
