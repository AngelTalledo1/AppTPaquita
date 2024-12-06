using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class SqlServerService
    {
        private readonly string _connectionString;

        public SqlServerService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> AgregarClienteAsync(
            string nombre,
            string apePaterno,
            string apeMaterno,
            int idTipoDoc,
            string numDoc,
            string telefono,
            string direccion,
            string email)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("pa_AgregarCliente", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    // Agregar parámetros
                    command.Parameters.AddWithValue("@Nombre", nombre);
                    command.Parameters.AddWithValue("@apePaterno", string.IsNullOrWhiteSpace(apePaterno) ? (object)DBNull.Value : apePaterno);
                    command.Parameters.AddWithValue("@apeMaterno", string.IsNullOrWhiteSpace(apeMaterno) ? (object)DBNull.Value : apeMaterno);
                    command.Parameters.AddWithValue("@id_tipoDoc", idTipoDoc);
                    command.Parameters.AddWithValue("@numDoc", numDoc);
                    command.Parameters.AddWithValue("@Telefono", telefono);
                    command.Parameters.AddWithValue("@direccion", direccion);
                    command.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email);

                    // Ejecutar el procedimiento almacenado
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<int> ModificarClienteAsync(
            int idCliente,
            string nombre,
            string apePaterno,
            string apeMaterno,
            int idTipoDoc,
            string numDoc,
            string telefono,
            string direccion,
            string email
            )
        {


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("pa_ModificarCliente", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    // Agregar parámetros
                    command.Parameters.AddWithValue("@id_cliente", idCliente);
                    command.Parameters.AddWithValue("@Nombre", nombre);
                    command.Parameters.AddWithValue("@apePaterno", string.IsNullOrWhiteSpace(apePaterno) ? (object)DBNull.Value : apePaterno);
                    command.Parameters.AddWithValue("@apeMaterno", string.IsNullOrWhiteSpace(apeMaterno) ? (object)DBNull.Value : apeMaterno);
                    command.Parameters.AddWithValue("@id_tipoDoc", idTipoDoc);
                    command.Parameters.AddWithValue("@numDoc", numDoc);
                    command.Parameters.AddWithValue("@Telefono", telefono);
                    command.Parameters.AddWithValue("@direccion", direccion);
                    command.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email);

                    // Ejecutar el procedimiento almacenado
                    return await command.ExecuteNonQueryAsync();
                }
            }
           
        }
        public async Task<int> AgregarTransportistaAsync(
            string nombre,
            string apePaterno,
            string apeMaterno,
            int idTipoDoc,
            string numDoc,
            string telefono,
            string direccion,
            string email,
            int idCat,
            string licencia)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("pa_AgregarTrabajador", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    // Agregar parámetros
                    command.Parameters.AddWithValue("@Nombre", nombre);
                    command.Parameters.AddWithValue("@apePaterno", string.IsNullOrWhiteSpace(apePaterno) ? (object)DBNull.Value : apePaterno);
                    command.Parameters.AddWithValue("@apeMaterno", string.IsNullOrWhiteSpace(apeMaterno) ? (object)DBNull.Value : apeMaterno);
                    command.Parameters.AddWithValue("@id_tipoDoc", idTipoDoc);
                    command.Parameters.AddWithValue("@numDoc", numDoc);
                    command.Parameters.AddWithValue("@Telefono", telefono);
                    command.Parameters.AddWithValue("@direccion", direccion);
                    command.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email);
                    command.Parameters.AddWithValue("@id_categoria", idCat);
                    command.Parameters.AddWithValue("@licencia", idCat);
                    // Ejecutar el procedimiento almacenado
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<(int IdUsuario, int IdTipoUsuario)> VerificarCredencialesAsync(string username, string contraseña)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    Console.WriteLine("que");
                    using (var command = new SqlCommand("pa_verificarCredenciales", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar parámetros
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@contraseña", contraseña);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int idUsuario = reader.GetInt32(0); // Primera columna: id_usuario
                                int idTipoUsuario = reader.GetInt32(1); // Segunda columna: id_tipo_usuario
                                return (idUsuario, idTipoUsuario);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar credenciales: {ex.Message}");
            }

            // Devuelve null si no se encontraron coincidencias
            return (0, 0);
        }
        public async Task<string> obtenerTipoUser(int idTipoUsuario)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("obtenerTipoUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar parámetros
                        command.Parameters.AddWithValue("@id", idTipoUsuario);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return reader.GetString(0); // Primera columna: descripcion
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener tipo de usuario: {ex.Message}");
            }

            // Devuelve null si no se encontraron coincidencias
            return null;
        }
        public async Task<List<Pedido>> ListarPedidosAdminAsync()
        {
            var pedidos = new List<Pedido>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("pa_ListPedidosDet", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            pedidos.Add(new Pedido
                            {
                                IdPedido = reader.GetInt32(reader.GetOrdinal("id_pedido")),
                                Usuario = reader.GetString(reader.GetOrdinal("nombre_usuario")),
                                Cliente = reader.GetString(reader.GetOrdinal("nombre_cliente")),
                                Cantidad = reader.GetInt32(reader.GetOrdinal("cantidad")),
                                Viajes = reader.GetInt32(reader.GetOrdinal("viajes")),
                                Origen = reader.GetString(reader.GetOrdinal("origen_descripcion")),
                                OrigSector = reader.GetString(reader.GetOrdinal("origen_sector")),
                                Servicios = reader.GetString(reader.GetOrdinal("servicios_relacionados")),

                                Destino = reader.GetString(reader.GetOrdinal("destino_descripcion")),
                                DestSector = reader.GetString(reader.GetOrdinal("destino_sector")),

                                EstadoPedido = reader["estado_pedido"]?.ToString() ?? "Sin estado",
                                FechaSolicitud = reader.GetDateTime(reader.GetOrdinal("fecha_solicitud")),
                                FechaEntrega = reader.IsDBNull(reader.GetOrdinal("fecha_entrega"))
                                    ? null
                                    : reader.GetDateTime(reader.GetOrdinal("fecha_entrega"))
                            });
                        }
                    }
                }
            }

            return pedidos;
        }
       
        public async Task<List<Cliente>> ObtenerClientesAsync()
        {
            var clientes = new List<Cliente>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("pa_MostrarClientes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            clientes.Add(new Cliente
                            {
                                IdPersona = reader.GetInt32(reader.GetOrdinal("id_persona")),
                                IdCliente = reader.GetInt32(reader.GetOrdinal("id_cliente")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                ApePaterno = reader.GetString(reader.GetOrdinal("apePaterno")),
                                ApeMaterno = reader.GetString(reader.GetOrdinal("apeMaterno")),
                                NumDoc = reader.GetString(reader.GetOrdinal("numDoc")),
                                Telefono = reader.GetString(reader.GetOrdinal("Telefono")),
                                Direccion = reader.GetString(reader.GetOrdinal("direccion")),
                                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email"))
                            });
                        }
                    }
                }
            }

            return clientes;
        }


        public async Task<List<Viaje>> ObtenerViajesAsync()
        {
            var viajes = new List<Viaje>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("pa_ListViajesDatos", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            viajes.Add(new Viaje
                            {
                                IdViaje = reader.GetInt32(reader.GetOrdinal("id_viaje")),
                                IdPedido = reader.GetInt32(reader.GetOrdinal("id_pedido")),
                                TractoAsig = reader.IsDBNull(reader.GetOrdinal("placa_tracto")) ? null : reader.GetString(reader.GetOrdinal("placa_tracto")),
                                CisternaAsig = reader.IsDBNull(reader.GetOrdinal("placa_cisterna")) ? null : reader.GetString(reader.GetOrdinal("placa_cisterna")),
                                Cantidad = reader.IsDBNull(reader.GetOrdinal("cantidad_viaje")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("cantidad_viaje")),
                                TrabajadoresAsig = reader.IsDBNull(reader.GetOrdinal("trabajadores")) ? null : reader.GetString(reader.GetOrdinal("trabajadores")),

                                ultEstado = reader.IsDBNull(reader.GetOrdinal("estado_ultimo_registro")) ? null : reader.GetString(reader.GetOrdinal("estado_ultimo_registro")),

                            });
                        }
                    }
                }
            }

            return viajes;
        }


        public async Task<List<Trabajador>> ObtenerTrabajadoresAsync()
        {
            var trabajadores = new List<Trabajador>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("pa_MostrarTrabajadores", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            trabajadores.Add(new Trabajador
                            {
                                IdTrabajador = reader.IsDBNull(reader.GetOrdinal("id_trabajador")) ? 0 : reader.GetInt32(reader.GetOrdinal("id_trabajador")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(reader.GetOrdinal("Nombre")),
                                apePaterno = reader.IsDBNull(reader.GetOrdinal("apePaterno")) ? null : reader.GetString(reader.GetOrdinal("apePaterno")),
                                apeMaterno = reader.IsDBNull(reader.GetOrdinal("apeMaterno")) ? null : reader.GetString(reader.GetOrdinal("apeMaterno")),
                                // Aseguramos que numDoc sea tratado como un string en caso de que contenga texto
                                numDoc = reader.IsDBNull(reader.GetOrdinal("numDoc")) ? null : reader.GetString(reader.GetOrdinal("numDoc")),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                                direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? null : reader.GetString(reader.GetOrdinal("direccion")),
                                email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                                categoria = reader.IsDBNull(reader.GetOrdinal("categoria")) ? null : reader.GetString(reader.GetOrdinal("categoria")),
                                licencia = reader.IsDBNull(reader.GetOrdinal("licencia")) ? null : reader.GetString(reader.GetOrdinal("licencia")),
                                usuario = reader.IsDBNull(reader.GetOrdinal("usuario")) ? null : reader.GetString(reader.GetOrdinal("usuario")),
                                password = reader.IsDBNull(reader.GetOrdinal("contraseña")) ? null : reader.GetString(reader.GetOrdinal("contraseña"))


                            });
                        }
                    }
                }
            }

            return trabajadores;
        }

        public async Task<List<Seguimiento>> ObtenerEstadosViaje()
        {
            var seguimiento = new List<Seguimiento>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("pa_ListarSeguimientoViaje", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            seguimiento.Add(new Seguimiento
                            {
                                IdSeguimiento = reader.GetInt32(reader.GetOrdinal("id_seguimiento")),
                                IdViaje = reader.GetInt32(reader.GetOrdinal("id_viaje")),
                                FechaHora = reader.GetDateTime(reader.GetOrdinal("fechaHora")),
                                EstadoViaje = reader.GetString(reader.GetOrdinal("estadoViajeDescripcion")),
                                Comentario = reader.IsDBNull(reader.GetOrdinal("Comentario")) ? null : reader.GetString(reader.GetOrdinal("Comentario")),
                                Evidencia = reader.IsDBNull(reader.GetOrdinal("evidencia"))? null: reader.GetSqlBinary(reader.GetOrdinal("evidencia")).Value,

                            });
                        }
                    }
                }
            }

            return seguimiento;
        }
    }
}
