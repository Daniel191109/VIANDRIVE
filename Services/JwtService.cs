<<<<<<< HEAD
using System.IdentityModel.Tokens.Jwt;
=======
﻿using System.IdentityModel.Tokens.Jwt;
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
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

<<<<<<< HEAD
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
=======
        public string GenerarToken(string usuarioId, string rol)
        {
            var claims = new[]
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId),
                new Claim(ClaimTypes.Role, rol)
            };

<<<<<<< HEAD
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
=======
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
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
