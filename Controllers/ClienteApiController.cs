<<<<<<< HEAD
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VIANDRIVEFULL.Data;
=======
﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using VIANDRIVEFULL.Data;
using VIANDRIVEFULL.Data.Repositories;
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
using VIANDRIVEFULL.Helpers;
using VIANDRIVEFULL.Models;
using VIANDRIVEFULL.Models.DTOs;
using VIANDRIVEFULL.Services;

<<<<<<< HEAD
=======



>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
namespace VIANDRIVEFULL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
<<<<<<< HEAD
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
=======
    public class ClienteApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly INotificacionService _notificacion;
        private readonly ICorreoService _correoService;

        public ClienteApiController(AppDbContext context, INotificacionService notificacion, ICorreoService correoService)
        {
            _context = context;
            _notificacion = notificacion;
            _correoService = correoService;
        }

>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] ClienteRegistroDto dto)
        {
            if (!ModelState.IsValid)
<<<<<<< HEAD
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
=======
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.Clave))
                return BadRequest("La contraseña es obligatoria.");

            if (!SeguridadHelper.ValidarClave(dto.Clave))
                return BadRequest("La contraseña debe tener mínimo 8 caracteres, incluir letras, números y un carácter especial.");

            var cliente = new Cliente
            {
                NombreCompleto = dto.NombreCompleto,
                Correo = dto.Correo,
                Celular = dto.Celular,
                MetodoPagoPreferido = dto.MetodoPagoPreferido,
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
                FechaRegistro = DateTime.Now,
                Clave = SeguridadHelper.EncriptarClave(dto.Clave)
            };

<<<<<<< HEAD
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
=======
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            // Envío de confirmación por correo
            var mensaje = $"Hola {dto.NombreCompleto}, tu registro fue exitoso en Viandrive.";
            bool exito = await _correoService.EnviarCorreoAsync(dto.Correo, "Registro exitoso", mensaje);

            // Registro en NotificacionLog
            _context.NotificacionLog.Add(new NotificacionLog
            {
                Tipo = "Correo",
                Destinatario = dto.Correo,
                FechaEnvio = DateTime.Now,
                Estado = exito ? "Enviado" : "Error",
                Mensaje = mensaje
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Cliente registrado y confirmación enviada por correo",
                clienteId = cliente.Id,
                confirmacion = exito ? "Enviada" : "Fallida"
            });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ClienteLoginDto dto)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Correo == dto.Correo);

            if (cliente == null || !HashHelper.Validar(dto.Clave, cliente.Clave))
            {
                return Unauthorized(new { mensaje = "Credenciales inválidas" });
            }

            var token = TokenHelper.GenerarToken(cliente.Id, cliente.NombreCompleto, cliente.Correo);
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72

            return Ok(new
            {
                mensaje = "Inicio de sesión exitoso",
                clienteId = cliente.Id,
                nombre = cliente.NombreCompleto,
                token
            });
        }
<<<<<<< HEAD

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
=======
        

>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72

        [HttpPut("actualizar-foto")]
        public async Task<IActionResult> ActualizarFoto([FromBody] ClienteFotoDto dto)
        {
<<<<<<< HEAD
            if (dto == null || dto.ClienteId <= 0 || string.IsNullOrWhiteSpace(dto.FotoPerfilUrl))
                return BadRequest(new { mensaje = "ClienteId y FotoPerfilUrl son obligatorios." });

=======
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
            var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado" });

<<<<<<< HEAD
            cliente.FotoPerfilUrl = dto.FotoPerfilUrl.Trim();
=======
            cliente.FotoPerfilUrl = dto.FotoPerfilUrl;
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Foto actualizada", clienteId = cliente.Id });
        }
    }
}
