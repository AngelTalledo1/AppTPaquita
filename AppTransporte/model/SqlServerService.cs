
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.Metadata;
using iTextSharp.text.pdf;
using iTextSharp.text;
#pragma warning disable CS8603, CS1998, CS8625, CS8601, CS8600, CS8612, CS0612

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
            string? apePaterno,
            string? apeMaterno,
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
        // Método para agregar a tu clase SqlServerService existente
        public (DataTable MetricasGenerales, DataTable PorCliente, DataTable DetalleSolicitudes) ObtenerReporteAtencionSolicitudes(DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable metricasGenerales = new DataTable();
            DataTable porCliente = new DataTable();
            DataTable detalleSolicitudes = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_ReporteAtencionSolicitudes", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 180; // Aumentamos el timeout a 3 minutos para asegurar

                        // Parámetros
                        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                        cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                        // Ejecutar el procedimiento y manejar los múltiples resultados
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);

                        // Verificar que tenemos todos los conjuntos de resultados esperados
                        if (ds.Tables.Count >= 1)
                            metricasGenerales = ds.Tables[0];

                        if (ds.Tables.Count >= 2)
                            porCliente = ds.Tables[1];

                        if (ds.Tables.Count >= 3)
                            detalleSolicitudes = ds.Tables[2];
                    }
                }
            }
            catch (Exception ex)
            {
                // Registrar el error para su posterior análisis
                Console.WriteLine($"Error en GetReporteAtencionSolicitudes: {ex.Message}");
                throw; // Re-lanzar la excepción para que sea manejada en capas superiores
            }

            return (metricasGenerales, porCliente, detalleSolicitudes);
        }
        public async Task<int> ActualizarUsuarioAsync(int idUsuario, string username, string contraseña, int idTipoUsuario, bool estado, int idPersona, int? idEmpresa = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("PA_ActualizarUsuario", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_usuario", idUsuario);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@contraseña", contraseña);
                    command.Parameters.AddWithValue("@id_tipoUsuario", idTipoUsuario);
                    command.Parameters.AddWithValue("@estado", estado);
                    command.Parameters.AddWithValue("@id_persona", idPersona);
                    command.Parameters.AddWithValue("@id_empresa", (object)idEmpresa ?? DBNull.Value);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task<int> EliminarUsuarioAsync(int idUsuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("PA_EliminarUsuario", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_usuario", idUsuario);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task<int> InsertarServicioAsync(string descripcion)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("pa_InsertarServicio", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@descripcion", descripcion);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<int> AgregarEmpresaAsync(string razonSocial, string ruc, string direccion = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("pa_AgregarEmpresa", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@razonSocial", razonSocial);
                    command.Parameters.AddWithValue("@ruc", ruc);
                    command.Parameters.AddWithValue("@direccion", (object)direccion ?? DBNull.Value);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }



        public async Task<List<Empresa>> ObtenerEmpresasAsync(string? filtro = null)
        {
            var empresas = new List<Empresa>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("pa_ObtenerEmpresas", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@filtro", (object?)filtro ?? DBNull.Value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            empresas.Add(new Empresa
                            {
                                id_empresa = reader.GetInt32(reader.GetOrdinal("id_empresa")),
                                razonSocial = reader.GetString(reader.GetOrdinal("razonSocial")),
                                RUC = reader.GetString(reader.GetOrdinal("ruc")),
                                Direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? null : reader.GetString(reader.GetOrdinal("direccion"))
                            });
                        }
                    }
                }
            }

            return empresas;
        }
        public async Task<int> ActualizarEmpresaAsync(int idEmpresa, string razonSocial, string ruc, string direccion = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("PA_ActualizarEmpresa", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_empresa", idEmpresa);
                    command.Parameters.AddWithValue("@razonSocial", razonSocial);
                    command.Parameters.AddWithValue("@ruc", ruc);
                    command.Parameters.AddWithValue("@direccion", (object)direccion ?? DBNull.Value);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> EliminarEmpresaAsync(int idEmpresa)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("PA_eliminarEmpresa", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_empresa", idEmpresa);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task<int> ActualizarServicioAsync(int idServicio, string descripcion)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("pa_ActualizarServicio", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_servicio", idServicio);
                    command.Parameters.AddWithValue("@descripcion", descripcion);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<int> EliminarServicioAsync(int idServicio)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("pa_EliminarServicio", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_servicio", idServicio);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<Cliente?> ObtenerClientePorUsuarioAsync(int idUsuario)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("pa_ObtenerClientePorUsuario", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetro del procedimiento almacenado
                        command.Parameters.AddWithValue("@id_usuario", idUsuario);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Cliente
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
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener cliente: {ex.Message}");
            }


            return null; // Devuelve null si no se encuentra el cliente
        }

        public async Task AgregarSolicitudAsync(Solicitud solicitud)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("pa_AgregarSolicitud", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros del procedimiento
                    command.Parameters.AddWithValue("@descripcion", solicitud.Descripcion);
                    command.Parameters.AddWithValue("@comentario", (object)solicitud.Comentario ?? DBNull.Value);
                    command.Parameters.AddWithValue("@id_cliente", solicitud.IdCliente);
                    command.Parameters.AddWithValue("@id_servicio", solicitud.IdServicio);
                    // Ejecutar el procedimiento
                    await command.ExecuteNonQueryAsync();
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
        // Modifica el método en la clase SqlServerService

        public async Task<int> ActualizarEstadoSeguimientoAsync(
            int idViaje,
            string comentario = null,
            string evidenciaUrl = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("pa_ActualizarEstadoSeguimiento", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregar parámetros
                    command.Parameters.AddWithValue("@id_viaje", idViaje);
                    command.Parameters.AddWithValue("@comentario", string.IsNullOrWhiteSpace(comentario) ? (object)DBNull.Value : comentario);
                    command.Parameters.AddWithValue("@evidenciaUrl", string.IsNullOrWhiteSpace(evidenciaUrl) ? (object)DBNull.Value : evidenciaUrl);

                    // Parámetro de retorno para obtener el resultado del procedimiento
                    SqlParameter returnValue = new SqlParameter("@ReturnValue", SqlDbType.Int);
                    returnValue.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(returnValue);

                    // Ejecutar el procedimiento almacenado
                    await command.ExecuteNonQueryAsync();

                    // Obtener y devolver el valor de retorno del procedimiento
                    return (int)returnValue.Value;
                }
            }
        }

        public async Task<UsuarioResponse> VerificarCredencialesAsync(string username, string contraseña)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
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
                                return new UsuarioResponse(idUsuario, idTipoUsuario);
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
            return null;
        }
        public async Task<int> ActualizarSolicitudAsync(Solicitud solicitud)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("pa_ActualizarSolicitud", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@id_solicitud", solicitud.IdSolicitud);
                    command.Parameters.AddWithValue("@descripcion", solicitud.Descripcion);
                    command.Parameters.AddWithValue("@id_estadoSolicitud", solicitud.IdEstadoSolicitud);
                    command.Parameters.AddWithValue("@comentario", (object)solicitud.Comentario ?? DBNull.Value);
                    command.Parameters.AddWithValue("@id_servicio", solicitud.IdServicio);

                    return await command.ExecuteNonQueryAsync();
                }
            }
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

        public async Task<List<Viaje>> ObtenerViajesModAsync(int? idPedido, int? idUsuario)
        {
            var viajes = new List<Viaje>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("pa_ListViajes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    if (idUsuario.HasValue)
                    {
                        command.Parameters.Add(new SqlParameter("@idUsuario", idUsuario.Value));
                    }

                    if (idPedido.HasValue)
                    {
                        command.Parameters.Add(new SqlParameter("@idPedido", idPedido.Value));
                    }

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
        public async Task<List<Pedido>> ListarPedidosPorUsuario(int idUsuario)
        {
            var pedidos = new List<Pedido>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("pa_ListPedidosUsuario", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregar el parámetro idUsuario al comando
                    command.Parameters.AddWithValue("@idUsuarioCliente", idUsuario);

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
                                ApePaterno = reader.IsDBNull(reader.GetOrdinal("apePaterno")) ? null : reader.GetString(reader.GetOrdinal("apePaterno")),
                                ApeMaterno = reader.IsDBNull(reader.GetOrdinal("apeMaterno")) ? null : reader.GetString(reader.GetOrdinal("apeMaterno")),
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
        public DataTable ObtenerReporteServicios(DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable resultado = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("sp_ReporteServicios", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                    // Llenar el DataTable con los resultados
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(resultado);
                    }
                }
            }

            return resultado;
        }
        // Método para agregar a tu clase SqlServerService existente
        public (DataTable Resumen, DataTable Detalle) GetReportePedidos(DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable resumenTable = new DataTable();
            DataTable detalleTable = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("sp_ReportePedidos", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        resumenTable.Load(reader);
                        if (reader.NextResult())
                        {
                            detalleTable.Load(reader);
                        }
                    }
                }
            }

            return (resumenTable, detalleTable);
        }
        public byte[] GenerarReporteTrabajadorPDF(List<ReporteTrabajador> reporteData, DateTime fechaInicio,
            DateTime fechaFin, string tipoReporte)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 36, 36, 36, 36);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();
                iTextSharp.text.Font titleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA,
                                                                          18,
                                                                          iTextSharp.text.Font.BOLD);
                Paragraph titulo = new Paragraph("Reporte de Trabajador", titleFont);
                titulo.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                titulo.SpacingAfter = 20;
                document.Add(titulo);
                iTextSharp.text.Font normalFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12);
                Paragraph info = new Paragraph($"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}", normalFont);
                info.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                info.SpacingAfter = 5;
                document.Add(info);
                Paragraph infoTipo = new Paragraph($"Tipo de reporte: {tipoReporte}", normalFont);
                infoTipo.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                infoTipo.SpacingAfter = 20;
                document.Add(infoTipo);
                iTextSharp.text.Font subtitleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.BOLD);
                Paragraph resumenTitulo = new Paragraph("Resumen", subtitleFont);
                resumenTitulo.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                resumenTitulo.SpacingAfter = 10;
                document.Add(resumenTitulo);
                PdfPTable resumenTable = new PdfPTable(2);
                resumenTable.WidthPercentage = 100;
                resumenTable.SpacingAfter = 20;
                PdfPCell headerCell1 = new PdfPCell(new Phrase("Descripción", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD)));
                headerCell1.BackgroundColor = new BaseColor(220, 220, 220); // Light gray
                headerCell1.Padding = 5;
                PdfPCell headerCell2 = new PdfPCell(new Phrase("Valor", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD)));
                headerCell2.BackgroundColor = new BaseColor(220, 220, 220); // Light gray
                headerCell2.Padding = 5;
                resumenTable.AddCell(headerCell1);
                resumenTable.AddCell(headerCell2);
                int totalTrabajadores = reporteData.Select(r => r.IdTrabajador).Distinct().Count();
                int totalViajes = reporteData.Sum(r => r.TotalViajes);
                int volumenTotal = reporteData.Sum(r => r.VolumenTransportado);
                PdfPCell cellDesc1 = new PdfPCell(new Phrase("Total de Trabajadores", normalFont));
                cellDesc1.Padding = 5;
                PdfPCell cellVal1 = new PdfPCell(new Phrase(totalTrabajadores.ToString(), normalFont));
                cellVal1.Padding = 5;
                PdfPCell cellDesc2 = new PdfPCell(new Phrase("Total de Viajes", normalFont));
                cellDesc2.Padding = 5;
                PdfPCell cellVal2 = new PdfPCell(new Phrase(totalViajes.ToString(), normalFont));
                cellVal2.Padding = 5;
                PdfPCell cellDesc3 = new PdfPCell(new Phrase("Volumen Total Transportado", normalFont));
                cellDesc3.Padding = 5;
                PdfPCell cellVal3 = new PdfPCell(new Phrase($"{volumenTotal:N2} L", normalFont));
                cellVal3.Padding = 5;

                resumenTable.AddCell(cellDesc1);
                resumenTable.AddCell(cellVal1);
                resumenTable.AddCell(cellDesc2);
                resumenTable.AddCell(cellVal2);
                resumenTable.AddCell(cellDesc3);
                resumenTable.AddCell(cellVal3);

                document.Add(resumenTable);

                // Tabla de detalle
                Paragraph detalleTitulo = new Paragraph("Detalle por Trabajador", subtitleFont);
                detalleTitulo.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                detalleTitulo.SpacingAfter = 10;
                document.Add(detalleTitulo);

                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;

                // Cabecera de la tabla de detalle
                iTextSharp.text.Font headerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD);
                BaseColor headerColor = new BaseColor(220, 220, 220); // Light gray

                PdfPCell headerTrabajador = new PdfPCell(new Phrase("Trabajador", headerFont));
                headerTrabajador.BackgroundColor = headerColor;
                headerTrabajador.Padding = 5;

                PdfPCell headerCategoria = new PdfPCell(new Phrase("Categoría", headerFont));
                headerCategoria.BackgroundColor = headerColor;
                headerCategoria.Padding = 5;

                PdfPCell headerViajes = new PdfPCell(new Phrase("Viajes", headerFont));
                headerViajes.BackgroundColor = headerColor;
                headerViajes.Padding = 5;

                PdfPCell headerSeguimientos = new PdfPCell(new Phrase("Seguimientos", headerFont));
                headerSeguimientos.BackgroundColor = headerColor;
                headerSeguimientos.Padding = 5;

                PdfPCell headerVolumen = new PdfPCell(new Phrase("Volumen", headerFont));
                headerVolumen.BackgroundColor = headerColor;
                headerVolumen.Padding = 5;

                PdfPCell headerPeriodo = new PdfPCell(new Phrase("Periodo", headerFont));
                headerPeriodo.BackgroundColor = headerColor;
                headerPeriodo.Padding = 5;

                table.AddCell(headerTrabajador);
                table.AddCell(headerCategoria);
                table.AddCell(headerViajes);
                table.AddCell(headerSeguimientos);
                table.AddCell(headerVolumen);
                table.AddCell(headerPeriodo);

                // Filas de datos
                bool colorAlternado = false;
                foreach (var item in reporteData)
                {
                    BaseColor bgColor = colorAlternado
                        ? BaseColor.WHITE
                        : new BaseColor(245, 245, 245); // Very light gray

                    colorAlternado = !colorAlternado;

                    PdfPCell cellNombre = new PdfPCell(new Phrase(item.NombreCompleto, normalFont));
                    cellNombre.BackgroundColor = bgColor;
                    cellNombre.Padding = 5;

                    PdfPCell cellCategoria = new PdfPCell(new Phrase(item.Categoria, normalFont));
                    cellCategoria.BackgroundColor = bgColor;
                    cellCategoria.Padding = 5;

                    PdfPCell cellViajes = new PdfPCell(new Phrase(item.TotalViajes.ToString(), normalFont));
                    cellViajes.BackgroundColor = bgColor;
                    cellViajes.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    cellViajes.Padding = 5;

                    PdfPCell cellSeguimientos = new PdfPCell(new Phrase(item.TotalSeguimientos.ToString(), normalFont));
                    cellSeguimientos.BackgroundColor = bgColor;
                    cellSeguimientos.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    cellSeguimientos.Padding = 5;

                    PdfPCell cellVolumen = new PdfPCell(new Phrase($"{item.VolumenTransportado:N2}", normalFont));
                    cellVolumen.BackgroundColor = bgColor;
                    cellVolumen.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                    cellVolumen.Padding = 5;

                    PdfPCell cellPeriodo = new PdfPCell(new Phrase(item.Periodo, normalFont));
                    cellPeriodo.BackgroundColor = bgColor;
                    cellPeriodo.Padding = 5;

                    table.AddCell(cellNombre);
                    table.AddCell(cellCategoria);
                    table.AddCell(cellViajes);
                    table.AddCell(cellSeguimientos);
                    table.AddCell(cellVolumen);
                    table.AddCell(cellPeriodo);
                }

                document.Add(table);

                // Añadir pie de página
                iTextSharp.text.Font footerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10);
                footerFont.Color = BaseColor.GRAY;

                Paragraph footer = new Paragraph($"Reporte generado el {DateTime.Now:dd/MM/yyyy HH:mm:ss}", footerFont);
                footer.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                footer.SpacingBefore = 20;
                document.Add(footer);

                document.Close();
                return ms.ToArray();
            }
        }

        public async Task<List<ReporteTrabajador>> ObtenerReporteTrabajadorAsync(
        int? idTrabajador,
        DateTime fechaInicio,
        DateTime fechaFin,
        string tipoReporte)
        {
            var reportes = new List<ReporteTrabajador>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("pa_ReporteTrabajador", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregar parámetros
                    command.Parameters.AddWithValue("@IdTrabajador", (object)idTrabajador ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", fechaFin);
                    command.Parameters.AddWithValue("@TipoReporte", tipoReporte);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reportes.Add(new ReporteTrabajador
                            {
                                IdTrabajador = reader.GetInt32(reader.GetOrdinal("id_trabajador")),
                                NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto")),
                                Categoria = reader.GetString(reader.GetOrdinal("Categoria")),
                                TotalViajes = reader.GetInt32(reader.GetOrdinal("TotalViajes")),
                                TotalSeguimientos = reader.GetInt32(reader.GetOrdinal("TotalSeguimientos")),
                                VolumenTransportado = reader.GetInt32(reader.GetOrdinal("VolumenTransportado")),
                                Periodo = reader.GetString(reader.GetOrdinal("Periodo"))
                            });
                        }
                    }
                }
            }

            return reportes;
        }

        public async Task<List<Usuario>> ObtenerUsuariosAsync(bool? estadoFiltro = true)
        {
            var usuarios = new List<Usuario>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_ObtenerUsuarios", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Añadir el parámetro de filtro por estado
                    if (estadoFiltro.HasValue)
                        command.Parameters.AddWithValue("@estadoFiltro", estadoFiltro.Value);
                    else
                        command.Parameters.AddWithValue("@estadoFiltro", DBNull.Value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            usuarios.Add(new Usuario
                            {
                                IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                                Username = reader.GetString(reader.GetOrdinal("username")),
                                Contraseña = reader.GetString(reader.GetOrdinal("contraseña")),
                                IdTipoUsuario = reader.GetInt32(reader.GetOrdinal("id_tipoUsuario")),
                                Estado = reader.GetBoolean(reader.GetOrdinal("estado")),
                                IdPersona = reader.GetInt32(reader.GetOrdinal("id_persona")),
                                IdEmpresa = reader.IsDBNull(reader.GetOrdinal("id_empresa")) ?
                                    null : reader.GetInt32(reader.GetOrdinal("id_empresa")),
                                TipoUsuario = reader.GetString(reader.GetOrdinal("TipoUsuario")),
                                Nombres = reader.GetString(reader.GetOrdinal("Nombre")),
                                Apellidos = reader.GetString(reader.GetOrdinal("apePaterno")),
                                Correo = reader.GetString(reader.GetOrdinal("email")),
                                Telefono = reader.GetString(reader.GetOrdinal("telefono"))
                            });
                        }
                    }
                }
            }

            return usuarios;
        }




        public async Task<List<Trabajador>> ObtenerTrabajadoresAsync(string categoria = null)
        {
            var trabajadores = new List<Trabajador>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("pa_MostrarTrabajadores", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    if (!string.IsNullOrEmpty(categoria))
                    {
                        command.Parameters.Add(new SqlParameter("@categoria", SqlDbType.VarChar, 20)
                        {
                            Value = categoria
                        });
                    }

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
                                Evidencia = reader.IsDBNull(reader.GetOrdinal("evidencia")) ? null : reader.GetString(reader.GetOrdinal("evidencia")),


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
        public async Task<List<Solicitud>> ObtenerSolicitudesAsync(int? id_Cliente = null)
        {
            var solicitud = new List<Solicitud>();


            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("pa_ListSolicitudes", connection))

                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregamos el parámetro solo si se proporciona un ID de usuario
                    if (id_Cliente.HasValue)
                    {
                        command.Parameters.Add(new SqlParameter("@id_cliente", SqlDbType.Int)
                        {
                            Value = id_Cliente.Value
                        });
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            solicitud.Add(new Solicitud
                            {
                                IdSolicitud = reader.GetInt32(reader.GetOrdinal("id_solicitud")),
                                Descripcion = reader.GetString(reader.GetOrdinal("SolicitudDescripcion")),
                                FechaSolicitud = reader.GetDateTime(reader.GetOrdinal("fecha")),
                                IdEstadoSolicitud = reader.GetInt32(reader.GetOrdinal("id_estadoSolicitud")),
                                EstadoSolicitud = reader.GetString(reader.GetOrdinal("Estado")),
                                Comentario = reader.IsDBNull(reader.GetOrdinal("SolicitudComentario")) ? null : reader.GetString(reader.GetOrdinal("SolicitudComentario")),
                                IdCliente = reader.GetInt32(reader.GetOrdinal("id_cliente")),
                                Cliente = reader.IsDBNull(reader.GetOrdinal("ClienteNombreCompleto")) ? null : reader.GetString(reader.GetOrdinal("ClienteNombreCompleto")),
                                Fecha = reader.GetDateTime(reader.GetOrdinal("fecha"))
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
        public async Task CrearPedidoAsync(
        int idSolicitud,
        int cantidad,
        int viajes,
        int idOrigen,
        int idDestino,
        int idEstadoPedido,
        string listaServicios)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("pa_CrearPedido", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros del procedimiento
                    command.Parameters.AddWithValue("@id_solicitud", idSolicitud);
                    command.Parameters.AddWithValue("@cantidad", cantidad);
                    command.Parameters.AddWithValue("@viajes", viajes);
                    command.Parameters.AddWithValue("@id_origen", idOrigen);
                    command.Parameters.AddWithValue("@id_destino", idDestino);
                    command.Parameters.AddWithValue("@id_estadoPedido", idEstadoPedido);
                    command.Parameters.AddWithValue("@lista_servicios", (object)listaServicios ?? DBNull.Value);

                    // Ejecutar el procedimiento
                    await command.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task<List<Servicio>> ObtenerServiciosAsync(string filtro = null)
        {
            var servicios = new List<Servicio>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("pa_MostrarServicios", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@filtro", (object)filtro ?? DBNull.Value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            servicios.Add(new Servicio
                            {
                                IdServicio = reader.GetInt32(reader.GetOrdinal("id_servicio")),
                                Descripcion = reader.GetString(reader.GetOrdinal("descripcion")),
                                Estado = reader.GetBoolean(reader.GetOrdinal("estado"))
                            });
                        }
                    }
                }
            }

            return servicios;
        }

        public async Task<int> AgregarVehiculo(
            string placa,
            string modelo,
            string añoFabricacion,
            DateTime? emisionPoliza,
            DateTime? vencimientoPoliza,
            DateTime? emisionCITV,
            DateTime? vencimientoCITV,
            DateTime? emisionCubicacion,
            DateTime? vencimientoCubicacion,
            byte[] imagen,
            byte[] poliza,
            byte[] citv,
            byte[] cubicacion,
            byte[] tarjetaPropiedad,
            string tipoVehiculo
            )
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("pa_AgregarVehiculo", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    // Agregar parámetros
                    command.Parameters.AddWithValue("@placa", placa);
                    command.Parameters.AddWithValue("@modelo", string.IsNullOrWhiteSpace(modelo) ? (object)DBNull.Value : modelo);
                    command.Parameters.AddWithValue("@añoFabricacion", string.IsNullOrWhiteSpace(añoFabricacion) ? (object)DBNull.Value : añoFabricacion);
                    command.Parameters.AddWithValue("@emisionPoliza", emisionPoliza);
                    command.Parameters.AddWithValue("@vencimientoPoliza", vencimientoPoliza);
                    command.Parameters.AddWithValue("@emisionCITV", emisionCITV);
                    command.Parameters.AddWithValue("@vencimientoCITV", vencimientoCITV);
                    command.Parameters.AddWithValue("@emisionCubicacion", emisionCubicacion);
                    command.Parameters.AddWithValue("@vencimientoCubicacion", vencimientoCubicacion);
                    command.Parameters.AddWithValue("@imagen", imagen);
                    command.Parameters.AddWithValue("@poliza", poliza);
                    command.Parameters.AddWithValue("@citv", citv);
                    command.Parameters.AddWithValue("@cubicacion", cubicacion);
                    command.Parameters.AddWithValue("@tarjetaPropiedad", tarjetaPropiedad);
                    command.Parameters.AddWithValue("@tipoVehiculo", tipoVehiculo);

                    // Ejecutar el procedimiento almacenado
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<List<Viaje>> listarViajes()
        {
            return null;
        }
        
            public async Task<List<TareaAdicional>> ObtenerTareasUsuarioAsync(int idUsuario)
            {
                var tareas = new List<TareaAdicional>();

                try
                {
                    using var connection = new SqlConnection(_connectionString);
                    using var command = new SqlCommand("SP_ObtenerTareasUsuario", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@id_usuario", idUsuario);

                    await connection.OpenAsync();
                    using var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        tareas.Add(new TareaAdicional
                        {
                            id_tareaAdicional = reader.GetInt32("id_tareaAdicional"),
                            fecha_tarea = reader.GetDateTime("fecha_tarea"),
                            hora_inicio = TimeSpan.Parse(reader["hora_inicio"].ToString()),
                            hora_fin = TimeSpan.Parse(reader["hora_fin"].ToString()),
                            descripcion = reader.GetString("descripcion"),
                            fecha_creacion = reader.GetDateTime("fecha_creacion"),
                            fecha_modificacion = reader.GetDateTime("fecha_modificacion"),
                            duracion_minutos = reader.GetInt32("duracion_minutos"),
                            nombre_usuario = reader.GetString("nombre_usuario"),
                            id_usuario = idUsuario,
                            estado = true
                        });
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al obtener tareas: {ex.Message}");
                    throw;
                }

                return tareas;
            }

            public async Task<List<TareaAdicional>> ObtenerTareasPorFechaAsync(DateTime fecha, int idUsuario)
            {
                var tareas = new List<TareaAdicional>();

                try
                {
                    using var connection = new SqlConnection(_connectionString);
                    using var command = new SqlCommand("SP_ObtenerTareasPorFecha", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@fecha_tarea", fecha.Date);
                    command.Parameters.AddWithValue("@id_usuario", idUsuario);

                    await connection.OpenAsync();
                    using var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        tareas.Add(new TareaAdicional
                        {
                            id_tareaAdicional = reader.GetInt32("id_tareaAdicional"),
                            fecha_tarea = reader.GetDateTime("fecha_tarea"),
                            hora_inicio = TimeSpan.Parse(reader["hora_inicio"].ToString()),
                            hora_fin = TimeSpan.Parse(reader["hora_fin"].ToString()),
                            descripcion = reader.GetString("descripcion"),
                            fecha_creacion = reader.GetDateTime("fecha_creacion"),
                            fecha_modificacion = reader.GetDateTime("fecha_modificacion"),
                            duracion_minutos = reader.GetInt32("duracion_minutos"),
                            nombre_usuario = reader.GetString("nombre_usuario"),
                            id_usuario = idUsuario,
                            estado = true
                        });
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al obtener tareas por fecha: {ex.Message}");
                    throw;
                }

                return tareas;
            }

            public async Task<RespuestaProcedimiento> InsertarTareaAsync(TareaAdicional tarea)
            {
                try
                {
                    using var connection = new SqlConnection(_connectionString);
                    using var command = new SqlCommand("SP_InsertarTareaAdicional", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@fecha_tarea", tarea.fecha_tarea.Date);
                    command.Parameters.AddWithValue("@hora_inicio", tarea.hora_inicio);
                    command.Parameters.AddWithValue("@hora_fin", tarea.hora_fin);
                    command.Parameters.AddWithValue("@descripcion", tarea.descripcion);
                    command.Parameters.AddWithValue("@id_usuario", tarea.id_usuario);

                    await connection.OpenAsync();
                    using var reader = await command.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        return new RespuestaProcedimiento
                        {
                            id_tareaAdicional = reader.IsDBNull("id_tareaAdicional") ? null : reader.GetInt32("id_tareaAdicional"),
                            Mensaje = reader.GetString("Mensaje"),
                            FilasAfectadas = 1
                        };
                    }
                }
                catch (SqlException ex)
                {
                    return new RespuestaProcedimiento
                    {
                        FilasAfectadas = 0,
                        Mensaje = ex.Message
                    };
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al insertar tarea: {ex.Message}");
                    return new RespuestaProcedimiento
                    {
                        FilasAfectadas = 0,
                        Mensaje = $"Error inesperado: {ex.Message}"
                    };
                }

                return new RespuestaProcedimiento { FilasAfectadas = 0, Mensaje = "Error desconocido" };
            }

            public async Task<RespuestaProcedimiento> EliminarTareaAsync(int idTarea)
            {
                try
                {
                    using var connection = new SqlConnection(_connectionString);
                    using var command = new SqlCommand("SP_EliminarTareaAdicional", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@id_tareaAdicional", idTarea);

                    await connection.OpenAsync();
                    using var reader = await command.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        return new RespuestaProcedimiento
                        {
                            FilasAfectadas = reader.GetInt32("FilasAfectadas"),
                            Mensaje = reader.GetString("Mensaje")
                        };
                    }
                }
                catch (SqlException ex)
                {
                    return new RespuestaProcedimiento
                    {
                        FilasAfectadas = 0,
                        Mensaje = ex.Message
                    };
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al eliminar tarea: {ex.Message}");
                    return new RespuestaProcedimiento
                    {
                        FilasAfectadas = 0,
                        Mensaje = $"Error inesperado: {ex.Message}"
                    };
                }

                return new RespuestaProcedimiento { FilasAfectadas = 0, Mensaje = "Error desconocido" };
            }
        public async Task<(int Resultado, string Mensaje)> AsignarViajeAsync(
    int idViaje,
    int? idTracto,
    int? idCisterna,
    int cantidad,
    int? idTransportista,
    int? idAyudante)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("SP_AsignarViaje", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@id_viaje", idViaje);
                    command.Parameters.AddWithValue("@id_tracto", (object)idTracto ?? DBNull.Value);
                    command.Parameters.AddWithValue("@id_cisterna", (object)idCisterna ?? DBNull.Value);
                    command.Parameters.AddWithValue("@cantidad", cantidad);
                    command.Parameters.AddWithValue("@id_transportista", (object)idTransportista ?? DBNull.Value);
                    command.Parameters.AddWithValue("@id_ayudante", (object)idAyudante ?? DBNull.Value);

                    using var reader = await command.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        return (reader.GetInt32("Resultado"), reader.GetString("Mensaje"));
                    }

                    return (0, "Error desconocido");
                }
            }
        }
        public async Task<List<Trabajador>> ObtenerTrabajadoresDisponiblesAsync(string categoria)
        {
            var trabajadores = new List<Trabajador>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_ObtenerTrabajadoresDisponibles", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@categoria", categoria);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            trabajadores.Add(new Trabajador
                            {
                                IdTrabajador = reader.GetInt32("id_trabajador"),
                                Nombre = reader.GetString("Nombre"),
                                apePaterno = reader.IsDBNull("apePaterno") ? null : reader.GetString("apePaterno"),
                                apeMaterno = reader.IsDBNull("apeMaterno") ? null : reader.GetString("apeMaterno"),
                                categoria = reader.GetString("categoria"),
                                numDoc = reader.IsDBNull("numDoc") ? null : reader.GetString("numDoc"),
                                Telefono = reader.IsDBNull("Telefono") ? null : reader.GetString("Telefono"),
                                // Otros campos que necesites
                            });
                        }
                    }
                }
            }

            return trabajadores;
        }

        // Obtener cisternas disponibles
        public async Task<List<Vehiculo>> ObtenerCisternasDisponiblesAsync()
        {
            var cisternas = new List<Vehiculo>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_ObtenerCisternasDisponibles", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            cisternas.Add(new Vehiculo
                            {
                                IdVehiculo = reader.GetInt32("id_cisterna"),
                                Placa = reader.GetString("placa"),
                                AñoFabricacion = reader.IsDBNull("AñoFabricacion") ? null : reader.GetString("AñoFabricacion"),
                                Estado = reader.GetBoolean("estado")
                            });
                        }
                    }
                }
            }

            return cisternas;
        }

        // Obtener tractos disponibles
        public async Task<List<Vehiculo>> ObtenerTractosDisponiblesAsync()
        {
            var tractos = new List<Vehiculo>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_ObtenerTractosDisponibles", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tractos.Add(new Vehiculo
                            {
                                IdVehiculo = reader.GetInt32("id_tracto"),
                                Placa = reader.GetString("placa"),
                                Modelo = reader.IsDBNull("modelo") ? null : reader.GetString("modelo"),
                                AñoFabricacion = reader.IsDBNull("AñoFabricacion") ? null : reader.GetString("AñoFabricacion"),
                                Estado = reader.GetBoolean("estado")
                            });
                        }
                    }
                }
            }

            return tractos;
        }
    }
    }


