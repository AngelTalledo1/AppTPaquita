using Microsoft.Data.SqlClient;
using System.Data;

namespace AppTransporte.model
{
    // Clase para métodos de pedidos automáticos que extiende la funcionalidad del SqlServerService
    public class PedidosAutomaticosService
    {
        private readonly string _connectionString;

        public PedidosAutomaticosService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Método nuevo que usa pa_ModificarPedidoAutomatico para cambiar estado, modificar y eliminar
        public async Task<RespuestaPedidoAutomatico> ModificarPedidoAutomaticoAsync(
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
                using var connection = new SqlConnection(_connectionString);
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

        // Método actualizado que usa el nuevo PA
        public async Task<RespuestaPedidoAutomatico> CambiarEstadoPedidoAutomaticoAsync(int idPedidoAutomatico, bool nuevoEstado)
        {
            string accion = nuevoEstado ? "REACTIVAR" : "DESACTIVAR";
            return await ModificarPedidoAutomaticoAsync(idPedidoAutomatico, accion);
        }

        // Método actualizado que usa el nuevo PA para eliminar (desactivar permanentemente)
        public async Task<RespuestaPedidoAutomatico> EliminarPedidoAutomaticoAsync(int idPedidoAutomatico)
        {
            return await ModificarPedidoAutomaticoAsync(idPedidoAutomatico, "DESACTIVAR");
        }

        // Método actualizado que usa el nuevo PA para modificaciones
        public async Task<RespuestaPedidoAutomatico> ActualizarPedidoAutomaticoAsync(PedidoAutomatico pedido)
        {
            return await ModificarPedidoAutomaticoAsync(
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