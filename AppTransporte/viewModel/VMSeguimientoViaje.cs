using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AppTransporte.Interfaces;
using AppTransporte.model;
using Google.Cloud.Storage.V1;


namespace AppTransporte.viewModel
{
    public class VMSeguimientoViaje : INotifyPropertyChanged
    {


        private bool _isBusy;
        public ObservableCollection<Seguimiento> seguimiento { get; set; } = new();
        public ObservableCollection<Seguimiento> seguimientoFiltrados { get; set; } = new();
        private int? _idViajeSeleccionado;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public int? IdViajeSeleccionado
        {
            get => _idViajeSeleccionado;
            set
            {
                if (_idViajeSeleccionado != value)
                {
                    _idViajeSeleccionado = value;
                    OnPropertyChanged(nameof(IdViajeSeleccionado));
                    FiltrarViajes(); // Filtra los viajes automáticamente al cambiar el IdPedido
                }
            }
        }
        public VMSeguimientoViaje()
        {
            InicializarViajes();
        }
        public VMSeguimientoViaje(int idViaje)
        {
            this.IdViajeSeleccionado = idViaje;
            InicializarViajes(); // Carga los viajes y aplica el filtro automáticamente
        }


        private async void InicializarViajes()
        {
            IsBusy = true;

            try
            {
                var seguimientoBD = await App.Database.ObtenerEstadosViaje();

                this.seguimiento.Clear();

                // Validar que MainPage no sea null y que sea del tipo correcto
               
                    foreach (var segui in seguimientoBD)
                    {
                        // Validar que la URL no sea null antes de pasarla al método
                        if (!string.IsNullOrEmpty(segui.Evidencia))
                        {
                            segui.Evidencia = await GetSignedUrlForImageAsync(segui.Evidencia);
                        }
                        this.seguimiento.Add(segui);
                    }
                
                

                FiltrarViajes(); // Aplica el filtro después de cargar los datos
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar los estados: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private static Dictionary<string, (string Url, DateTime Expiration)> _signedUrlCache = new();

        // Este método recibe la URL no firmada (almacenada en la BD)
        // y devuelve, de forma asíncrona, la URL firmada usando Google Cloud Storage.
        // Se utiliza un caché de 30 minutos para evitar regenerarla si ya existe.
        public async Task<string> GetSignedUrlForImageAsync(string unsignedUrl)
        {
            if (string.IsNullOrWhiteSpace(unsignedUrl))
                return unsignedUrl;

            if (_signedUrlCache.TryGetValue(unsignedUrl, out var cacheEntry) && DateTime.UtcNow < cacheEntry.Expiration)
            {
                return cacheEntry.Url;
            }

            string prefix = $"https://storage.googleapis.com/pqt_bucket/";
            string objectName = unsignedUrl.StartsWith(prefix)
                ? unsignedUrl.Substring(prefix.Length)
                : unsignedUrl;

            string signedUrl = await GenerateSignedUrlAsync(objectName);
            _signedUrlCache[unsignedUrl] = (signedUrl, DateTime.UtcNow.AddMinutes(30));
            return signedUrl;
        }

        private async Task<string> GenerateSignedUrlAsync(string objectName)
        {
            string credentialPath = await GoogleCloudAuthHelper.GetCredentialFilePathAsync();
            var urlSigner = UrlSigner.FromServiceAccountPath(credentialPath);
            string signedUrl = urlSigner.Sign("pqt_bucket", objectName, TimeSpan.FromMinutes(30), System.Net.Http.HttpMethod.Get);
            return signedUrl;
        }
        private void FiltrarViajes()
        {
            if (seguimiento == null || !seguimiento.Any())
            {
                seguimientoFiltrados.Clear();
                return;
            }

            var seguimientoFiltradosTemp = seguimiento.AsEnumerable();

            if (IdViajeSeleccionado.HasValue)
            {
                seguimientoFiltradosTemp = seguimientoFiltradosTemp.Where(v => v.IdViaje == IdViajeSeleccionado.Value);
            }

            seguimientoFiltrados.Clear();

            foreach (var segui in seguimientoFiltradosTemp)
            {
                seguimientoFiltrados.Add(segui);
            }
        }

        

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
