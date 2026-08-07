using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;

namespace AppOcorrencias.Models
{
    public class Adolescente : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public string OcorrenciaUid { get; set; }

        string _nome;
        public string Nome
        {
            get => _nome;
            set { _nome = value; OnPropertyChanged(); }
        }

        DateTime _dataNascimento = DateTime.Now;
        public DateTime DataNascimento
        {
            get => _dataNascimento;
            set { _dataNascimento = value; OnPropertyChanged(); }
        }

        string _escola;
        public string Escola
        {
            get => _escola;
            set { _escola = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string nome = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
    }
}