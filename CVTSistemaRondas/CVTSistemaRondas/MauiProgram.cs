using Controls.UserDialogs.Maui;
using DevExpress.Maui;
using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;

namespace CVTSistemaRondas
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseDevExpress()
                .UseDevExpressControls()
                .UseDevExpressEditors()
                .UseDevExpressCollectionView()
                .UseMauiApp<App>()
                .UseUserDialogs(
                () =>
                {
                    //setup your default styles for dialogs
                    AlertConfig.DefaultBackgroundColor = Colors.Purple;
                #if ANDROID
                    AlertConfig.DefaultMessageFontFamily = "OpenSans-Regular.ttf";
                #else
                    AlertConfig.DefaultMessageFontFamily = "OpenSans-Regular";
                #endif
                    ToastConfig.DefaultCornerRadius = 15;
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseBarcodeReader();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
