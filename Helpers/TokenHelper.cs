using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace VIANDRIVEFULL.Helpers
{
    public static class TokenHelper
    {
        private const string ClaveSecreta = "ViandriveClaveJWT2025SuperSegura"; // Mínimo 16 caracteres

        public static string GenerarToken(int clienteId, string nombre, string correo)
        {
            var claims = new[]
            {
                new Claim("clienteId", clienteId.ToString()),
                new Claim("nombre", nombre),
                new Claim("correo", correo)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ClaveSecreta));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "Viandrive",
                audience: "ClientesViandrive",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
