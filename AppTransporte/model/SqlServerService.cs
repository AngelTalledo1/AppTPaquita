
    using Microsoft.Data.SqlClient;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public async Task<int> AgregarTrabajadorAsync(
            string nombre,
            string apePaterno,
            string apeMaterno,
            int idTipoDoc,
            string numDoc,
            string telefono,
            string direccion,
            string email,
            int idCat,
            string? licencia)
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
                    if (licencia is not null)
                    {
                        command.Parameters.AddWithValue("@licencia", licencia);
                    }

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
        public async Task<int> ModificarTrabajadorAsync(
            int id_trabajador,
            string nombre,
            string apePaterno,
            string apeMaterno,
            int idTipoDoc,
            string numDoc,
            string telefono,
            string direccion,
            string email,
            int idCategoria,
            string? licencia
            )
        {


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("pa_ModificarTrabajador", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    // Agregar parámetros
                    command.Parameters.AddWithValue("@id_trabajador", id_trabajador);
                    command.Parameters.AddWithValue("@Nombre", nombre);
                    command.Parameters.AddWithValue("@apePaterno", string.IsNullOrWhiteSpace(apePaterno) ? (object)DBNull.Value : apePaterno);
                    command.Parameters.AddWithValue("@apeMaterno", string.IsNullOrWhiteSpace(apeMaterno) ? (object)DBNull.Value : apeMaterno);
                    command.Parameters.AddWithValue("@id_tipoDoc", idTipoDoc);
                    command.Parameters.AddWithValue("@numDoc", numDoc);
                    command.Parameters.AddWithValue("@Telefono", telefono);
                    command.Parameters.AddWithValue("@direccion", direccion);
                    command.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email);
                    command.Parameters.AddWithValue("@id_categoria", idCategoria);
                    if (licencia != "")
                    {
                        command.Parameters.AddWithValue("@licencia", licencia);
                    }

                    // Ejecutar el procedimiento almacenado
                    return await command.ExecuteNonQueryAsync();
                }
            }

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
                                IdSolicitud = reader.GetInt32(reader.GetOrdinal("idSolicitud")),

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
                                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Contraseña = reader.GetString(reader.GetOrdinal("Contraseña"))
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
                                idtipoDoc = reader.GetInt32(reader.GetOrdinal("id_tipoDoc")),
                                idcategoria = reader.GetInt32(reader.GetOrdinal("id_categoria")),
                                numDoc = reader.IsDBNull(reader.GetOrdinal("numDoc")) ? null : reader.GetString(reader.GetOrdinal("numDoc")),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                                direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? null : reader.GetString(reader.GetOrdinal("direccion")),
                                email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                                categoria = reader.IsDBNull(reader.GetOrdinal("categoria")) ? null : reader.GetString(reader.GetOrdinal("categoria")),
                                licencia = reader.IsDBNull(reader.GetOrdinal("licencia")) ? null : reader.GetString(reader.GetOrdinal("licencia")),
                                usuario = reader.IsDBNull(reader.GetOrdinal("Usuario")) ? null : reader.GetString(reader.GetOrdinal("Usuario")),
                                password = reader.IsDBNull(reader.GetOrdinal("Contraseña")) ? null : reader.GetString(reader.GetOrdinal("Contraseña"))


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
                                Evidencia = reader.IsDBNull(reader.GetOrdinal("evidencia")) ? null : reader.GetSqlBinary(reader.GetOrdinal("evidencia")).Value,


                            });
                        }
                    }
                }
            }
            return seguimiento;
        }

        public async Task<List<Ubicacion>> ObtenerUbicacionesAsync()
        {
            var ubicaciones = new List<Ubicacion>();


            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("pa_MostrarOrigenDestino", connection))

                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ubicaciones.Add(new Ubicacion
                            {
                                IdUbicacion = reader.GetInt32(reader.GetOrdinal("id_origen")),
                                Descripcion = reader.GetString(reader.GetOrdinal("descripcion")),
                                Sector = reader.GetString(reader.GetOrdinal("sector")),
                                Referencias = reader.IsDBNull(reader.GetOrdinal("referencias")) ? null : reader.GetString(reader.GetOrdinal("referencias")),
                                CoordenadasMaps = reader.IsDBNull(reader.GetOrdinal("coordenadas_maps")) ? null : reader.GetString(reader.GetOrdinal("coordenadas_maps"))

                            });
                        }
                    }
                }
            }
            return ubicaciones;
        }
        public async Task<int> eliminarTrabajadorAsync(int idTrabajador)
        {

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("pa_EliminarTrabajador", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    // Agregar parámetros
                    command.Parameters.AddWithValue("@id_trabajador", idTrabajador);
                    // Ejecutar el procedimiento almacenado
                    return await command.ExecuteNonQueryAsync();
                }
            }

        }
        public async Task<int> eliminarClienteAsync(int idCliente)
        {

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("pa_EliminarCliente", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    // Agregar parámetros
                    command.Parameters.AddWithValue("@id_cliente", idCliente);
                    // Ejecutar el procedimiento almacenado
                    return await command.ExecuteNonQueryAsync();
                }
            }

        }


        public async Task<List<Solicitud>> ObtenerSolicitudesAsync()
        {
            var solicitud = new List<Solicitud>();


            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("pa_ListSolicitudes", connection))

                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            solicitud.Add(new Solicitud
                            {
                                IdSolicitud = reader.GetInt32(reader.GetOrdinal("id_solicitud")),
                                Descripcion = reader.GetString(reader.GetOrdinal("SolicitudDescripcion")),
                                IdEstadoSolicitud = reader.GetInt32(reader.GetOrdinal("id_estadoSolicitud")),
                                EstadoSolicitud = reader.GetString(reader.GetOrdinal("Estado")),
                                Comentario = reader.IsDBNull(reader.GetOrdinal("SolicitudComentario")) ? null : reader.GetString(reader.GetOrdinal("SolicitudComentario")),
                                IdCliente = reader.GetInt32(reader.GetOrdinal("id_cliente")),
                                Cliente = reader.IsDBNull(reader.GetOrdinal("ClienteNombreCompleto")) ? null : reader.GetString(reader.GetOrdinal("ClienteNombreCompleto"))

                            });
                        }
                    }
                }
            }
            return solicitud;
        }


        public async Task<List<Vehiculo>> ObtenerTractoAsync(string placa = null, string ordenarPor = null)
        {
            var tracto = new List<Vehiculo>();


            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("pa_MostrarTractos", connection))

                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@placa", (object)placa ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ordenarPor", (object)ordenarPor ?? DBNull.Value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tracto.Add(new Vehiculo
                            {
                                IdVehiculo = reader.GetInt32(reader.GetOrdinal("id_tracto")),
                                Placa = reader.GetString(reader.GetOrdinal("placa")),
                                Modelo = reader.IsDBNull(reader.GetOrdinal("modelo")) ? null : reader.GetString(reader.GetOrdinal("modelo")),
                                AñoFabricacion = reader.IsDBNull(reader.GetOrdinal("AñoFabricacion")) ? null : reader.GetString(reader.GetOrdinal("AñoFabricacion")),
                                EmisionPoliza = reader.IsDBNull(reader.GetOrdinal("emision_poliza")) ? null : reader.GetDateTime(reader.GetOrdinal("emision_poliza")),
                                VencimientoPoliza = reader.IsDBNull(reader.GetOrdinal("vencimiento_poliza")) ? null : reader.GetDateTime(reader.GetOrdinal("vencimiento_poliza")),
                                EmisionCITV = reader.IsDBNull(reader.GetOrdinal("emision_CITV")) ? null : reader.GetDateTime(reader.GetOrdinal("emision_CITV")),
                                VencimientoCITV = reader.IsDBNull(reader.GetOrdinal("vencimiento_CITV")) ? null : reader.GetDateTime(reader.GetOrdinal("vencimiento_CITV")),
                                Estado = reader.GetBoolean(reader.GetOrdinal("estado")),
                                Imagen = reader.IsDBNull(reader.GetOrdinal("imagen")) ? null : reader.GetSqlBinary(reader.GetOrdinal("imagen")).Value,
                                Poliza = reader.IsDBNull(reader.GetOrdinal("poliza")) ? null : reader.GetSqlBinary(reader.GetOrdinal("poliza")).Value,
                                CITV = reader.IsDBNull(reader.GetOrdinal("citv")) ? null : reader.GetSqlBinary(reader.GetOrdinal("citv")).Value,
                                Cubicacion = reader.IsDBNull(reader.GetOrdinal("cubicacion")) ? null : reader.GetSqlBinary(reader.GetOrdinal("cubicacion")).Value,
                                TarjetaPropiedad = reader.IsDBNull(reader.GetOrdinal("tarjetaPropiedad")) ? null : reader.GetSqlBinary(reader.GetOrdinal("tarjetaPropiedad")).Value,

                            });
                        }
                    }
                }
            }
            return tracto;
        }
        public async Task<int> eliminarUbicacionAsync(int idUbicacion)
        {

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_EliminarOrigenYDestino", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_origen", idUbicacion);
                    command.Parameters.AddWithValue("@id_destino", idUbicacion);
                    return await command.ExecuteNonQueryAsync();
                }
            }

        }
        public async Task<int> ModificarUbicacionAsync(
             int idUbicacion,
              string descripcion,
             string sector,
             string referencias,
             string coordenadas)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("pa_ModificarOrigenYDestino", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@id_origen", idUbicacion);
                    command.Parameters.AddWithValue("@id_destino", idUbicacion);
                    command.Parameters.AddWithValue("@descripcion", descripcion);
                    command.Parameters.AddWithValue("@sector", sector);
                    command.Parameters.AddWithValue("@referencias", referencias);
                    command.Parameters.AddWithValue("@coordenadas_maps", coordenadas);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<int> AgregarUbicacionAsync(
    string descripcion,
    string sector,
    string referencias,
    string coordenadas)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("pa_InsertarOrigenYDestino", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    // Agregar parámetros
                    command.Parameters.AddWithValue("@descripcion", descripcion);
                    command.Parameters.AddWithValue("@sector", sector);
                    command.Parameters.AddWithValue("@referencias", referencias);
                    command.Parameters.AddWithValue("@coordenadas_maps", coordenadas);
                    // Ejecutar el procedimiento almacenado
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<List<Vehiculo>> ObtenerCisternaAsync(string placa = null, string ordenarPor = null)
        {
            var cisterna = new List<Vehiculo>();


            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("pa_MostrarCisterna", connection))

                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@placa", (object)placa ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ordenarPor", (object)ordenarPor ?? DBNull.Value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            cisterna.Add(new Vehiculo
                            {
                                IdVehiculo = reader.GetInt32(reader.GetOrdinal("id_cisterna")),
                                Placa = reader.GetString(reader.GetOrdinal("placa")),
                                AñoFabricacion = reader.IsDBNull(reader.GetOrdinal("AñoFabricacion")) ? null : reader.GetString(reader.GetOrdinal("AñoFabricacion")),
                                EmisionCubicacion = reader.IsDBNull(reader.GetOrdinal("emision_cubicacion")) ? null : reader.GetDateTime(reader.GetOrdinal("emision_cubicacion")),
                                VencimientoCubicacion = reader.IsDBNull(reader.GetOrdinal("vencimiento_cubicacion")) ? null : reader.GetDateTime(reader.GetOrdinal("vencimiento_cubicacion")),
                                EmisionPoliza = reader.IsDBNull(reader.GetOrdinal("emision_poliza")) ? null : reader.GetDateTime(reader.GetOrdinal("emision_poliza")),
                                VencimientoPoliza = reader.IsDBNull(reader.GetOrdinal("vencimiento_poliza")) ? null : reader.GetDateTime(reader.GetOrdinal("vencimiento_poliza")),
                                EmisionCITV = reader.IsDBNull(reader.GetOrdinal("emision_CITV")) ? null : reader.GetDateTime(reader.GetOrdinal("emision_CITV")),
                                VencimientoCITV = reader.IsDBNull(reader.GetOrdinal("vencimiento_CITV")) ? null : reader.GetDateTime(reader.GetOrdinal("vencimiento_CITV")),
                                Estado = reader.GetBoolean(reader.GetOrdinal("estado")),
                                Imagen = reader.IsDBNull(reader.GetOrdinal("imagen")) ? null : reader.GetSqlBinary(reader.GetOrdinal("imagen")).Value,
                                Poliza = reader.IsDBNull(reader.GetOrdinal("poliza")) ? null : reader.GetSqlBinary(reader.GetOrdinal("poliza")).Value,
                                CITV = reader.IsDBNull(reader.GetOrdinal("citv")) ? null : reader.GetSqlBinary(reader.GetOrdinal("citv")).Value,
                                Cubicacion = reader.IsDBNull(reader.GetOrdinal("cubicacion")) ? null : reader.GetSqlBinary(reader.GetOrdinal("cubicacion")).Value,
                                TarjetaPropiedad = reader.IsDBNull(reader.GetOrdinal("tarjetaPropiedad")) ? null : reader.GetSqlBinary(reader.GetOrdinal("tarjetaPropiedad")).Value,

                            });
                        }
                    }
                }
            }
            return cisterna;
        }
    }
}
