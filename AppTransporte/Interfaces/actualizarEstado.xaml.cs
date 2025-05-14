using AppTransporte.model;
using Microsoft.Maui.Controls;
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using System.IO;
using SkiaSharp;
using System.Threading.Tasks;

namespace AppTransporte.Interfaces;
#pragma warning disable CS8603, CS1998, CS4014, CS8618, CS0414

public partial class actualizarEstado : ContentPage
{
    private StorageClient _storageClient;
    private bool _isInitialized = false;
    private readonly string bucketName = "pqt_bucket";
    private int _idUsuario;
    private int _idTipoUsuario;
    private bool isUploading = false; // Para prevenir múltiples ejecuciones
    private Viaje _viaje;
    private string? _fileUrl = null;  // Almacena la URL de la imagen subida
    private Dictionary<int, string> _estadosViaje = new Dictionary<int, string>();

    // Variable para almacenar la imagen comprimida (no se sube hasta Guardar)
    private byte[]? _imageData = null;

    public actualizarEstado(Viaje viaje, int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        _viaje = viaje;
        _idUsuario = idUsuario;
        _idTipoUsuario = idTipoUsuario;
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        // Mostrar indicador de carga
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        // Inicializar almacenamiento
        await InitializeStorageAsync();

        // En este punto se pueden cargar otros datos si se desea

        // Ocultar indicador de carga
        LoadingIndicator.IsVisible = false;
        LoadingIndicator.IsRunning = false;
    }

    // Declarar el caché estático para URLs firmadas
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

    // Captura y comprime la imagen, la muestra en la UI y la guarda en _imageData
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

    // El método de guardar ahora sube la imagen (si fue capturada) y luego actualiza el estado
    private async void GuardarButton_Clicked(object sender, EventArgs e)
    {
        GuardarButton.IsEnabled = false;
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        try
        {
            string comentario = descripcionEntry.Text;
            // Si se capturó una imagen, se sube en este momento
            if (_imageData != null)
            {
                string fileName = $"photo_viaje{_viaje.IdViaje}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                using (var uploadStream = new MemoryStream(_imageData))
                {
                    _fileUrl = await UploadFileAsync(uploadStream, fileName);
                    Console.WriteLine($"Foto subida: {_fileUrl}");
                }
            }

            // Actualizar el seguimiento en la base de datos con la URL (si existe)
            int result = await App.Database.ActualizarEstadoSeguimientoAsync(
                _viaje.IdViaje,
                comentario,
                _fileUrl
            );

            if (result == 1)
            {
                await DisplayAlert("Éxito", "El estado del viaje ha sido actualizado correctamente", "OK");
                await Navigation.PopAsync();
            }
            else if (result == -2)
            {
                await DisplayAlert("Aviso", "Este viaje ya se encuentra en su estado final", "OK");
            }
            else if (result == -1)
            {
                await DisplayAlert("Error", "El viaje no existe", "OK");
            }
            else
            {
                await DisplayAlert("Error", "No se pudo actualizar el estado del viaje", "OK");
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
