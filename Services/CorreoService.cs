using System.Net.Http;
using System.Text;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class CorreoService : ICorreoService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CorreoService> _logger;

    public CorreoService(IConfiguration configuration, ILogger<CorreoService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpo)
    {
        var apiKey = _configuration["ElasticEmail:ApiKey"];
        var remitente = _configuration["ElasticEmail:From"];
        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(remitente))
        {
            _logger.LogWarning("Correo no enviado: faltan ElasticEmail:ApiKey o ElasticEmail:From.");
            return false;
        }

        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        var payload = $"apikey={HttpUtility.UrlEncode(apiKey)}" +
                      $"&from={HttpUtility.UrlEncode(remitente)}" +
                      $"&to={HttpUtility.UrlEncode(destinatario)}" +
                      $"&subject={HttpUtility.UrlEncode(asunto)}" +
                      $"&bodyText={HttpUtility.UrlEncode(cuerpo)}";
        using var content = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded");

        try
        {
            using var response = await client.PostAsync("https://api.elasticemail.com/v2/email/send", content);
            if (!response.IsSuccessStatusCode)
                _logger.LogWarning("Proveedor de correo respondió con código HTTP {StatusCode}.", (int)response.StatusCode);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar correo transaccional.");
            return false;
        }
    }
}
