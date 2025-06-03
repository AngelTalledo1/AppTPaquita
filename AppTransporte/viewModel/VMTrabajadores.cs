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

                // Cargar todos los trabajadores sin filtro
                var todosTrabajadores = await App.Database.ObtenerTrabajadoresAsync();

                _todosTrabajadores.Clear();
                foreach (var trabajador in todosTrabajadores)
                {
                    _todosTrabajadores.Add(trabajador);
                }

                // Cargar categorías dinámicamente desde los datos
                CargarCategoriasDesdeBaseDatos();

                FiltrarTrabajadores();

                // DEBUG: Mostrar qué categorías encontramos
                System.Diagnostics.Debug.WriteLine("=== CATEGORÍAS ENCONTRADAS ===");
                foreach (var cat in Categorias)
                {
                    System.Diagnostics.Debug.WriteLine($"Categoría: {cat}");
                }

                // DEBUG: Mostrar algunos trabajadores y sus categorías
                System.Diagnostics.Debug.WriteLine("=== TRABAJADORES Y SUS CATEGORÍAS ===");
                foreach (var trabajador in _todosTrabajadores.Take(5))
                {
                    System.Diagnostics.Debug.WriteLine($"Trabajador: {trabajador.NombreTrabajador} - Categoría: '{trabajador.categoria}'");
                }
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

                // DEBUG: Mostrar qué estamos filtrando
                System.Diagnostics.Debug.WriteLine($"=== FILTRANDO ===");
                System.Diagnostics.Debug.WriteLine($"Categoría seleccionada: '{CategoriaSeleccionada}'");
                System.Diagnostics.Debug.WriteLine($"Total trabajadores antes del filtro: {_todosTrabajadores.Count}");

                // Filtrar por categoría
                if (!string.IsNullOrEmpty(CategoriaSeleccionada) && CategoriaSeleccionada != "Todos")
                {
                    trabajadoresFiltrados = trabajadoresFiltrados.Where(t =>
                    {
                        bool coincide = string.Equals(t.categoria?.Trim(), CategoriaSeleccionada.Trim(), StringComparison.OrdinalIgnoreCase);

                        // DEBUG: Mostrar cada comparación
                        if (!coincide)
                        {
                            System.Diagnostics.Debug.WriteLine($"No coincide - Trabajador: {t.NombreTrabajador}, Su categoría: '{t.categoria}' vs Filtro: '{CategoriaSeleccionada}'");
                        }

                        return coincide;
                    });
                }

                // Filtrar por texto de búsqueda
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    trabajadoresFiltrados = trabajadoresFiltrados.Where(t =>
                        (!string.IsNullOrEmpty(t.NombreTrabajador) && t.NombreTrabajador.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(t.numDoc) && t.numDoc.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(t.Telefono) && t.Telefono.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(t.categoria) && t.categoria.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
                }

                // Ordenar alfabéticamente
                trabajadoresFiltrados = trabajadoresFiltrados.OrderBy(t => t.NombreTrabajador);

                var resultados = trabajadoresFiltrados.ToList();

                // DEBUG: Mostrar resultados
                System.Diagnostics.Debug.WriteLine($"Trabajadores después del filtro: {resultados.Count}");
                foreach (var trabajador in resultados.Take(3))
                {
                    System.Diagnostics.Debug.WriteLine($"- {trabajador.NombreTrabajador} ({trabajador.categoria})");
                }

                // Actualizar la colección
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