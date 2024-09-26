using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte
{
    internal class Conexion
    {
        private string connectionString = "Server=tcp:apptransporte.database.windows.net,1433;Initial Catalog=dbpaquita;Persist Security Info=False;User ID=userpaquita;Password=appPaquita123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


        public async Task<bool> VerificarCredenciales(string usuario, string contraseña)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("pa_VerificarCredenciales", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Parámetros del procedimiento almacenado
                    cmd.Parameters.AddWithValue("@Usuario", usuario);
                    cmd.Parameters.AddWithValue("@Contraseña", contraseña);

                    // Ejecutar el procedimiento almacenado y obtener el resultado
                    int count = (int)await cmd.ExecuteScalarAsync();

                    // Si el resultado es 1, las credenciales son correctas
                    return count == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar las credenciales: {ex.Message}");
                return false;
            }
        }

        public async Task ConectarBaseDeDatos()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Conexión establecida correctamente");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al conectar con la base de datos: {ex.Message}");
            }
        }

    }
}
