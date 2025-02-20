using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class GoogleCloudAuthHelper
    {
        private static string _localCredentialPath;

        public static async Task<string> GetCredentialFilePathAsync()
        {
            if (!string.IsNullOrEmpty(_localCredentialPath))
                return _localCredentialPath;

            // Ruta de destino en almacenamiento interno
            string targetPath = Path.Combine(FileSystem.AppDataDirectory, "projecto-rocketbot-71a45741c162.json");

            if (!File.Exists(targetPath))
            {
                try
                {
                    using var stream = await FileSystem.OpenAppPackageFileAsync("projecto-rocketbot-71a45741c162.json");
                    using var fileStream = File.Create(targetPath);
                    await stream.CopyToAsync(fileStream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error copiando archivo JSON: {ex.Message}");
                    return null;
                }
            }

            _localCredentialPath = targetPath;
            return targetPath;
        }
    }
}