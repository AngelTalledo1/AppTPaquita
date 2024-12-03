
using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.Interfaces
{
    public partial class ValidatingPopup : Popup
    {
        public ValidatingPopup()
        {
            var layout = new VerticalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var frame = new Frame
            {
                BackgroundColor = Color.FromRgb(0,0,0),
                Padding = 20,
                CornerRadius = 20,
                HasShadow = true,
                WidthRequest = 300,
                HeightRequest = 150,
                Content = new VerticalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new ActivityIndicator { IsRunning = true, IsVisible = true },
                        new Label { Text = "Validando datos...", HorizontalOptions = LayoutOptions.Center }
                    }
                }
            };

            layout.Children.Add(frame);
            this.Content = layout;  // Asigna el contenido al Popup
        }
    }
}
