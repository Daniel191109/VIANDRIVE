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

        public string GenerarToken(string usuarioId, string rol, string? nombre = null, string? correo = null)
        {
            var claveRaw = _config["Jwt:Key"]?.Trim();
            if (string.IsNullOrWhiteSpace(claveRaw) || Encoding.UTF8.GetByteCount(claveRaw) < 32)
                throw new InvalidOperationException("Jwt:Key debe configurarse con al menos 32 bytes mediante una variable segura de entorno.");

            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("Jwt:Issuer y Jwt:Audience deben estar configurados.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId),
                new Claim(ClaimTypes.Role, rol)
            };

            if (rol.Equals("cliente", StringComparison.OrdinalIgnoreCase))
                claims.Add(new Claim("clienteId", usuarioId));
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                claims.Add(new Claim(ClaimTypes.Name, nombre));
                claims.Add(new Claim("nombre", nombre));
            }
            if (!string.IsNullOrWhiteSpace(correo))
                claims.Add(new Claim("correo", correo));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveRaw));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
