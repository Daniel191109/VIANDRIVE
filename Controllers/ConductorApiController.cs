using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VIANDRIVEFULL.Data;
using VIANDRIVEFULL.Models;
using VIANDRIVEFULL.Models.DTOs;

namespace VIANDRIVEFULL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConductorApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConductorApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerfilConductor(int id)
        {
            var conductor = await _context.Conductores.FindAsync(id);

            if (conductor == null)
            {
                return NotFound(new { error = "Conductor no encontrado." });
            }

            // Conteo real de viajes completados desde la base de datos
            int totalViajesCompletados = await _context.Viajes
                .CountAsync(v => v.ConductorId == id && v.Estado == "Completado");

            return Ok(new
            {
                nombre = conductor.Nombre,
                calificacion = conductor.Calificacion,
                viajesCompletados = conductor.ViajesCompletados,
                gananciasHoy = conductor.GananciasHoy,
                selfieBase64 = conductor.SelfieBase64
            });
        }

        [HttpPost("actualizar-estado")]
        public IActionResult ActualizarEstado([FromBody] EstadoConductorDto dto)
        {
            var conductor = _context.Conductores.Find(dto.ConductorId);
            if (conductor == null) return NotFound(new { error = "Conductor no encontrado." });

            conductor.Disponible = dto.Disponible;
            _context.SaveChanges();

            return Ok(new { mensaje = "Estado actualizado correctamente." });
        }

        [HttpPost("registrar")]
        public IActionResult RegistrarConductor([FromBody] ConductorRegistroDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var documentoExiste = _context.Conductores.Any(c => c.Documento == dto.Document);
            if (documentoExiste)
                return Conflict(new { error = "Ya existe un conductor registrado con este número de documento." });

            var emailExiste = _context.Usuarios.Any(u => u.Email == dto.Email) ||
                              _context.Conductores.Any(c => c.Email == dto.Email);
            if (emailExiste)
                return Conflict(new { error = "Ya existe una cuenta registrada con este correo electrónico." });

            var conductor = new Conductor
            {
                Nombre = dto.Name,
                Documento = dto.Document,
                Telefono = dto.Phone,
                Edad = dto.Age,
                Email = dto.Email,
                TipoLicencia = dto.LicenseType,
                NumeroLicencia = dto.LicenseNumber,
                Placa = dto.Plate,
                Color = dto.Color,
                Modelo = dto.Model,
                AnioVehiculo = dto.Year,
                PasswordHash = EncriptarSHA256(dto.Password),
                SelfieBase64 = dto.SelfieBase64,
                TarjetaPropiedadBase64 = dto.PropertyCardBase64,
                SoatBase64 = dto.SoatBase64,
                FotosVehiculoBase64 = dto.VehiclePhotosBase64,
                Disponible = false,
                Calificacion = 5.0m,
                GananciasHoy = 0.0m
            };

            _context.Conductores.Add(conductor);
            _context.SaveChanges();

            return Ok(new
            {
                mensaje = "Conductor registrado correctamente.",
                id = conductor.Id,
                documento = conductor.Documento
            });
        }

        [HttpPost("registrar-token")]
        public IActionResult RegistrarToken([FromBody] TokenRegistroDto dto)
        {
            if (dto.ConductorId <= 0 || string.IsNullOrWhiteSpace(dto.PushToken))
                return BadRequest(new { error = "ConductorId y PushToken son obligatorios." });

            var conductor = _context.Conductores.Find(dto.ConductorId);
            if (conductor == null)
                return NotFound(new { error = "Conductor no encontrado." });

            conductor.PushToken = dto.PushToken;
            _context.SaveChanges();

            return Ok(new { mensaje = "Token registrado correctamente." });
        }

        internal static string EncriptarSHA256(string textoPlano)
        {
            if (string.IsNullOrEmpty(textoPlano)) return string.Empty;

            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(textoPlano);
            var hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}