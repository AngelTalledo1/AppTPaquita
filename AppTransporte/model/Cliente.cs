

namespace AppTransporte.model
{
    public class Cliente
    {
        public int IdPersona { get; set; }
        public int IdCliente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string ApePaterno { get; set; } = string.Empty;
        public string ApeMaterno { get; set; } = string.Empty;
        public string NumDoc { get; set; } = "N/A";
        public string Telefono { get; set; } = "N/A";
        public string Direccion { get; set; } = "N/A";
        public string Email { get; set; } = "N/A";
        public int? IdUsuario { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
        public int? IdTipoUsuario { get; set; }
        public int Estado { get; set; }

        // Propiedad calculada para el nombre completo
        public string NombreCompleto => $"{Nombre} {ApePaterno} {ApeMaterno}";
    }
}
