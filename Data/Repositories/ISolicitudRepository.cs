using System.Collections.Generic;
using System.Threading.Tasks;
using VIANDRIVEFULL.Models;
using VIANDRIVEFULL.Models.DTOs;

namespace VIANDRIVEFULL.Data.Repositories
{
    public interface ISolicitudRepository
    {
        Task<SolicitudServicio> ObtenerPorIdAsync(int id);

        // Mantén el alias por si usas el nombre corto en otras partes
        Task<SolicitudServicio> ObtenerPorId(int id);

        Task<bool> Registrar(SolicitudViajeDto dto);
        Task<List<SolicitudServicio>> ObtenerSolicitudesPorEstadoAsync(string estado);
        Task ActualizarAsync(SolicitudServicio solicitud);
    }
}