using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using AppTransporte.model;

namespace AppTransporte.Interfaces
{
    public partial class PedidosProgramadosPage : ContentPage
    {
        //public ObservableCollection<PedidoProgramado> PedidosProgramados { get; set; } = new ObservableCollection<PedidoProgramado>();

        public PedidosProgramadosPage()
        {
            InitializeComponent();
         //   BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
           // PedidosProgramados.Clear();
            //var pedidos = await App.Database.ObtenerPedidosProgramadosAsync();
            //foreach (var pedido in pedidos)
            {
               // PedidosProgramados.Add(pedido);
            }
        }
    }
}