using System.Security.Cryptography;
using System.Text;

namespace VIANDRIVEFULL.Helpers
{
    public static class SeguridadHelper
    {
        /// <summary>
        /// Valida que la clave tenga mínimo 8 caracteres, al menos una letra, un número y un carácter especial.
        /// </summary>
        public static bool ValidarClave(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave)) return false;

            var tieneLetra = clave.Any(char.IsLetter);
            var tieneNumero = clave.Any(char.IsDigit);
            var tieneEspecial = clave.Any(c => !char.IsLetterOrDigit(c));
            var longitudMinima = clave.Length >= 8;

            return tieneLetra && tieneNumero && tieneEspecial && longitudMinima;
        }

        /// <summary>
        /// Encripta la clave usando BCrypt con sal aleatoria y la devuelve como hash.
        /// </summary>

        public static string EncriptarClave(string clave)
        {
            return HashHelper.Generar(clave);
        }

    }
}
