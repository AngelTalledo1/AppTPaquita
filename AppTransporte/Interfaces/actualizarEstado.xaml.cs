using AppTransporte.model;
using AppTransporte.viewModel;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;

namespace AppTransporte.Interfaces;

public partial class actualizarEstado : ContentPage
{
    private StorageClient _storageClient;
    private bool _isInitialized = false;
    private readonly string _bucketName = "pqt_bucket";
    private int _idUsuario;
    private int _idTipoUsuario;
    private bool isUploading = false; // Para prevenir múltiples ejecuciones
    public actualizarEstado(Viaje viaje, int idUsuario, int idTipoUsuario)
	{
		InitializeComponent();
        InitializeStorageAsync();
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;

    }


    private async Task InitializeStorageAsync()
    {
        try
        {
            string credentialPath = await GoogleCloudAuthHelper.GetCredentialFilePathAsync();

            var credential = GoogleCredential.FromFile(credentialPath);
            _storageClient = StorageClient.Create(credential);

            _isInitialized = true;
        }
        catch (Exception ex)
        {
            // Manejar errores de inicialización
            Console.WriteLine($"Error inicializando Storage: {ex.Message}");
            _isInitialized = false;
        }


    }

    private async void OnCaptureAndUploadPhotoClicked(object sender, EventArgs e)
    {
        if (isUploading) return;

        try
        {
            isUploading = true;
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            // Verificar permiso de cámara
            var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (cameraStatus != PermissionStatus.Granted)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                    UploadStatus.Text = "Se requiere permiso de la cámara");
                return;
            }

            // Capturar foto
            var photo = await MediaPicker.CapturePhotoAsync();
            if (photo == null)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    UploadStatus.Text = "No se tomó ninguna foto.");
                Console.WriteLine("no se tomo foto");
                return;
            }

            // Leer y almacenar en memoria
            byte[] imageData;
            using (var sourceStream = await photo.OpenReadAsync())
            {
                using var memoryStream = new MemoryStream();
                await sourceStream.CopyToAsync(memoryStream);
                imageData = memoryStream.ToArray();

                // Eliminar archivo temporal
                try { File.Delete(photo.FullPath); }
                catch (Exception ex) { Console.WriteLine($"Error eliminando archivo: {ex.Message}"); }
            }

            // Mostrar imagen en UI
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                CapturedImage.Source = ImageSource.FromStream(() => new MemoryStream(imageData));
            });

            // Subir a Google Cloud
            string fileName = $"photo_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
            string fileUrl = "";

            using (var uploadStream = new MemoryStream(imageData))
            {
                fileUrl = await UploadFileAsync(uploadStream, fileName);
                Console.WriteLine($"Foto subida: {fileUrl}");
            }

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                UploadStatus.Text = !string.IsNullOrEmpty(fileUrl)
                    ? $"Foto subida con éxito: {fileUrl}"
                    : "Error al subir la foto.";
            });
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
                UploadStatus.Text = $"Error: {ex.Message}");
        }
        finally
        {
            isUploading = false;
        }
    }
    private async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        try
        {
            var obj = await _storageClient.UploadObjectAsync(
                _bucketName,
                $"uploads/{fileName}",
                null,
                fileStream);

            string fileUrl = $"https://storage.googleapis.com/{_bucketName}/{obj.Name}";
            return fileUrl;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al subir el archivo: {ex.Message}");
            return null;
        }
    }

    private void Btn_atrasEvidencias(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VTMisViajes(_idTipoUsuario, _idUsuario));
    }

    
}