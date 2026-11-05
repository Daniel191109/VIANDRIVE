using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VIANDRIVEFULL.Data;
using VIANDRIVEFULL.Models;
using VIANDRIVEFULL.Models.DTOs;
using VIANDRIVEFULL.Services;
using VIANDRIVEFULL.Data.Repositories;

namespace VIANDRIVEFULL.Controllers
{
    [ApiController]
    [Route("api/solicitudes")]
    public class SolicitudesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly RegistroPagoService _registroPagoService;
        private readonly SolicitudServicioService _service;
        private readonly ISolicitudRepository _repository;

        public SolicitudesController(
            AppDbContext context,
            SolicitudServicioService service,
            RegistroPagoService registroPagoService,
            ISolicitudRepository repository)
        {
            _context = context;
            _service = service;
            _registroPagoService = registroPagoService;
            _repository = repository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var solicitud = await _repository.ObtenerPorIdAsync(id);
            if (solicitud == null)
                return NotFound(new { mensaje = "Solicitud no encontrada." });

            return Ok(new
            {
                id = solicitud.Id,
                clienteId = solicitud.ClienteId,
                nombrePasajero = solicitud.Cliente != null ? solicitud.Cliente.Nombre : "Pasajero Viandrive",
                conductorId = solicitud.ConductorId,
                nombreConductor = solicitud.Conductor != null ? solicitud.Conductor.Nombre : "Conductor Asignado",
                vehiculoId = solicitud.VehiculoId, // <--- ID del vehículo incluido
                vehiculoInfo = solicitud.Vehiculo != null ? $"{solicitud.Vehiculo.Marca} {solicitud.Vehiculo.Modelo} • {solicitud.Vehiculo.Placa}" : "Vehículo Registrado",
                vehiculo = solicitud.Vehiculo != null ? new
                {
                    id = solicitud.Vehiculo.Id,
                    marca = solicitud.Vehiculo.Marca,
                    modelo = solicitud.Vehiculo.Modelo,
                    placa = solicitud.Vehiculo.Placa,
                    color = solicitud.Vehiculo.Color
                } : null, // <--- Objeto completo por si tu DTO del cliente lo requiere
                calificacionConductor = 5.0,
                totalViajesConductor = 0,
                estado = solicitud.Estado,
                origen = solicitud.Origen,
                destino = solicitud.Destino,
                latOrigen = solicitud.LatOrigen,
                lngOrigen = solicitud.LngOrigen,
                latDestino = solicitud.LatDestino,
                lngDestino = solicitud.LngDestino,
                latConductor = solicitud.LatConductor,
                lngConductor = solicitud.LngConductor,
                precioSugerido = solicitud.PrecioSugerido,
                tarifaSugerida = solicitud.TarifaSugerida,
                ofertaPasajero = solicitud.OfertaPasajero,
                contraofertaConductor = solicitud.ContraofertaConductor,
                precioFinal = solicitud.PrecioFinal,
                medioPago = solicitud.MedioPago,
                pagoConfirmado = solicitud.PagoConfirmado,
                ofertaAceptada = solicitud.OfertaAceptada,
                fechaSolicitud = solicitud.FechaSolicitud
            });
        }

        [HttpGet("pendientes")]
        public async Task<IActionResult> ObtenerSolicitudesPendientes()
        {
            var ahora = DateTime.Now;

            var pendientes = await _context.SolicitudesServicio
                .Where(s => s.Estado == "Pendiente" || s.Estado == "ConPostulaciones")
                .ToListAsync();

            bool huboCambios = false;
            foreach (var s in pendientes)
            {
                if ((ahora - s.FechaSolicitud).TotalMinutes > 10)
                {
                    s.Estado = "Cancelada";
                    s.MotivoCancelacion = "Expirado: El tiempo límite de 10 minutos ha expirado.";
                    huboCambios = true;
                }
            }

            if (huboCambios)
            {
                await _context.SaveChangesAsync();
            }

            var vigentes = pendientes
                .Where(s => s.Estado == "Pendiente" || s.Estado == "ConPostulaciones")
                .OrderByDescending(s => s.Id)
                .Select(s => new
                {
                    id = s.Id,
                    nombrePasajero = "Pasajero Viandrive",
                    origen = s.Origen,
                    destino = s.Destino,
                    oferta = s.OfertaPasajero > 0 ? s.OfertaPasajero : s.PrecioSugerido,
                    latOrigen = s.LatOrigen,
                    lngOrigen = s.LngOrigen,
                    latDestino = s.LatDestino,
                    lngDestino = s.LngDestino,
                    minutosTranscurridos = (int)(ahora - s.FechaSolicitud).TotalMinutes
                }).ToList();

            return Ok(vigentes);
        }

        [HttpPost("crear-basica")]
        public async Task<IActionResult> CrearSolicitudBasica([FromBody] SolicitudViajeDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Destino))
                return BadRequest("Datos inválidos.");

            var resultado = await _repository.Registrar(dto);
            if (resultado) return Ok(new { mensaje = "Solicitud registrada correctamente." });

            return StatusCode(500, "Error al registrar la solicitud.");
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] SolicitudServicioDto dto)
        {
            if (dto == null) return BadRequest(new { mensaje = "Los datos de la solicitud no son válidos." });

            var usuario = User.Identity?.Name ?? "Sistema";
            var solicitud = await _service.CrearSolicitudAsync(dto, usuario);

            return Ok(new
            {
                mensaje = "Solicitud registrada correctamente.",
                solicitud = new
                {
                    id = solicitud.Id,
                    clienteId = solicitud.ClienteId,
                    conductorId = solicitud.ConductorId,
                    vehiculoId = solicitud.VehiculoId,
                    origen = solicitud.Origen,
                    destino = solicitud.Destino,
                    latOrigen = solicitud.LatOrigen,
                    lngOrigen = solicitud.LngOrigen,
                    latDestino = solicitud.LatDestino,
                    lngDestino = solicitud.LngDestino,
                    fechaSolicitud = solicitud.FechaSolicitud,
                    estado = solicitud.Estado,
                    precioSugerido = solicitud.PrecioSugerido,
                    ofertaPasajero = solicitud.OfertaPasajero,
                    medioPago = solicitud.MedioPago
                }
            });
        }

        [HttpPut("{id}/aceptar")]
        public async Task<IActionResult> Aceptar(int id)
        {
            var resultado = await _service.AceptarSolicitudAsync(id);
            if (!resultado.Exito) return NotFound(new { mensaje = resultado.Mensaje });

            return Ok(new { mensaje = "Solicitud aceptada correctamente.", solicitud = new { id = resultado.Solicitud.Id, estado = resultado.Solicitud.Estado } });
        }

        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> Cancelar(int id, [FromBody] CancelacionDTO dto)
        {
            var usuario = User.Identity?.Name ?? "Sistema";
            var resultado = await _service.CancelarSolicitudAsync(id, dto?.Motivo ?? "Cancelado por el usuario/conductor", usuario);

            if (!resultado.Exito) return BadRequest(new { mensaje = resultado.Mensaje });

            return Ok(new { mensaje = "Solicitud cancelada correctamente.", solicitud = new { id = resultado.Solicitud.Id, estado = resultado.Solicitud.Estado } });
        }

        [HttpPost("contraoferta")]
        public async Task<IActionResult> RegistrarContraoferta([FromBody] ContraofertaDto dto)
        {
            if (dto == null || dto.SolicitudId <= 0) return BadRequest(new { mensaje = "Los datos de la oferta son inválidos." });

            var usuario = User.Identity?.Name ?? $"Conductor_{dto.ConductorId}";

            if (dto.EsContraoferta)
            {
                var resultado = await _service.RegistrarContraofertaAsync(dto.SolicitudId, dto.TarifaOfrecida, usuario, dto.LatConductor, dto.LngConductor);
                if (!resultado.Exito) return BadRequest(new { mensaje = resultado.Mensaje });

                return Ok(new { mensaje = "Contraoferta registrada.", solicitud = new { id = resultado.Solicitud.Id, estado = resultado.Solicitud.Estado } });
            }
            else
            {
                var resultado = await _service.AceptarSolicitudAsync(dto.SolicitudId, dto.ConductorId, dto.LatConductor, dto.LngConductor);
                if (!resultado.Exito) return BadRequest(new { mensaje = resultado.Mensaje });

                return Ok(new { mensaje = "Viaje aceptado.", solicitud = new { id = resultado.Solicitud.Id, estado = resultado.Solicitud.Estado } });
            }
        }

        [HttpPut("{id}/contraoferta")]
        public async Task<IActionResult> RegistrarContraofertaPorId(int id, [FromBody] decimal valor)
        {
            var usuario = User.Identity?.Name ?? "Sistema";
            var resultado = await _service.RegistrarContraofertaAsync(id, valor, usuario);
            if (!resultado.Exito) return BadRequest(new { mensaje = resultado.Mensaje });
            return Ok(new { mensaje = "Contraoferta registrada.", solicitud = new { id = resultado.Solicitud.Id, estado = resultado.Solicitud.Estado } });
        }

        [HttpPut("{id}/rechazar-oferta")]
        public async Task<IActionResult> RechazarOferta(int id)
        {
            var usuario = User.Identity?.Name ?? "Sistema";
            var resultado = await _service.RechazarOfertaAsync(id, usuario);
            if (!resultado.Exito) return BadRequest(new { mensaje = resultado.Mensaje });
            return Ok(new { mensaje = "Oferta rechazada.", solicitud = new { id = resultado.Solicitud.Id } });
        }

        [HttpPut("{id}/aceptar-oferta")]
        public async Task<IActionResult> AceptarOferta(int id)
        {
            var usuario = User.Identity?.Name ?? "Sistema";
            var resultado = await _service.AceptarOfertaAsync(id, usuario);
            if (!resultado.Exito) return BadRequest(new { mensaje = resultado.Mensaje });
            return Ok(new { mensaje = "Oferta aceptada.", solicitud = new { id = resultado.Solicitud.Id, estado = resultado.Solicitud.Estado } });
        }

        [HttpPut("{id}/confirmar-pago")]
        public async Task<IActionResult> ConfirmarPago(int id)
        {
            var usuario = User.Identity?.Name ?? "Sistema";
            var resultado = await _service.ConfirmarPagoAsync(id, usuario);
            if (!resultado.Exito) return BadRequest(new { mensaje = resultado.Mensaje });
            return Ok(new { mensaje = "Pago confirmado.", solicitud = new { id = resultado.Solicitud.Id } });
        }

        [HttpPut("{id}/finalizar")]
        public async Task<IActionResult> Finalizar(int id, [FromBody] FinalizarServicioDto dto)
        {
            var usuario = User.Identity?.Name ?? "Sistema";
            var resultado = await _service.FinalizarSolicitudConCalificacionAsync(id, dto, usuario);

            if (!resultado.Exito) return BadRequest(new { mensaje = resultado.Mensaje });

            return Ok(new { mensaje = "Solicitud finalizada.", solicitud = new { id = resultado.Solicitud.Id, estado = resultado.Solicitud.Estado } });
        }

        [HttpPut("{id}/calificar-conductor")]
        public async Task<IActionResult> CalificarConductor(int id, [FromBody] CalificacionConductorDTO dto)
        {
            var usuario = User.Identity?.Name ?? "Sistema";
            var resultado = await _service.CalificarConductorAsync(id, dto, usuario);
            if (!resultado.Exito) return BadRequest(new { mensaje = resultado.Mensaje });
            return Ok(new { mensaje = "Calificación registrada.", solicitud = new { id = resultado.Solicitud.Id } });
        }

        [HttpPut("{id}/calificar-cliente")]
        public async Task<IActionResult> CalificarCliente(int id, [FromBody] CalificacionClienteDTO dto)
        {
            var usuario = User.Identity?.Name ?? "Sistema";
            var resultado = await _service.CalificarClienteAsync(id, dto, usuario);
            if (!resultado.Exito) return BadRequest(new { mensaje = resultado.Mensaje });
            return Ok(new { mensaje = resultado.Mensaje });
        }

        [HttpPost("registrar-pago/{id}")]
        public async Task<IActionResult> RegistrarPago(int id, [FromBody] PagoSolicitudDTO data)
        {
            var usuario = User.Identity?.Name ?? "Sistema";
            var result = await _registroPagoService.RegistrarPagoAsync(id, data, usuario);
            if (!result.Exito) return BadRequest(new { mensaje = result.Mensaje });
            return Ok(new { mensaje = result.Mensaje });
        }
    }
}