using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;  
namespace AppTransporte.model
{
    public class GoogleCloudStorageUploader
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName = "https://console.cloud.google.com/storage/browser/pqt_bucket"; // Reemplázalo con tu bucket

        public GoogleCloudStorageUploader()
        {
            // Ruta del archivo JSON con las credenciales
            string credentialPath = "C:/Users/gudal/Downloads/projecto-rocketbot-71a45741c162.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

            // Crear el cliente de Google Cloud Storage
            _storageClient = StorageClient.Create();
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            try
            {
                var obj = await _storageClient.UploadObjectAsync(
                    _bucketName,
                    $"uploads/{fileName}", // Carpeta en el bucket
                    null,
                    fileStream);

                string fileUrl = $"https://storage.googleapis.com/{_bucketName}/{obj.Name}";
                Console.WriteLine($"Archivo subido: {fileUrl}");

                return fileUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir el archivo: {ex.Message}");
                return null;
            }
        }
    }
}
