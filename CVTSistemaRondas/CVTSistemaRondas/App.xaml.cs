namespace CVTSistemaRondas
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());

            #region Modo luminoso - telefono
            Application.Current.UserAppTheme = AppTheme.Light;
            //this.RequestedThemeChanged += (s, e) => { Application.Current.UserAppTheme = AppTheme.Light; };
            #endregion
        }

        public static int Iduser { get; set; }
        public static string UserSistema { get; set; }
        public static string NombreUsuario { get; set; }
        public static int idPerfil { get; set; }
        public static bool vali { get; set; }
        public static string Fconsoli { get; set; }
        public static string DptoConsolidado { get; set; }

        public static string lati { get; set; }
        public static string longi { get; set; }

        public static string altit { get; set; }
    }
}
