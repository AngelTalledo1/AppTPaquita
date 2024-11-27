using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using AppTransporte.model;

using Microsoft.Data.SqlClient;

namespace AppTransporte
{
    public class Conexion
    {
        public string connectionString { get; } = "Server=192.168.0.26\\SQLEXPRESS;Database=db_TransporteModific;Integrated Security=True;";

        public async Task ProbarConexionAsync()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Conexión establecida correctamente.");
                }
            }
            catch (Exception ex)
            {
                // Registra el error o utiliza un logger
                Console.WriteLine($"Error al conectar con la base de datos: {ex.Message}");
            }
        }


        /*public async Task<bool> VerificarCredenciales(string categoria, string usuario, string contraseña)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Definir la consulta SQL
                    string query = @"
                SELECT CASE 
                    WHEN EXISTS (
                        SELECT 1
                        FROM Usuario
                        WHERE Categoria = @Categoria
                          AND Username = @Usuario
                          AND contraseña = @Contraseña
                          AND estado = 1  -- Verifica que el usuario esté activo
                    ) THEN 1
                    ELSE 0
                END";

                    // Crear el comando con la consulta y los parámetros
                    SqlCommand cmd = new SqlCommand(query, connection);

                    // Parámetros de la consulta
                    cmd.Parameters.AddWithValue("@Categoria", categoria);
                    cmd.Parameters.AddWithValue("@Usuario", usuario);
                    cmd.Parameters.AddWithValue("@Contraseña", contraseña);

                    // Ejecutar la consulta y obtener el resultado
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
        }*/

        public async Task<List<Cliente>> ObtenerClientesAsync(string nombre = null)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Llama al procedimiento almacenado
                    var clientes = await connection.QueryAsync<Cliente, Persona, Cliente>(
                        "pa_MostrarClientes",
                        (cliente, persona) =>
                        {
                            cliente.Persona = persona;
                            return cliente;
                        },
                        splitOn: "id_persona", // Define dónde se separan los datos entre Cliente y Persona
                        param: new { nombre },
                        commandType: System.Data.CommandType.StoredProcedure
                    );

                    return clientes.AsList();
                }
            }
            catch (Exception ex)
            {
                // Maneja o registra el error
                Console.WriteLine($"Error al obtener clientes: {ex.Message}");
                return new List<Cliente>(); // Retorna una lista vacía en caso de error
            }
        }

    }
}
