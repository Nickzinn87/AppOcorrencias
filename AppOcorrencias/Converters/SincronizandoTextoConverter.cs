using System.Globalization;

namespace AppOcorrencias.Converters
{
    public class SincronizandoTextoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool sincronizando && sincronizando ? "Sincronizando..." : "🔄  Sincronizar Agora";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}