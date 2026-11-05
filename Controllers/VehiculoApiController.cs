using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VIANDRIVEFULL.Data;
using VIANDRIVEFULL.Models;
using VIANDRIVEFULL.Services; // si FirebaseService está en carpeta Services

namespace VIANDRIVEFULL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiculoApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly FirebaseService _firebase;

        public VehiculoApiController(AppDbContext context, FirebaseService firebase)
        {
            _context = context;
            _firebase = firebase;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarVehiculo([FromBody] VehiculoRegistroDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var conductor = _context.Conductores.Find(dto.ConductorId);
            if (conductor == null)
                return NotFound(new { error = "Conductor no encontrado." });

            var placaExiste = _context.Vehiculos.Any(v => v.Placa == dto.Placa);
            if (placaExiste)
                return Conflict(new { error = "Ya existe un vehículo con esa placa." });

            var vehiculo = new Vehiculo
            {
                Placa = dto.Placa,
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                Color = dto.Color,
                Año = dto.Año,
                ConductorId = dto.ConductorId
            };

            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();

            // Notificación push
            if (!string.IsNullOrWhiteSpace(conductor.PushToken))
            {
                var titulo = "Vehículo registrado";
                var cuerpo = $"Se ha registrado el vehículo {vehiculo.Placa} a tu nombre.";
                await _firebase.EnviarNotificacionAsync(conductor.PushToken, titulo, cuerpo, conductor.Id, vehiculo.Placa);
            }

            return Ok(new
            {
                mensaje = "Vehículo registrado correctamente.",
                vehiculo = new
                {
                    vehiculo.Id,
                    vehiculo.Placa,
                    vehiculo.Marca,
                    vehiculo.Modelo,
                    vehiculo.Color,
                    vehiculo.Año,
                    vehiculo.ConductorId
                }
            });
        }
    }
}


