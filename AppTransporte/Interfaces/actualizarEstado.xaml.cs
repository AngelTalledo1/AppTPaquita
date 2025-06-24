using AppTransporte.model;
using Microsoft.Maui.Controls;
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using System.IO;
using SkiaSharp;
using System.Threading.Tasks;
using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;
#pragma warning disable CS8603, CS1998, CS4014, CS8618, CS0414

public partial class actualizarEstado : ContentPage
{
    private readonly ActualizarEstadoViajeViewModel _viewModel;
    private StorageClient _storageClient;
    private bool _isInitialized = false;
    private readonly string bucketName = "pqt_bucket";
    private int _idUsuario;
    private int _idTipoUsuario;
    private bool isUploading = false;
    private Viaje _viaje;
    private string? _fileUrl = null;
    private Dictionary<int, string> _estadosViaje = new Dictionary<int, string>();

    // Variable para almacenar la imagen comprimida
    private byte[]? _imageData = null;

    public actualizarEstado(Viaje viaje, int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        _viaje = viaje;
        _idUsuario = idUsuario;
        _idTipoUsuario = idTipoUsuario;
        _viewModel = new ActualizarEstadoViajeViewModel(new ViajeInfo(), 0);
        InitializeAsync();
        CargarInformacionViajeAsync(viaje.IdViaje);
    }

    private async void CargarInformacionViajeAsync(int idViaje)
    {
        try
        {
            ShowLoading();

            var viajeInfo = await App.Database.ObtenerInfoViajeAsync(idViaje);
            var Trabajador = await App.Database.ObtenerTrabajadorPorUsuarioAsync(_idUsuario);

            // Actualizar el ViewModel
            _viewModel.ViajeInfo = viajeInfo;
            _viewModel.IdTrabajador = Trabajador.IdTrabajador;

            // Cargar estados disponibles
            _viewModel.CargarEstadosDisponiblesAsync();

            BindingContext = _viewModel;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo cargar la información del viaje: {ex.Message}", "OK");
        }
        finally
        {
            HideLoading();
        }
    }

    private async void InitializeAsync()
    {
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        await InitializeStorageAsync();
        LoadingIndicator.IsVisible = false;
        LoadingIndicator.IsRunning = false;
    }

    private static Dictionary<string, (string Url, DateTime Expiration)> _signedUrlCache = new();

    private void ShowLoading()
    {
        LoadingOverlay.IsVisible = true;
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
    }

    private void HideLoading()
    {
        LoadingOverlay.IsVisible = false;
        LoadingIndicator.IsVisible = false;
        LoadingIndicator.IsRunning = false;
    }

    public async Task<string> GetSignedUrlForImageAsync(string unsignedUrl)
    {
        if (string.IsNullOrWhiteSpace(unsignedUrl))
            return unsignedUrl;

        if (_signedUrlCache.TryGetValue(unsignedUrl, out var cacheEntry) && DateTime.UtcNow < cacheEntry.Expiration)
        {
            return cacheEntry.Url;
        }

        string prefix = $"https://storage.googleapis.com/{bucketName}/";
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
        string signedUrl = urlSigner.Sign(bucketName, objectName, TimeSpan.FromMinutes(30), System.Net.Http.HttpMethod.Get);
        return signedUrl;
    }

    private byte[] CompressImage(byte[] imageData, int quality = 50, int maxWidth = 1024)
    {
        using var inputStream = new MemoryStream(imageData);
        using var original = SKBitmap.Decode(inputStream);
        if (original == null)
            return imageData;

        int width = original.Width;
        int height = original.Height;
        if (width > maxWidth)
        {
            float ratio = (float)maxWidth / width;
            width = maxWidth;
            height = (int)(height * ratio);
        }
        using var resized = original.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium);
        if (resized == null)
            return imageData;
        using var image = SKImage.FromBitmap(resized);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, quality);
        return data.ToArray();
    }

    private async Task InitializeStorageAsync()
    {
        try
        {
            string credentialPath = await GoogleCloudAuthHelper.GetCredentialFilePathAsync();
            var credential = GoogleCredential.FromFile(credentialPath);
            _storageClient = StorageClient.Create(credential);
            _isInitialized = true;
            UpdateStatusMessage("Listo para capturar evidencia fotográfica", "#2e7d32");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inicializando Storage: {ex.Message}");
            UpdateStatusMessage($"Error al inicializar almacenamiento: {ex.Message}", "#c62828");
            _isInitialized = false;
        }
    }

    // En actualizarEstado.xaml.cs - método OnCaptureAndUploadPhotoClicked
    private async void OnCaptureAndUploadPhotoClicked(object sender, EventArgs e)
    {
        if (isUploading) return;

        try
        {
            if (!_isInitialized)
            {
                UpdateStatusMessage("El almacenamiento no está inicializado. Inténtalo de nuevo.", "#c62828");
                return;
            }

            isUploading = true;
            UpdateStatusMessage("Solicitando permisos de cámara...", "#1565c0");

            var status = await Permissions.RequestAsync<Permissions.Camera>();
            var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (cameraStatus != PermissionStatus.Granted)
            {
                UpdateStatusMessage("Se requiere permiso de la cámara", "#c62828");
                isUploading = false;
                return;
            }

            UpdateStatusMessage("Abriendo cámara...", "#1565c0");
            var photo = await MediaPicker.CapturePhotoAsync();
            if (photo == null)
            {
                UpdateStatusMessage("No se tomó ninguna foto", "#f9a825");
                Console.WriteLine("No se tomó foto");
                isUploading = false;
                return;
            }

            UpdateStatusMessage("Procesando imagen...", "#1565c0");

            using (var sourceStream = await photo.OpenReadAsync())
            {
                using var memoryStream = new MemoryStream();
                await sourceStream.CopyToAsync(memoryStream);
                _imageData = CompressImage(memoryStream.ToArray());

                // Mostrar la imagen en la interfaz sin convertirla a base64
                _viewModel.FotoEvidencia = ImageSource.FromStream(() => new MemoryStream(_imageData));

                // No asignamos Base64 al ViewModel, ya que el backend sólo acepta URLs
                _viewModel.FotoBytes = null;

                try { File.Delete(photo.FullPath); }
                catch (Exception ex) { Console.WriteLine($"Error eliminando archivo: {ex.Message}"); }
            }

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                CapturedImage.Source = ImageSource.FromStream(() => new MemoryStream(_imageData));
                ImageContainer.IsVisible = true;
                UpdateStatusMessage("Imagen lista para guardar", "#2e7d32");
            });
        }
        catch (Exception ex)
        {
            UpdateStatusMessage($"Error: {ex.Message}", "#c62828");
            Console.WriteLine($"Error en la captura de foto: {ex}");
        }
        finally
        {
            isUploading = false;
        }
    }


    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    // **MÉTODO COMPLETAMENTE REESCRITO PARA USAR EL VIEWMODEL**
    // En actualizarEstado.xaml.cs - método GuardarButton_Clicked
    private async void GuardarButton_Clicked(object sender, EventArgs e)
    {
        GuardarButton.IsEnabled = false;
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        try
        {
            // Obtener el comentario del Entry y asignarlo al ViewModel
            _viewModel.Comentario = descripcionEntry.Text;

            // Siempre subir la imagen primero si existe, para obtener una URL
            if (_imageData != null)
            {
                UpdateStatusMessage("Subiendo imagen...", "#1565c0");
                string fileName = $"photo_viaje{_viaje.IdViaje}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                using (var uploadStream = new MemoryStream(_imageData))
                {
                    _fileUrl = await UploadFileAsync(uploadStream, fileName);
                    Console.WriteLine($"Foto subida: {_fileUrl}");

                    // Asignar la URL al ViewModel, nunca datos Base64
                    _viewModel.FotoBytes = _fileUrl;
                }
            }

            // Proceder con la operación usando la URL de la imagen (no datos Base64)
            bool resultado = false;

            if (_viewModel.EstadoSeleccionado != null)
            {
                // Si hay un estado seleccionado, usar la lógica del ViewModel
                resultado = await _viewModel.GuardarActualizacion();
            }
            else
            {
                // Si no hay estado seleccionado, solo actualizar seguimiento
                int result = await App.Database.ActualizarSeguimientoViajeAsync(
                    _viaje.IdViaje,
                    _viewModel.Comentario,
                    _fileUrl
                );
                resultado = result > 0;
                _viewModel.ResultadoOperacion = resultado
                    ? "Seguimiento actualizado correctamente"
                    : "No se pudo actualizar el seguimiento";
                _viewModel.OperacionExitosa = resultado;
            }

            // Mostrar el resultado
            if (_viewModel.OperacionExitosa)
            {
                await DisplayAlert("Éxito", _viewModel.ResultadoOperacion, "OK");
                await Navigation.PushAsync(new VTMisViajes(_idUsuario,_idTipoUsuario));
            }
            else
            {
                await DisplayAlert("Error", _viewModel.ResultadoOperacion, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al guardar: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            GuardarButton.IsEnabled = true;
        }
    }

    private async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        if (!_isInitialized)
        {
            Console.WriteLine("Storage client no está inicializado");
            return null;
        }

        try
        {
            var obj = await _storageClient.UploadObjectAsync(
                bucketName,
                $"uploads/{fileName}",
                null,
                fileStream);

            string fileUrl = $"https://storage.googleapis.com/{bucketName}/{obj.Name}";
            return fileUrl;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al subir el archivo: {ex.Message}");
            return null;
        }
    }

    private async void UpdateStatusMessage(string message, string colorHex)
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            UploadStatus.Text = message;
            UploadStatus.TextColor = Color.FromArgb(colorHex);
            StatusContainer.IsVisible = true;
        });
    }

    private async void Btn_atrasEvidencias(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
#pragma warning restore CS8603, CS1998, CS4014, CS8618