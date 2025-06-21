using AppTransporte.model;
using AppTransporte.viewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AppTransporte.Interfaces;

public partial class VEModificarUsuario : ContentPage, INotifyPropertyChanged
{
    public Usuario userContext { get; set; }
    private VMEmpresas vmEmpresas;
    private int _idUsuario;
    private int _idTipoUsuario;
    private bool passwordVisible = false;
    private bool _isBusy = false;

    // Propiedades para binding
    public ObservableCollection<TipoUsuario> TiposUsuario { get; set; } = new();

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            OnPropertyChanged(nameof(IsBusy));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public VEModificarUsuario(Usuario usuario, int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();

        userContext = usuario;
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;

        // Configurar el BindingContext ANTES de cargar datos
        this.BindingContext = this;

        // Configurar campos básicos
        username_entry.Text = usuario.Username;
        password_entry.Text = usuario.Contraseña;
        estado_switch.IsToggled = usuario.Estado;
        estado_label.Text = usuario.Estado ? "Activo" : "Inactivo";
        persona_label.Text = $"{usuario.Nombres} {usuario.Apellidos}";

        estado_switch.Toggled += (s, e) => {
            estado_label.Text = estado_switch.IsToggled ? "Activo" : "Inactivo";
        };

        // Cargar datos de forma asíncrona
        CargarDatosAsync(usuario.IdTipoUsuario, usuario.IdEmpresa);
    }

    private async void CargarDatosAsync(int idTipoUsuarioActual, int? idEmpresaActual)
    {
        try
        {
            IsBusy = true;

            // Cargar tipos de usuario y empresas en paralelo
            var taskTipos = CargarTiposUsuarioAsync(idTipoUsuarioActual);
            var taskEmpresas = CargarEmpresasAsync(idEmpresaActual);

            await Task.WhenAll(taskTipos, taskEmpresas);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar datos: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CargarTiposUsuarioAsync(int idTipoUsuarioActual)
    {
        try
        {
            // Obtener tipos de usuario desde la base de datos
            var tiposUsuarioBD = await App.Database.ObtenerTiposUsuarioAsync();

            // Limpiar y agregar a la colección en el hilo principal
            Device.BeginInvokeOnMainThread(() =>
            {
                TiposUsuario.Clear();
                foreach (var tipo in tiposUsuarioBD)
                {
                    TiposUsuario.Add(tipo);
                }

                // Seleccionar el tipo actual del usuario
                var tipoActual = TiposUsuario.FirstOrDefault(t => t.IdTipoUsuario == idTipoUsuarioActual);
                if (tipoActual != null)
                {
                    tipoUsuario_picker.SelectedItem = tipoActual;
                }
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar los tipos de usuario: {ex.Message}", "OK");
        }
    }

    private async Task CargarEmpresasAsync(int? idEmpresaUsuario = null)
    {
        try
        {
            // Inicializar el ViewModel de empresas
            vmEmpresas = new VMEmpresas();

            // Esperar a que se carguen los datos
            await Task.Delay(500);

            // Configurar en el hilo principal
            Device.BeginInvokeOnMainThread(() =>
            {
                // Limpiar items actuales
                empresa_picker.Items.Clear();

                // Agregar cada empresa al picker
                foreach (var empresa in vmEmpresas.Empresas)
                {
                    empresa_picker.Items.Add(empresa.razonSocial);
                }

                // Si el usuario ya tenía una empresa asignada, seleccionarla
                if (idEmpresaUsuario.HasValue)
                {
                    var empresaIndex = vmEmpresas.Empresas.ToList().FindIndex(e => e.id_empresa == idEmpresaUsuario.Value);
                    if (empresaIndex >= 0)
                    {
                        empresa_picker.SelectedIndex = empresaIndex;
                    }
                }
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar las empresas: {ex.Message}", "OK");
        }
    }

    private void TogglePasswordVisibility(object sender, EventArgs e)
    {
        passwordVisible = !passwordVisible;
        password_entry.IsPassword = !passwordVisible;

        // Cambiar el ícono del botón según el estado
        if (passwordVisible)
        {
            ((Button)sender).ImageSource = "oculto.png";
        }
        else
        {
            ((Button)sender).ImageSource = "mostrarcontra.png";
        }
    }

    private void Btn_atras(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEUsuarios(_idUsuario, _idTipoUsuario));
    }

    private async void btn_actualizarUsuario(object sender, EventArgs e)
    {
        try
        {
            IsBusy = true;

            // Validar que se haya seleccionado un tipo de usuario
            var tipoUsuarioSeleccionado = tipoUsuario_picker.SelectedItem as TipoUsuario;
            if (tipoUsuarioSeleccionado == null)
            {
                await DisplayAlert("Error", "Debe seleccionar un tipo de usuario", "OK");
                return;
            }

            // Obtener ID de empresa (si se seleccionó alguna)
            int? idEmpresa = null;
            if (empresa_picker.SelectedIndex >= 0 && vmEmpresas?.Empresas != null)
            {
                idEmpresa = vmEmpresas.Empresas[empresa_picker.SelectedIndex].id_empresa;
            }

            // Actualizar el usuario
            int resultado = await App.Database.ActualizarUsuarioAsync(
                userContext.IdUsuario,
                username_entry.Text,
                password_entry.Text,
                tipoUsuarioSeleccionado.IdTipoUsuario,
                estado_switch.IsToggled,
                userContext.IdPersona,
                idEmpresa);

            if (resultado > 0)
            {
                await DisplayAlert("Éxito", "Usuario actualizado correctamente", "OK");
                await Navigation.PushAsync(new VEUsuarios(_idUsuario, _idTipoUsuario));
            }
            else
            {
                await DisplayAlert("Error", "No se pudo actualizar el usuario", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un problema: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}