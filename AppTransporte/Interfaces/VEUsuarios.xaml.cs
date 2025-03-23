using AppTransporte.model;
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
        Users = new ObservableCollection<UserItem>
        {
            new UserItem {
                Number = 1,
                Name = "Carlos Mendez",
                Email = "cmendez@example.com",
                Department = "Ventas",
                Status = "Activo",
                StatusColor = Color.FromHex("#008800")
            },
            new UserItem {
                Number = 2,
                Name = "Maria Sánchez",
                Email = "msanchez@example.com",
                Department = "Logística",
                Status = "Inactivo",
                StatusColor = Color.FromHex("#d9342d")
            },
            // More users...
        };

        this.BindingContext = this;
    }
    private void Btn_atrasUsuario(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuPrincipal(idUsuario,idTipoUsuario));
    }
}
