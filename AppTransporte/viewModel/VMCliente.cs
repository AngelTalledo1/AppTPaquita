using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AppTransporte.model;


namespace AppTransporte;
public class VMCliente : INotifyPropertyChanged
{
    private readonly ClienteService _clienteService;
    public ObservableCollection<Cliente> Clientes { get; set; } = new ObservableCollection<Cliente>();

    private bool isBusy;
    public bool IsBusy
    {
        get => isBusy;
        set
        {
            isBusy = value;
            OnPropertyChanged();
        }
    }

    public VMCliente()
    {
        _clienteService = new ClienteService();
        LoadClientes();
    }

    public async void LoadClientes(string? nombre = null)
    {
        try
        {
            IsBusy = true;

            // Obtén la lista de clientes desde el servicio
            var clientes = await _clienteService.ObtenerClientesAsync(nombre);

            // Limpia y actualiza la colección
            Clientes.Clear();
            foreach (var cliente in clientes)
            {
                Clientes.Add(cliente);
            }

            // Verifica si hay datos
            Console.WriteLine($"Clientes cargados: {Clientes.Count}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
