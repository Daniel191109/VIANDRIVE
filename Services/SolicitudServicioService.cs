using System.Collections.Generic;
using System.Threading.Tasks;
using ViandriveApp.Models;

namespace ViandriveApp.Services
{
    public static class SolicitudService
    {
        private static readonly ApiService _apiService = new ApiService();

        public static async Task<List<SolicitudViajeDTO>> ObtenerSolicitudesPendientes(int conductorId)
        {
            // Nota: Ajustado a "Pendiente" con P mayúscula para que coincida con el backend
            var endpoint = $"api/solicitudes?estado=Pendiente&conductorId={conductorId}";
            var solicitudes = await _apiService.GetAsync<List<SolicitudViajeDTO>>(endpoint);

            return solicitudes ?? new List<SolicitudViajeDTO>();
        }

        public static async Task<bool> AceptarSolicitud(int solicitudId, int conductorId)
        {
            // Coincide con el flujo de AceptarSolicitudAsync en tu backend
            var endpoint = "api/solicitudes/aceptar";
            var payload = new
            {
                id = solicitudId,
                conductorIdParam = conductorId
            };

            var response = await _apiService.PostAsync(endpoint, payload);
            return response != null && response.IsSuccessStatusCode;
        }
    }
}
