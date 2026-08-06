using System.Text;
using AppOcorrencias.ViewModels;

namespace AppOcorrencias.Views
{
    public partial class pgLogin : ContentPage
    {
        bool _atualizandoTexto = false;

        public pgLogin()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
        }

        void OnCPFTextChanged(object sender, TextChangedEventArgs e)
        {
            // 🔹 no Android a formatação já é feita nativamente pelo CpfTextWatcher
            // (evita crash de composição de teclado/IME) — aqui só cobre Windows/iOS/Mac
#if ANDROID
            return;
#else
            if (_atualizandoTexto) return;

            var entry = (Entry)sender;
            var textoFormatado = FormatarCPF(e.NewTextValue);

            if (textoFormatado == entry.Text) return;

            _atualizandoTexto = true;
            entry.Text = textoFormatado;

            try
            {
                entry.CursorPosition = textoFormatado.Length;
            }
            catch { }

            _atualizandoTexto = false;
#endif
        }

        string FormatarCPF(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return string.Empty;

            var digitos = new string(valor.Where(char.IsDigit).ToArray());
            if (digitos.Length > 11)
                digitos = digitos.Substring(0, 11);

            var sb = new StringBuilder();
            for (int i = 0; i < digitos.Length; i++)
            {
                if (i == 3 || i == 6)
                    sb.Append('.');
                else if (i == 9)
                    sb.Append('-');

                sb.Append(digitos[i]);
            }

            return sb.ToString();
        }
    }
}