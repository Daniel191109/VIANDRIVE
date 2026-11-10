<<<<<<< HEAD
using Microsoft.Extensions.Configuration;
using VIANDRIVEFULL.Services;

namespace VIANDRIVEFULL.Helpers
{
    // Compatibilidad para código antiguo: ya no mantiene una clave JWT hardcodeada.
    public static class TokenHelper
    {
        public static string GenerarToken(int clienteId, string nombre, string correo, IConfiguration configuration)
        {
            return new JwtService(configuration).GenerarToken(
                clienteId.ToString(), "cliente", nombre, correo);
=======
﻿using System;
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
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
        }
    }
}
