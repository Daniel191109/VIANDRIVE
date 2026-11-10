using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VIANDRIVEFULL.Data;
using VIANDRIVEFULL.Models;

namespace VIANDRIVEFULL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostulacionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostulacionesController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/postulaciones
        [HttpPost]
        public async Task<IActionResult> RegistrarPostulacion([FromBody] PostulacionDto dto)
        {
            if (dto == null || dto.SolicitudId <= 0 || dto.ConductorId <= 0)
                return BadRequest(new { mensaje = "Los datos de la postulación son inválidos." });

            var solicitud = await _context.SolicitudesServicio.FindAsync(dto.SolicitudId);
            if (solicitud == null)
                return NotFound(new { mensaje = "Solicitud no encontrada." });

            if (solicitud.Estado == "EnCamino" || solicitud.Estado == "Finalizado")
                return BadRequest(new { mensaje = "La carrera ya fue asignada a otro conductor." });

            var postulacionExistente = await _context.Postulaciones
                .FirstOrDefaultAsync(p => p.SolicitudId == dto.SolicitudId && p.ConductorId == dto.ConductorId);

            if (postulacionExistente != null)
            {
                postulacionExistente.TarifaOfrecida = dto.TarifaOfrecida;
                postulacionExistente.Estado = "Pendiente";
                _context.Postulaciones.Update(postulacionExistente);
            }
            else
            {
                var vehiculo = await _context.Vehiculos
                    .FirstOrDefaultAsync(v => v.ConductorId == dto.ConductorId);

                var nuevaPostulacion = new PostulacionSolicitud
                {
                    SolicitudId = dto.SolicitudId,
                    ConductorId = dto.ConductorId,
                    TarifaOfrecida = dto.TarifaOfrecida,
                    Estado = "Pendiente",
                    FechaCreacion = DateTime.Now,
                    NombreConductor = "Conductor Viandrive",
                    Vehiculo = vehiculo != null ? $"{vehiculo.Marca} {vehiculo.Modelo}" : "Vehículo registrado",
                    Placa = vehiculo?.Placa ?? "",
                    Calificacion = 5.0
                };

                _context.Postulaciones.Add(nuevaPostulacion);
            }

            solicitud.Estado = "ConPostulaciones";
            await _context.SaveChangesAsync();

            return Ok(new { exito = true, mensaje = "Oferta enviada correctamente al cliente." });
        }

        // GET: api/postulaciones/solicitud/5
        [HttpGet("solicitud/{solicitudId}")]
        public async Task<IActionResult> ObtenerPostulacionesPorSolicitud(int solicitudId)
        {
            var postulaciones = await _context.Postulaciones
                .Where(p => p.SolicitudId == solicitudId && p.Estado == "Pendiente")
                .OrderByDescending(p => p.FechaCreacion)
                .Select(p => new
                {
                    id = p.Id,
                    conductorId = p.ConductorId,
                    nombreConductor = p.NombreConductor,
                    vehiculo = p.Vehiculo,
                    placa = p.Placa,
                    calificacion = p.Calificacion,
                    tarifaOfrecida = p.TarifaOfrecida
                })
                .ToListAsync();

            return Ok(new { exito = true, postulaciones });
        }

        // PUT: api/postulaciones/aceptar/15
        [HttpPut("aceptar/{postulacionId}")]
        public async Task<IActionResult> AceptarPostulacion(int postulacionId)
        {
            var postulacionSeleccionada = await _context.Postulaciones.FindAsync(postulacionId);
            if (postulacionSeleccionada == null)
                return NotFound(new { mensaje = "La postulación no existe." });

            var solicitud = await _context.SolicitudesServicio.FindAsync(postulacionSeleccionada.SolicitudId);
            if (solicitud == null)
                return NotFound(new { mensaje = "Solicitud no encontrada." });

            postulacionSeleccionada.Estado = "Aceptada";

            var otrasPostulaciones = await _context.Postulaciones
                .Where(p => p.SolicitudId == solicitud.Id && p.Id != postulacionId)
                .ToListAsync();

            foreach (var otra in otrasPostulaciones)
            {
                otra.Estado = "Rechazada";
            }

            solicitud.ConductorId = postulacionSeleccionada.ConductorId;
            solicitud.OfertaPasajero = postulacionSeleccionada.TarifaOfrecida;
            solicitud.Estado = "EnCamino";

            await _context.SaveChangesAsync();

            return Ok(new { exito = true, mensaje = "Conductor asignado e inicio de viaje." });
        }

        // GET: api/postulaciones/estado?solicitudId=5&conductorId=1
        [HttpGet("estado")]
        public async Task<IActionResult> ConsultarEstadoConductor([FromQuery] int solicitudId, [FromQuery] int conductorId)
        {
            var postulacion = await _context.Postulaciones
                .Where(p => p.SolicitudId == solicitudId && p.ConductorId == conductorId)
                .OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync();

            if (postulacion == null)
                return Ok(new { estado = "SinPostular" });

            return Ok(new { estado = postulacion.Estado });
        }
    }

    public class PostulacionDto
    {
        public int SolicitudId { get; set; }
        public int ConductorId { get; set; }
        public decimal TarifaOfrecida { get; set; }
    }
}