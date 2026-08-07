using System.Collections.ObjectModel;
using System.Windows.Input;
using AppOcorrencias.Models;
using AppOcorrencias.Services;
using AppOcorrencias.Views;

namespace AppOcorrencias.ViewModels
{
    public class OcorrenciaPasso2ViewModel : BasePageViewModel
    {
        public ObservableCollection<Adolescente> Adolescentes { get; }

        string _novoNome;
        public string NovoNome
        {
            get => _novoNome;
            set { _novoNome = value; OnPropertyChanged(); }
        }

        DateTime _novaDataNascimento = DateTime.Now;
        public DateTime NovaDataNascimento
        {
            get => _novaDataNascimento;
            set { _novaDataNascimento = value; OnPropertyChanged(); }
        }

        string _novaEscola;
        public string NovaEscola
        {
            get => _novaEscola;
            set { _novaEscola = value; OnPropertyChanged(); }
        }

        public ICommand AdicionarAdolescenteCommand { get; }
        public ICommand RemoverAdolescenteCommand { get; }
        public ICommand VoltarCommand { get; }
        public ICommand ProximoCommand { get; }

        public OcorrenciaPasso2ViewModel()
        {
            Adolescentes = new ObservableCollection<Adolescente>(NovaOcorrenciaSingleton.Instance.Adolescentes);

            AdicionarAdolescenteCommand = new Command(() =>
            {
                if (string.IsNullOrWhiteSpace(NovoNome))
                    return;

                var adolescente = new Adolescente
                {
                    OcorrenciaUid = NovaOcorrenciaSingleton.Instance.OcorrenciaAtual.Uid,
                    Nome = NovoNome,
                    DataNascimento = NovaDataNascimento,
                    Escola = NovaEscola
                };

                Adolescentes.Add(adolescente);
                NovaOcorrenciaSingleton.Instance.Adolescentes.Add(adolescente);

                NovoNome = string.Empty;
                NovaDataNascimento = DateTime.Now;
                // 🔹 NÃO limpa NovaEscola — mantém pré-preenchido pro próximo adolescente,
                // já que normalmente estudam juntos, mas continua editável
            });

            RemoverAdolescenteCommand = new Command<Adolescente>((adolescente) =>
            {
                if (adolescente == null) return;
                Adolescentes.Remove(adolescente);
                NovaOcorrenciaSingleton.Instance.Adolescentes.Remove(adolescente);
            });

            VoltarCommand = new Command(async () =>
            {
                await Shell.Current.Navigation.PopAsync();
            });

            ProximoCommand = new Command(async () =>
            {
                if (Adolescentes.Count == 0)
                {
                    await Shell.Current.DisplayAlert("Atenção", "Adicione ao menos um adolescente.", "OK");
                    return;
                }

                await Shell.Current.Navigation.PushAsync(new pgOcorrenciaPasso3());
            });
        }
    }
}