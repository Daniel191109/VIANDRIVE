using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace VIANDRIVEFULL.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerarToken(string usuarioId, string rol)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId),
                new Claim(ClaimTypes.Role, rol)
            };

            // 🔐 Validación de clave JWT
            var claveRaw = _config["Jwt:Key"]?.Trim();

            if (string.IsNullOrEmpty(claveRaw) || claveRaw.Length < 16)
                throw new Exception("La clave JWT debe tener al menos 16 caracteres.");

            Console.WriteLine("Clave JWT: " + claveRaw);
            Console.WriteLine("Longitud: " + claveRaw.Length);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveRaw));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
