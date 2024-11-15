﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace AppTransporte
{
    public class Conexion
    {
        private string connectionString = "Data Source=SQL5111.site4now.net;Initial Catalog=db_aaecc9_paquitaappdb;User Id=db_aaecc9_paquitaappdb_admin;Password=paquita123";


        /*public async Task<bool> VerificarCredenciales(string categoria, string usuario, string contraseña)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("paVerificarCredenciales", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Parámetros del procedimiento almacenado
                    cmd.Parameters.AddWithValue("@Categoria", categoria);
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
        }*/


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


        public async Task<bool> VerificarCredenciales(string categoria, string usuario, string contraseña)
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
        }



    }
}
