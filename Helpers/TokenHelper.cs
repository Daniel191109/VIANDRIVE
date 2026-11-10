using Microsoft.Extensions.Configuration;
using VIANDRIVEFULL.Services;

namespace VIANDRIVEFULL.Helpers
{
    // Compatibilidad para código antiguo: ya no mantiene una clave JWT hardcodeada.
    public static class TokenHelper
    {
        public static string GenerarToken(int clienteId, string nombre, string correo, IConfiguration configuration)
        {
            return new JwtService(configuration).GenerarToken(
                clienteId.ToString(), "cliente", nombre, correo);
        }
    }
}
