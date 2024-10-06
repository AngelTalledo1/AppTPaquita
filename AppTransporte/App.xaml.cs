using AppTransporte.Interfaces;
using AppTransporte.model;
namespace AppTransporte
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new VEtransportistas());
        }
    }
}
