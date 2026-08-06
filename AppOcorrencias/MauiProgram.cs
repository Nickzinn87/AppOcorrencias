using AppOcorrencias;
using Microsoft.Maui.Handlers;
#if ANDROID
using Android.Views;
using AppOcorrencias.Platforms.Android;
#endif

namespace AppOcorrencias
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

#if ANDROID
            EntryHandler.Mapper.AppendToMapping("NoAutofill", (handler, view) =>
            {
                handler.PlatformView.ImportantForAutofill = ImportantForAutofill.No;
            });

            EntryHandler.Mapper.AppendToMapping("CpfMask", (handler, view) =>
            {
                if (view is Entry entry && entry.StyleId == "cpf")
                {
                    if (handler.PlatformView.Tag?.ToString() == "cpf-watcher-added")
                        return;

                    handler.PlatformView.AddTextChangedListener(new CpfTextWatcher(handler.PlatformView));
                    handler.PlatformView.Tag = new Java.Lang.String("cpf-watcher-added");
                }
            });
#endif

            return builder.Build();
        }
    }
}