using AppOcorrencias.ViewModels;
using System.Text;

namespace AppOcorrencias.Views
{
    public partial class pgLogin : ContentPage
    {
        public pgLogin()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
        }

        void OnCPFUnfocused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            entry.Text = FormatarCPF(entry.Text);
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