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

        public async Task<IEnumerable<SolicitudViajeDTO>> ObtenerSolicitudesPendientesAsync(int conductorId)
        {
            // Lógica real de backend usando tu repositorio o base de datos
            var solicitudes = await _solicitudRepository.ObtenerPendientesAsync(conductorId);
            
            // Mapeo o conversión a DTO según la estructura de tus entidades
            var listaDto = new List<SolicitudViajeDTO>();
            
            if (solicitudes != null)
            {
                foreach (var s in solicitudes)
                {
                    listaDto.Add(new SolicitudViajeDTO
                    {
                        ClienteId = s.ClienteId,
                        Origen = s.Origen,
                        Destino = s.Destino,
                        OfertaPasajero = s.OfertaPasajero,
                        MedioPago = s.MedioPago
                    });
                }
            }

            return listaDto;
        }

        public async Task<bool> AceptarSolicitudAsync(int solicitudId, int conductorId)
        {
            // Lógica de backend para actualizar el estado de la solicitud en la BD
            bool resultado = await _solicitudRepository.AceptarSolicitudAsync(solicitudId, conductorId);
            return resultado;
        }
    }
}
