// MÉTODO ACTUALIZADO PARA SqlServerService.cs
// Reemplaza el método existente ObtenerPedidosAutomaticosAsync por este:

public async Task<List<PedidoAutomatico>> ObtenerPedidosAutomaticosAsync(DateTime? fechaDesde = null, DateTime? fechaHasta = null)
{
    var pedidos = new List<PedidoAutomatico>();

    try
    {
        using var connection = new SqlConnection(_connectionString);
        
        // Query actualizada para la nueva estructura de tabla
        string query = @"
            SELECT 
                pa.id_pedidoAutomatico,
                pa.id_usuario,
                pa.id_tipoServicio,
                s.descripcion as tipoServicio,
                pa.dias_semana,
                pa.hora_programada,
                pa.fecha_inicio,
                pa.fecha_fin,
                pa.estado,
                pa.fecha_creacion,
                pa.fecha_modificacion,
                pa.ultimo_procesamiento,
                pa.descripcion,
                pa.id_cliente,
                pa.id_origen,
                pa.id_destino,
                -- Información de cliente
                CASE 
                    WHEN pa.id_cliente IS NOT NULL THEN 
                        CONCAT(p_cliente.Nombre, ' ', 
                               ISNULL(p_cliente.apePaterno, ''), ' ', 
                               ISNULL(p_cliente.apeMaterno, ''))
                    ELSE NULL 
                END as nombre_cliente,
                -- Información de origen
                o.descripcion as descripcion_origen,
                -- Información de destino
                d.descripcion as descripcion_destino
            FROM PedidosAutomaticos pa
            INNER JOIN Servicio s ON pa.id_tipoServicio = s.id_servicio
            LEFT JOIN Cliente c ON pa.id_cliente = c.id_cliente
            LEFT JOIN Persona p_cliente ON c.id_persona = p_cliente.id_persona
            LEFT JOIN Origen o ON pa.id_origen = o.id_origen
            LEFT JOIN Destino d ON pa.id_destino = d.id_destino
            WHERE 1=1";

        // Agregar filtros de fecha si se proporcionan
        if (fechaDesde.HasValue)
        {
            query += " AND pa.fecha_inicio >= @fechaDesde";
        }
        
        if (fechaHasta.HasValue)
        {
            query += " AND pa.fecha_fin <= @fechaHasta";
        }

        query += " ORDER BY pa.fecha_creacion DESC";

        using var command = new SqlCommand(query, connection);
        
        if (fechaDesde.HasValue)
        {
            command.Parameters.AddWithValue("@fechaDesde", fechaDesde.Value.Date);
        }
        
        if (fechaHasta.HasValue)
        {
            command.Parameters.AddWithValue("@fechaHasta", fechaHasta.Value.Date);
        }

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
                
                // Nuevos campos según la estructura de tabla actualizada
                IdCliente = reader.IsDBNull("id_cliente") ? null : reader.GetInt32("id_cliente"),
                IdOrigen = reader.IsDBNull("id_origen") ? null : reader.GetInt32("id_origen"),
                IdDestino = reader.IsDBNull("id_destino") ? null : reader.GetInt32("id_destino"),
                
                // Información descriptiva de los JOINs
                NombreCliente = reader.IsDBNull("nombre_cliente") ? null : reader.GetString("nombre_cliente").Trim(),
                DescripcionOrigen = reader.IsDBNull("descripcion_origen") ? null : reader.GetString("descripcion_origen"),
                DescripcionDestino = reader.IsDBNull("descripcion_destino") ? null : reader.GetString("descripcion_destino")
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