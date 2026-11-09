using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

public class CorreoService : ICorreoService
{
    private const string ApiKey = "D4F9BEE006CCE16C3E8547962B2A525A1D7D42DCF6B9887EFE28D939C1EE86C1ED6319F685BBF7E32312534B3C7E250D";
    private const string Remitente = "viandrive25@gmail.com";

    public async Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpo)
    {
        // Usar un handler dedicado para asegurar la compatibilidad SSL en Android y Windows
        var handler = new HttpClientHandler();

        using var client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(30) // Evitar cuelgues si la red móvil es lenta
        };

        var payload = $"apikey={HttpUtility.UrlEncode(ApiKey)}" +
                      $"&from={HttpUtility.UrlEncode(Remitente)}" +
                      $"&to={HttpUtility.UrlEncode(destinatario)}" +
                      $"&subject={HttpUtility.UrlEncode(asunto)}" +
                      $"&bodyText={HttpUtility.UrlEncode(cuerpo)}";

        var content = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded");

        try
        {
            var response = await client.PostAsync("https://api.elasticemail.com/v2/email/send", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            // Opcional: Registrar el error en consola para diagnóstico
            System.Diagnostics.Debug.WriteLine($"Error al enviar correo: {ex.Message}");
            return false;
        }
    }
}