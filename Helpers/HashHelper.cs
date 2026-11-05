namespace VIANDRIVEFULL.Helpers
{
    public static class HashHelper
    {
        public static string Generar(string passwordPlano)
        {
            return BCrypt.Net.BCrypt.HashPassword(passwordPlano);
        }

        public static bool Validar(string passwordPlano, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(passwordPlano, hash);
        }
    }
}

