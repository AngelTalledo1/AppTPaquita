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
}
