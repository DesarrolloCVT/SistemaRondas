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
    public class GetSSIDAndroid : IGetSSID
    {
        public string GetSSID()
        {
            WifiManager? wifiManager = AndroidApp.Context.GetSystemService(Context.WifiService) as WifiManager;
            WifiInfo currentWifi = wifiManager.ConnectionInfo;
            if (currentWifi != null)
            {
                String wifiSSID = currentWifi.SSID ??
                                throw new InvalidOperationException();
                return wifiSSID;
                // Now you have the SSID!
            }
            else
            {
                return "NULL";
            }
        }
    }
}
