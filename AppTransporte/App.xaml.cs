//using AndroidX.AppCompat.View.Menu;
using AppTransporte.Interfaces;
using AppTransporte.model;
using Microsoft.Maui.Networking;

namespace AppTransporte
{
    public partial class App : Application
    {
        public static SqlServerService? Database { get; set; }

        public App()
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Light;
            Database = new SqlServerService("Data Source=SQL8011.site4now.net;Initial Catalog=db_aaecc9_paquitaappdb;User Id=db_aaecc9_paquitaappdb_admin;Password=paquita123;Connection Timeout=60");

            MainPage = new NavigationPage(new Login());

            // Verificar la conexión a internet después de inicializar MainPagew
            // (necesitamos MainPage inicializado para mostrar el DisplayAlert)
            MainThread.BeginInvokeOnMainThread(async () => {
                await Task.Delay(100); // Pequeño retraso para asegurar que la UI esté lista
                await VerificarConexionInicial();
            });
        }

        // Método para verificar la conexión inicial
        private async Task VerificarConexionInicial()
        {
            if (!IsConnected())
            {
                // Mostrar alerta de error de conexión
                bool salir = await MainPage.DisplayAlert(
                    "Sin conexión a internet",
                    "Verifica si tienes conexión a internet y vuelve a intentarlo.",
                    "Salir",
                    null);

                if (salir)
                {
                    // Si el usuario presiona "Salir", cerrar la aplicación
                    Application.Current.Quit();
                }
            }
        }

        // Método para verificar la conexión a internet
        private bool IsConnected()
        {
            var current = Connectivity.NetworkAccess;
            return current == NetworkAccess.Internet;
        }
    }
}