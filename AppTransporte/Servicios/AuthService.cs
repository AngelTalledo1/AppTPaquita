using AppTransporte.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppTransporte.Servicios
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://emmanuel8a-001-site1.ktempurl.com") };
        }

        public async Task<UsuarioResponse> VerificarCredencialesAsync(string username, string contraseña)
        {
            var login = new UsuarioLogin
            {
                Username = username,
                Contraseña = contraseña
            };
            var jsonContent = new StringContent(JsonSerializer.Serialize(login), Encoding.UTF8, "application/json");
            string credentials = "11201737:60-dayfreetrial";
            var encodedCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);
        try
            {
                // Realizamos la solicitud POST
                var response = await _httpClient.PostAsync("api/verificar-credenciales", jsonContent);
                Console.WriteLine("que");
                if (response.IsSuccessStatusCode)
                {
                    // Deserializamos el resultado de la respuesta, por ejemplo si el API retorna un objeto de usuario
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var usuarioResponse = JsonSerializer.Deserialize<UsuarioResponse>(responseBody);

                    return usuarioResponse; //
                }
                else
                {
                    // Si la respuesta no es satisfactoria (por ejemplo 404 o 401)
                    Console.WriteLine("Error: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al consumir la API: {ex.Message}");
            }

            return null;
        }
        public async Task<string> ObtenerTipoUsuarioAsync(int id)
        {
            try
            {
                // Definir la URL base de tu API (ajústala a la dirección de tu servidor)
                var apiUrl = $"http://emmanuel8a-001-site1.ktempurl.com/api/obtener-tipo-user/{id}";

                // Realizar la solicitud GET
                var response = await _httpClient.GetAsync(apiUrl);

                // Verificar si la respuesta es exitosa
                if (response.IsSuccessStatusCode)
                {

                    // Procesar la respuesta (deserialización en este caso)
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Deserializar la respuesta como un array de strings
                    var tipoUsuarioArray = JsonSerializer.Deserialize<string[]>(responseContent);

                    // Retornar solo el primer elemento del array (si existe)
                    return tipoUsuarioArray?.FirstOrDefault() ?? "Tipo de usuario no encontrado";
                }
                else
                {
                    return $"Error: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                return $"Excepción: {ex.Message}";
            }
        }
    }
}
