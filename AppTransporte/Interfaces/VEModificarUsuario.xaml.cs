using AppTransporte.model;
using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class VEModificarUsuario : ContentPage
{
    public Usuario userContext { get; set; }
    private VMEmpresas vmEmpresas;
    private int _idUsuario;
    private int _idTipoUsuario;
    private int? empresaSeleccionada;
    private bool passwordVisible = false;
    public VEModificarUsuario(Usuario usuario, int idUsuario, int idTipoUsuario)
	{
		InitializeComponent();
        userContext = usuario;
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        username_entry.Text = usuario.Username;
        password_entry.Text = usuario.Contraseña;
        estado_switch.IsToggled = usuario.Estado;
        estado_label.Text = usuario.Estado ? "Activo" : "Inactivo";
        persona_label.Text = $"{usuario.Nombres} {usuario.Apellidos}";
        estado_switch.Toggled += (s, e) => {
            estado_label.Text = estado_switch.IsToggled ? "Activo" : "Inactivo";
        };
        ConfigurarTipoUsuario(usuario.IdTipoUsuario);
        CargarEmpresas(usuario.IdEmpresa);

    }
    private void ConfigurarTipoUsuario(int idTipoUsuario)
    {
            switch (idTipoUsuario)
        {
            case 1:
                tipoUsuario_picker.SelectedIndex = 0; // Administrador
                break;
            case 2:
                tipoUsuario_picker.SelectedIndex = 1; // Cliente
                break;
            case 3:
                tipoUsuario_picker.SelectedIndex = 2; // Empleado
                break;
            default:
                tipoUsuario_picker.SelectedIndex = -1;
                break;
        }
    }
    private async void CargarEmpresas(int? idEmpresaUsuario = null)
    {
        try
        {
            // Inicializar el ViewModel de empresas
            vmEmpresas = new VMEmpresas();

            // Esperar a que se carguen los datos
            await Task.Delay(500); // Pequeña espera para asegurar que se carguen los datos

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
            ((Button)sender).ImageSource = "eyeclosed.png"; // Icono para ocultar contraseña
        }
        else
        {
            ((Button)sender).ImageSource = "eye.png"; // Icono para mostrar contraseña
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
            // Obtener el ID del tipo de usuario seleccionado
            int idTipoUsuario = tipoUsuario_picker.SelectedIndex + 1; // Asumiendo que los índices coinciden con IDs

            // Obtener ID de empresa (si se seleccionó alguna)
            int? idEmpresa = null;
            if (empresa_picker.SelectedIndex >= 0)
            {
                idEmpresa = vmEmpresas.Empresas[empresa_picker.SelectedIndex].id_empresa;
            }

            // Actualizar el usuario
            int resultado = await App.Database.ActualizarUsuarioAsync(
                userContext.IdUsuario,
                username_entry.Text,
                password_entry.Text,
                idTipoUsuario,
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

    }
}