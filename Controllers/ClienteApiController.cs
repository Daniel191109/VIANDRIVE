using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VIANDRIVEFULL.Data;
using VIANDRIVEFULL.Helpers;
using VIANDRIVEFULL.Models;
using VIANDRIVEFULL.Models.DTOs;
using VIANDRIVEFULL.Services;

namespace VIANDRIVEFULL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/clientes")]
    public class ClienteApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICorreoService _correoService;
        private readonly JwtService _jwtService;
        private readonly ILogger<ClienteApiController> _logger;

        public ClienteApiController(
            AppDbContext context,
            ICorreoService correoService,
            JwtService jwtService,
            ILogger<ClienteApiController> logger)
        {
            _context = context;
            _correoService = correoService;
            _jwtService = jwtService;
            _logger = logger;
        }

        // Rutas compatibles: /api/ClienteApi/registrar y /api/clientes/registrar
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] ClienteRegistroDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (!SeguridadHelper.ValidarClave(dto.Clave))
                return BadRequest(new { mensaje = "La contraseña debe tener mínimo 8 caracteres, incluir letras, números y un carácter especial." });

            var correo = dto.Correo.Trim().ToLowerInvariant();
            var celular = dto.Celular.Trim();

            var correoExiste = await _context.Clientes.AnyAsync(c => c.Correo.ToLower() == correo)
                || await _context.Conductores.AnyAsync(c => c.Email != null && c.Email.ToLower() == correo);
            if (correoExiste)
                return Conflict(new { mensaje = "Ya existe un cliente registrado con ese correo electrónico." });

            var celularExiste = await _context.Clientes.AnyAsync(c => c.Celular == celular);
            if (celularExiste)
                return Conflict(new { mensaje = "Ya existe un cliente registrado con ese número de celular." });

            var cliente = new Cliente
            {
                NombreCompleto = dto.NombreCompleto.Trim(),
                Correo = correo,
                Celular = celular,
                MetodoPagoPreferido = string.IsNullOrWhiteSpace(dto.MetodoPagoPreferido)
                    ? "Efectivo"
                    : dto.MetodoPagoPreferido.Trim(),
                FechaRegistro = DateTime.Now,
                Clave = SeguridadHelper.EncriptarClave(dto.Clave)
            };

            try
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "No se pudo guardar el registro del cliente {Correo}", correo);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { mensaje = "No fue posible guardar el cliente. Verifica la estructura de la base de datos." });
            }

            // Un fallo del proveedor de correo no revierte un registro ya persistido.
            var mensaje = $"Hola {cliente.NombreCompleto}, tu registro fue exitoso en Viandrive.";
            var correoEnviado = await _correoService.EnviarCorreoAsync(correo, "Registro exitoso", mensaje);

            try
            {
                _context.NotificacionLog.Add(new NotificacionLog
                {
                    Tipo = "Correo",
                    Destinatario = correo,
                    FechaEnvio = DateTime.Now,
                    Estado = correoEnviado ? "Enviado" : "Error",
                    Mensaje = mensaje,
                    ClienteId = cliente.Id
                });
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning(ex, "El cliente {ClienteId} se guardó, pero no se pudo registrar el log de correo", cliente.Id);
            }

            return Ok(new
            {
                mensaje = "Cliente registrado correctamente.",
                clienteId = cliente.Id,
                confirmacion = correoEnviado ? "Enviada" : "No disponible"
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ClienteLoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Correo) || string.IsNullOrWhiteSpace(dto.Clave))
                return BadRequest(new { mensaje = "Correo y contraseña son obligatorios." });

            var correo = dto.Correo.Trim().ToLowerInvariant();
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Correo.ToLower() == correo);

            if (cliente == null || !HashHelper.Validar(dto.Clave, cliente.Clave))
                return Unauthorized(new { mensaje = "Credenciales inválidas" });

            // El token se firma con la misma clave, emisor y audiencia que valida Program.cs.
            var token = _jwtService.GenerarToken(
                cliente.Id.ToString(), "cliente", cliente.NombreCompleto, cliente.Correo);

            return Ok(new
            {
                mensaje = "Inicio de sesión exitoso",
                clienteId = cliente.Id,
                nombre = cliente.NombreCompleto,
                token
            });
        }

        // Perfil utilizado por la aplicación móvil: /api/ClienteApi/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPerfil(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID del cliente no es válido." });

            var cliente = await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado." });

            var viajesCompletados = await _context.SolicitudesServicio
                .CountAsync(s => s.ClienteId == id && s.Estado == "Finalizada");

            var calificacion = await _context.SolicitudesServicio
                .Where(s => s.ClienteId == id && s.CalificacionPasajero.HasValue)
                .AverageAsync(s => (double?)s.CalificacionPasajero) ?? 5.0;

            return Ok(new
            {
                clienteId = cliente.Id,
                nombreCompleto = cliente.NombreCompleto,
                correo = cliente.Correo,
                celular = cliente.Celular,
                viajes = viajesCompletados,
                calificacion,
                fotoPerfilUrl = cliente.FotoPerfilUrl
            });
        }

        [HttpPut("actualizar-foto")]
        public async Task<IActionResult> ActualizarFoto([FromBody] ClienteFotoDto dto)
        {
            if (dto == null || dto.ClienteId <= 0 || string.IsNullOrWhiteSpace(dto.FotoPerfilUrl))
                return BadRequest(new { mensaje = "ClienteId y FotoPerfilUrl son obligatorios." });

            var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado" });

            cliente.FotoPerfilUrl = dto.FotoPerfilUrl.Trim();
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Foto actualizada", clienteId = cliente.Id });
        }
    }
}
