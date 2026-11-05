using Microsoft.AspNetCore.Mvc;
using VIANDRIVEFULL.Helpers;

namespace VIANDRIVEFULL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioTestController : ControllerBase
    {
        [HttpPost("validar")]
        public IActionResult ValidarUsuario([FromBody] CredencialesDto dto)
        {
            // Simulación: hash previamente generado (como si viniera de la BD)
            string hashSimulado = HashHelper.Generar("clave123");

            bool esValido = HashHelper.Validar(dto.Clave, hashSimulado);

            return Ok(new
            {
                Usuario = dto.Usuario,
                ClaveIngresada = dto.Clave,
                HashSimulado = hashSimulado,
                ClaveCorrecta = esValido
            });
        }
    }

    public class CredencialesDto
    {
        public string Usuario { get; set; }
        public string Clave { get; set; }
    }
}
