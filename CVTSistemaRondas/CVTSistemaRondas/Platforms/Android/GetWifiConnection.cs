using Android.Content;
using Android.Net.Wifi;
using CVTSistemaRondas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidApp = Android.App.Application;

namespace CVTSistemaRondas.Platforms.Android
{
    public class GetWifiConnection : IGetWifiState
    {
        public bool IsConnectedWifi()
        {
            WifiManager? wifiManager = AndroidApp.Context.GetSystemService(Context.WifiService) as WifiManager;

            if (wifiManager.IsWifiEnabled)
                return true;
            else
                return false;
        }
    }
}
