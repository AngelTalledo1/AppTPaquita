using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.viewModel
{
    public class VMEstado //: INotifyPropertyChanged
    {
        /* private string _titulo;
         private string _imagenEstado;
         private string _estadoPedido;

         public string Titulo
         {
             get => _titulo;
             set
             {
                 if (_titulo != value)
                 {
                     _titulo = value;
                     OnPropertyChanged(nameof(Titulo));
                 }
             }
         }

         public string ImagenEstado
         {
             get => _imagenEstado;
             private set
             {
                 if (_imagenEstado != value)
                 {
                     _imagenEstado = value;
                     OnPropertyChanged(nameof(ImagenEstado));
                 }
             }
         }

         public string EstadoPedido
         {
             get => _estadoPedido;
             set
             {
                 if (_estadoPedido != value)
                 {
                     _estadoPedido = value;
                     OnPropertyChanged(nameof(EstadoPedido));
                     ActualizarImagenEstado();
                 }
             }
         }

         private void ActualizarImagenEstado()
         {
             ImagenEstado = _estadoPedido switch
             {
                 "Pendiente" => "pendiente.png",    
                 "Asignado" => "asignado.png",    
                 "Entregado" => "entregado.png",
                 "En curso" => "encurso.png",
                 "cancelado" => "cancelado.png",    
                 _ => "default.png",                
             };
         }

         public event PropertyChangedEventHandler PropertyChanged;

         public ICommand UpdateOrderCommand { get; }
         public event Action OnOrderCompleted;
         private double _progressBarHeight = 50; // Altura inicial de la barra
         public double ProgressBarHeight
         {
             get { return _progressBarHeight; }
             set
             {
                 _progressBarHeight = value;
                 OnPropertyChanged(nameof(ProgressBarHeight));
             }
         }

         // Properties to bind icon and color for each step
         public string Step1Icon { get; set; } = "boton.png";
         public string Step2Icon { get; set; } = "redondo.png";
         public string Step3Icon { get; set; } = "redondo.png";
         public string Step4Icon { get; set; } = "redondo.png";
         public string Step5Icon { get; set; } = "redondo.png";
         public string Step6Icon { get; set; } = "redondo.png";

         public string Step2Color { get; set; } = "Gray";
         public string Step3Color { get; set; } = "Gray";
         public string Step4Color { get; set; } = "Gray";
         public string Step5Color { get; set; } = "Gray";
         public string Step6Color { get; set; } = "Gray";

         private int currentStep = 1;

         public VMEstado()
         {
             UpdateOrderCommand = new Command(UpdateOrderStatus);
         }

         private void UpdateOrderStatus()
         {
             currentStep++;
             switch (currentStep)
             {
                 case 2:
                     Step2Icon = "boton.png";
                     Step2Color = "Black";
                     ProgressBarHeight += 50;
                     break;
                 case 3:
                     Step3Icon = "boton.png";
                     Step3Color = "Black";
                     ProgressBarHeight += 50;
                     break;
                 case 4:
                     Step4Icon = "boton.png";
                     Step4Color = "Green";
                     ProgressBarHeight += 50;
                     break;
                 case 5:
                     Step5Icon = "boton.png";
                     Step5Color = "Black";
                     ProgressBarHeight += 50;
                     break;
                 case 6:
                     Step6Icon = "boton.png";
                     Step6Color = "Black";
                     ProgressBarHeight += 50;
                     break;
             }
             OnPropertyChanged(nameof(Step2Icon));
             OnPropertyChanged(nameof(Step2Color));
             OnPropertyChanged(nameof(Step3Icon));
             OnPropertyChanged(nameof(Step3Color));
             OnPropertyChanged(nameof(Step4Icon));
             OnPropertyChanged(nameof(Step4Color));
             OnPropertyChanged(nameof(Step5Icon));
             OnPropertyChanged(nameof(Step5Color));
             OnPropertyChanged(nameof(Step6Icon));
             OnPropertyChanged(nameof(Step6Color));
             if (currentStep >= 6)
             {
                 // Disparar el evento para mostrar la ventana emergente
                 OnOrderCompleted?.Invoke();
             }
         }
         protected void OnPropertyChanged(string propertyName)
         {
             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
         }*/
    }
}
