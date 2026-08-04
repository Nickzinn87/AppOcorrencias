using Microsoft.Maui.Graphics;

namespace AppOcorrencias.Views
{
    public partial class pgAssinatura : ContentPage
    {
        readonly List<List<PointF>> _tracos = new List<List<PointF>>();
        List<PointF> _tracoAtual;
        readonly SignatureDrawable _drawable;

        // Evento que a NovaOcorrenciaViewModel escuta pra receber o base64
        public event Action<string> AssinaturaConfirmada;

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

            AssinaturaConfirmada?.Invoke(base64);

            await Navigation.PopModalAsync();
        }
    }

    // Classe auxiliar que desenha os traços no GraphicsView
    public class SignatureDrawable : IDrawable
    {
        readonly List<List<PointF>> _tracos;

        public SignatureDrawable(List<List<PointF>> tracos)
        {
            _tracos = tracos;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
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