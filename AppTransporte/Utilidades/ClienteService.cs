using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;
using Dapper;
using AppTransporte.model; 

namespace AppTransporte;

public class ClienteService
{
    private readonly Conexion _conexion;

    public ClienteService()
    {
        _conexion = new Conexion(); // Crear la instancia
    }

    public async Task<List<Cliente>> ObtenerClientesAsync(string? nombre = null)
    {
        try
        {
            // Usar la propiedad ConnectionString
            using (var connection = new SqlConnection(_conexion.connectionString))
            {
                var parameters = new { nombre };
                var query = "EXEC pa_MostrarClientes @nombre";

                var result = await connection.QueryAsync<Cliente, Persona, Usuario, Cliente>(
                    query,
                    (cliente, persona, usuario) =>
                    {
                        cliente.Persona = persona;
                        usuario.Persona = persona;
                        return cliente;
                    },
                    param: parameters,
                    splitOn: "id_persona,id_usuario");

                return result.ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener clientes: {ex.Message}");
            return new List<Cliente>();
        }
    }
}
