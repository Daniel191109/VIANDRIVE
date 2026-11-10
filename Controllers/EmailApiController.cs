using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using VIANDRIVEFULL.Models;

namespace VIANDRIVEFULL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailApiController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailApiController> _logger;

        public EmailApiController(IConfiguration config, ILogger<EmailApiController> logger)
        {
            _config = config;
            _logger = logger;
        }

        [HttpPost("enviar-token")]
        public async Task<IActionResult> EnviarToken([FromBody] TokenRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token))
                return BadRequest(new { error = "El correo y el token son obligatorios." });

            var senderEmail = _config["Gmail:Correo"];
            var senderPassword = _config["Gmail:ClaveApp"];
            if (string.IsNullOrWhiteSpace(senderEmail) || string.IsNullOrWhiteSpace(senderPassword))
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { error = "El servicio de correo no está configurado en el servidor." });

            try
            {
                using var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                };

                var asunto = "Código de Verificación - Viandrive";
                var encabezado = "Tu código de verificación es:";
                var detalle = "Estás a un paso de completar tu cuenta.";

                if (request.Tipo == "LoginPage")
                {
                    asunto = "Código de Ingreso - Viandrive";
                    encabezado = "Tu código de acceso para iniciar sesión es:";
                    detalle = "Alguien intentó iniciar sesión en tu cuenta de Viandrive.";
                }
                else if (request.Tipo == "RegistroConductorPage")
                {
                    asunto = "Código de Verificación - Registro de Conductor";
                    encabezado = "Tu código para registrarte como conductor es:";
                    detalle = "Completa tu registro para empezar a conducir.";
                }
                else if (request.Tipo == "MainPage")
                {
                    asunto = "Código de Verificación - Registro de Usuario";
                    encabezado = "Tu código para registrarte como cliente es:";
                    detalle = "Completa tu registro para empezar a viajar con Viandrive.";
                }

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, "Viandrive Seguridad"),
                    Subject = asunto,
                    Body = $"<div style='font-family:Arial;text-align:center;padding:25px'><h2>Viandrive Seguridad</h2><p>{detalle}</p><p>{encabezado}</p><h1>{System.Net.WebUtility.HtmlEncode(request.Token)}</h1></div>",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(request.Email.Trim());
                await smtpClient.SendMailAsync(mailMessage);
                return Ok(new { mensaje = "Token enviado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo enviar el correo de verificación.");
                return StatusCode(500, new { error = "No se pudo enviar el correo." });
            }
        }
    }
}
