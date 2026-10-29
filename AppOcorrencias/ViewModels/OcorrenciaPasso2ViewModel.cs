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

            AdicionarAdolescenteCommand = new Command(async () =>
            {
                if (string.IsNullOrWhiteSpace(NovoNome))
                    return;

                if (NovaDataNascimento.Date >= DateTime.Today)
                {
                    await Shell.Current.DisplayAlert("Atenção", "Informe a data de nascimento do adolescente.", "OK");
                    return;
                }

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
                // preencheu um nome mas esqueceu de clicar em "Adicionar"
                if (!string.IsNullOrWhiteSpace(NovoNome))
                {
                    await Shell.Current.DisplayAlert("Atenção", "Você preencheu um nome mas não clicou em \"Adicionar Adolescente\".", "OK");
                    return;
                }

                if (await OcorrenciaValidator.BloquearSeInvalidoAsync(OcorrenciaValidator.ValidarAdolescentes(Adolescentes))) return;

                await Shell.Current.Navigation.PushAsync(new pgOcorrenciaPasso3());
            });
        }
    }
}