using AppTransporte.Interfaces;
using AppTransporte.model;

namespace AppTransporte
{
    public partial class App : Application
    {
        public static SqlServerService? Database { get; set; }
        public App()
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Light;
            Database = new SqlServerService("Data Source=SQL8011.site4now.net;Initial Catalog=db_aaecc9_paquitaappdb;User Id=db_aaecc9_paquitaappdb_admin;Password=paquita123;Connection Timeout=60;");
            MainPage = new NavigationPage(new Login());

        }
    }
}
