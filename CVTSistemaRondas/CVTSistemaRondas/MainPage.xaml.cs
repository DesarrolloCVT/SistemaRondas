#if ANDROID
using Android.Content;
using Android.Locations;
using Android.Widget;

#endif
using Controls.UserDialogs.Maui;
using CVTSistemaRondas.Models;
using CVTSistemaRondas.Views;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace CVTSistemaRondas
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            txtUsuario.Focus();

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!await CheckPermissions())
            {   
                await DisplayAlert("Atención!", "No se han habilitado los permisos", "Ok");
            }
        }

        private async Task<bool> CheckPermissions()
        {
            PermissionStatus cameraStatus = await CheckPermissions<Permissions.Camera>();
            PermissionStatus locationStatus = await CheckPermissions<Permissions.LocationWhenInUse>();

            return IsGranted(cameraStatus) && IsGranted(locationStatus);
        }
        private async Task<PermissionStatus> CheckPermissions<TPermission>() where TPermission : Permissions.BasePermission, new()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<TPermission>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<TPermission>();
            }

            return status;
        }
        private static bool IsGranted(PermissionStatus status)
        {
            return status == PermissionStatus.Granted || status == PermissionStatus.Limited;
        }

#if ANDROID
        private bool IsLocationServiceEnabled()
        {
            LocationManager locationManager = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);
            return locationManager.IsProviderEnabled(LocationManager.GpsProvider);
        }
#endif


        private async void Loging_ClickedAsync(object sender, EventArgs e)
        {
            try
            {
                using (UserDialogs.Instance.Loading("Verificando Datos"))
                {
                    await Task.Delay(10);
                    var ssid = DependencyService.Get<IGetSSID>().GetSSID();
                    string sd = Regex.Replace(ssid, @"[^\w\d]", string.Empty);

                    loging.IsEnabled = false;

                    var ACC = Connectivity.NetworkAccess;
                    if (ACC == NetworkAccess.Internet)
                    {
                        string usuario = txtUsuario.Text.ToLower();
                        string clave = txtContraseña.Text.ToLower();

                        try
                        {
                            HttpClient ClientHttp = new()
                            {
                                BaseAddress = new Uri("http://wsintranet.cvt.local/")
                            };
                            try
                            {
                                var rest = ClientHttp.GetAsync("api/Usuario?usuario=" + usuario + "&pass=" + clave).Result;

                                if (rest.IsSuccessStatusCode)
                                {
                                    var resultadoStr = rest.Content.ReadAsStringAsync().Result;
                                    var listado = JsonConvert.DeserializeObject<int>(resultadoStr);

                                    if (listado != 0)
                                    {
                                        try
                                        {
                                            var rest2 = ClientHttp.GetAsync("api/Usuario?idUser=" + listado).Result;
                                            var resultadoStr2 = rest2.Content.ReadAsStringAsync().Result;
                                            List<UsuarioClass> du = JsonConvert.DeserializeObject<List<UsuarioClass>>(resultadoStr2) ??
                                                throw new InvalidOperationException();
                                            foreach (var d in du)
                                            {
                                                App.Iduser = listado;
                                                App.UserSistema = d.UsuarioSistema;
                                                App.NombreUsuario = d.NombreUsuario.ToString();
                                            }

                                            DependencyService.Get<IAudio>().PlayAudioFile("Correcto.mp3");
                                            await Navigation.PushAsync(new EscanearCodigo());
                                            txtUsuario.Text = string.Empty;
                                            txtContraseña.Text = string.Empty;
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Excepcion .1 detectada: " + ex.InnerException?.Message);
                                            DependencyService.Get<IAudio>().PlayAudioFile("terran-error.mp3");
                                            await DisplayAlert("Alerta", "No tiene los perfiles necesarios para poder acceder a esta APP", "OK");
                                            txtUsuario.Text = string.Empty;
                                            txtContraseña.Text = string.Empty;
                                            txtUsuario.Focus();
                                            loging.IsEnabled = true;
                                        }
                                    }
                                    else
                                    {
                                        DependencyService.Get<IAudio>().PlayAudioFile("terran-error.mp3");
                                        await DisplayAlert("Alerta", "Usuario o Contraseña No Existen ", "Aceptar");
                                        txtUsuario.Text = string.Empty;
                                        txtContraseña.Text = string.Empty;
                                        txtUsuario.Focus();
                                        loging.IsEnabled = true;
                                    }
                                }
                                else 
                                {
                                    DependencyService.Get<IAudio>().PlayAudioFile("terran-error.mp3");
                                    await DisplayAlert("Alerta", "Se ha producido un error, verifique su conexión ", "Aceptar");
                                    txtUsuario.Text = string.Empty;
                                    txtContraseña.Text = string.Empty;
                                    txtUsuario.Focus();
                                    loging.IsEnabled = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Excepcion .2 detectada: " + ex.InnerException?.Message);
                                await DisplayAlert("Alerta", "Debe Conectarse a la Red Local", "Aceptar");
                                txtUsuario.Text = string.Empty;
                                txtContraseña.Text = string.Empty;
                                txtUsuario.Focus();
                                loging.IsEnabled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Excepcion .3 detectada: " + ex.InnerException?.Message);
                        }
                    }
                    else
                    {
                        DependencyService.Get<IAudio>().PlayAudioFile("terran-error.mp3");
                        await DisplayAlert("Alerta", "Debe Conectarse a la Red Local", "Aceptar");
                        loging.IsEnabled = true;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error Loging_ClickedAsync: {ex.Message}");
            }
            
        }
        protected override bool OnBackButtonPressed()
        {
            Dispatcher.Dispatch(async () => {
                //await DisplayAlert("Cerrar Sesión", "Desea cerrar la sesión", "Cancelar", "Ok");
                bool answer = await DisplayAlert("¿Cerrar Aplicación?", "", "Si", "No");
                if (answer)
                {
                    await Navigation.PopToRootAsync();
                    Application.Current?.Quit();
                }
                else
                {
                    return;
                }
            });
            // Return true to prevent back button 
            return true;
        }
    }

}
