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
                        UsuarioId = s.ClienteId, // O el campo correspondiente de tu BD
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

        public async Task<bool> AceptarSolicitudAsync(int solicitudId, int conductorId)
        {
            bool resultado = await _solicitudRepository.AceptarSolicitudAsync(solicitudId, conductorId);
            return resultado;
        }
    }
}
