using System;
using System.IO;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    /// <summary>
    /// A helper class to manage Google Cloud credential files within a MAUI application.
    /// <para>
    /// This class copies a bundled credential file from the application package
    /// to the application's data directory to make it accessible at runtime.
    /// </para>
    /// <para>
    /// SECURITY NOTE: Bundling a service account key file within a client application is
    /// a significant security risk. The key can be extracted from the app package,
    /// potentially giving attackers broad access to your Google Cloud resources.
    /// A more secure approach is to have the client application communicate with a
    /// backend API that handles authentication with Google Cloud services.
    /// </para>
    /// </summary>
    public class GoogleCloudAuthHelper
    {
        private static string _localCredentialPath;

        /// <summary>
        /// Asynchronously gets the local file path for the Google Cloud credential file.
        /// If the file doesn't exist in the app's data directory, it is copied from the app package.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the local file path to the credential file if successful; otherwise, null.
        /// </returns>
        public static async Task<string> GetCredentialFilePathAsync()
        {
            // Return the cached path if it's already been determined.
            if (!string.IsNullOrEmpty(_localCredentialPath))
                return _localCredentialPath;

            // Define the name of the credential file.
            string credentialFileName = "projecto-rocketbot-71a45741c162.json";
            // Define the target path in the app's local data directory.
            string targetPath = Path.Combine(FileSystem.AppDataDirectory, credentialFileName);

            // If the file doesn't exist at the target path, copy it from the app package.
            if (!File.Exists(targetPath))
            {
                try
                {
                    // Open the bundled credential file as a stream.
                    using var stream = await FileSystem.OpenAppPackageFileAsync(credentialFileName);
                    // Create a new file at the target path.
                    using var fileStream = File.Create(targetPath);
                    // Copy the content from the bundled file to the new file.
                    await stream.CopyToAsync(fileStream);
                }
                catch (Exception ex)
                {
                    // Log the error if the file cannot be copied.
                    Console.WriteLine($"Error copiando archivo JSON: {ex.Message}");
                    return null;
                }
            }

            // Cache and return the local path.
            _localCredentialPath = targetPath;
            return targetPath;
        }
    }
}