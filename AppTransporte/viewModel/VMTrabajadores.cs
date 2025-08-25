using AppTransporte.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AppTransporte.viewModel
{
    public class VMTrabajadores : INotifyPropertyChanged
    {
        private ObservableCollection<Trabajador> _trabajadores = new();
        private ObservableCollection<Trabajador> _todosTrabajadores = new();
        private ObservableCollection<string> _categorias = new();
        private string _categoriaSeleccionada = "Todos";
        private string _searchText = "";
        private bool _isBusy = false;
        private bool _mostrarSoloActivos = true; // NUEVO FILTRO

        // Propiedades públicas
        public ObservableCollection<Trabajador> Trabajadores
        {
            get => _trabajadores;
            set
            {
                _trabajadores = value;
                OnPropertyChanged(nameof(Trabajadores));
            }
        }

        public ObservableCollection<string> Categorias
        {
            get => _categorias;
            set
            {
                _categorias = value;
                OnPropertyChanged(nameof(Categorias));
            }
        }

        public string CategoriaSeleccionada
        {
            get => _categoriaSeleccionada;
            set
            {
                if (_categoriaSeleccionada != value)
                {
                    _categoriaSeleccionada = value;
                    OnPropertyChanged(nameof(CategoriaSeleccionada));
                    FiltrarTrabajadores();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    FiltrarTrabajadores();
                }
            }
        }

        // NUEVA PROPIEDAD PARA FILTRO DE ACTIVOS
        public bool MostrarSoloActivos
        {
            get => _mostrarSoloActivos;
            set
            {
                if (_mostrarSoloActivos != value)
                {
                    _mostrarSoloActivos = value;
                    OnPropertyChanged(nameof(MostrarSoloActivos));

                    // ✅ CAMBIO PRINCIPAL: Recargar desde BD igual que VMUsuario
                    CargarTodosLosTrabajadoresAsync();
                }
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        // Constructor
        public VMTrabajadores()
        {
            _ = CargarTodosLosTrabajadoresAsync();
        }

        public async Task CargarTodosLosTrabajadoresAsync()
        {
            try
            {
                IsBusy = true;

                // ✅ NUEVO: Pasar el filtro de estado igual que VMUsuario
                // Si MostrarSoloActivos = true, solo traer activos
                // Si MostrarSoloActivos = false, traer todos
                var todosTrabajadores = await App.Database.ObtenerTrabajadoresAsync(
                    categoria: null,
                    incluirInactivos: !MostrarSoloActivos  // Si MostrarSoloActivos = false, incluir inactivos
                );

                _todosTrabajadores.Clear();
                foreach (var trabajador in todosTrabajadores)
                {
                    _todosTrabajadores.Add(trabajador);
                }

                CargarCategoriasDesdeBaseDatos();
                FiltrarTrabajadores(); // Solo para buscar por texto y categoría

                System.Diagnostics.Debug.WriteLine($"✅ Trabajadores cargados: {todosTrabajadores.Count}");
                System.Diagnostics.Debug.WriteLine($"✅ MostrarSoloActivos: {MostrarSoloActivos}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar trabajadores: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CargarCategoriasDesdeBaseDatos()
        {
            try
            {
                Categorias.Clear();
                Categorias.Add("Todos");

                // Obtener categorías únicas de los trabajadores cargados
                var categoriasUnicas = _todosTrabajadores
                    .Where(t => !string.IsNullOrWhiteSpace(t.categoria))
                    .Select(t => t.categoria.Trim())
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                foreach (var categoria in categoriasUnicas)
                {
                    Categorias.Add(categoria);
                }

                // Si no hay categorías, agregar las por defecto
                if (Categorias.Count == 1) // Solo "Todos"
                {
                    Categorias.Add("Administrador");
                    Categorias.Add("Transportista");
                    Categorias.Add("Ayudante");
                }

                CategoriaSeleccionada = "Todos";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar categorías: {ex.Message}");
            }
        }

        private void FiltrarTrabajadores()
        {
            try
            {
                var trabajadoresFiltrados = _todosTrabajadores.AsEnumerable();

                System.Diagnostics.Debug.WriteLine($"🟢 === FILTRANDO ===");
                System.Diagnostics.Debug.WriteLine($"🟢 Total trabajadores: {_todosTrabajadores.Count}");

               
                // Filtrar por categoría
                if (!string.IsNullOrEmpty(CategoriaSeleccionada) && CategoriaSeleccionada != "Todos")
                {
                    trabajadoresFiltrados = trabajadoresFiltrados.Where(t =>
                        string.Equals(t.categoria?.Trim(), CategoriaSeleccionada.Trim(), StringComparison.OrdinalIgnoreCase)
                    );
                }

                // Filtrar por texto de búsqueda
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    trabajadoresFiltrados = trabajadoresFiltrados.Where(t =>
                        (!string.IsNullOrEmpty(t.NombreTrabajador) && t.NombreTrabajador.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(t.numDoc) && t.numDoc.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(t.Telefono) && t.Telefono.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(t.categoria) && t.categoria.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    );
                }

                var resultados = trabajadoresFiltrados.OrderBy(t => t.NombreTrabajador).ToList();

                System.Diagnostics.Debug.WriteLine($"Resultados finales: {resultados.Count}");

                Trabajadores.Clear();
                foreach (var trabajador in resultados)
                {
                    Trabajadores.Add(trabajador);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al filtrar trabajadores: {ex.Message}");
            }
        }
        public async Task ActualizarDatos()
        {
            await CargarTodosLosTrabajadoresAsync();
        }

        // Método para recargar desde el code-behind
        public async Task CargarDatosAsync()
        {
            await CargarTodosLosTrabajadoresAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}