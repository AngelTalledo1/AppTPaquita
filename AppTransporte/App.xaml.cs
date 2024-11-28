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
            Database = new SqlServerService("Data Source=SQL8011.site4now.net;Initial Catalog=db_aaecc9_paquitaappdb;User Id=db_aaecc9_paquitaappdb_admin;Password=paquita123");
<<<<<<< HEAD
            MainPage = new NavigationPage(new VCMisSolicitudes());
=======
            MainPage = new NavigationPage(new Login());
>>>>>>> 961cf8159267e1c170df2fdf342fe5e4ce2f18b7


        }
    }
}
