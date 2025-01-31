using Android.App;
using Android.Runtime;
using CVTSistemaRondas.Models;
using CVTSistemaRondas.Platforms.Android;

namespace CVTSistemaRondas
{
    [Application(UsesCleartextTraffic = true)]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
            DependencyService.Register<IGetSSID, GetSSIDAndroid>();
            DependencyService.Register<IGetWifiState, GetWifiConnection>();
            DependencyService.Register<IAudio, AudioService>();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
