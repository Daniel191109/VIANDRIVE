using Microsoft.EntityFrameworkCore;
using VIANDRIVEFULL.Data;
using VIANDRIVEFULL.Data.Repositories;
using VIANDRIVEFULL.Models;
using VIANDRIVEFULL.Models.DTOs;

namespace VIANDRIVEFULL.Services
{
    public class SolicitudServicioService
    {
        private readonly AppDbContext _context;
        private readonly ISolicitudRepository _solicitudRepository;

        public SolicitudServicioService(AppDbContext context, ISolicitudRepository solicitudRepository)
        {
            _context = context;
            _solicitudRepository = solicitudRepository;
        }

        public class ResultadoOperacion
        {
            public bool Exito { get; set; }
            public string Mensaje { get; set; } = string.Empty;
            public SolicitudServicio? Solicitud { get; set; }
        }

        public async Task<IEnumerable<SolicitudViajeDto>> ObtenerSolicitudesPendientesAsync(int conductorId)
        {
            var solicitudes = await _solicitudRepository.ObtenerPendientesAsync(conductorId);
            return solicitudes.Select(s => new SolicitudViajeDto
            {
                UsuarioId = s.ClienteId,
                Origen = s.Origen ?? string.Empty,
                Destino = s.Destino ?? string.Empty,
                Tarifa = s.OfertaPasajero,
                Duracion = string.Empty,
                Estado = s.Estado ?? "Pendiente",
                LatOrigen = s.LatOrigen,
                LngOrigen = s.LngOrigen,
                LatDestino = s.LatDestino,
                LngDestino = s.LngDestino
            }).ToList();
        }

        public async Task<SolicitudServicio> CrearSolicitudAsync(SolicitudServicioDto dto, string usuario)
        {
            if (dto.ClienteId <= 0)
                throw new ArgumentException("ClienteId debe ser el Id del cliente autenticado.");
            if (string.IsNullOrWhiteSpace(dto.Origen) || string.IsNullOrWhiteSpace(dto.Destino))
                throw new ArgumentException("Origen y destino son obligatorios.");
            if (dto.OfertaPasajero < 0)
                throw new ArgumentException("La oferta no puede ser negativa.");

            var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == dto.ClienteId);
            if (!clienteExiste)
                throw new KeyNotFoundException("El cliente indicado no existe.");

            var oferta = dto.OfertaPasajero > 0 ? dto.OfertaPasajero : 8000m;
            var solicitud = new SolicitudServicio
            {
                ClienteId = dto.ClienteId,
                ConductorId = null,
                VehiculoId = null,
                Origen = dto.Origen.Trim(),
                Destino = dto.Destino.Trim(),
                LatOrigen = dto.LatOrigen,
                LngOrigen = dto.LngOrigen,
                LatDestino = dto.LatDestino,
                LngDestino = dto.LngDestino,
                FechaSolicitud = DateTime.Now,
                Estado = "Pendiente",
                PrecioSugerido = oferta,
                OfertaPasajero = oferta,
                MedioPago = string.IsNullOrWhiteSpace(dto.MedioPago) ? "Efectivo" : dto.MedioPago.Trim(),
                PagoConfirmado = false,
                OfertaAceptada = false,
                OfertaRechazada = false
            };

            _context.SolicitudesServicio.Add(solicitud);
            await _context.SaveChangesAsync();
            return solicitud;
        }

        public async Task<ResultadoOperacion> AceptarSolicitudAsync(int solicitudId, int conductorId, double lat = 0, double lng = 0)
        {
            if (conductorId <= 0)
                return new ResultadoOperacion { Exito = false, Mensaje = "ConductorId es obligatorio." };

            var solicitud = await _context.SolicitudesServicio.FindAsync(solicitudId);
            if (solicitud == null)
                return new ResultadoOperacion { Exito = false, Mensaje = "Solicitud no encontrada." };
            if (!await _context.Conductores.AnyAsync(c => c.Id == conductorId))
                return new ResultadoOperacion { Exito = false, Mensaje = "Conductor no encontrado." };
            if (solicitud.Estado != "Pendiente" && solicitud.Estado != "ConPostulaciones")
                return new ResultadoOperacion { Exito = false, Mensaje = "La solicitud ya no está disponible." };

            solicitud.ConductorId = conductorId;
            solicitud.LatConductor = lat;
            solicitud.LngConductor = lng;
            solicitud.Estado = "Aceptada";
            await _context.SaveChangesAsync();
            return new ResultadoOperacion { Exito = true, Mensaje = "Viaje aceptado.", Solicitud = solicitud };
        }

        public async Task<ResultadoOperacion> CancelarSolicitudAsync(int id, string motivo, string usuario)
        {
            var solicitud = await _context.SolicitudesServicio.FindAsync(id);
            if (solicitud == null)
                return new ResultadoOperacion { Exito = false, Mensaje = "Solicitud no encontrada." };
            if (solicitud.Estado == "Finalizada")
                return new ResultadoOperacion { Exito = false, Mensaje = "Una solicitud finalizada no se puede cancelar." };

            solicitud.Estado = "Cancelada";
            solicitud.FechaCancelacion = DateTime.Now;
            solicitud.MotivoCancelacion = string.IsNullOrWhiteSpace(motivo) ? "Cancelada" : motivo.Trim();
            solicitud.UsuarioCancela = usuario;
            await _context.SaveChangesAsync();
            return new ResultadoOperacion { Exito = true, Mensaje = "Cancelada", Solicitud = solicitud };
        }

        public async Task<ResultadoOperacion> RegistrarContraofertaAsync(int solicitudId, decimal tarifa, string usuario, double lat = 0, double lng = 0, int conductorId = 0)
        {
            var solicitud = await _context.SolicitudesServicio.FindAsync(solicitudId);
            if (solicitud == null)
                return new ResultadoOperacion { Exito = false, Mensaje = "Solicitud no encontrada." };
            if (tarifa <= 0)
                return new ResultadoOperacion { Exito = false, Mensaje = "La tarifa ofrecida debe ser mayor que cero." };
            if (conductorId > 0 && !await _context.Conductores.AnyAsync(c => c.Id == conductorId))
                return new ResultadoOperacion { Exito = false, Mensaje = "Conductor no encontrado." };

            if (conductorId > 0)
                solicitud.ConductorId = conductorId;
            solicitud.ContraofertaConductor = tarifa;
            solicitud.LatConductor = lat;
            solicitud.LngConductor = lng;
            if (solicitud.Estado == "Pendiente")
                solicitud.Estado = "ConPostulaciones";
            await _context.SaveChangesAsync();
            return new ResultadoOperacion { Exito = true, Mensaje = "Contraoferta registrada.", Solicitud = solicitud };
        }

        public async Task<ResultadoOperacion> RechazarOfertaAsync(int id, string usuario)
        {
            var solicitud = await _context.SolicitudesServicio.FindAsync(id);
            if (solicitud == null)
                return new ResultadoOperacion { Exito = false, Mensaje = "Solicitud no encontrada." };
            solicitud.OfertaRechazada = true;
            solicitud.FechaRechazoOferta = DateTime.Now;
            await _context.SaveChangesAsync();
            return new ResultadoOperacion { Exito = true, Mensaje = "Oferta rechazada.", Solicitud = solicitud };
        }

        public async Task<ResultadoOperacion> AceptarOfertaAsync(int id, string usuario)
        {
            var solicitud = await _context.SolicitudesServicio.FindAsync(id);
            if (solicitud == null)
                return new ResultadoOperacion { Exito = false, Mensaje = "Solicitud no encontrada." };
            solicitud.OfertaAceptada = true;
            solicitud.PrecioFinal = solicitud.ContraofertaConductor ?? solicitud.PrecioSugerido;
            solicitud.Estado = "Aceptada";
            await _context.SaveChangesAsync();
            return new ResultadoOperacion { Exito = true, Mensaje = "Oferta aceptada.", Solicitud = solicitud };
        }

        public async Task<ResultadoOperacion> ConfirmarPagoAsync(int id, string usuario)
        {
            var solicitud = await _context.SolicitudesServicio.FindAsync(id);
            if (solicitud == null)
                return new ResultadoOperacion { Exito = false, Mensaje = "Solicitud no encontrada." };
            solicitud.PagoConfirmado = true;
            solicitud.FechaPagoConfirmado = DateTime.Now;
            await _context.SaveChangesAsync();
            return new ResultadoOperacion { Exito = true, Mensaje = "Pago confirmado.", Solicitud = solicitud };
        }

        public async Task<ResultadoOperacion> FinalizarSolicitudConCalificacionAsync(int id, FinalizarServicioDto dto, string usuario)
        {
            var solicitud = await _context.SolicitudesServicio.FindAsync(id);
            if (solicitud == null)
                return new ResultadoOperacion { Exito = false, Mensaje = "Solicitud no encontrada." };
            if (dto == null)
                return new ResultadoOperacion { Exito = false, Mensaje = "Los datos de finalización son obligatorios." };

            solicitud.Estado = "Finalizada";
            solicitud.FechaFinalizacion = DateTime.Now;
            if (dto.TarifaFinal > 0)
                solicitud.PrecioFinal = dto.TarifaFinal;
            if (dto.CalificacionPasajero >= 1 && dto.CalificacionPasajero <= 5)
                solicitud.CalificacionPasajero = dto.CalificacionPasajero;
            await _context.SaveChangesAsync();
            return new ResultadoOperacion { Exito = true, Mensaje = "Finalizada", Solicitud = solicitud };
        }

        public async Task<ResultadoOperacion> CalificarConductorAsync(int id, CalificacionConductorDTO dto, string usuario)
        {
            var solicitud = await _context.SolicitudesServicio.FindAsync(id);
            if (solicitud == null)
                return new ResultadoOperacion { Exito = false, Mensaje = "Solicitud no encontrada." };
            if (dto == null || dto.Calificacion < 1 || dto.Calificacion > 5)
                return new ResultadoOperacion { Exito = false, Mensaje = "La calificación debe estar entre 1 y 5." };

            solicitud.CalificacionConductor = dto.Calificacion;
            solicitud.ComentarioConductor = dto.Comentario;
            solicitud.FechaCalificacionConductor = DateTime.Now;
            solicitud.UsuarioCalificaConductor = usuario;
            await _context.SaveChangesAsync();
            return new ResultadoOperacion { Exito = true, Mensaje = "Calificación registrada.", Solicitud = solicitud };
        }

        public async Task<ResultadoOperacion> CalificarClienteAsync(int id, CalificacionClienteDTO dto, string usuario)
        {
            var solicitud = await _context.SolicitudesServicio.FindAsync(id);
            if (solicitud == null)
                return new ResultadoOperacion { Exito = false, Mensaje = "Solicitud no encontrada." };
            if (dto == null || dto.Calificacion < 1 || dto.Calificacion > 5)
                return new ResultadoOperacion { Exito = false, Mensaje = "La calificación debe estar entre 1 y 5." };

            solicitud.CalificacionPasajero = dto.Calificacion;
            await _context.SaveChangesAsync();
            return new ResultadoOperacion { Exito = true, Mensaje = "Calificación registrada con éxito.", Solicitud = solicitud };
        }
    }
}
