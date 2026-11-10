namespace VIANDRIVEFULL.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly IConfiguration _config;

        public NotificacionService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> EnviarSmsAsync(string celular, string mensaje)
        {
            // 🚫 Envío de SMS desactivado temporalmente
            // Puedes reactivar esta lógica cuando lo decidas

            // 📝 Registro opcional en consola para trazabilidad
            Console.WriteLine($"[SMS DESACTIVADO] A: {celular} | Mensaje: {mensaje}");

            return true; // Simula éxito para no romper el flujo
        }
    }
}
