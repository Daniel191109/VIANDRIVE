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

        public async Task<bool> CrearSolicitudAsync(SolicitudViajeDto solicitud)
        {
            // Lógica para registrar la solicitud en la base de datos
            return await Task.FromResult(true);
        }

        public async Task<bool> AceptarSolicitudAsync(int solicitudId, int conductorId)
        {
            return await _solicitudRepository.AceptarSolicitudAsync(solicitudId, conductorId);
        }

        public async Task<bool> AceptarSolicitudAsync(int solicitudId, int conductorId, decimal tarifa, string extra)
        {
            // Sobrecarga por si el controlador envía más parámetros
            return await _solicitudRepository.AceptarSolicitudAsync(solicitudId, conductorId);
        }

        public async Task<bool> CancelarSolicitudAsync(int solicitudId, string motivo)
        {
            return await Task.FromResult(true);
        }

        public async Task<bool> RegistrarContraofertaAsync(int solicitudId, decimal nuevaOferta)
        {
            return await Task.FromResult(true);
        }

        public async Task<bool> RechazarOfertaAsync(int solicitudId)
        {
            return await Task.FromResult(true);
        }

        public async Task<bool> AceptarOfertaAsync(int solicitudId)
        {
            return await Task.FromResult(true);
        }

        public async Task<bool> ConfirmarPagoAsync(int solicitudId)
        {
            return await Task.FromResult(true);
        }

        public async Task<bool> FinalizarSolicitudConCalificacionAsync(int solicitudId)
        {
            return await Task.FromResult(true);
        }

        public async Task<bool> CalificarConductorAsync(int solicitudId, int calificacion)
        {
            return await Task.FromResult(true);
        }

        public async Task<bool> CalificarClienteAsync(int solicitudId, int calificacion)
        {
            return await Task.FromResult(true);
        }
    }
}
