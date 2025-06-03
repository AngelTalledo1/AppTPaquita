using AppTransporte.model;
using AppTransporte.viewModel;
using Microsoft.Maui.Graphics.Text;
using System.Collections.ObjectModel;
#pragma warning disable CS4014, CS8602, CS8618
namespace AppTransporte.Interfaces;

public partial class VCNuevaSolicitud : ContentPage
{
    public Cliente cliente { get; set; }
    public int idUsuario { get; set; }
    public int idTipoUsuario { get; set; }
    private VMServicio servicioViewModel;
    private Solicitud solicitudActual;

    public VCNuevaSolicitud(Cliente cliente, int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        this.cliente = cliente;
        this.idUsuario = idUsuario;
        this.idTipoUsuario = idTipoUsuario;
        TituloNuev.Text = "Nueva Solicitud";
        IdClienteLabel.Text = cliente.ApePaterno + " " + cliente.ApeMaterno + ", " + cliente.Nombre;
        Solicitar.IsVisible = true;
        Cancelar.IsVisible = true;

        // Inicializar y configurar el ViewModel de Servicios
        servicioViewModel = new VMServicio();
        BindingContext = servicioViewModel;

        // Configurar el binding para el ServicioPicker
        ServicioPicker.SetBinding(Picker.ItemsSourceProperty, nameof(VMServicio.Servicios));
        ServicioPicker.ItemDisplayBinding = new Binding("Descripcion");
    }

    public VCNuevaSolicitud(Solicitud solicitud, int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        this.idUsuario = idUsuario;
        this.idTipoUsuario = idTipoUsuario;
        this.solicitudActual = solicitud;

        TituloNuev.Text = $"Modificar Solicitud {solicitud.IdSolicitud}";
        IdClienteLabel.Text = $"{solicitud.IdCliente}";
        descripcionEntry.Text = solicitud.Descripcion;
        Eliminar.IsVisible = true;
        Actualizar.IsVisible = true;

        // Inicializar y configurar el ViewModel de Servicios
        servicioViewModel = new VMServicio();
        BindingContext = servicioViewModel;

        // Configurar el binding para el ServicioPicker
        ServicioPicker.SetBinding(Picker.ItemsSourceProperty, nameof(VMServicio.Servicios));
        ServicioPicker.ItemDisplayBinding = new Binding("Descripcion");

        // Suscribirse al evento PropertyChanged para detectar cuando los servicios estén cargados
        servicioViewModel.PropertyChanged += ServicioViewModel_PropertyChanged;
    }

    private void ServicioViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(VMServicio.Servicios) && servicioViewModel.Servicios.Count > 0 && solicitudActual != null)
        {
            // Una vez que los servicios estén cargados, seleccionar el servicio de la solicitud
            SetServicioSeleccionado(solicitudActual.IdServicio);

            // Desuscribirse para evitar múltiples llamadas
            servicioViewModel.PropertyChanged -= ServicioViewModel_PropertyChanged;
        }
    }

    private void SetServicioSeleccionado(int idServicio)
    {
        if (ServicioPicker.ItemsSource is ObservableCollection<Servicio> servicios)
        {
            var servicioSeleccionado = servicios.FirstOrDefault(s => s.IdServicio == idServicio);
            if (servicioSeleccionado != null)
            {
                ServicioPicker.SelectedItem = servicioSeleccionado;
            }
        }
    }

    private void Btn_atrasSolPedido(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VCMisSolicitudes(idUsuario, idTipoUsuario));
    }

    private void Btn_cancelar(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VCMisSolicitudes(idUsuario, idTipoUsuario));
    }

    private async void Btn_Solicitar(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(descripcionEntry.Text))
            {
                await DisplayAlert("Error", "La descripción es obligatoria.", "OK");
                return;
            }

            if (ServicioPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Debe seleccionar un servicio.", "OK");
                return;
            }

            var servicioSeleccionado = (Servicio)ServicioPicker.SelectedItem;

            var nuevaSolicitud = new Solicitud
            {
                Descripcion = descripcionEntry.Text,
                IdEstadoSolicitud = 1,
                IdCliente = cliente.IdCliente,
                FechaSolicitud = DateTime.Now,
                IdServicio = servicioSeleccionado.IdServicio
            };

            await App.Database.AgregarSolicitudAsync(nuevaSolicitud);

            await DisplayAlert("Éxito", "Solicitud creada exitosamente.", "OK");
            Navigation.PushAsync(new VCMisSolicitudes(idUsuario, idTipoUsuario));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error al crear la solicitud: {ex.Message}", "OK");
        }
    }

    private async void Btn_EliminarSolicitud(object sender, EventArgs e)
    {
        if (solicitudActual != null)
        {
            bool respuesta = await DisplayAlert("Confirmación",
                $"¿Está seguro de eliminar la solicitud {solicitudActual.IdSolicitud}?",
                "Sí", "No");

            if (respuesta)
            {
                try
                {
                    // Por ahora, mostrar mensaje indicando que la función está en desarrollo
                    await DisplayAlert("Información", "La funcionalidad de eliminación está en desarrollo.", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Ocurrió un error al eliminar la solicitud: {ex.Message}", "OK");
                }
            }
        }
        else
        {
            await DisplayAlert("Error", "No se pudo obtener la información de la solicitud.", "OK");
        }
    }

    private async void Btn_ModificarSolicitud(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(descripcionEntry.Text))
            {
                await DisplayAlert("Error", "La descripción es obligatoria.", "OK");
                return;
            }

            if (ServicioPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Debe seleccionar un servicio.", "OK");
                return;
            }

            if (solicitudActual != null)
            {
                var servicioSeleccionado = (Servicio)ServicioPicker.SelectedItem;

                solicitudActual.Descripcion = descripcionEntry.Text;
                solicitudActual.IdServicio = servicioSeleccionado.IdServicio;

                // Llamar al método para actualizar la solicitud
                int resultado = await App.Database.ActualizarSolicitudAsync(solicitudActual);

                if (resultado > 0)
                {
                    await DisplayAlert("Éxito", "Solicitud actualizada correctamente.", "OK");
                    await Navigation.PushAsync(new VCMisSolicitudes(idUsuario, idTipoUsuario));
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo actualizar la solicitud.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "No se pudo obtener la información de la solicitud.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error al modificar la solicitud: {ex.Message}", "OK");
        }
    }
}
