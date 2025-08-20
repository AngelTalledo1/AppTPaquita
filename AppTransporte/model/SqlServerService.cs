using Microsoft.Data.SqlClient;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using SyncSizeF = Syncfusion.Drawing.SizeF;
using System.Data;
#pragma warning disable CS8603, CS1998, CS8625, CS8601, CS8600, CS8612, CS0612

namespace AppTransporte.model
{
    public static class DataReaderExtensions
    {


        public static bool HasColumn(this SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }

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


        // Reporte de Atención de Solicitudes por Cliente
        public (DataTable MetricasCliente, DataTable DetalleSolicitudes) ObtenerReporteAtencionSolicitudesPorCliente(
            int idCliente,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            DataTable metricasCliente = new DataTable();
            DataTable detalleSolicitudes = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_ReporteAtencionSolicitudesPorCliente", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 180;

                        // Parámetros
                        cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                        cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);

                        if (ds.Tables.Count >= 1)
                            metricasCliente = ds.Tables[0];

                        if (ds.Tables.Count >= 2)
                            detalleSolicitudes = ds.Tables[1];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerReporteAtencionSolicitudesPorCliente: {ex.Message}");
                throw;
            }

            return (metricasCliente, detalleSolicitudes);
        }

        // Reporte de Trabajadores por Cliente
        public DataTable ObtenerReporteTrabajadoresPorCliente(int idCliente)
        {
            DataTable resultado = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_ReporteTrabajadoresPorCliente", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                        cmd.CommandTimeout = 120; // Aumentar el timeout por si acaso

                        // Utilizar SqlDataReader para ver si hay datos
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            resultado.Load(reader);

                            // Registrar para debug cuántas filas se recuperaron
                            System.Diagnostics.Debug.WriteLine($"Filas recuperadas: {resultado.Rows.Count}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Registrar el error para depuración
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerReporteTrabajadoresPorCliente: {ex.Message}");
                throw; // Re-lanzar la excepción para manejarla en la capa superior
            }

            return resultado;
        }

        // Reporte de Pedidos por Cliente
        public (DataTable ResumenPedidos, DataTable DetallePedidos) ObtenerReportePedidosPorCliente(
            int idCliente,
            string tipoPedido = null)
        {
            DataTable resumenPedidos = new DataTable();
            DataTable detallePedidos = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("sp_ReportePedidosPorCliente", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                    cmd.Parameters.AddWithValue("@TipoPedido", (object)tipoPedido ?? DBNull.Value);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);

                        if (ds.Tables.Count > 0)
                            resumenPedidos = ds.Tables[0];
                        if (ds.Tables.Count > 1)
                            detallePedidos = ds.Tables[1];
                    }
                }
            }

            return (resumenPedidos, detallePedidos);
        }



        // Reporte de Desvíos por Cliente
        public (DataTable ResumenDesvios, DataTable DetalleDesvios) ObtenerReporteDesviosPorCliente(
            int idCliente,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            DataTable resumenDesvios = new DataTable();
            DataTable detalleDesvios = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("sp_ReporteDesviosPorCliente", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        resumenDesvios.Load(reader);
                        if (reader.NextResult())
                        {
                            detalleDesvios.Load(reader);
                        }
                    }
                }
            }

            return (resumenDesvios, detalleDesvios);
        }
        
        
        public async Task<List<TareaAdicionalTrabajador>> ObtenerTareasAdicionalesTrabajadorAsync(
            int idTrabajador,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            var tareas = new List<TareaAdicionalTrabajador>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_ReporteTareasAdicionalesTrabajador", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 120; // 2 minutos de timeout

                        // Parámetros
                        command.Parameters.AddWithValue("@id_usuario", idTrabajador);
                        command.Parameters.AddWithValue("@fecha_inicio", fechaInicio.Date);
                        command.Parameters.AddWithValue("@fecha_fin", fechaFin.Date);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                tareas.Add(new TareaAdicionalTrabajador
                                {
                                    IdTareaAdicional = reader.GetInt32(reader.GetOrdinal("id_tareaAdicional")),
                                    FechaTarea = reader.GetDateTime(reader.GetOrdinal("fecha_tarea")),
                                    HoraInicio = TimeSpan.Parse(reader["hora_inicio"].ToString()),
                                    HoraFin = TimeSpan.Parse(reader["hora_fin"].ToString()),
                                    Descripcion = reader.GetString(reader.GetOrdinal("descripcion")),
                                    Estado = reader.GetBoolean(reader.GetOrdinal("estado"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener tareas adicionales del trabajador: {ex.Message}");
                throw;
            }

            return tareas;
        }

        public async Task<ReporteDiarioCompleto> ObtenerReporteDiarioCompletoAsync(
            int idTrabajador,
            DateTime fecha)
        {
            var reporte = new ReporteDiarioCompleto
            {
                IdTrabajador = idTrabajador,
                Fecha = fecha.Date
            };

            try
            {
                // Obtener información del trabajador
                var trabajadores = await ObtenerTrabajadoresAsync();
                var trabajador = trabajadores.FirstOrDefault(t => t.IdTrabajador == idTrabajador);

                if (trabajador != null)
                {
                    reporte.NombreTrabajador = $"{trabajador.Nombre} {trabajador.apePaterno} {trabajador.apeMaterno}".Trim();
                    reporte.Categoria = trabajador.categoria;
                }

                // Obtener actividades diarias
                reporte.Actividades = await ObtenerActividadesDiariasTrabajadorAsync(idTrabajador, fecha);

                // Obtener tareas adicionales (solo del día seleccionado)
                var tareas = await ObtenerTareasAdicionalesTrabajadorAsync(idTrabajador, fecha, fecha);
                reporte.TareasAdicionales = tareas;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener reporte diario completo: {ex.Message}");
                throw;
            }

            return reporte;
        }


        
        public async Task<List<ReporteDiarioCompleto>> ObtenerReportePeriodoTrabajadorAsync(
            int idTrabajador,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            var reportes = new List<ReporteDiarioCompleto>();

            // Obtener información del trabajador
            var trabajadores = await ObtenerTrabajadoresAsync();
            var trabajador = trabajadores.FirstOrDefault(t => t.IdTrabajador == idTrabajador);

            if (trabajador == null)
            {
                throw new Exception($"No se encontró el trabajador con ID {idTrabajador}");
            }

            string nombreTrabajador = $"{trabajador.Nombre} {trabajador.apePaterno} {trabajador.apeMaterno}".Trim();
            string categoria = trabajador.categoria;

            // Obtener todas las tareas adicionales para el período
            var todasLasTareas = await ObtenerTareasAdicionalesTrabajadorAsync(idTrabajador, fechaInicio, fechaFin);

            // Para cada día en el período
            for (DateTime fecha = fechaInicio.Date; fecha <= fechaFin.Date; fecha = fecha.AddDays(1))
            {
                // Obtener actividades para este día
                var actividadesDia = await ObtenerActividadesDiariasTrabajadorAsync(idTrabajador, fecha);

                // Si no hay actividades ni tareas para este día, podemos saltar al siguiente
                var tareasDia = todasLasTareas.Where(t => t.FechaTarea.Date == fecha.Date).ToList();

                if (actividadesDia.Count == 0 && tareasDia.Count == 0)
                    continue;

                // Crear el reporte para este día
                var reporteDia = new ReporteDiarioCompleto
                {
                    IdTrabajador = idTrabajador,
                    NombreTrabajador = nombreTrabajador,
                    Categoria = categoria,
                    Fecha = fecha,
                    Actividades = actividadesDia,
                    TareasAdicionales = tareasDia
                };

                reportes.Add(reporteDia);
            }

            return reportes;
        }

        
        public async Task<List<ActividadDiariaTrabajador>> ObtenerActividadesDiariasTrabajadorAsync(
            int idTrabajador,
            DateTime fecha)
        {
            var actividades = new List<ActividadDiariaTrabajador>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_ReporteDiarioTrabajador", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 120; // 2 minutos de timeout

                        // Parámetros
                        command.Parameters.AddWithValue("@id_trabajador", idTrabajador);
                        command.Parameters.AddWithValue("@fecha", fecha.Date);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                actividades.Add(new ActividadDiariaTrabajador
                                {
                                    IdViaje = reader.GetInt32(reader.GetOrdinal("id_viaje")),
                                    IdPedido = reader.GetInt32(reader.GetOrdinal("id_pedido")),
                                    Cantidad = reader.IsDBNull(reader.GetOrdinal("cantidad")) ? 
                                    null : Convert.ToInt32(reader.GetValue(reader.GetOrdinal("cantidad"))),
                                    EstadoActual = reader.IsDBNull(reader.GetOrdinal("estado_actual")) ?
                                    null : reader.GetInt32(reader.GetOrdinal("estado_actual")).ToString(),
                                    FechaHora = reader.GetDateTime(reader.GetOrdinal("fechaHora")),
                                    Evidencia = reader.IsDBNull(reader.GetOrdinal("evidencia")) ?
                                        null : reader.GetString(reader.GetOrdinal("evidencia")),
                                    Comentario = reader.IsDBNull(reader.GetOrdinal("Comentario")) ?
                                        null : reader.GetString(reader.GetOrdinal("Comentario"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener actividades diarias del trabajador: {ex.Message}");
                throw;
            }

            return actividades;
        }


        // Reporte de Programación por Cliente
        public DataTable ObtenerReporteProgramacionCliente(
            int idCliente,
            string periodo = "SemanaActual")
        {
            DataTable resultado = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("sp_ReporteProgramacionCliente", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                    cmd.Parameters.AddWithValue("@Periodo", periodo);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(resultado);
                    }
                }
            }

            return resultado;
        }


        // Método mejorado para manejar sp_ReporteTrabajosPorTrabajadorCliente
        public (DataTable ResumenTrabajador, DataTable DetalleViajes) ObtenerReporteTrabajosPorTrabajadorCliente(
            int idCliente,
            int? idTrabajador = null)
        {
            DataTable resumenTrabajador = new DataTable();
            DataTable detalleViajes = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_ReporteTrabajosPorTrabajadorCliente", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 180; // Timeout de 3 minutos
                        cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                        cmd.Parameters.AddWithValue("@IdTrabajador", (object)idTrabajador ?? DBNull.Value);

                        // Usar SqlDataAdapter para obtener múltiples resultados
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);

                            if (ds.Tables.Count > 0)
                                resumenTrabajador = ds.Tables[0];

                            if (ds.Tables.Count > 1)
                                detalleViajes = ds.Tables[1];

                            System.Diagnostics.Debug.WriteLine($"ResumenTrabajador filas: {resumenTrabajador?.Rows?.Count ?? 0}");
                            System.Diagnostics.Debug.WriteLine($"DetalleViajes filas: {detalleViajes?.Rows?.Count ?? 0}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerReporteTrabajosPorTrabajadorCliente: {ex.Message}");
                throw; // Re-lanzar para manejo en la capa superior
            }

            return (resumenTrabajador, detalleViajes);
        }

        // Método para obtener el resumen como una lista de objetos tipados
        public async Task<List<TrabajadorViajeResumen>> ObtenerResumenTrabajadorViajesAsync(
            int idCliente,
            int? idTrabajador = null)
        {
            var resumenes = new List<TrabajadorViajeResumen>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_ReporteTrabajosPorTrabajadorCliente", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdCliente", idCliente);
                        command.Parameters.AddWithValue("@IdTrabajador", (object)idTrabajador ?? DBNull.Value);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            // Leer el primer conjunto de resultados (resumen)
                            while (await reader.ReadAsync())
                            {
                                resumenes.Add(new TrabajadorViajeResumen
                                {
                                    IdTrabajador = reader.GetInt32(reader.GetOrdinal("id_trabajador")),
                                    NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto")),
                                    Categoria = reader.GetString(reader.GetOrdinal("Categoria")),
                                    NumLicencia = reader.IsDBNull(reader.GetOrdinal("NumLicencia")) ?
                                        null : reader.GetString(reader.GetOrdinal("NumLicencia")),
                                    TotalViajesRealizados = reader.GetInt32(reader.GetOrdinal("TotalViajesRealizados")),
                                    TotalPedidosAtendidos = reader.GetInt32(reader.GetOrdinal("TotalPedidosAtendidos")),
                                    VolumenTotalTransportado = reader.GetDecimal(reader.GetOrdinal("VolumenTotalTransportado"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerResumenTrabajadorViajesAsync: {ex.Message}");
                throw;
            }

            return resumenes;
        }

        // Método para obtener el detalle como una lista de objetos tipados
        public async Task<List<DetalleViajeTrabajador>> ObtenerDetalleViajesTrabajadorAsync(
            int idCliente,
            int? idTrabajador = null)
        {
            var detalles = new List<DetalleViajeTrabajador>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_ReporteTrabajosPorTrabajadorCliente", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdCliente", idCliente);
                        command.Parameters.AddWithValue("@IdTrabajador", (object)idTrabajador ?? DBNull.Value);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            // Saltar el primer conjunto de resultados (resumen)
                            if (await reader.NextResultAsync())
                            {
                                // Leer el segundo conjunto de resultados (detalle de viajes)
                                while (await reader.ReadAsync())
                                {
                                    detalles.Add(new DetalleViajeTrabajador
                                    {
                                        IdViaje = reader.GetInt32(reader.GetOrdinal("id_viaje")),
                                        FechaProgramada = reader.GetDateTime(reader.GetOrdinal("fecha_programada")),
                                        Volumen = reader.GetDecimal(reader.GetOrdinal("Volumen")),
                                        Tracto = reader.IsDBNull(reader.GetOrdinal("Tracto")) ?
                                            string.Empty : reader.GetString(reader.GetOrdinal("Tracto")),
                                        Cisterna = reader.IsDBNull(reader.GetOrdinal("Cisterna")) ?
                                            string.Empty : reader.GetString(reader.GetOrdinal("Cisterna")),
                                        Origen = reader.IsDBNull(reader.GetOrdinal("Origen")) ?
                                            string.Empty : reader.GetString(reader.GetOrdinal("Origen")),
                                        Destino = reader.IsDBNull(reader.GetOrdinal("Destino")) ?
                                            string.Empty : reader.GetString(reader.GetOrdinal("Destino")),
                                        UltimoEstado = reader.IsDBNull(reader.GetOrdinal("UltimoEstado")) ?
                                            string.Empty : reader.GetString(reader.GetOrdinal("UltimoEstado")),
                                        Completado = reader.GetString(reader.GetOrdinal("Completado")) == "Sí",

                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerDetalleViajesTrabajadorAsync: {ex.Message}");
                throw;
            }

            return detalles;
        }

        // Método combinado que devuelve ambas listas en una tupla
        public async Task<(List<TrabajadorViajeResumen> Resumen, List<DetalleViajeTrabajador> Detalles)>
            ObtenerReporteTrabajadorViajesCompletoAsync(
            int idCliente,
            int? idTrabajador = null)
        {
            var resumen = await ObtenerResumenTrabajadorViajesAsync(idCliente, idTrabajador);
            var detalles = await ObtenerDetalleViajesTrabajadorAsync(idCliente, idTrabajador);

            return (resumen, detalles);


        }



        public DataTable GetTrabajadoresPorCliente(int idCliente)
        {
            DataTable dt = new DataTable();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("pa_TrabajadoresPorCliente", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdCliente", idCliente);

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
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
                using (var command = new SqlCommand("pa_ListPedidosDetConEstadoReal", connection))
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

                                // Usar el estado real de los viajes en lugar del estado del pedido
                                EstadoPedido = reader["estado_viaje_actual"]?.ToString() ?? "Sin estado",
                                ultEstado = reader["estado_viaje_actual"]?.ToString() ?? "Sin estado",

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
                using (var command = new SqlCommand("pa_ListPedidosUsuarioConEstadoReal", connection))
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

                                // Usar el estado real de los viajes en lugar del estado del pedido
                                EstadoPedido = reader["estado_viaje_actual"]?.ToString() ?? "Sin estado",
                                ultEstado = reader["estado_viaje_actual"]?.ToString() ?? "Sin estado",

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
        
        
        // REEMPLAZA COMPLETAMENTE tu método GenerarReporteTrabajadorPDF por este:
        // MÉTODO CON LOS 3 ERRORES CORREGIDOS:
        public async Task<byte[]> GenerarReporteTrabajadorPDF(
            List<ReporteTrabajador> reporteData,
            DateTime fechaInicio,
            DateTime fechaFin,
            string tipoReporte)
        {
            try
            {
                // Crear el documento PDF
                using (PdfDocument document = new PdfDocument())
                {
                    // Crear página
                    PdfPage page = document.Pages.Add();
                    PdfGraphics graphics = page.Graphics;

                    // Configurar fuentes
                    PdfStandardFont titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);
                    PdfStandardFont companyFont = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
                    PdfStandardFont headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
                    PdfStandardFont normalFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
                    PdfStandardFont boldFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
                    PdfStandardFont infoFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12);

                    // Colores
                    PdfSolidBrush redBrush = new PdfSolidBrush(new PdfColor(203, 67, 53)); // #cb4335 - Rojo empresarial
                    PdfSolidBrush blackBrush = new PdfSolidBrush(new PdfColor(0, 0, 0));
                    PdfSolidBrush grayBrush = new PdfSolidBrush(new PdfColor(85, 85, 85));

                    float yPosition = 20;

                    try
                    {
                        using var logoStream = await FileSystem.OpenAppPackageFileAsync("Resources/Raw/paquitaaa.png");
                        PdfBitmap logo = new PdfBitmap(logoStream); // 
                        graphics.DrawImage(logo, 20, yPosition, 80, 60);
                    }
                    catch
                    {
                        // Fallback
                        graphics.DrawRectangle(new PdfPen(redBrush), 20, yPosition, 80, 60);
                        graphics.DrawString("LOGO", normalFont, redBrush, 35, yPosition + 25);
                    }

                    // 2. INFORMACIÓN DE LA EMPRESA (centro)
                    float centerX = page.Size.Width / 2;

                    // Nombre de la empresa
                    Syncfusion.Drawing.SizeF companyNameSize = companyFont.MeasureString("TRANSPORTES PAQUITA S.R.L");
                    graphics.DrawString("TRANSPORTES PAQUITA S.R.L", companyFont, redBrush,
                        centerX - (companyNameSize.Width / 2), yPosition + 5);

                    // RUC
                    Syncfusion.Drawing.SizeF rucSize = infoFont.MeasureString("RUC: 20102423985");
                    graphics.DrawString("RUC: 20102423985", infoFont, blackBrush,
                        centerX - (rucSize.Width / 2), yPosition + 28);

                    // Teléfono
                    Syncfusion.Drawing.SizeF phoneSize = infoFont.MeasureString("Teléfono: 981229253");
                    graphics.DrawString("Teléfono: 981229253", infoFont, blackBrush,
                        centerX - (phoneSize.Width / 2), yPosition + 48);

                    yPosition += 80;

                    // 3. LÍNEA SEPARADORA ROJA
                    PdfPen redPen = new PdfPen(redBrush, 2);
                    graphics.DrawLine(redPen, 20, yPosition, page.Size.Width - 20, yPosition);
                    yPosition += 25;

                    // ==============================================
                    // TÍTULO DEL REPORTE
                    // ==============================================
                    Syncfusion.Drawing.SizeF titleSize = titleFont.MeasureString("REPORTE DE TRABAJADOR");
                    graphics.DrawString("REPORTE DE TRABAJADOR", titleFont, redBrush,
                        centerX - (titleSize.Width / 2), yPosition);
                    yPosition += 35;

                    // ==============================================
                    // INFORMACIÓN DEL PERIODO
                    // ==============================================
                    graphics.DrawString($"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}",
                        headerFont, blackBrush, 20, yPosition);
                    yPosition += 20;

                    graphics.DrawString($"Tipo de Reporte: {tipoReporte}",
                        normalFont, blackBrush, 20, yPosition);
                    yPosition += 30;

                    // ==============================================
                    // RESUMEN DEL REPORTE
                    // ==============================================

                    // Calcular totales
                    int totalTrabajadores = reporteData.Select(r => r.IdTrabajador).Distinct().Count();
                    int totalViajes = reporteData.Sum(r => r.TotalViajes);
                    int totalVolumen = reporteData.Sum(r => r.VolumenTransportado);

                    // Dibujar marco para resumen
                    PdfPen grayPen = new PdfPen(new PdfColor(200, 200, 200));
                    PdfSolidBrush lightGrayBrush = new PdfSolidBrush(new PdfColor(248, 249, 250));

                    graphics.DrawRectangle(grayPen, lightGrayBrush, 20, yPosition, 555, 80);

                    // Título del resumen
                    graphics.DrawString("RESUMEN DEL REPORTE", boldFont, blackBrush, 30, yPosition + 10);

                    // Línea separadora
                    graphics.DrawLine(grayPen, 30, yPosition + 25, 565, yPosition + 25);

                    // Datos del resumen en columnas - MEJORADO EL ESPACIADO
                    float col1X = 50, col2X = 220, col3X = 390; // Más separados
                    float resumenY = yPosition + 35;

                    // Columna 1: Total trabajadores
                    graphics.DrawString("Total trabajadores:", normalFont, grayBrush, col1X, resumenY);
                    graphics.DrawString(totalTrabajadores.ToString(), boldFont, blackBrush, col1X, resumenY + 15);

                    // Columna 2: Total viajes  
                    graphics.DrawString("Total viajes:", normalFont, grayBrush, col2X, resumenY);
                    graphics.DrawString(totalViajes.ToString(), boldFont, blackBrush, col2X, resumenY + 15);

                    // Columna 3: Volumen total - CENTRADO MEJOR
                    string volumenText = "Volumen total transportado:";
                    Syncfusion.Drawing.SizeF volumenTextSize = normalFont.MeasureString(volumenText);
                    graphics.DrawString(volumenText, normalFont, grayBrush, col3X, resumenY);

                    string volumenValue = $"{totalVolumen:N2} L";
                    Syncfusion.Drawing.SizeF volumenValueSize = boldFont.MeasureString(volumenValue);
                    graphics.DrawString(volumenValue, boldFont, blackBrush,
                        col3X + (volumenTextSize.Width / 2) - (volumenValueSize.Width / 2), resumenY + 15);

                    yPosition += 100;

                    // ==============================================
                    // TABLA DE DATOS
                    // ==============================================
                    if (reporteData.Any())
                    {
                        // Crear la tabla
                        PdfGrid table = new PdfGrid();

                        // Configurar columnas - ESPACIADO MEJORADO
                        table.Columns.Add(6);
                        table.Columns[0].Width = 130; // Trabajador (más ancho)
                        table.Columns[1].Width = 85;  // Categoría
                        table.Columns[2].Width = 50;  // Viajes
                        table.Columns[3].Width = 50;  // Seg.
                        table.Columns[4].Width = 85;  // Volumen (más ancho)
                        table.Columns[5].Width = 95;  // Periodo (más ancho)

                        // Estilo de encabezado
                        PdfGridRowStyle headerRowStyle = new PdfGridRowStyle();
                        headerRowStyle.BackgroundBrush = new PdfSolidBrush(new PdfColor(240, 240, 240));
                        headerRowStyle.TextBrush = new PdfSolidBrush(new PdfColor(51, 51, 51));
                        headerRowStyle.Font = boldFont;

                        // Agregar fila de encabezado
                        PdfGridRow headerRow = table.Headers.Add(1)[0];
                        headerRow.Style = headerRowStyle;
                        headerRow.Height = 25;

                        headerRow.Cells[0].Value = "Trabajador";
                        headerRow.Cells[1].Value = "Categoría";
                        headerRow.Cells[2].Value = "Viajes";
                        headerRow.Cells[3].Value = "Seg.";
                        headerRow.Cells[4].Value = "Volumen";
                        headerRow.Cells[5].Value = "Periodo";

                        // Centrar encabezados de columnas numéricas
                        headerRow.Cells[2].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                        headerRow.Cells[3].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                        headerRow.Cells[4].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                        headerRow.Cells[5].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);

                        // Agregar datos
                        for (int i = 0; i < reporteData.Count; i++)
                        {
                            var item = reporteData[i];
                            PdfGridRow row = table.Rows.Add();
                            row.Height = 22; // Altura ligeramente mayor

                            // Alternar color de filas
                            if (i % 2 == 1)
                            {
                                row.Style.BackgroundBrush = new PdfSolidBrush(new PdfColor(245, 245, 245));
                            }

                            row.Cells[0].Value = item.NombreCompleto;
                            row.Cells[1].Value = item.Categoria;
                            row.Cells[2].Value = item.TotalViajes.ToString();
                            row.Cells[3].Value = item.TotalSeguimientos.ToString();
                            row.Cells[4].Value = $"{item.VolumenTransportado:N2}";
                            row.Cells[5].Value = item.Periodo;

                            // Aplicar fuente normal a todas las celdas
                            for (int j = 0; j < row.Cells.Count; j++)
                            {
                                row.Cells[j].Style.Font = normalFont;
                                row.Cells[j].Style.TextBrush = blackBrush;
                            }

                            // Alineación mejorada
                            row.Cells[2].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                            row.Cells[3].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                            row.Cells[4].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                            row.Cells[5].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                        }

                        // Dibujar la tabla
                        PdfLayoutResult result = table.Draw(page, 20, yPosition);
                        yPosition = result.Bounds.Bottom + 20;
                    }

                   
                    string fechaGeneracion = $"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    graphics.DrawString(fechaGeneracion, normalFont, grayBrush, 20, page.Size.Height - 40);

                    // Número de página
                    string numeroPagina = $"Página 1 de 1";
                    Syncfusion.Drawing.SizeF textSize = normalFont.MeasureString(numeroPagina);
                    graphics.DrawString(numeroPagina, normalFont, grayBrush,
                        page.Size.Width - textSize.Width - 20, page.Size.Height - 40);

                    // Convertir a bytes
                    using (MemoryStream stream = new MemoryStream())
                    {
                        document.Save(stream);
                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar PDF: {ex.Message}", ex);
            }
        }

        public async Task<byte[]> GenerarReporteActividadDiariaPDF(
            ReporteDiarioCompleto reporte)
        {
            try
            {
                using (PdfDocument document = new PdfDocument())
                {
                    // Crear página
                    PdfPage page = document.Pages.Add();
                    PdfGraphics graphics = page.Graphics;

                    // Configurar fuentes
                    PdfStandardFont titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);
                    PdfStandardFont companyFont = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
                    PdfStandardFont headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
                    PdfStandardFont normalFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
                    PdfStandardFont boldFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
                    PdfStandardFont infoFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12);

                    // Colores
                    PdfSolidBrush redBrush = new PdfSolidBrush(new PdfColor(203, 67, 53));
                    PdfSolidBrush blackBrush = new PdfSolidBrush(new PdfColor(0, 0, 0));
                    PdfSolidBrush grayBrush = new PdfSolidBrush(new PdfColor(85, 85, 85));
                    PdfSolidBrush lightGrayBrush = new PdfSolidBrush(new PdfColor(248, 249, 250));

                    float yPosition = 20;

                    // Logo e información de la empresa
                    try
                    {
                        using var logoStream = await FileSystem.OpenAppPackageFileAsync("Resources/Raw/paquitaaa.png");
                        PdfBitmap logo = new PdfBitmap(logoStream);
                        graphics.DrawImage(logo, 20, yPosition, 80, 60);
                    }
                    catch
                    {
                        graphics.DrawRectangle(new PdfPen(redBrush), 20, yPosition, 80, 60);
                        graphics.DrawString("LOGO", normalFont, redBrush, 35, yPosition + 25);
                    }

                    float centerX = page.Size.Width / 2;

                    // Información de la empresa
                    SyncSizeF companyNameSize = companyFont.MeasureString("TRANSPORTES PAQUITA S.R.L");
                    graphics.DrawString("TRANSPORTES PAQUITA S.R.L", companyFont, redBrush,
                        centerX - (companyNameSize.Width / 2), yPosition + 5);

                    graphics.DrawString("RUC: 20102423985", infoFont, blackBrush,
                        centerX - (infoFont.MeasureString("RUC: 20102423985").Width / 2), yPosition + 28);

                    graphics.DrawString("Teléfono: 981229253", infoFont, blackBrush,
                        centerX - (infoFont.MeasureString("Teléfono: 981229253").Width / 2), yPosition + 48);

                    yPosition += 80;

                    // Línea separadora
                    PdfPen redPen = new PdfPen(redBrush, 2);
                    graphics.DrawLine(redPen, 20, yPosition, page.Size.Width - 20, yPosition);
                    yPosition += 25;

                    // Título del reporte
                    string titulo = "REPORTE DE ACTIVIDAD DIARIA";
                    SyncSizeF titleSize = titleFont.MeasureString(titulo);
                    graphics.DrawString(titulo, titleFont, redBrush,
                        centerX - (titleSize.Width / 2), yPosition);
                    yPosition += 35;

                    // Información del trabajador
                    graphics.DrawString($"Trabajador: {reporte.NombreTrabajador}", headerFont, blackBrush, 20, yPosition);
                    yPosition += 25;
                    graphics.DrawString($"Categoría: {reporte.Categoria}", normalFont, blackBrush, 20, yPosition);
                    yPosition += 20;
                    graphics.DrawString($"Fecha: {reporte.Fecha:dd/MM/yyyy}", normalFont, blackBrush, 20, yPosition);
                    yPosition += 30;

                    // Tabla de actividades
                    if (reporte.Actividades.Any())
                    {
                        PdfGrid table = new PdfGrid();
                        table.Columns.Add(6);

                        // Configurar anchos de columna
                        table.Columns[0].Width = 60;  // Hora
                        table.Columns[1].Width = 80;  // ID Viaje
                        table.Columns[2].Width = 80;  // ID Pedido
                        table.Columns[3].Width = 100; // Cantidad
                        table.Columns[4].Width = 120; // Estado
                        table.Columns[5].Width = 100; // Evidencia

                        // Estilo de encabezado
                        PdfGridRowStyle headerStyle = new PdfGridRowStyle();
                        headerStyle.BackgroundBrush = new PdfSolidBrush(new PdfColor(240, 240, 240));
                        headerStyle.TextBrush = blackBrush;
                        headerStyle.Font = boldFont;

                        // Encabezados
                        PdfGridRow headerRow = table.Headers.Add(1)[0];
                        headerRow.Style = headerStyle;
                        headerRow.Height = 25;

                        headerRow.Cells[0].Value = "Hora";
                        headerRow.Cells[1].Value = "ID Viaje";
                        headerRow.Cells[2].Value = "ID Pedido";
                        headerRow.Cells[3].Value = "Cantidad";
                        headerRow.Cells[4].Value = "Estado";
                        headerRow.Cells[5].Value = "Evidencia";

                        // Centrar encabezados
                        foreach (PdfGridCell cell in headerRow.Cells)
                        {
                            cell.Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                        }

                        // Agregar datos
                        foreach (var actividad in reporte.Actividades.OrderBy(a => a.FechaHora))
                        {
                            PdfGridRow row = table.Rows.Add();
                            row.Height = 22;

                            row.Cells[0].Value = actividad.FechaHora.ToString("HH:mm");
                            row.Cells[1].Value = actividad.IdViaje.ToString();
                            row.Cells[2].Value = actividad.IdPedido.ToString();
                            row.Cells[3].Value = actividad.Cantidad?.ToString() ?? "-";
                            row.Cells[4].Value = actividad.EstadoActual ?? "-";
                            row.Cells[5].Value = !string.IsNullOrEmpty(actividad.Evidencia) ? "Sí" : "No";

                            // Aplicar estilos a las celdas
                            foreach (PdfGridCell cell in row.Cells)
                            {
                                cell.Style.Font = normalFont;
                                cell.Style.TextBrush = blackBrush;
                                cell.Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                            }
                        }

                        // Dibujar la tabla
                        table.Draw(page, 20, yPosition);
                    }

                    // Pie de página
                    string fechaGeneracion = $"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    graphics.DrawString(fechaGeneracion, normalFont, grayBrush, 20, page.Size.Height - 40);

                    string numeroPagina = $"Página 1 de 1";
                    SyncSizeF textSize = normalFont.MeasureString(numeroPagina);
                    graphics.DrawString(numeroPagina, normalFont, grayBrush,
                        page.Size.Width - textSize.Width - 20, page.Size.Height - 40);

                    // Generar PDF
                    using (MemoryStream stream = new MemoryStream())
                    {
                        document.Save(stream);
                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar PDF de actividad diaria: {ex.Message}", ex);
            }
        }

        public async Task<byte[]> GenerarReporteTareasAdicionalesPDF(
            List<TareaAdicionalTrabajador> tareasReporte,
            string nombreTrabajador,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            try
            {
                using (PdfDocument document = new PdfDocument())
                {
                    // Crear página
                    PdfPage page = document.Pages.Add();
                    PdfGraphics graphics = page.Graphics;

                    // Configurar fuentes
                    PdfStandardFont titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);
                    PdfStandardFont companyFont = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
                    PdfStandardFont headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
                    PdfStandardFont normalFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
                    PdfStandardFont boldFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
                    PdfStandardFont infoFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12);

                    // Colores
                    PdfSolidBrush redBrush = new PdfSolidBrush(new PdfColor((byte)203, (byte)67, (byte)53));
                    PdfSolidBrush blackBrush = new PdfSolidBrush(new PdfColor((byte)0, (byte)0, (byte)0));
                    PdfSolidBrush grayBrush = new PdfSolidBrush(new PdfColor((byte)85, (byte)85, (byte)85));
                    PdfSolidBrush lightGrayBrush = new PdfSolidBrush(new PdfColor((byte)248, (byte)249, (byte)250));

                    float yPosition = 20;

                    // Logo e información de la empresa
                    try
                    {
                        using var logoStream = await FileSystem.OpenAppPackageFileAsync("Resources/Raw/paquitaaa.png");
                        PdfBitmap logo = new PdfBitmap(logoStream);
                        graphics.DrawImage(logo, 20, yPosition, 80, 60);
                    }
                    catch
                    {
                        graphics.DrawRectangle(new PdfPen(redBrush), 20, yPosition, 80, 60);
                        graphics.DrawString("LOGO", normalFont, redBrush, 35, yPosition + 25);
                    }

                    float centerX = page.Size.Width / 2;

                    // Información de la empresa
                    SyncSizeF companyNameSize = companyFont.MeasureString("TRANSPORTES PAQUITA S.R.L");
                    graphics.DrawString("TRANSPORTES PAQUITA S.R.L", companyFont, redBrush,
                        centerX - (companyNameSize.Width / 2), yPosition + 5);

                    graphics.DrawString("RUC: 20102423985", infoFont, blackBrush,
                        centerX - (infoFont.MeasureString("RUC: 20102423985").Width / 2), yPosition + 28);

                    graphics.DrawString("Teléfono: 981229253", infoFont, blackBrush,
                        centerX - (infoFont.MeasureString("Teléfono: 981229253").Width / 2), yPosition + 48);

                    yPosition += 80;

                    // Línea separadora
                    PdfPen redPen = new PdfPen(redBrush, 2);
                    graphics.DrawLine(redPen, 20, yPosition, page.Size.Width - 20, yPosition);
                    yPosition += 25;

                    // Título del reporte
                    string titulo = "REPORTE DE TAREAS ADICIONALES";
                    SyncSizeF titleSize = titleFont.MeasureString(titulo);
                    graphics.DrawString(titulo, titleFont, redBrush,
                        centerX - (titleSize.Width / 2), yPosition);
                    yPosition += 35;

                    // Información del trabajador y período
                    graphics.DrawString($"Trabajador: {nombreTrabajador}", headerFont, blackBrush, 20, yPosition);
                    yPosition += 25;
                    graphics.DrawString($"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}", normalFont, blackBrush, 20, yPosition);
                    yPosition += 30;

                    // Resumen de tareas
                    PdfPen grayPen = new PdfPen(new PdfColor((byte)200, (byte)200, (byte)200));
                    graphics.DrawRectangle(grayPen, lightGrayBrush, 20, yPosition, 555, 80);

                    // Título del resumen
                    graphics.DrawString("RESUMEN DE TAREAS", boldFont, blackBrush, 30, yPosition + 10);
                    graphics.DrawLine(grayPen, 30, yPosition + 25, 565, yPosition + 25);

                    // Datos del resumen
                    int totalTareas = tareasReporte.Count;
                    int tareasCompletadas = tareasReporte.Count(t => t.Estado);
                    int tareasPendientes = totalTareas - tareasCompletadas;

                    float col1X = 50, col2X = 220, col3X = 390;
                    float resumenY = yPosition + 35;

                    graphics.DrawString("Total tareas:", normalFont, grayBrush, col1X, resumenY);
                    graphics.DrawString(totalTareas.ToString(), boldFont, blackBrush, col1X, resumenY + 15);

                    graphics.DrawString("Completadas:", normalFont, grayBrush, col2X, resumenY);
                    graphics.DrawString(tareasCompletadas.ToString(), boldFont, blackBrush, col2X, resumenY + 15);

                    graphics.DrawString("Pendientes:", normalFont, grayBrush, col3X, resumenY);
                    graphics.DrawString(tareasPendientes.ToString(), boldFont, blackBrush, col3X, resumenY + 15);

                    yPosition += 100;

                    // Tabla de tareas
                    if (tareasReporte.Any())
                    {
                        PdfGrid table = new PdfGrid();
                        table.Columns.Add(5);

                        // Configurar anchos de columna
                        table.Columns[0].Width = 85;  // Fecha
                        table.Columns[1].Width = 85;  // Horario
                        table.Columns[2].Width = 200; // Descripción
                        table.Columns[3].Width = 85;  // Duración
                        table.Columns[4].Width = 85;  // Estado

                        // Estilo de encabezado
                        PdfGridRowStyle headerStyle = new PdfGridRowStyle();
                        headerStyle.BackgroundBrush = new PdfSolidBrush(new PdfColor((byte)240, (byte)240, (byte)240));
                        headerStyle.TextBrush = blackBrush;
                        headerStyle.Font = boldFont;

                        // Encabezados
                        PdfGridRow headerRow = table.Headers.Add(1)[0];
                        headerRow.Style = headerStyle;
                        headerRow.Height = 25;

                        headerRow.Cells[0].Value = "Fecha";
                        headerRow.Cells[1].Value = "Horario";
                        headerRow.Cells[2].Value = "Descripción";
                        headerRow.Cells[3].Value = "Duración";
                        headerRow.Cells[4].Value = "Estado";

                        // Centrar encabezados
                        foreach (PdfGridCell cell in headerRow.Cells)
                        {
                            cell.Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                        }

                        // Agregar datos ordenados por fecha y hora
                        foreach (var tarea in tareasReporte.OrderBy(t => t.FechaTarea).ThenBy(t => t.HoraInicio))
                        {
                            PdfGridRow row = table.Rows.Add();
                            row.Height = 22;

                            row.Cells[0].Value = tarea.FechaTarea.ToString("dd/MM/yyyy");
                            row.Cells[1].Value = $"{tarea.HoraInicio:hh\\:mm} - {tarea.HoraFin:hh\\:mm}";
                            row.Cells[2].Value = tarea.Descripcion;
                            row.Cells[3].Value = tarea.DuracionFormateada;
                            row.Cells[4].Value = tarea.Estado ? "Completada" : "Pendiente";

                            // Aplicar estilos a las celdas
                            foreach (PdfGridCell cell in row.Cells)
                            {
                                cell.Style.Font = normalFont;
                                cell.Style.TextBrush = blackBrush;
                            }

                            // Alineación específica
                            row.Cells[0].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                            row.Cells[1].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                            row.Cells[2].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Left);
                            row.Cells[3].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                            row.Cells[4].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                        }

                        // Dibujar la tabla
                        table.Draw(page, 20, yPosition);
                    }

                    // Pie de página
                    string fechaGeneracion = $"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    graphics.DrawString(fechaGeneracion, normalFont, grayBrush, 20, page.Size.Height - 40);

                    string numeroPagina = $"Página 1 de 1";
                    SyncSizeF textSize = normalFont.MeasureString(numeroPagina);
                    graphics.DrawString(numeroPagina, normalFont, grayBrush,
                        page.Size.Width - textSize.Width - 20, page.Size.Height - 40);

                    // Generar PDF
                    using (MemoryStream stream = new MemoryStream())
                    {
                        document.Save(stream);
                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar PDF de tareas adicionales: {ex.Message}", ex);
            }
        }

        public async Task<byte[]> GenerarReporteServiciosPDF(
            List<ReporteServicio> datosReporte,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            try
            {
                using (PdfDocument document = new PdfDocument())
                {
                    // Crear página
                    PdfPage page = document.Pages.Add();
                    PdfGraphics graphics = page.Graphics;

                    // Configurar fuentes
                    PdfStandardFont titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);
                    PdfStandardFont companyFont = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
                    PdfStandardFont headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
                    PdfStandardFont normalFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
                    PdfStandardFont boldFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
                    PdfStandardFont infoFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12);

                    // Colores corporativos
                    PdfSolidBrush redBrush = new PdfSolidBrush(new PdfColor((byte)203, (byte)67, (byte)53)); // #cb4335
                    PdfSolidBrush blackBrush = new PdfSolidBrush(new PdfColor((byte)0, (byte)0, (byte)0));
                    PdfSolidBrush grayBrush = new PdfSolidBrush(new PdfColor((byte)85, (byte)85, (byte)85));
                    PdfSolidBrush whiteBrush = new PdfSolidBrush(new PdfColor((byte)255, (byte)255, (byte)255));

                    float yPosition = 20;

                    // ==============================================
                    // ENCABEZADO CORPORATIVO
                    // ==============================================

                    // 1. LOGO (izquierda)
                    try
                    {
                        using var logoStream = await FileSystem.OpenAppPackageFileAsync("Resources/Raw/paquitaaa.png");
                        PdfBitmap logo = new PdfBitmap(logoStream);
                        graphics.DrawImage(logo, 20, yPosition, 80, 60);
                    }
                    catch
                    {
                        // Fallback si no se encuentra el logo
                        graphics.DrawRectangle(new PdfPen(redBrush), 20, yPosition, 80, 60);
                        graphics.DrawString("LOGO", normalFont, redBrush, 35, yPosition + 25);
                    }

                    // 2. INFORMACIÓN DE LA EMPRESA (centro)
                    float centerX = page.Size.Width / 2;

                    // Nombre de la empresa
                    SyncSizeF companyNameSize = companyFont.MeasureString("TRANSPORTES PAQUITA S.R.L");
                    graphics.DrawString("TRANSPORTES PAQUITA S.R.L", companyFont, redBrush,
                        centerX - (companyNameSize.Width / 2), yPosition + 5);

                    // RUC
                    SyncSizeF rucSize = infoFont.MeasureString("RUC: 20102423985");
                    graphics.DrawString("RUC: 20102423985", infoFont, blackBrush,
                        centerX - (rucSize.Width / 2), yPosition + 28);

                    // Teléfono
                    SyncSizeF phoneSize = infoFont.MeasureString("Teléfono: 981229253");
                    graphics.DrawString("Teléfono: 981229253", infoFont, blackBrush,
                        centerX - (phoneSize.Width / 2), yPosition + 48);

                    yPosition += 80;

                    // 3. LÍNEA SEPARADORA ROJA
                    PdfPen redPen = new PdfPen(redBrush, 2);
                    graphics.DrawLine(redPen, 20, yPosition, page.Size.Width - 20, yPosition);
                    yPosition += 25;

                    // ==============================================
                    // TÍTULO DEL REPORTE
                    // ==============================================
                    SyncSizeF titleSize = titleFont.MeasureString("REPORTE DE SERVICIOS");
                    graphics.DrawString("REPORTE DE SERVICIOS", titleFont, redBrush,
                        centerX - (titleSize.Width / 2), yPosition);
                    yPosition += 35;

                    // ==============================================
                    // INFORMACIÓN DEL PERIODO
                    // ==============================================
                    graphics.DrawString($"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}",
                        headerFont, blackBrush, 20, yPosition);
                    yPosition += 20;

                    graphics.DrawString($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}",
                        normalFont, grayBrush, 20, yPosition);
                    yPosition += 30;

                    // ==============================================
                    // RESUMEN EJECUTIVO
                    // ==============================================
                    if (datosReporte != null && datosReporte.Any())
                    {
                        // Calcular totales
                        int totalServicios = datosReporte.Count;
                        int totalPedidos = datosReporte.Sum(r => r.CantidadPedidos);
                        int volumenTotalSolicitado = datosReporte.Sum(r => r.VolumenSolicitado);
                        int volumenTotalTransportado = datosReporte.Sum(r => r.VolumenTransportado);
                        double promedioEficiencia = datosReporte.Average(r => r.PorcentajeCumplimiento);

                        // Marco para resumen
                        PdfPen grayPen = new PdfPen(new PdfColor((byte)200, (byte)200, (byte)200));
                        PdfSolidBrush lightGrayBrush = new PdfSolidBrush(new PdfColor((byte)248, (byte)249, (byte)250));

                        graphics.DrawRectangle(grayPen, lightGrayBrush, 20, yPosition, 555, 120);

                        // Título del resumen
                        graphics.DrawString("RESUMEN EJECUTIVO", boldFont, blackBrush, 30, yPosition + 12);

                        // Línea separadora
                        graphics.DrawLine(grayPen, 30, yPosition + 30, 565, yPosition + 30);

                        // Distribución en 2x2 con mejor espaciado
                        float margenIzq = 40;
                        float anchoColumna = 260;
                        float alturaFila = 35;

                        // Columna 1 - Fila 1: Total servicios
                        float col1X = margenIzq;
                        float fila1Y = yPosition + 45;

                        graphics.DrawString("Total servicios:", normalFont, grayBrush, col1X, fila1Y);
                        graphics.DrawString(totalServicios.ToString(),
                            new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold),
                            redBrush, col1X, fila1Y + 15);

                        // Columna 2 - Fila 1: Total pedidos
                        float col2X = col1X + anchoColumna;

                        graphics.DrawString("Total pedidos:", normalFont, grayBrush, col2X, fila1Y);
                        graphics.DrawString(totalPedidos.ToString(),
                            new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold),
                            redBrush, col2X, fila1Y + 15);

                        // Columna 1 - Fila 2: Volumen transportado
                        float fila2Y = fila1Y + alturaFila;

                        graphics.DrawString("Volumen transportado:", normalFont, grayBrush, col1X, fila2Y);
                        graphics.DrawString($"{volumenTotalTransportado:N0} L",
                            new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold),
                            redBrush, col1X, fila2Y + 15);

                        // Columna 2 - Fila 2: Eficiencia promedio
                        graphics.DrawString("Eficiencia promedio:", normalFont, grayBrush, col2X, fila2Y);
                        graphics.DrawString($"{promedioEficiencia:N1}%",
                            new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold),
                            redBrush, col2X, fila2Y + 15);

                        yPosition += 140;
                    }

                    // ==============================================
                    // TABLA DE DETALLE DE SERVICIOS
                    // ==============================================
                    if (datosReporte != null && datosReporte.Any())
                    {
                        graphics.DrawString("DETALLE POR TIPO DE SERVICIO", headerFont, blackBrush, 20, yPosition);
                        yPosition += 25;

                        // Crear tabla de detalle
                        PdfGrid detalleTable = new PdfGrid();
                        detalleTable.Columns.Add(5);

                        // Anchos de columna optimizados
                        detalleTable.Columns[0].Width = 160; // Tipo de Servicio
                        detalleTable.Columns[1].Width = 80;  // Pedidos
                        detalleTable.Columns[2].Width = 90;  // Vol. Solicitado
                        detalleTable.Columns[3].Width = 90;  // Vol. Transportado
                        detalleTable.Columns[4].Width = 95;  // % Cumplimiento

                        // Estilo de encabezado
                        PdfGridRowStyle headerRowStyle = new PdfGridRowStyle();
                        headerRowStyle.BackgroundBrush = redBrush;
                        headerRowStyle.TextBrush = whiteBrush;
                        headerRowStyle.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);

                        // Agregar fila de encabezado
                        PdfGridRow headerRow = detalleTable.Headers.Add(1)[0];
                        headerRow.Style = headerRowStyle;
                        headerRow.Height = 28;

                        headerRow.Cells[0].Value = "Tipo de Servicio";
                        headerRow.Cells[1].Value = "Pedidos";
                        headerRow.Cells[2].Value = "Vol. Solicitado";
                        headerRow.Cells[3].Value = "Vol. Transportado";
                        headerRow.Cells[4].Value = "% Cumplimiento";

                        // Alineación de encabezados
                        headerRow.Cells[0].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Left);
                        headerRow.Cells[1].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                        headerRow.Cells[2].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                        headerRow.Cells[3].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                        headerRow.Cells[4].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);

                        // Agregar datos
                        for (int i = 0; i < datosReporte.Count; i++)
                        {
                            var item = datosReporte[i];
                            PdfGridRow row = detalleTable.Rows.Add();
                            row.Height = 22;

                            // Alternar color de filas
                            if (i % 2 == 1)
                            {
                                row.Style.BackgroundBrush = new PdfSolidBrush(new PdfColor((byte)245, (byte)245, (byte)245));
                            }

                            row.Cells[0].Value = item.TipoServicio;
                            row.Cells[1].Value = item.CantidadPedidos.ToString();
                            row.Cells[2].Value = $"{item.VolumenSolicitado:N0} L";
                            row.Cells[3].Value = $"{item.VolumenTransportado:N0} L";
                            row.Cells[4].Value = $"{item.PorcentajeCumplimiento:N1}%";

                            // Aplicar fuente y alineación
                            for (int j = 0; j < row.Cells.Count; j++)
                            {
                                row.Cells[j].Style.Font = normalFont;
                                row.Cells[j].Style.TextBrush = blackBrush;
                            }

                            // Alineación de celdas
                            row.Cells[0].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Left);
                            row.Cells[1].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                            row.Cells[2].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                            row.Cells[3].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                            row.Cells[4].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);

                            // Color especial para % Cumplimiento según eficiencia
                            if (item.PorcentajeCumplimiento >= 90)
                            {
                                row.Cells[4].Style.TextBrush = new PdfSolidBrush(new PdfColor((byte)34, (byte)139, (byte)34)); // Verde
                            }
                            else if (item.PorcentajeCumplimiento >= 70)
                            {
                                row.Cells[4].Style.TextBrush = new PdfSolidBrush(new PdfColor((byte)255, (byte)140, (byte)0)); // Naranja
                            }
                            else
                            {
                                row.Cells[4].Style.TextBrush = new PdfSolidBrush(new PdfColor((byte)220, (byte)20, (byte)60)); // Rojo
                            }
                        }

                        // Dibujar la tabla
                        PdfLayoutResult result = detalleTable.Draw(page, 20, yPosition);
                        yPosition = result.Bounds.Bottom + 20;

                        // Agregar leyenda de colores para % Cumplimiento
                        graphics.DrawString("Leyenda:", boldFont, blackBrush, 20, yPosition);
                        yPosition += 15;

                        // Verde - Excelente (SINTAXIS SYNCFUSION CORRECTA)
                        PdfSolidBrush verdeBrush = new PdfSolidBrush(new PdfColor((byte)34, (byte)139, (byte)34));
                        graphics.DrawRectangle(verdeBrush, 20, yPosition, 12, 12);
                        graphics.DrawString("≥ 90% - Excelente", normalFont, blackBrush, 40, yPosition + 2);

                        // Naranja - Bueno (SINTAXIS SYNCFUSION CORRECTA)
                        PdfSolidBrush naranjaBrush = new PdfSolidBrush(new PdfColor((byte)255, (byte)140, (byte)0));
                        graphics.DrawRectangle(naranjaBrush, 150, yPosition, 12, 12);
                        graphics.DrawString("70-89% - Bueno", normalFont, blackBrush, 170, yPosition + 2);

                        // Rojo - Mejorable (SINTAXIS SYNCFUSION CORRECTA)
                        PdfSolidBrush rojoBrush = new PdfSolidBrush(new PdfColor((byte)220, (byte)20, (byte)60));
                        graphics.DrawRectangle(rojoBrush, 280, yPosition, 12, 12);
                        graphics.DrawString("< 70% - Mejorable", normalFont, blackBrush, 300, yPosition + 2);
                    }

                    // ==============================================
                    // PIE DE PÁGINA
                    // ==============================================
                    string fechaGeneracion = $"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    graphics.DrawString(fechaGeneracion, normalFont, grayBrush, 20, page.Size.Height - 40);

                    // Número de página
                    string numeroPagina = $"Página 1 de 1";
                    SyncSizeF textSize = normalFont.MeasureString(numeroPagina);
                    graphics.DrawString(numeroPagina, normalFont, grayBrush,
                        page.Size.Width - textSize.Width - 20, page.Size.Height - 40);

                    // Convertir a bytes
                    using (MemoryStream stream = new MemoryStream())
                    {
                        document.Save(stream);
                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar PDF de servicios: {ex.Message}", ex);
            }
        }
        
        public async Task<byte[]> GenerarReportePedidosPDF(
            List<ResumenPedido> datosResumen,
            List<DetallePedido> datosDetalle,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            try
            {
                using (PdfDocument document = new PdfDocument())
                {
                    // Crear página
                    PdfPage page = document.Pages.Add();
                    PdfGraphics graphics = page.Graphics;

                    // Configurar fuentes
                    PdfStandardFont titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);
                    PdfStandardFont companyFont = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
                    PdfStandardFont headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
                    PdfStandardFont normalFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
                    PdfStandardFont boldFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
                    PdfStandardFont infoFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12);

                    // Colores - CORREGIDO: usar byte en lugar de int
                    PdfSolidBrush redBrush = new PdfSolidBrush(new PdfColor((byte)203, (byte)67, (byte)53)); // #cb4335
                    PdfSolidBrush blackBrush = new PdfSolidBrush(new PdfColor((byte)0, (byte)0, (byte)0));
                    PdfSolidBrush grayBrush = new PdfSolidBrush(new PdfColor((byte)85, (byte)85, (byte)85));
                    PdfSolidBrush whiteBrush = new PdfSolidBrush(new PdfColor((byte)255, (byte)255, (byte)255));

                    float yPosition = 20;

                    // ==============================================
                    // ENCABEZADO DE LA EMPRESA
                    // ==============================================

                    // 1. LOGO (izquierda)
                    try
                    {
                        using var logoStream = await FileSystem.OpenAppPackageFileAsync("Resources/Raw/paquitaaa.png");
                        PdfBitmap logo = new PdfBitmap(logoStream);
                        graphics.DrawImage(logo, 20, yPosition, 80, 60);
                    }
                    catch
                    {
                        // Fallback si no se encuentra el logo
                        graphics.DrawRectangle(new PdfPen(redBrush), 20, yPosition, 80, 60);
                        graphics.DrawString("LOGO", normalFont, redBrush, 35, yPosition + 25);
                    }

                    // 2. INFORMACIÓN DE LA EMPRESA (centro)
                    float centerX = page.Size.Width / 2;

                    // Nombre de la empresa
                    SyncSizeF companyNameSize = companyFont.MeasureString("TRANSPORTES PAQUITA S.R.L");
                    graphics.DrawString("TRANSPORTES PAQUITA S.R.L", companyFont, redBrush,
                        centerX - (companyNameSize.Width / 2), yPosition + 5);

                    // RUC
                    SyncSizeF rucSize = infoFont.MeasureString("RUC: 20102423985");
                    graphics.DrawString("RUC: 20102423985", infoFont, blackBrush,
                        centerX - (rucSize.Width / 2), yPosition + 28);

                    // Teléfono
                    SyncSizeF phoneSize = infoFont.MeasureString("Teléfono: 981229253");
                    graphics.DrawString("Teléfono: 981229253", infoFont, blackBrush,
                        centerX - (phoneSize.Width / 2), yPosition + 48);

                    yPosition += 80;

                    // 3. LÍNEA SEPARADORA ROJA
                    PdfPen redPen = new PdfPen(redBrush, 2);
                    graphics.DrawLine(redPen, 20, yPosition, page.Size.Width - 20, yPosition);
                    yPosition += 25;

                    
                    // TÍTULO DEL REPORTE

                    SyncSizeF titleSize = titleFont.MeasureString("REPORTE DE PEDIDOS");
                    graphics.DrawString("REPORTE DE PEDIDOS", titleFont, redBrush,
                        centerX - (titleSize.Width / 2), yPosition);
                    yPosition += 35;

                    // INFORMACIÓN DEL PERIODO
                    
                    graphics.DrawString($"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}",
                        headerFont, blackBrush, 20, yPosition);
                    yPosition += 20;

                    graphics.DrawString($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}",
                        normalFont, grayBrush, 20, yPosition);
                    yPosition += 30;

                    // RESUMEN EJECUTIVO

                    if (datosResumen != null && datosResumen.Any())
                    {
                        // Calcular totales
                        int totalPedidos = datosResumen.Sum(r => r.CantidadPedidos);
                        int volumenTotal = datosResumen.Sum(r => r.VolumenTotal);
                        int pedidosEntregados = datosResumen.Sum(r => r.PedidosEntregados);
                        double promedioAtencion = datosResumen.Average(r => r.PromedioDiasAtencion);

                        // Marco para resumen - MÁS ALTO Y MEJOR DISTRIBUIDO
                        PdfPen grayPen = new PdfPen(new PdfColor((byte)200, (byte)200, (byte)200));
                        PdfSolidBrush lightGrayBrush = new PdfSolidBrush(new PdfColor((byte)248, (byte)249, (byte)250));

                        // Altura aumentada de 90 a 120
                        graphics.DrawRectangle(grayPen, lightGrayBrush, 20, yPosition, 555, 120);

                        // Título del resumen
                        graphics.DrawString("RESUMEN EJECUTIVO", boldFont, blackBrush, 30, yPosition + 12);

                        // Línea separadora
                        graphics.DrawLine(grayPen, 30, yPosition + 30, 565, yPosition + 30);

                        // NUEVA DISTRIBUCIÓN: 2x2 con mejor espaciado
                        float margenIzq = 40;
                        float anchoColumna = 260; // Ancho de cada columna
                        float alturaFila = 35;    // Altura de cada fila

                        // Columna 1 - Fila 1: Total pedidos
                        float col1X = margenIzq;
                        float fila1Y = yPosition + 45;

                        graphics.DrawString("Total pedidos:", normalFont, grayBrush, col1X, fila1Y);
                        graphics.DrawString(totalPedidos.ToString(),
                            new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold),
                            redBrush, col1X, fila1Y + 15);

                        // Columna 2 - Fila 1: Volumen total
                        float col2X = col1X + anchoColumna;

                        graphics.DrawString("Volumen total:", normalFont, grayBrush, col2X, fila1Y);
                        graphics.DrawString($"{volumenTotal:N0} L",
                            new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold),
                            redBrush, col2X, fila1Y + 15);

                        // Columna 1 - Fila 2: Pedidos entregados
                        float fila2Y = fila1Y + alturaFila;

                        graphics.DrawString("Pedidos entregados:", normalFont, grayBrush, col1X, fila2Y);
                        graphics.DrawString($"{pedidosEntregados:N0}",
                            new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold),
                            redBrush, col1X, fila2Y + 15);

                        // Columna 2 - Fila 2: Promedio días atención
                        graphics.DrawString("Promedio días atención:", normalFont, grayBrush, col2X, fila2Y);
                        graphics.DrawString($"{promedioAtencion:N1} días",
                            new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold),
                            redBrush, col2X, fila2Y + 15);

                        yPosition += 140; // Aumentado el espacio total
                    }

                    // TABLA DE RESUMEN POR ESTADO

                    if (datosResumen != null && datosResumen.Any())
                    {
                        graphics.DrawString("RESUMEN POR ESTADO DE PEDIDOS", headerFont, blackBrush, 20, yPosition);
                        yPosition += 25;

                        // Crear tabla de resumen
                        PdfGrid resumenTable = new PdfGrid();
                        resumenTable.Columns.Add(6);
                        resumenTable.Columns[0].Width = 100; // Estado
                        resumenTable.Columns[1].Width = 70;  // Cantidad
                        resumenTable.Columns[2].Width = 80;  // Volumen
                        resumenTable.Columns[3].Width = 70;  // Viajes
                        resumenTable.Columns[4].Width = 80;  // Entregados
                        resumenTable.Columns[5].Width = 95;  // Promedio días

                        // Estilo de encabezado - CORREGIDO
                        PdfGridRowStyle headerRowStyle = new PdfGridRowStyle();
                        headerRowStyle.BackgroundBrush = redBrush;
                        headerRowStyle.TextBrush = whiteBrush; // Usar whiteBrush en lugar de conversión
                        headerRowStyle.Font = boldFont;

                        // Agregar fila de encabezado
                        PdfGridRow headerRow = resumenTable.Headers.Add(1)[0];
                        headerRow.Style = headerRowStyle;
                        headerRow.Height = 25;

                        headerRow.Cells[0].Value = "Estado";
                        headerRow.Cells[1].Value = "Pedidos";
                        headerRow.Cells[2].Value = "Volumen (L)";
                        headerRow.Cells[3].Value = "Viajes";
                        headerRow.Cells[4].Value = "Entregados";
                        headerRow.Cells[5].Value = "Prom. Días";

                        // Centrar encabezados
                        for (int i = 1; i < 6; i++)
                        {
                            headerRow.Cells[i].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                        }

                        // Agregar datos
                        for (int i = 0; i < datosResumen.Count; i++)
                        {
                            var item = datosResumen[i];
                            PdfGridRow row = resumenTable.Rows.Add();
                            row.Height = 22;

                            // Alternar color de filas - CORREGIDO
                            if (i % 2 == 1)
                            {
                                row.Style.BackgroundBrush = new PdfSolidBrush(new PdfColor((byte)245, (byte)245, (byte)245));
                            }

                            row.Cells[0].Value = item.EstadoPedido;
                            row.Cells[1].Value = item.CantidadPedidos.ToString();
                            row.Cells[2].Value = $"{item.VolumenTotal:N0}";
                            row.Cells[3].Value = item.ViajesSolicitados.ToString();
                            row.Cells[4].Value = item.PedidosEntregados.ToString();
                            row.Cells[5].Value = $"{item.PromedioDiasAtencion:N1}";

                            // Aplicar fuente y alineación
                            for (int j = 0; j < row.Cells.Count; j++)
                            {
                                row.Cells[j].Style.Font = normalFont;
                                row.Cells[j].Style.TextBrush = blackBrush;
                                if (j > 0) // Centrar columnas numéricas
                                {
                                    row.Cells[j].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                                }
                            }
                        }

                        // Dibujar la tabla
                        PdfLayoutResult result = resumenTable.Draw(page, 20, yPosition);
                        yPosition = result.Bounds.Bottom + 30;
                    }

                    // ==============================================
                    // VERIFICAR SI NECESITAMOS NUEVA PÁGINA
                    // ==============================================
                    if (yPosition > page.Size.Height - 200) // Si queda poco espacio
                    {
                        page = document.Pages.Add();
                        graphics = page.Graphics;
                        yPosition = 20;
                    }

                    // ==============================================
                    // TABLA DE DETALLE DE PEDIDOS
                    // ==============================================
                    if (datosDetalle != null && datosDetalle.Any())
                    {
                        graphics.DrawString("DETALLE DE PEDIDOS", headerFont, blackBrush, 20, yPosition);
                        yPosition += 25;

                        // Crear tabla de detalle
                        PdfGrid detalleTable = new PdfGrid();
                        detalleTable.Columns.Add(7);
                        detalleTable.Columns[0].Width = 40;  // ID
                        detalleTable.Columns[1].Width = 90;  // Cliente
                        detalleTable.Columns[2].Width = 85;  // Origen
                        detalleTable.Columns[3].Width = 85;  // Destino
                        detalleTable.Columns[4].Width = 70;  // Estado
                        detalleTable.Columns[5].Width = 60;  // Volumen
                        detalleTable.Columns[6].Width = 65;  // Viajes

                        // Estilo de encabezado - CORREGIDO
                        PdfGridRowStyle detalleHeaderStyle = new PdfGridRowStyle();
                        detalleHeaderStyle.BackgroundBrush = new PdfSolidBrush(new PdfColor((byte)240, (byte)240, (byte)240));
                        detalleHeaderStyle.TextBrush = blackBrush;
                        detalleHeaderStyle.Font = boldFont;

                        // Agregar fila de encabezado
                        PdfGridRow detalleHeaderRow = detalleTable.Headers.Add(1)[0];
                        detalleHeaderRow.Style = detalleHeaderStyle;
                        detalleHeaderRow.Height = 25;

                        detalleHeaderRow.Cells[0].Value = "ID";
                        detalleHeaderRow.Cells[1].Value = "Cliente";
                        detalleHeaderRow.Cells[2].Value = "Origen";
                        detalleHeaderRow.Cells[3].Value = "Destino";
                        detalleHeaderRow.Cells[4].Value = "Estado";
                        detalleHeaderRow.Cells[5].Value = "Volumen";
                        detalleHeaderRow.Cells[6].Value = "Viajes";

                        // Centrar encabezados apropiados
                        detalleHeaderRow.Cells[0].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                        detalleHeaderRow.Cells[5].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                        detalleHeaderRow.Cells[6].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);

                        // Agregar datos (limitar a los primeros 20 para que quepa en la página)
                        int maxItems = Math.Min(datosDetalle.Count, 20);
                        for (int i = 0; i < maxItems; i++)
                        {
                            var item = datosDetalle[i];
                            PdfGridRow row = detalleTable.Rows.Add();
                            row.Height = 20;

                            // Alternar color de filas - CORREGIDO
                            if (i % 2 == 1)
                            {
                                row.Style.BackgroundBrush = new PdfSolidBrush(new PdfColor((byte)250, (byte)250, (byte)250));
                            }

                            row.Cells[0].Value = item.IdPedido.ToString();
                            row.Cells[1].Value = TruncateText(item.Cliente, 15);
                            row.Cells[2].Value = TruncateText(item.Origen, 12);
                            row.Cells[3].Value = TruncateText(item.Destino, 12);
                            row.Cells[4].Value = TruncateText(item.Estado, 10);
                            row.Cells[5].Value = $"{item.Volumen:N0}";
                            row.Cells[6].Value = $"{item.ViajesRealizados}/{item.ViajesSolicitados}";

                            // Aplicar fuente y alineación
                            for (int j = 0; j < row.Cells.Count; j++)
                            {
                                row.Cells[j].Style.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 9);
                                row.Cells[j].Style.TextBrush = blackBrush;
                            }

                            // Centrar columnas específicas
                            row.Cells[0].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                            row.Cells[5].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                            row.Cells[6].Style.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                        }

                        // Nota si hay más datos
                        if (datosDetalle.Count > 20)
                        {
                            PdfGridRow noteRow = detalleTable.Rows.Add();
                            noteRow.Cells[0].Value = "...";
                            noteRow.Cells[1].Value = $"Se muestran los primeros 20 de {datosDetalle.Count} pedidos";
                            noteRow.Cells[1].ColumnSpan = 6;
                            noteRow.Cells[1].Style.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 9, PdfFontStyle.Italic);
                            noteRow.Cells[1].Style.TextBrush = grayBrush;
                        }

                        // Dibujar la tabla
                        detalleTable.Draw(page, 20, yPosition);
                    }

                    // ==============================================
                    // PIE DE PÁGINA
                    // ==============================================
                    string fechaGeneracion = $"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    graphics.DrawString(fechaGeneracion, normalFont, grayBrush, 20, page.Size.Height - 40);

                    // Número de página
                    string numeroPagina = $"Página 1 de 1";
                    SyncSizeF textSize = normalFont.MeasureString(numeroPagina);
                    graphics.DrawString(numeroPagina, normalFont, grayBrush,
                        page.Size.Width - textSize.Width - 20, page.Size.Height - 40);

                    // Convertir a bytes
                    using (MemoryStream stream = new MemoryStream())
                    {
                        document.Save(stream);
                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar PDF de pedidos: {ex.Message}", ex);
            }
        }

        // 3. MÉTODO AUXILIAR PARA TRUNCAR TEXTO (agregar dentro de SqlServerService)
        private static string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength - 3) + "...";
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
                                Username = reader.IsDBNull(reader.GetOrdinal("username")) ?
                                    null : reader.GetString(reader.GetOrdinal("username")),
                                Contraseña = reader.IsDBNull(reader.GetOrdinal("contraseña")) ?
                                    null : reader.GetString(reader.GetOrdinal("contraseña")),
                                IdTipoUsuario = reader.GetInt32(reader.GetOrdinal("id_tipoUsuario")),
                                Estado = reader.GetBoolean(reader.GetOrdinal("estado")),
                                IdPersona = (int)(reader.IsDBNull(reader.GetOrdinal("id_persona")) ?
                                    (int?)null : reader.GetInt32(reader.GetOrdinal("id_persona"))),
                                IdEmpresa = reader.IsDBNull(reader.GetOrdinal("id_empresa")) ?
                                    null : reader.GetInt32(reader.GetOrdinal("id_empresa")),
                                TipoUsuario = reader.IsDBNull(reader.GetOrdinal("TipoUsuario")) ?
                                    null : reader.GetString(reader.GetOrdinal("TipoUsuario")),
                                Nombres = reader.IsDBNull(reader.GetOrdinal("Nombre")) ?
                                    null : reader.GetString(reader.GetOrdinal("Nombre")),
                                Apellidos = reader.IsDBNull(reader.GetOrdinal("apePaterno")) ?
                                    null : reader.GetString(reader.GetOrdinal("apePaterno")),
                                Correo = reader.IsDBNull(reader.GetOrdinal("email")) ?
                                    null : reader.GetString(reader.GetOrdinal("email")),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ?
                                    null : reader.GetString(reader.GetOrdinal("telefono"))
                            });
                        }
                    }
                }
            }
            return usuarios;

        }
        public class RecogidaResult
        {
            public bool Exitoso { get; set; }
            public string Mensaje { get; set; }
            public int CantidadRegistrada { get; set; }
            public int TotalRecogidoViaje { get; set; }
        }



        public async Task<Trabajador> ObtenerTrabajadorPorUsuarioAsync(int idUsuario)
        {
            try
            {
                // Primero obtenemos el ID del trabajador usando el nuevo procedimiento
                int idTrabajador = 0;

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("pa_obtenerTrabajadorPorUsuario", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idUsuario", idUsuario);

                        var result = await command.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            idTrabajador = Convert.ToInt32(result);
                            Console.WriteLine(idTrabajador);
                        }
                        else
                        {
                            // No se encontró un trabajador asociado a este usuario
                            return null;
                        }
                    }
                }

                // Si encontramos un ID válido, ahora obtenemos los detalles del trabajador
                if (idTrabajador > 0)
                {
                    // Usamos el método existente para obtener todos los trabajadores
                    var trabajadores = await ObtenerTrabajadoresAsync("Transportista");
                    return trabajadores.FirstOrDefault(t => t.IdTrabajador == idTrabajador);
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener trabajador por usuario: {ex.Message}");
                throw;
            }
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
        // Agregar este método a la clase SqlServerService
        public async Task<string> ObtenerEstadoRealPedidoAsync(int idPedido)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("pa_ObtenerEstadoRealPedido", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_pedido", idPedido);

                        var resultado = await command.ExecuteScalarAsync();
                        return resultado?.ToString() ?? "Pendiente";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener estado real del pedido: {ex.Message}");
                return "Sin estado";
            }
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
    string tipoVehiculo)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("pa_AgregarVehiculo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar parámetros
                        command.Parameters.AddWithValue("@placa", placa);
                        command.Parameters.AddWithValue("@modelo", string.IsNullOrWhiteSpace(modelo) ? (object)DBNull.Value : modelo);
                        command.Parameters.AddWithValue("@añoFabricacion", string.IsNullOrWhiteSpace(añoFabricacion) ? (object)DBNull.Value : añoFabricacion);
                        command.Parameters.AddWithValue("@emisionPoliza", (object)emisionPoliza ?? DBNull.Value);
                        command.Parameters.AddWithValue("@vencimientoPoliza", (object)vencimientoPoliza ?? DBNull.Value);
                        command.Parameters.AddWithValue("@emisionCITV", (object)emisionCITV ?? DBNull.Value);
                        command.Parameters.AddWithValue("@vencimientoCITV", (object)vencimientoCITV ?? DBNull.Value);
                        command.Parameters.AddWithValue("@emisionCubicacion", (object)emisionCubicacion ?? DBNull.Value);
                        command.Parameters.AddWithValue("@vencimientoCubicacion", (object)vencimientoCubicacion ?? DBNull.Value);
                        command.Parameters.AddWithValue("@imagen", (object)imagen ?? DBNull.Value);
                        command.Parameters.AddWithValue("@poliza", (object)poliza ?? DBNull.Value);
                        command.Parameters.AddWithValue("@citv", (object)citv ?? DBNull.Value);
                        command.Parameters.AddWithValue("@cubicacion", (object)cubicacion ?? DBNull.Value);
                        command.Parameters.AddWithValue("@tarjetaPropiedad", (object)tarjetaPropiedad ?? DBNull.Value);
                        command.Parameters.AddWithValue("@tipoVehiculo", tipoVehiculo);

                        // Ejecutar y leer la respuesta
                        using var reader = await command.ExecuteReaderAsync();
                        if (await reader.ReadAsync())
                        {
                            int resultado = reader.GetInt32("Resultado");
                            string mensaje = reader.GetString("Mensaje");

                            if (resultado == 0)
                            {
                                throw new Exception(mensaje);
                            }

                            return resultado;
                        }

                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al agregar vehículo: {ex.Message}");
                throw;
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
        public async Task<int> EliminarVehiculoAsync(int idVehiculo, string tipoVehiculo)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("pa_EliminarVehiculo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 60;

                        // Agregar parámetros
                        command.Parameters.AddWithValue("@id_vehiculo", idVehiculo);
                        command.Parameters.AddWithValue("@tipoVehiculo", tipoVehiculo ?? (object)DBNull.Value);

                        System.Diagnostics.Debug.WriteLine($"Ejecutando pa_EliminarVehiculo - ID: {idVehiculo}, Tipo: {tipoVehiculo}");

                        int resultado = await command.ExecuteNonQueryAsync();

                        System.Diagnostics.Debug.WriteLine($"Vehículo eliminado. Filas afectadas: {resultado}");

                        return resultado;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"Error SQL al eliminar vehículo: {sqlEx.Message}");

                // Si el procedimiento no existe, usar método de SQL directo
                if (sqlEx.Number == 2812) // Procedure not found
                {
                    System.Diagnostics.Debug.WriteLine("Procedimiento no encontrado, usando SQL directo");
                    return await EliminarVehiculoConSQLDirectoBDReal(idVehiculo, tipoVehiculo);
                }

                // Si es error de clave foránea, dar mensaje más específico
                if (sqlEx.Number == 547) // Foreign key constraint
                {
                    throw new Exception($"No se puede eliminar el vehículo porque está siendo usado en viajes activos. Elimine primero los viajes asociados.");
                }

                throw new Exception($"Error de base de datos al eliminar vehículo: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error general al eliminar vehículo: {ex.Message}");
                throw new Exception($"Error inesperado al eliminar vehículo: {ex.Message}");
            }
        }

        // MÉTODO PARA VER DEPENDENCIAS ANTES DE ELIMINAR

        public async Task<List<Dictionary<string, object>>> VerDependenciasVehiculoAsync(int idVehiculo, string tipoVehiculo)
        {
            var dependencias = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("pa_VerDependenciasVehiculo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_vehiculo", idVehiculo);
                        command.Parameters.AddWithValue("@tipoVehiculo", tipoVehiculo);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var dependencia = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    dependencia[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }
                                dependencias.Add(dependencia);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener dependencias: {ex.Message}");
                // No lanzar error, solo retornar lista vacía
            }

            return dependencias;
        }

        // MÉTODO DE RESPALDO CON SQL DIRECTO

        private async Task<int> EliminarVehiculoConSQLDirectoBDReal(int idVehiculo, string tipoVehiculo)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Verificar que el vehículo existe
                            string verificarExistenciaSql = "";
                            if (tipoVehiculo?.ToLower() == "tracto")
                            {
                                verificarExistenciaSql = "SELECT COUNT(*) FROM Tracto WHERE id_tracto = @id";
                            }
                            else if (tipoVehiculo?.ToLower() == "cisterna")
                            {
                                verificarExistenciaSql = "SELECT COUNT(*) FROM Cisterna WHERE id_cisterna = @id";
                            }
                            else
                            {
                                throw new Exception($"Tipo de vehículo no válido: {tipoVehiculo}");
                            }

                            using (SqlCommand verificarCmd = new SqlCommand(verificarExistenciaSql, connection, transaction))
                            {
                                verificarCmd.Parameters.AddWithValue("@id", idVehiculo);
                                int existe = (int)await verificarCmd.ExecuteScalarAsync();

                                if (existe == 0)
                                {
                                    throw new Exception($"No se encontró el vehículo con ID {idVehiculo} en la tabla {tipoVehiculo}");
                                }
                            }

                            // Verificar si tiene viajes asignados
                            string verificarViajesSql = "";
                            if (tipoVehiculo?.ToLower() == "tracto")
                            {
                                verificarViajesSql = "SELECT COUNT(*) FROM Viaje WHERE id_tracto = @id";
                            }
                            else
                            {
                                verificarViajesSql = "SELECT COUNT(*) FROM Viaje WHERE id_cisterna = @id";
                            }

                            using (SqlCommand verificarViajesCmd = new SqlCommand(verificarViajesSql, connection, transaction))
                            {
                                verificarViajesCmd.Parameters.AddWithValue("@id", idVehiculo);
                                int viajesAsignados = (int)await verificarViajesCmd.ExecuteScalarAsync();

                                if (viajesAsignados > 0)
                                {
                                    throw new Exception($"No se puede eliminar el vehículo porque está asignado a {viajesAsignados} viaje(s). Elimine primero los viajes asociados.");
                                }
                            }

                            // Proceder con la eliminación
                            string deleteEvidenciasSql = "";
                            string deleteVehiculoSql = "";

                            if (tipoVehiculo?.ToLower() == "tracto")
                            {
                                deleteEvidenciasSql = "DELETE FROM Evidencias_Tracto WHERE id_tracto = @id_vehiculo";
                                deleteVehiculoSql = "DELETE FROM Tracto WHERE id_tracto = @id_vehiculo";
                            }
                            else // cisterna
                            {
                                deleteEvidenciasSql = "DELETE FROM Evidencias_Cisterna WHERE id_cisterna = @id_vehiculo";
                                deleteVehiculoSql = "DELETE FROM Cisterna WHERE id_cisterna = @id_vehiculo";
                            }

                            // Eliminar evidencias primero
                            using (SqlCommand deleteEvidenciasCmd = new SqlCommand(deleteEvidenciasSql, connection, transaction))
                            {
                                deleteEvidenciasCmd.Parameters.AddWithValue("@id_vehiculo", idVehiculo);
                                await deleteEvidenciasCmd.ExecuteNonQueryAsync();
                                System.Diagnostics.Debug.WriteLine($"Evidencias eliminadas para {tipoVehiculo} ID: {idVehiculo}");
                            }

                            // Eliminar el vehículo
                            using (SqlCommand deleteVehiculoCmd = new SqlCommand(deleteVehiculoSql, connection, transaction))
                            {
                                deleteVehiculoCmd.Parameters.AddWithValue("@id_vehiculo", idVehiculo);

                                System.Diagnostics.Debug.WriteLine($"Ejecutando DELETE directo para {tipoVehiculo} ID: {idVehiculo}");

                                int filasAfectadas = await deleteVehiculoCmd.ExecuteNonQueryAsync();

                                System.Diagnostics.Debug.WriteLine($"Filas eliminadas (SQL directo): {filasAfectadas}");

                                transaction.Commit();
                                return filasAfectadas;
                            }
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en EliminarVehiculoConSQLDirectoBDReal: {ex.Message}");
                throw;
            }
        }

        // MÉTODO PARA FORZAR ELIMINACIÓN (CON VIAJES)
        public async Task<int> EliminarVehiculoForzarAsync(int idVehiculo, string tipoVehiculo)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("pa_EliminarVehiculo_Forzar", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 120; // Más tiempo porque elimina más datos

                        // Agregar parámetros
                        command.Parameters.AddWithValue("@id_vehiculo", idVehiculo);
                        command.Parameters.AddWithValue("@tipoVehiculo", tipoVehiculo ?? (object)DBNull.Value);

                        System.Diagnostics.Debug.WriteLine($"Ejecutando pa_EliminarVehiculo_Forzar - ID: {idVehiculo}, Tipo: {tipoVehiculo}");

                        int resultado = await command.ExecuteNonQueryAsync();

                        System.Diagnostics.Debug.WriteLine($"Vehículo eliminado forzadamente. Filas afectadas: {resultado}");

                        return resultado;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"Error SQL al forzar eliminación: {sqlEx.Message}");
                throw new Exception($"Error de base de datos al forzar eliminación: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error general al forzar eliminación: {ex.Message}");
                throw new Exception($"Error inesperado al forzar eliminación: {ex.Message}");
            }
        }

        public async Task<(int Resultado, string Mensaje)> ModificarVehiculoAsync(
     int idVehiculo,
     string placa,
     string modelo,
     string añoFabricacion,
     DateTime? emisionPoliza,
     DateTime? vencimientoPoliza,
     DateTime? emisionCITV,
     DateTime? vencimientoCITV,
     DateTime emisionCubicacion,
     DateTime vencimientoCubicacion,
     string tipoVehiculo)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("pa_ModificarVehiculo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar parámetros
                        command.Parameters.AddWithValue("@id_vehiculo", idVehiculo);
                        command.Parameters.AddWithValue("@placa", placa);
                        command.Parameters.AddWithValue("@modelo", string.IsNullOrWhiteSpace(modelo) ? (object)DBNull.Value : modelo);
                        command.Parameters.AddWithValue("@añoFabricacion", string.IsNullOrWhiteSpace(añoFabricacion) ? (object)DBNull.Value : añoFabricacion);
                        command.Parameters.AddWithValue("@emisionPoliza", (object)emisionPoliza ?? DBNull.Value);
                        command.Parameters.AddWithValue("@vencimientoPoliza", (object)vencimientoPoliza ?? DBNull.Value);
                        command.Parameters.AddWithValue("@emisionCITV", (object)emisionCITV ?? DBNull.Value);
                        command.Parameters.AddWithValue("@vencimientoCITV", (object)vencimientoCITV ?? DBNull.Value);
                        command.Parameters.AddWithValue("@emisionCubicacion", emisionCubicacion);
                        command.Parameters.AddWithValue("@vencimientoCubicacion", vencimientoCubicacion);
                        command.Parameters.AddWithValue("@tipoVehiculo", tipoVehiculo);

                        // Parámetro de retorno para obtener el resultado
                        SqlParameter returnValue = new SqlParameter("@ReturnValue", SqlDbType.Int);
                        returnValue.Direction = ParameterDirection.ReturnValue;
                        command.Parameters.Add(returnValue);

                        try
                        {
                            // Ejecutar el procedimiento almacenado
                            await command.ExecuteNonQueryAsync();

                            // Obtener el valor de retorno
                            int resultado = (int)returnValue.Value;
                            return (resultado, "Vehículo modificado correctamente");
                        }
                        catch (SqlException sqlEx)
                        {
                            // Capturar mensajes de error específicos del procedimiento almacenado
                            string mensajeError = sqlEx.Message;

                            // Registrar el error para depuración
                            System.Diagnostics.Debug.WriteLine($"Error SQL al modificar vehículo: {mensajeError}");

                            // Devolver el mensaje de error específico
                            return (0, mensajeError);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Capturar y registrar cualquier otra excepción
                System.Diagnostics.Debug.WriteLine($"Error general al modificar vehículo: {ex.Message}");
                return (0, $"Error inesperado: {ex.Message}");
            }
        }

        public async Task<RespuestaPedidoAutomatico> InsertarPedidoAutomaticoAsync(PedidoAutomatico pedido)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("pa_CrearPedidoAutomatico", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Parámetros requeridos por el nuevo PA
                command.Parameters.AddWithValue("@id_usuario", pedido.IdUsuario);
                command.Parameters.AddWithValue("@id_cliente", pedido.IdCliente.Value);
                command.Parameters.AddWithValue("@id_tipoServicio", pedido.IdTipoServicio); 
                command.Parameters.AddWithValue("@dias_semana", pedido.DiasSemana);
                command.Parameters.AddWithValue("@hora_programada", pedido.HoraProgramada);
                command.Parameters.AddWithValue("@fecha_inicio", pedido.FechaInicio.Date);
                command.Parameters.AddWithValue("@fecha_fin", pedido.FechaFin.Date);
                command.Parameters.AddWithValue("@id_origen", pedido.IdOrigen.Value);
                command.Parameters.AddWithValue("@id_destino", pedido.IdDestino.Value);
                command.Parameters.AddWithValue("@descripcion", (object)pedido.Descripcion ?? DBNull.Value);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    // El PA devuelve id_pedidoAutomatico y pedidos_generados
                    return new RespuestaPedidoAutomatico
                    {
                        IdPedidoAutomatico = reader.IsDBNull("id_pedidoAutomatico") ? null : reader.GetInt32("id_pedidoAutomatico"),
                        Mensaje = $"Pedido automático creado exitosamente. Pedidos generados: {(reader.IsDBNull("pedidos_generados") ? 0 : reader.GetInt32("pedidos_generados"))}",
                        FilasAfectadas = reader.IsDBNull("id_pedidoAutomatico") ? 0 : 1
                    };
                }
                else
                {
                    return new RespuestaPedidoAutomatico
                    {
                        FilasAfectadas = 1,
                        Mensaje = "Pedido automático creado exitosamente"
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RespuestaPedidoAutomatico
                {
                    FilasAfectadas = 0,
                    Mensaje = ex.Message
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al insertar pedido automático: {ex.Message}");
                return new RespuestaPedidoAutomatico
                {
                    FilasAfectadas = 0,
                    Mensaje = $"Error inesperado: {ex.Message}"
                };
            }
        }
        // En tu clase Database, REEMPLAZA o agrega este método:

        public async Task<List<Pedido>> ObtenerPedidosConEstadoAsync(int? idUsuario = null, string filtroEstado = null)
        {
            var pedidos = new List<Pedido>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("pa_ObtenerPedidosConEstado", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros
                        command.Parameters.AddWithValue("@id_usuario", (object)idUsuario ?? DBNull.Value);
                        command.Parameters.AddWithValue("@filtro_estado", (object)filtroEstado ?? DBNull.Value);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var pedido = new Pedido
                                {
                                    IdPedido = reader.GetInt32("IdPedido"),
                                    Cantidad = reader.GetInt32("Cantidad"),
                                    Viajes = reader.GetInt32("Viajes"),
                                    FechaSolicitud = reader.GetDateTime("FechaSolicitud"),
                                    FechaEntrega = reader.GetDateTime("FechaEntrega"),
                                    Destino = reader.GetString("Destino"),
                                    Cliente = reader.GetString("Cliente"),
                                    Usuario = reader.GetString("Usuario"),
                                    Servicios = reader.IsDBNull("Servicios") ? "" : reader.GetString("Servicios"),

                                    EstadoPedido = reader.GetString("EstadoPedido")
                                };

                                pedidos.Add(pedido);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener pedidos con estado: {ex.Message}");
            }

            return pedidos;
        }

        public async Task<List<PedidoAutomatico>> ObtenerPedidosAutomaticosAsync(DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            var pedidos = new List<PedidoAutomatico>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("SP_ObtenerPedidosAutomaticos", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@fecha_desde", (object)fechaDesde ?? DBNull.Value);
                command.Parameters.AddWithValue("@fecha_hasta", (object)fechaHasta ?? DBNull.Value);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    pedidos.Add(new PedidoAutomatico
                    {
                        IdPedidoAutomatico = reader.GetInt32("id_pedidoAutomatico"),
                        IdUsuario = reader.GetInt32("id_usuario"),
                        IdTipoServicio = reader.GetInt32("id_tipoServicio"),
                        TipoServicio = reader.GetString("tipoServicio"),
                        DiasSemana = reader.GetString("dias_semana"),
                        HoraProgramada = TimeSpan.TryParse(reader["hora_programada"]?.ToString(), out var tiempo) ? tiempo : TimeSpan.Zero,
                        FechaInicio = reader.GetDateTime("fecha_inicio"),
                        FechaFin = reader.GetDateTime("fecha_fin"),
                        Estado = reader.GetBoolean("estado"),
                        FechaCreacion = reader.GetDateTime("fecha_creacion"),
                        FechaModificacion = reader.GetDateTime("fecha_modificacion"),
                        UltimoProcesamiento = reader.IsDBNull("ultimo_procesamiento") ? null : reader.GetDateTime("ultimo_procesamiento"),
                        Descripcion = reader.IsDBNull("descripcion") ? null : reader.GetString("descripcion"),
                             });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener pedidos automáticos: {ex.Message}");
                throw;
            }

            return pedidos;
        }
        public async Task<RespuestaPedidoAutomatico> ActualizarPedidoAutomaticoAsync(PedidoAutomatico pedido)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("SP_ActualizarPedidoAutomatico", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@id_pedidoAutomatico", pedido.IdPedidoAutomatico);
                command.Parameters.AddWithValue("@id_tipoServicio", pedido.IdTipoServicio);
                command.Parameters.AddWithValue("@dias_semana", pedido.DiasSemana);
                command.Parameters.AddWithValue("@hora_programada", pedido.HoraProgramada);
                command.Parameters.AddWithValue("@fecha_inicio", pedido.FechaInicio.Date);
                command.Parameters.AddWithValue("@fecha_fin", pedido.FechaFin.Date);
                command.Parameters.AddWithValue("@descripcion", (object)pedido.Descripcion ?? DBNull.Value);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new RespuestaPedidoAutomatico
                    {
                        FilasAfectadas = reader.GetInt32("FilasAfectadas"),
                        Mensaje = reader.GetString("Mensaje")
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RespuestaPedidoAutomatico
                {
                    FilasAfectadas = 0,
                    Mensaje = ex.Message
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar pedido automático: {ex.Message}");
                return new RespuestaPedidoAutomatico
                {
                    FilasAfectadas = 0,
                    Mensaje = $"Error inesperado: {ex.Message}"
                };
            }

            return new RespuestaPedidoAutomatico { FilasAfectadas = 0, Mensaje = "Error desconocido" };
        }

        public async Task<RespuestaPedidoAutomatico> EliminarPedidoAutomaticoAsync(int idPedidoAutomatico)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("SP_EliminarPedidoAutomatico", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@id_pedidoAutomatico", idPedidoAutomatico);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new RespuestaPedidoAutomatico
                    {
                        FilasAfectadas = reader.GetInt32("FilasAfectadas"),
                        Mensaje = reader.GetString("Mensaje")
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RespuestaPedidoAutomatico
                {
                    FilasAfectadas = 0,
                    Mensaje = ex.Message
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al eliminar pedido automático: {ex.Message}");
                return new RespuestaPedidoAutomatico
                {
                    FilasAfectadas = 0,
                    Mensaje = $"Error inesperado: {ex.Message}"
                };
            }

            return new RespuestaPedidoAutomatico { FilasAfectadas = 0, Mensaje = "Error desconocido" };
        }

        public async Task<RespuestaPedidoAutomatico> CambiarEstadoPedidoAutomaticoAsync(int idPedidoAutomatico, bool nuevoEstado)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("SP_CambiarEstadoPedidoAutomatico", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@id_pedidoAutomatico", idPedidoAutomatico);
                command.Parameters.AddWithValue("@nuevo_estado", nuevoEstado);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new RespuestaPedidoAutomatico
                    {
                        FilasAfectadas = reader.GetInt32("FilasAfectadas"),
                        Mensaje = reader.GetString("Mensaje")
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RespuestaPedidoAutomatico
                {
                    FilasAfectadas = 0,
                    Mensaje = ex.Message
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cambiar estado del pedido automático: {ex.Message}");
                return new RespuestaPedidoAutomatico
                {
                    FilasAfectadas = 0,
                    Mensaje = $"Error inesperado: {ex.Message}"
                };
            }

            return new RespuestaPedidoAutomatico { FilasAfectadas = 0, Mensaje = "Error desconocido" };
        }
        public async Task<List<Trabajador>> ObtenerTrabajadoresDisponiblesConEstadoAsync(string categoria)
        {
            var trabajadores = new List<Trabajador>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_ObtenerTrabajadoresConEstado", connection))
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
                                EstadoDescripcion = reader.GetString("estado_descripcion")
                            });
                        }
                    }
                }
            }

            return trabajadores;
        }
        public async Task<List<Vehiculo>> ObtenerCisternasDisponiblesConEstadoAsync()
        {
            var cisternas = new List<Vehiculo>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_ObtenerCisternasConEstado", connection))
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
                                Estado = reader.GetBoolean("estado"),
                                EstadoDescripcion = reader.GetString("estado_descripcion")
                            });
                        }
                    }
                }
            }

            return cisternas;
        }
        public async Task<List<Vehiculo>> ObtenerTractosDisponiblesConEstadoAsync()
        {
            var tractos = new List<Vehiculo>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_ObtenerTractosConEstado", connection))
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
                                Estado = reader.GetBoolean("estado"),
                                EstadoDescripcion = reader.GetString("estado_descripcion")
                            });
                        }
                    }
                }
            }

            return tractos;
        }
        public async Task<int> ModificarTrabajadorConEstadoAsync(
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
                       string? licencia,
                       bool estado)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("pa_ModificarTrabajadorConEstado", connection))
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
                    command.Parameters.AddWithValue("@licencia", string.IsNullOrWhiteSpace(licencia) ? (object)DBNull.Value : licencia);
                    command.Parameters.AddWithValue("@estado", estado);

                    // Ejecutar el procedimiento almacenado
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<int> AgregarTrabajadorConEstadoAsync(
               string nombre,
               string apePaterno,
               string apeMaterno,
               int idTipoDoc,
               string numDoc,
               string telefono,
               string direccion,
               string email,
               int idCat,
               string? licencia,
               bool estado)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("pa_AgregarTrabajadorConEstado", connection))
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
                    command.Parameters.AddWithValue("@licencia", string.IsNullOrWhiteSpace(licencia) ? (object)DBNull.Value : licencia);
                    command.Parameters.AddWithValue("@estado", estado);

                    // Ejecutar el procedimiento almacenado
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<(DataTable ResumenPedidos, DataTable DetallePedidos)> ObtenerReportePedidosPorClienteAsync(
     int idCliente,
     string tipoPedido = null,
     DateTime? fechaDesde = null,
     DateTime? fechaHasta = null)
        {
            DataTable resumenPedidos = new DataTable();
            DataTable detallePedidos = new DataTable();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_ReportePedidosPorCliente", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 180; // Timeout de 3 minutos

                        // Parámetros obligatorios
                        command.Parameters.AddWithValue("@IdCliente", idCliente);

                        // Parámetros opcionales
                        if (!string.IsNullOrEmpty(tipoPedido) && tipoPedido != "Todos los tipos")
                        {
                            command.Parameters.AddWithValue("@TipoPedido", tipoPedido);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@TipoPedido", DBNull.Value);
                        }

                        // Parámetros de fecha
                        if (fechaDesde.HasValue)
                        {
                            command.Parameters.AddWithValue("@FechaDesde", fechaDesde.Value.Date);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@FechaDesde", DBNull.Value);
                        }

                        if (fechaHasta.HasValue)
                        {
                            command.Parameters.AddWithValue("@FechaHasta", fechaHasta.Value.Date);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@FechaHasta", DBNull.Value);
                        }

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);

                            if (ds.Tables.Count > 0)
                                resumenPedidos = ds.Tables[0];
                            if (ds.Tables.Count > 1)
                                detallePedidos = ds.Tables[1];

                            // Debug para verificar los datos
                            System.Diagnostics.Debug.WriteLine($"Resumen Pedidos filas: {resumenPedidos?.Rows?.Count ?? 0}");
                            System.Diagnostics.Debug.WriteLine($"Detalle Pedidos filas: {detallePedidos?.Rows?.Count ?? 0}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerReportePedidosPorClienteAsync: {ex.Message}");
                throw new Exception($"Error al obtener el reporte de pedidos: {ex.Message}", ex);
            }

            return (resumenPedidos, detallePedidos);
        }
        public async Task<List<TipoUsuario>> ObtenerTiposUsuarioAsync()
        {
            var tiposUsuario = new List<TipoUsuario>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("SELECT id_tipoUsuario, descripcion FROM Tipo_Usuario ORDER BY descripcion", connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                tiposUsuario.Add(new TipoUsuario
                                {
                                    IdTipoUsuario = reader.GetInt32("id_tipoUsuario"),
                                    descripcion = reader.GetString("descripcion")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener tipos de usuario: {ex.Message}");
                throw;
            }

            return tiposUsuario;
        }


        public async Task<List<EstadoViaje>> ObtenerEstadosViajeAsync()
        {
            var estados = new List<EstadoViaje>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("pa_ObtenerEstadosViaje", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                estados.Add(new EstadoViaje
                                {
                                    IdEstadoViaje = reader.GetInt32(reader.GetOrdinal("id_estadoViaje")),
                                    Descripcion = reader.GetString(reader.GetOrdinal("descripcion")),
                                    Orden = Convert.ToInt32(reader["Orden"]),
                                    RequiereCantidad = Convert.ToBoolean(reader["RequiereCantidad"]),
                                    RequiereUbicacion = Convert.ToBoolean(reader["RequiereUbicacion"]),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener estados de viaje: {ex.Message}");
            }

            return estados;
        }
        // Agregar este método a SqlServerService
        public async Task<string> ObtenerEstadoActualPedidoAsync(int idPedido)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("pa_ObtenerEstadoActualPedido", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_pedido", idPedido);

                        var resultado = await command.ExecuteScalarAsync();
                        return resultado?.ToString() ?? "Sin estado";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener estado del pedido: {ex.Message}");
                return "Error al obtener estado";
            }
        }



        public async Task<ResultadoRecogida> RegistrarRecogidaViajeAsync(
        int idViaje,
        int cantidadRecogida,
        int idTrabajador,
        string ubicacion,
        string comentario,
        string evidencia_url)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("pa_RegistrarRecogidaViaje", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_viaje", idViaje);
                        command.Parameters.AddWithValue("@cantidad_recogida", cantidadRecogida);
                        command.Parameters.AddWithValue("@id_trabajador", idTrabajador);
                        command.Parameters.AddWithValue("@ubicacion", ubicacion ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@comentario", comentario ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@evidencia_url", evidencia_url ?? (object)DBNull.Value);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new ResultadoRecogida
                                {
                                    Exitoso = !reader.IsDBNull("Exitoso") && reader.GetInt32("Exitoso") == 1,
                                    Mensaje = reader.IsDBNull("Mensaje") ? string.Empty : reader.GetString("Mensaje"),
                                    TotalRecogidoViaje = reader.IsDBNull("TotalRecogidoViaje") ? 0 : reader.GetInt32("TotalRecogidoViaje")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al registrar recogida: {ex.Message}");
                return new ResultadoRecogida
                {
                    Exitoso = false,
                    Mensaje = $"Error: {ex.Message}",
                    TotalRecogidoViaje = 0
                };
            }

            return new ResultadoRecogida
            {
                Exitoso = false,
                Mensaje = "No se pudo procesar la solicitud",
                TotalRecogidoViaje = 0
            };
        }

        // **SOBRECARGA 1: Para cambios de estado específicos (ViewModel)**
        public async Task<int> ActualizarEstadoSeguimientoAsync(int idViaje, int idEstado, string comentario, string evidenciaBase64)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("pa_ActualizarEstadoEspecificoViaje", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_viaje", idViaje);
                        command.Parameters.AddWithValue("@id_estadoViaje", idEstado);
                        command.Parameters.AddWithValue("@comentario", comentario ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@evidencia_url", evidenciaBase64 ?? (object)DBNull.Value);

                        var result = await command.ExecuteScalarAsync();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar estado específico: {ex.Message}");
                return 0;
            }
        }

        // **SOBRECARGA 2: Para seguimiento sin cambio de estado (página actual)**
        public async Task<int> ActualizarEstadoSeguimientoAsync(int idViaje, string comentario, string evidenciaUrl)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("pa_ActualizarSeguimientoViaje", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_viaje", idViaje);
                        command.Parameters.AddWithValue("@comentario", comentario ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@evidencia_url", evidenciaUrl ?? (object)DBNull.Value);

                        var result = await command.ExecuteScalarAsync();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar seguimiento: {ex.Message}");
                return 0;
            }
        }

        // Método para actualizar seguimiento general (mantiene estado actual)
        public async Task<int> ActualizarSeguimientoViajeAsync(int idViaje, string comentario, string evidenciaBase64)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("pa_ActualizarSeguimientoViaje", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_viaje", idViaje);
                        command.Parameters.AddWithValue("@comentario", comentario ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@evidencia_url", evidenciaBase64 ?? (object)DBNull.Value);

                        var result = await command.ExecuteScalarAsync();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar seguimiento: {ex.Message}");
                return 0;
            }
        }
        // Versión simplificada para SqlServerService (opcional)
        public async Task<(bool Exitoso, string Mensaje)> RechazarSolicitudAsync(int idSolicitud, string comentario = null)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("pa_RechazarSolicitud", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@id_solicitud", idSolicitud);
                command.Parameters.AddWithValue("@comentario", (object)comentario ?? DBNull.Value);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return (reader.GetInt32("Exitoso") == 1, reader.GetString("Mensaje"));
                }

                return (false, "Error desconocido");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<(bool EnViaje, string MensajeDetalle)> VerificarTrabajadorEnViajeAsync(int idTrabajador)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_VerificarTrabajadorEnViaje", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_trabajador", idTrabajador);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                bool enViaje = reader.GetBoolean("EnViaje");
                                string detalleViaje = reader.IsDBNull("DetalleViaje") ?
                                    string.Empty : reader.GetString("DetalleViaje");

                                return (enViaje, detalleViaje);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al verificar trabajador en viaje: {ex.Message}");
                return (false, string.Empty);
            }

            return (false, string.Empty);
        }
        public async Task<(bool EnViaje, string MensajeDetalle)> VerificarCisternaEnViajeAsync(int idCisterna)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_VerificarCisternaEnViaje", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_cisterna", idCisterna);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                bool enViaje = reader.GetBoolean("EnViaje");
                                string detalleViaje = reader.IsDBNull("DetalleViaje") ?
                                    string.Empty : reader.GetString("DetalleViaje");

                                return (enViaje, detalleViaje);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al verificar cisterna en viaje: {ex.Message}");
                return (false, string.Empty);
            }

            return (false, string.Empty);
        }
        public async Task<(bool EnViaje, string MensajeDetalle)> VerificarTractoEnViajeAsync(int idTracto)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_VerificarTractoEnViaje", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_tracto", idTracto);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                bool enViaje = reader.GetBoolean("EnViaje");
                                string detalleViaje = reader.IsDBNull("DetalleViaje") ?
                                    string.Empty : reader.GetString("DetalleViaje");

                                return (enViaje, detalleViaje);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al verificar tracto en viaje: {ex.Message}");
                return (false, string.Empty);
            }

            return (false, string.Empty);
        }
        public async Task<Dictionary<string, (bool EnViaje, string MensajeDetalle)>> VerificarRecursosEnViajeAsync(
            int? idTransportista = null,
            int? idAyudante = null,
            int? idCisterna = null,
            int? idTracto = null)
        {
            var resultados = new Dictionary<string, (bool EnViaje, string MensajeDetalle)>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_VerificarMultiplesRecursosEnViaje", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_transportista", (object)idTransportista ?? DBNull.Value);
                        command.Parameters.AddWithValue("@id_ayudante", (object)idAyudante ?? DBNull.Value);
                        command.Parameters.AddWithValue("@id_cisterna", (object)idCisterna ?? DBNull.Value);
                        command.Parameters.AddWithValue("@id_tracto", (object)idTracto ?? DBNull.Value);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string tipoRecurso = reader.GetString("TipoRecurso");
                                bool enViaje = reader.GetBoolean("EnViaje");
                                string detalleViaje = reader.IsDBNull("DetalleViaje") ?
                                    string.Empty : reader.GetString("DetalleViaje");

                                resultados[tipoRecurso] = (enViaje, detalleViaje);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al verificar múltiples recursos: {ex.Message}");
            }

            return resultados;
        }

        // Método actualizado para obtener info del viaje (usando el PA corregido)
        public async Task<ViajeInfo> ObtenerInfoViajeAsync(int idViaje)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("pa_ObtenerInfoViaje", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_viaje", idViaje);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Verificar si hay un error
                                if (reader.HasColumn("ErrorMessage"))
                                {
                                    string errorMessage = reader["ErrorMessage"].ToString();
                                    Console.WriteLine(errorMessage);
                                    throw new Exception($"Error en la base de datos: {errorMessage}");
                                }

                                // Verificar si hay error de SQL
                                if (reader.HasColumn("ErrorNumber"))
                                {
                                    string sqlError = $"Error SQL {reader["ErrorNumber"]}: {reader["ErrorMessage"]}";
                                    Console.WriteLine(sqlError);
                                    throw new Exception($"Error de base de datos: {sqlError}");
                                }

                                return new ViajeInfo
                                {
                                    IdViaje = reader.GetInt32(reader.GetOrdinal("IdViaje")),
                                    Placa = reader.IsDBNull(reader.GetOrdinal("Placa")) ?
                                            string.Empty : reader.GetString(reader.GetOrdinal("Placa")).Trim(),
                                    CantidadPlanificada = reader.GetInt32(reader.GetOrdinal("CantidadPlanificada")),
                                    CantidadRecogida = reader.GetInt32(reader.GetOrdinal("CantidadRecogida")),
                                    Destino = reader.IsDBNull(reader.GetOrdinal("Destino")) ?
                                             string.Empty : reader.GetString(reader.GetOrdinal("Destino")),
                                    EstadoActual = new EstadoViaje
                                    {
                                        IdEstadoViaje = reader.GetInt32(reader.GetOrdinal("IdEstadoViaje")),
                                        Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ?
                                                    string.Empty : reader.GetString(reader.GetOrdinal("Descripcion")),
                                        // Usamos el ID como orden temporal
                                        Orden = reader.GetInt32(reader.GetOrdinal("IdEstadoViaje")),
                                        // Lógica específica: solo estado 4 requiere cantidad
                                        RequiereCantidad = reader.GetInt32(reader.GetOrdinal("IdEstadoViaje")) == 4,
                                        // Estados que requieren ubicación: 2, 3, 5, 6
                                        RequiereUbicacion = new int[] { 2, 3, 5, 6 }.Contains(reader.GetInt32(reader.GetOrdinal("IdEstadoViaje")))
                                    }
                                };
                            }
                            else
                            {
                                throw new Exception($"No se encontró información para el viaje ID: {idViaje}");
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"Error SQL al obtener información del viaje: {sqlEx.Message}");
                throw new Exception($"Error de base de datos: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener información del viaje: {ex.Message}");
                throw new Exception($"Error al obtener información del viaje: {ex.Message}", ex);
            }
        }
    }



}