using Android.Text;
using Android.Widget;

namespace AppOcorrencias.Platforms.Android
{
    public class CpfTextWatcher : Java.Lang.Object, ITextWatcher
    {
        readonly EditText _editText;
        bool _editando;

        public CpfTextWatcher(EditText editText)
        {
            _editText = editText;
        }

        public void BeforeTextChanged(Java.Lang.ICharSequence s, int start, int count, int after) { }

        public void OnTextChanged(Java.Lang.ICharSequence s, int start, int before, int count) { }

        public void AfterTextChanged(IEditable s)
        {
            if (_editando) return;
            _editando = true;

            try
            {
                var original = s.ToString();
                var digitos = new string(original.Where(char.IsDigit).ToArray());
                if (digitos.Length > 11)
                    digitos = digitos.Substring(0, 11);

                var sb = new System.Text.StringBuilder();
                for (int i = 0; i < digitos.Length; i++)
                {
                    if (i == 3 || i == 6)
                        sb.Append('.');
                    else if (i == 9)
                        sb.Append('-');

                    sb.Append(digitos[i]);
                }

                var formatado = sb.ToString();

                if (original != formatado)
                {
                    // 🔹 remove o próprio listener antes de mudar o texto,
                    // evitando conflito com o listener interno do MAUI (Entry<->EditText)
                    _editText.RemoveTextChangedListener(this);

                    _editText.Text = formatado;
                    _editText.SetSelection(formatado.Length);

                    _editText.AddTextChangedListener(this);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CPF WATCHER] ERRO: {ex}");
            }
            finally
            {
                _editando = false;
            }
        }
    }
}