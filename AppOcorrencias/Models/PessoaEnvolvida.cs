using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;

namespace AppOcorrencias.Models
{
    public class PessoaEnvolvida : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public string OcorrenciaUid { get; set; }

        public string Nome { get; set; }
        public string Documento { get; set; }
        public string Papel { get; set; }

        bool _assinado;
        public bool Assinado
        {
            get => _assinado;
            set { _assinado = value; OnPropertyChanged(); }
        }

        // 🔹 usado só em memória pra exibir a prévia (não salva no banco)
        ImageSource _assinaturaPreview;
        [Ignore]
        public ImageSource AssinaturaPreview
        {
            get => _assinaturaPreview;
            set { _assinaturaPreview = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string nome = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
    }
}