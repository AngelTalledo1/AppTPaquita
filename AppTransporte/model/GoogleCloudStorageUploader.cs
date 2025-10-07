using System;
using System.IO;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;

namespace AppTransporte.model
{
    /// <summary>
    /// Handles file uploads to Google Cloud Storage.
    /// <para>
    /// IMPORTANT: This class contains a significant security vulnerability.
    /// The path to the service account credentials JSON file is hardcoded,
    /// which exposes sensitive information directly in the source code.
    /// In a production environment, credentials should be managed securely,
    /// for example, through environment variables on the server, a secrets management service,
    /// or workload identity federation.
    /// </para>
    /// </summary>
    public class GoogleCloudStorageUploader
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName = "pqt_bucket"; // Bucket name should not be a URL.

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleCloudStorageUploader"/> class.
        /// <para>
        /// SECURITY WARNING: This constructor hardcodes the path to the Google Cloud credentials file.
        /// This is a major security risk and should be replaced with a secure method for providing credentials.
        /// </para>
        /// </summary>
        public GoogleCloudStorageUploader()
        {
            // SECURITY RISK: Hardcoded credential path.
            // This path is specific to a developer's machine and exposes the key file location.
            // This should be replaced with a secure credential management strategy.
            string credentialPath = "C:/Users/gudal/Downloads/projecto-rocketbot-71a45741c162.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

            // Create the Google Cloud Storage client.
            // This will automatically use the credentials set in the environment variable.
            _storageClient = StorageClient.Create();
        }

        /// <summary>
        /// Asynchronously uploads a file stream to a specified folder within the Google Cloud Storage bucket.
        /// </summary>
        /// <param name="fileStream">The stream of the file to upload.</param>
        /// <param name="fileName">The desired name for the file in the bucket.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the public URL of the uploaded file if successful; otherwise, null.
        /// </returns>
        /// <remarks>
        /// The file is uploaded to an 'uploads' folder within the bucket.
        /// Catches and prints exceptions to the console, returning null on failure.
        /// </remarks>
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            try
            {
                var obj = await _storageClient.UploadObjectAsync(
                    _bucketName,
                    $"uploads/{fileName}", // Specifies the folder and file name in the bucket.
                    null, // Content type is not specified, allowing Google Cloud to infer it.
                    fileStream);

                // Construct the public URL for the uploaded object.
                string fileUrl = $"https://storage.googleapis.com/{_bucketName}/{obj.Name}";
                Console.WriteLine($"Archivo subido: {fileUrl}");

                return fileUrl;
            }
            catch (Exception ex)
            {
                // In a real application, use a structured logging framework.
                Console.WriteLine($"Error al subir el archivo: {ex.Message}");
                return null;
            }
        }
    }
}