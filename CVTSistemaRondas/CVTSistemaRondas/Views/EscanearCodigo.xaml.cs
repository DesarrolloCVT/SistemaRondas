using CVTSistemaRondas.Models;
using Microsoft.Maui.Devices.Sensors;
using System.ComponentModel;
using ZXing.Net.Maui;

namespace CVTSistemaRondas.Views;


public partial class EscanearCodigo : INotifyPropertyChanged
{
    private int _result;
    private string _ubicacion;
    private string _fecha;
    private string _usuario;
    private bool _flag = true;
    public Location location;

    private CancellationTokenSource _cancelTokenSource;
    private bool _isCheckingLocation;

    public bool Flag
    {
        get => _flag;
        set
        {
            _flag = value;
            OnPropertyChanged(nameof(Flag));
        }
    }
    public int Result
    {
        get => _result;
        set
        {
            _result = value;
            OnPropertyChanged(nameof(Result));
        }
    }
    public string Ubicacion
    {
        get => _ubicacion;
        set
        {
            _ubicacion = value;
            OnPropertyChanged(nameof(Ubicacion));
        }
    }
    public string Fecha
    {
        get => _fecha;
        set
        {
            _fecha = value;
            OnPropertyChanged(nameof(Fecha));
        }
    }
    public string Usuario
    {
        get => _usuario;
        set
        {
            _usuario = value;
            OnPropertyChanged(nameof(Usuario));
        }
    }
    public object Android { get; private set; }
    public EscanearCodigo() => InitializeComponent();
    protected override void OnAppearing() {
        base.OnAppearing();
    } 
    private async void Localization()
    {
        try
        {
            _isCheckingLocation = true;

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);
            Console.WriteLine($"Latitude: {location?.Latitude}, Longitude: {location?.Longitude}, Altitude: {location?.Altitude}");
            Ubicacion = "http://maps.google.com/maps?q=" + location?.Latitude + "," + location?.Longitude;
        }
        catch (Exception ex)
        {
            Ubicacion = "Ubicación no registrada";
            _isCheckingLocation = false;
            Console.WriteLine(ex.ToString());
            Dispatcher.Dispatch(async () => {
                await DisplayAlert("Activar GPS", "El GPS no se encuentra activo", "Ok");
                Application.Current?.MainPage?.Navigation.PopModalAsync();
            });
        }
    }
    private void getDatos()
    {   
        Flag = true;
        DateTime ahora = DateTime.Now;
        Fecha = ahora.ToString("yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        Usuario = App.UserSistema;
    }
    private void Btn_Escanear_Clicked(object sender, EventArgs e)
    {
        if (DependencyService.Get<IGetWifiState>().IsConnectedWifi())
        {
            Localization();
            if (_isCheckingLocation)
            {
                var scanPage = new ContentPage()
                {
                    Title = "SCANNER"
                };
                var cameraBarcodeReaderView = new ZXing.Net.Maui.Controls.CameraBarcodeReaderView
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                };

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    getDatos();
                });
                scanPage.Content = cameraBarcodeReaderView;
                cameraBarcodeReaderView.CameraLocation = CameraLocation.Rear;

                /* Llamado a Pagina de ecaneo QR */
                Application.Current?.MainPage?.Navigation
                    .PushModalAsync(new NavigationPage(scanPage)
                    { BarTextColor = Colors.White, BarBackgroundColor = Colors.CadetBlue }, true);

                cameraBarcodeReaderView.BarcodesDetected += (sender, e) =>
                {
                    try
                    {
                        if (Flag)
                        {
                            Flag = false;
                            var first = e.Results?.FirstOrDefault();
                            if (string.IsNullOrEmpty(first?.ToString()))
                            {
                                Result = 0;
                                scanPage.DisplayAlert("Incorrecto", "Código no registrado.", "OK");
                            }
                            else
                            {
                                Result = int.Parse(first.Value);
                            }

                            if (!string.IsNullOrEmpty(Fecha) && !string.IsNullOrEmpty(Usuario) && !string.IsNullOrEmpty(Ubicacion) && Result != 0)
                            {
                                if (DependencyService.Get<IGetWifiState>().IsConnectedWifi())
                                {
                                    HttpClient ClientHttp = new()
                                    {
                                        BaseAddress = new Uri("http://wsintranet.cvt.local/")
                                    };
                                    var rest = ClientHttp.GetAsync("api/Ronda?Fecha=" + Fecha + "&Usuario=" + Usuario + "&Ubicacion=" + Ubicacion + "&Punto=" + Result).Result;
                                    #region PopUp de respuesta al detectar un código
                                    scanPage.Dispatcher.Dispatch(() => {
                                        Application.Current?.MainPage?.Navigation.PopModalAsync();
                                        if (rest.IsSuccessStatusCode)
                                        {
                                            scanPage.DisplayAlert("Correcto", "Entrada registrada correctamente.", "OK");
                                        }
                                        else
                                        {
                                            scanPage.DisplayAlert("Incorrecto", "No se ha registrado la ubicación.", "OK");
                                        }
                                    });
                                    #endregion
                                }
                                else
                                {
                                    Application.Current?.MainPage?.Navigation.PopModalAsync();
                                }
                            }
                            else
                            {
                                Application.Current?.MainPage?.Navigation.PopModalAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("BarcodesDetected: " + ex.ToString());
                    }
                };
            }
        }
        else
        {
            DisplayAlert("Alerta", "Debe estar conectado a la red local", "Ok");
        }
    }
    /* Metodo Ir Atras */
    protected override bool OnBackButtonPressed()
    {
        /* Pop-Up para desplegar alertas */
        Dispatcher.Dispatch(async () => {
            await DisplayAlert("¿Finalizar Sesión?", "Entrada registrada correctamente.", "OK");
            await Navigation.PopToRootAsync();
            Application.Current?.Quit();
        });
        // Return true to prevent back button 
        return true;
    }
}