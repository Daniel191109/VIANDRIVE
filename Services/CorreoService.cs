<<<<<<< HEAD
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
=======
﻿using System.Net.Http;
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
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
<<<<<<< HEAD
            _logger.LogError(ex, "Error al enviar correo transaccional.");
            return false;
        }
    }
}
=======
            // Opcional: Registrar el error en consola para diagnóstico
            System.Diagnostics.Debug.WriteLine($"Error al enviar correo: {ex.Message}");
            return false;
        }
    }
}
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
