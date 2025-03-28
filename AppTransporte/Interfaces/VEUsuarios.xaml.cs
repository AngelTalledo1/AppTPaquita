using AppTransporte.model;
using AppTransporte.viewModel;
using System.Collections.ObjectModel;

namespace AppTransporte.Interfaces;

public partial class VEUsuarios : ContentPage
{
    public ObservableCollection<UserItem> Users { get; set; }
    public int idTipoUsuario { get; set; }
    public int idUsuario { get; set; }
    public VEUsuarios(int idUsuario, int idTipoUsuario )
	{
        this.idTipoUsuario = idTipoUsuario;
        this.idUsuario = idUsuario;

        InitializeComponent();

        BindingContext = new VMUsuario();
    }
    private void Btn_atrasUsuario(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuPrincipal(idUsuario,idTipoUsuario));
    }

    private void editar_usuario(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var usuario = button.CommandParameter as Usuario;
        if (usuario != null)
        {
            Navigation.PushAsync(new VEModificarUsuario(usuario, idUsuario, idTipoUsuario));
        }
        else
        {
            DisplayAlert("Error", "No se pudo cargar la información del usuario", "OK");
        }
        }

    private async void eliminar_usuario(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var usuario = button.CommandParameter as Usuario;
        bool respuesta = await DisplayAlert("Confirmación",
                                           "¿Deseas eliminar el usuario de " + usuario.Nombres+" "+usuario.Apellidos+ "?",
                                           "Sí",
                                           "No");
        if (respuesta)
        {
            var resultado = await App.Database.EliminarUsuarioAsync(usuario.IdUsuario);
            if (resultado > 0)
            {
                await DisplayAlert("Exito", "Usuario eliminado Exitosamente", "OK");
                if (BindingContext is VMUsuario viewModel)
                {
                    await viewModel.ActualizarDatos();
                }
            }
            else
            {
                await DisplayAlert("Error", "No se pudo eliminar el usuario. Verifica los datos.", "OK");
            }
        }
    }
}
