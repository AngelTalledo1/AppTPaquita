using AppTransporte.Interfaces;

namespace AppTransporte
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Login();
        }
    }
}
