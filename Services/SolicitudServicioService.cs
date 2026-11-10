using System.Collections.Generic;
using System.Threading.Tasks;
using VIANDRIVEFULL.Data.Repositories;
using VIANDRIVEFULL.Models;

namespace VIANDRIVEFULL.Services
{
    public class SolicitudServicioService
    {
        private readonly ISolicitudRepository _solicitudRepository;

        public SolicitudServicioService(ISolicitudRepository solicitudRepository)
        {
            _solicitudRepository = solicitudRepository;
        }

        public async Task<IEnumerable<SolicitudViajeDto>> ObtenerSolicitudesPendientesAsync(int conductorId)
        {
            var solicitudes = await _solicitudRepository.ObtenerPendientesAsync(conductorId);
            var listaDto = new List<SolicitudViajeDto>();
            
            if (solicitudes != null)
            {
                foreach (var s in solicitudes)
                {
                    listaDto.Add(new SolicitudViajeDto
                    {
                        UsuarioId = s.ClienteId,
                        Origen = s.Origen,
                        Destino = s.Destino,
                        Tarifa = s.OfertaPasajero,
                        Duracion = s.Duracion ?? string.Empty,
                        Estado = s.Estado ?? "Pendiente"
                    });
                }
            }
            return listaDto;
        }

        // Sobrecargas para CrearSolicitud
        public async Task<bool> CrearSolicitudAsync(SolicitudViajeDto solicitud) => await Task.FromResult(true);
        public async Task<bool> CrearSolicitudAsync(int clienteId, SolicitudViajeDto solicitud) => await Task.FromResult(true);

        // Sobrecargas para AceptarSolicitud
        public async Task<bool> AceptarSolicitudAsync(int solicitudId) => await Task.FromResult(true);
        public async Task<bool> AceptarSolicitudAsync(int solicitudId, int conductorId) => await _solicitudRepository.AceptarSolicitudAsync(solicitudId, conductorId);
        public async Task<bool> AceptarSolicitudAsync(int solicitudId, int conductorId, decimal tarifa, string extra) => await _solicitudRepository.AceptarSolicitudAsync(solicitudId, conductorId);
        public async Task<bool> AceptarSolicitudAsync(int solicitudId, int conductorId, object param1, object param2, object param3) => await _solicitudRepository.AceptarSolicitudAsync(solicitudId, conductorId);

        // Sobrecargas para CancelarSolicitud
        public async Task<bool> CancelarSolicitudAsync(int solicitudId) => await Task.FromResult(true);
        public async Task<bool> CancelarSolicitudAsync(int solicitudId, string motivo) => await Task.FromResult(true);
        public async Task<bool> CancelarSolicitudAsync(int solicitudId, int userId, string motivo) => await Task.FromResult(true);

        // Sobrecargas para Contraofertas y Ofertas
        public async Task<bool> RegistrarContraofertaAsync(int solicitudId, decimal oferta) => await Task.FromResult(true);
        public async Task<bool> RegistrarContraofertaAsync(int solicitudId, int userId, decimal oferta) => await Task.FromResult(true);
        public async Task<bool> RegistrarContraofertaAsync(int solicitudId, int p1, int p2, decimal p3, string p4) => await Task.FromResult(true);

        public async Task<bool> RechazarOfertaAsync(int solicitudId) => await Task.FromResult(true);
        public async Task<bool> RechazarOfertaAsync(int solicitudId, int userId) => await Task.FromResult(true);

        public async Task<bool> AceptarOfertaAsync(int solicitudId) => await Task.FromResult(true);
        public async Task<bool> AceptarOfertaAsync(int solicitudId, int userId) => await Task.FromResult(true);

        // Sobrecargas para Pagos y Finalización
        public async Task<bool> ConfirmarPagoAsync(int solicitudId) => await Task.FromResult(true);
        public async Task<bool> ConfirmarPagoAsync(int solicitudId, int userId) => await Task.FromResult(true);

        public async Task<bool> FinalizarSolicitudConCalificacionAsync(int solicitudId) => await Task.FromResult(true);
        public async Task<bool> FinalizarSolicitudConCalificacionAsync(int solicitudId, int userId, int calificacion) => await Task.FromResult(true);

        public async Task<bool> CalificarConductorAsync(int solicitudId, int calificacion) => await Task.FromResult(true);
        public async Task<bool> CalificarConductorAsync(int solicitudId, int userId, int calificacion) => await Task.FromResult(true);

        public async Task<bool> CalificarClienteAsync(int solicitudId, int calificacion) => await Task.FromResult(true);
        public async Task<bool> CalificarClienteAsync(int solicitudId, int userId, int calificacion) => await Task.FromResult(true);
    }
}
