using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VIANDRIVEFULL.Data;
using VIANDRIVEFULL.Models.DTOs;

namespace VIANDRIVEFULL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login-conductor")]
        public async Task<IActionResult> LoginConductor([FromBody] LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { error = "Documento y contraseña son obligatorios." });

            var hash = EncriptarSHA256(dto.Password);
            var documentoLimpio = dto.Email?.Trim();

            Console.WriteLine($"--- INTENTO DE LOGIN ---");
            Console.WriteLine($"Documento recibido de MAUI: '{documentoLimpio}'");
            Console.WriteLine($"Hash generado: '{hash}'");

            // Buscamos al conductor por Documento
            var conductor = await _context.Conductores
                .FirstOrDefaultAsync(c => c.Documento == documentoLimpio);

            if (conductor == null)
            {
                Console.WriteLine("❌ El conductor NO existe en la base de datos.");
                return Unauthorized(new { error = "Documento o contraseña incorrectos." });
            }

            Console.WriteLine($"✅ Conductor encontrado: {conductor.Nombre}. Hash en BD: '{conductor.PasswordHash}'");

            if (conductor.PasswordHash != hash)
            {
                Console.WriteLine("❌ EL HASH DE LA CONTRASEÑA NO COINCIDE.");
                return Unauthorized(new { error = "Documento o contraseña incorrectos." });
            }

            // Retornamos el objeto anónimo formateado para la deserialización en MAUI
            return Ok(new
            {
                Id = conductor.Id,
                Nombre = conductor.Nombre,
                Email = conductor.Email,
                Documento = conductor.Documento,
                Placa = conductor.Placa,
                Modelo = conductor.Modelo,
                Color = conductor.Color,
                Anio = conductor.AnioVehiculo
            });
        }

        private static string EncriptarSHA256(string textoPlano)
        {
            if (string.IsNullOrEmpty(textoPlano)) return string.Empty;

            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(textoPlano);
            var hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}