using Microsoft.Data.SqlClient;
using System.Data;

namespace AppTransporte.model
{
    // Extensión para agregar métodos de pedidos automáticos al SqlServerService
    public static class SqlServerServiceExtensions
    {
        // Método nuevo que usa pa_ModificarPedidoAutomatico para cambiar estado, modificar y eliminar
        public static async Task<RespuestaPedidoAutomatico> ModificarPedidoAutomaticoAsync(
            this SqlServerService service,
            int idPedidoAutomatico, 
            string accion, 
            DateTime? nuevaFechaFin = null,
            string nuevosDiasSemana = null,
            TimeSpan? nuevaHora = null,
            string nuevaDescripcion = null,
            int? nuevoOrigen = null,
            int? nuevoDestino = null)
        {
            try
            {
                var connectionString = service.GetType()
                    .GetField("_connectionString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.GetValue(service)?.ToString();

                if (string.IsNullOrEmpty(connectionString))
                    throw new InvalidOperationException("No se pudo obtener la cadena de conexión");

                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand("pa_ModificarPedidoAutomatico", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 180 // 3 minutos de timeout para operaciones complejas
                };

                // Parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@id_pedidoAutomatico", idPedidoAutomatico);
                command.Parameters.AddWithValue("@accion", accion); // 'DESACTIVAR', 'MODIFICAR', 'REACTIVAR'
                command.Parameters.AddWithValue("@nueva_fecha_fin", (object)nuevaFechaFin ?? DBNull.Value);
                command.Parameters.AddWithValue("@nuevos_dias_semana", (object)nuevosDiasSemana ?? DBNull.Value);
                command.Parameters.AddWithValue("@nueva_hora", (object)nuevaHora ?? DBNull.Value);
                command.Parameters.AddWithValue("@nueva_descripcion", (object)nuevaDescripcion ?? DBNull.Value);
                command.Parameters.AddWithValue("@nuevo_origen", (object)nuevoOrigen ?? DBNull.Value);
                command.Parameters.AddWithValue("@nuevo_destino", (object)nuevoDestino ?? DBNull.Value);

                await connection.OpenAsync();

                // El PA no devuelve resultados, solo ejecuta la acción
                int resultado = await command.ExecuteNonQueryAsync();

                return new RespuestaPedidoAutomatico
                {
                    FilasAfectadas = 1, // Consideramos exitoso si no hay excepción
                    Mensaje = $"Operación '{accion}' ejecutada exitosamente en el pedido automático."
                };
            }
            catch (SqlException ex)
            {
                // Registrar el error específico de SQL
                System.Diagnostics.Debug.WriteLine($"Error SQL en ModificarPedidoAutomaticoAsync: {ex.Message}");
                return new RespuestaPedidoAutomatico
                {
                    FilasAfectadas = 0,
                    Mensaje = ex.Message
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error general en ModificarPedidoAutomaticoAsync: {ex.Message}");
                return new RespuestaPedidoAutomatico
                {
                    FilasAfectadas = 0,
                    Mensaje = $"Error inesperado: {ex.Message}"
                };
            }
        }

        // Método actualizado que usa el nuevo PA para cambiar estado
        public static async Task<RespuestaPedidoAutomatico> CambiarEstadoPedidoAutomaticoNuevoAsync(
            this SqlServerService service, 
            int idPedidoAutomatico, 
            bool nuevoEstado)
        {
            string accion = nuevoEstado ? "REACTIVAR" : "DESACTIVAR";
            return await service.ModificarPedidoAutomaticoAsync(idPedidoAutomatico, accion);
        }

        // Método actualizado que usa el nuevo PA para eliminar
        public static async Task<RespuestaPedidoAutomatico> EliminarPedidoAutomaticoNuevoAsync(
            this SqlServerService service, 
            int idPedidoAutomatico)
        {
            return await service.ModificarPedidoAutomaticoAsync(idPedidoAutomatico, "DESACTIVAR");
        }

        // Método actualizado que usa el nuevo PA para modificaciones
        public static async Task<RespuestaPedidoAutomatico> ActualizarPedidoAutomaticoNuevoAsync(
            this SqlServerService service, 
            PedidoAutomatico pedido)
        {
            return await service.ModificarPedidoAutomaticoAsync(
                pedido.IdPedidoAutomatico,
                "MODIFICAR",
                pedido.FechaFin,
                pedido.DiasSemana,
                pedido.HoraProgramada,
                pedido.Descripcion,
                pedido.IdOrigen,
                pedido.IdDestino
            );
        }
    }
}