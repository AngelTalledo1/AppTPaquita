using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using CommunityToolkit.Maui;

namespace AppTransporte
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF1cWWhBYVF1WmFZfVtgfV9FZVZURWY/P1ZhSXxWdk1iX31fc3FQR2ZdWUJ9XEI=");

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Lato-Bold.ttf", "bold");
                    fonts.AddFont("Lato-Regular.ttf", "midium");
                    fonts.AddFont("example-js.html", "correo");
                    fonts.AddFont("Comfortaa-Bold.ttf", "Comf-Bold");
                    fonts.AddFont("Comfortaa-Medium.ttf", "Comf-Medium");
                    fonts.AddFont("Comfortaa-Regular.ttf", "Comf-Reg");
                    fonts.AddFont("Comfortaa-SemiBold.ttf", "Comf-SemBold");
                    fonts.AddFont("Comfortaa-VariableFont_wght.ttf", "Comf-Variable");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}