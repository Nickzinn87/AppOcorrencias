using Microsoft.Maui.Graphics;

namespace AppOcorrencias.Views
{
    public partial class pgAssinatura : ContentPage
    {
        readonly List<List<PointF>> _tracos = new List<List<PointF>>();
        List<PointF> _tracoAtual;
        readonly SignatureDrawable _drawable;

        // 🔹 Func<string, Task> em vez de Action<string> — permite AWAIT de verdade
        public event Func<string, Task> AssinaturaConfirmada;

        public pgAssinatura(string nomeSignatario)
        {
            InitializeComponent();

            lblNomeSignatario.Text = $"Assinatura de: {nomeSignatario}";

            _drawable = new SignatureDrawable(_tracos);
            CanvasAssinatura.Drawable = _drawable;
        }

        void OnStart(object sender, TouchEventArgs e)
        {
            _tracoAtual = new List<PointF> { e.Touches[0] };
            _tracos.Add(_tracoAtual);
        }

        void OnDrag(object sender, TouchEventArgs e)
        {
            _tracoAtual?.Add(e.Touches[0]);
            CanvasAssinatura.Invalidate();
        }

        void OnEnd(object sender, TouchEventArgs e)
        {
            _tracoAtual = null;
        }

        void OnLimpar(object sender, EventArgs e)
        {
            _tracos.Clear();
            CanvasAssinatura.Invalidate();
        }

        async void OnConfirmar(object sender, EventArgs e)
        {
            if (_tracos.Count == 0)
            {
                await DisplayAlert("Atenção", "Colete a assinatura antes de confirmar.", "OK");
                return;
            }

            var imagem = await CanvasAssinatura.CaptureAsync();
            using var stream = await imagem.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var base64 = Convert.ToBase64String(memoryStream.ToArray());

            // 🔹 espera o handler (salvar no banco + gerar prévia) terminar ANTES de fechar a modal
            if (AssinaturaConfirmada != null)
                await AssinaturaConfirmada.Invoke(base64);

            await Navigation.PopModalAsync();
        }

        async void OnCancelar(object sender, EventArgs e)
        {
            // 🔹 não dispara AssinaturaConfirmada — simplesmente fecha sem salvar nada
            await Navigation.PopModalAsync();
        }
    }

    public class SignatureDrawable : IDrawable
    {
        readonly List<List<PointF>> _tracos;

        public SignatureDrawable(List<List<PointF>> tracos)
        {
            _tracos = tracos;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // 🔹 pinta o fundo branco ANTES dos traços — sem isso, a área "vazia"
            // fica transparente e o CaptureAsync converte pra preto na exportação
            canvas.FillColor = Colors.White;
            canvas.FillRectangle(dirtyRect);

            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 3;

            foreach (var traco in _tracos)
            {
                for (int i = 0; i < traco.Count - 1; i++)
                {
                    canvas.DrawLine(traco[i], traco[i + 1]);
                }
            }
        }
    }
}