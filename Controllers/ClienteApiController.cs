using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using VIANDRIVEFULL.Data;
using VIANDRIVEFULL.Data.Repositories;
using VIANDRIVEFULL.Helpers;
using VIANDRIVEFULL.Models;
using VIANDRIVEFULL.Models.DTOs;
using VIANDRIVEFULL.Services;




namespace VIANDRIVEFULL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] ClienteRegistroDto dto)
        {
            if (!ModelState.IsValid)
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
                FechaRegistro = DateTime.Now,
                Clave = SeguridadHelper.EncriptarClave(dto.Clave)
            };

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

            return Ok(new
            {
                mensaje = "Inicio de sesión exitoso",
                clienteId = cliente.Id,
                nombre = cliente.NombreCompleto,
                token
            });
        }
        


        [HttpPut("actualizar-foto")]
        public async Task<IActionResult> ActualizarFoto([FromBody] ClienteFotoDto dto)
        {
            var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado" });

            cliente.FotoPerfilUrl = dto.FotoPerfilUrl;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Foto actualizada", clienteId = cliente.Id });
        }
    }
}
