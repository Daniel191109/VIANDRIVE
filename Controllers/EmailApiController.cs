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
<<<<<<< HEAD
        private readonly ILogger<EmailApiController> _logger;

        public EmailApiController(IConfiguration config, ILogger<EmailApiController> logger)
        {
            _config = config;
            _logger = logger;
=======

        public EmailApiController(IConfiguration config)
        {
            _config = config;
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
        }

        [HttpPost("enviar-token")]
        public async Task<IActionResult> EnviarToken([FromBody] TokenRequest request)
        {
<<<<<<< HEAD
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token))
                return BadRequest(new { error = "El correo y el token son obligatorios." });

            var senderEmail = _config["Gmail:Correo"];
            var senderPassword = _config["Gmail:ClaveApp"];
            if (string.IsNullOrWhiteSpace(senderEmail) || string.IsNullOrWhiteSpace(senderPassword))
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { error = "El servicio de correo no está configurado en el servidor." });

            try
            {
=======
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Token))
            {
                return BadRequest(new { error = "El correo y el token son obligatorios." });
            }

            try
            {
                var senderEmail = _config["Gmail:Correo"];
                var senderPassword = _config["Gmail:ClaveApp"];

>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
                using var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                };

<<<<<<< HEAD
                var asunto = "Código de Verificación - Viandrive";
                var encabezado = "Tu código de verificación es:";
                var detalle = "Estás a un paso de completar tu cuenta.";
=======
                // DIFERENCIAR EL CONTENIDO SEGÚN EL TIPO
                string asunto = "Código de Verificación - Viandrive";
                string mensajeEncabezado = "Tu código de verificación es:";
                string mensajeDetalle = "Estás a un paso de completar tu cuenta.";
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72

                if (request.Tipo == "LoginPage")
                {
                    asunto = "Código de Ingreso - Viandrive";
<<<<<<< HEAD
                    encabezado = "Tu código de acceso para iniciar sesión es:";
                    detalle = "Alguien intentó iniciar sesión en tu cuenta de Viandrive.";
=======
                    mensajeEncabezado = "Tu código de acceso para iniciar sesión es:";
                    mensajeDetalle = "Alguien intentó iniciar sesión en tu cuenta de Viandrive.";
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
                }
                else if (request.Tipo == "RegistroConductorPage")
                {
                    asunto = "Código de Verificación - Registro de Conductor";
<<<<<<< HEAD
                    encabezado = "Tu código para registrarte como conductor es:";
                    detalle = "Completa tu registro para empezar a conducir.";
=======
                    mensajeEncabezado = "Tu código para registrarte como conductor es:";
                    mensajeDetalle = "Completa tu registro para empezar a conducir.";
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
                }
                else if (request.Tipo == "MainPage")
                {
                    asunto = "Código de Verificación - Registro de Usuario";
<<<<<<< HEAD
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
=======
                    mensajeEncabezado = "Tu código para registrarte como cliente es:";
                    mensajeDetalle = "Completa tu registro para empezar a viajar con Viandrive.";
                }

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, "Viandrive Seguridad"),
                    Subject = asunto,
                    Body = $"<div style='font-family: Arial; text-align: center; padding: 25px; background-color: #0B0F14; color: #F5F7FA; border-radius: 12px;'>" +
                           $"<h2 style='color: #FFC107; margin-bottom: 5px;'>Viandrive Seguridad</h2>" +
                           $"<p style='color: #8793A1; font-size: 14px; margin-top: 0;'>{mensajeDetalle}</p>" +
                           $"<p style='font-size: 16px; margin-top: 20px;'>{mensajeEncabezado}</p>" +
                           $"<h1 style='color: #16A34A; letter-spacing: 6px; font-size: 38px; background: #151B23; padding: 10px; border-radius: 8px; display: inline-block;'>{request.Token}</h1>" +
                           $"<p style='color: #8793A1; font-size: 12px; margin-top: 20px;'>Si no solicitaste este código, ignora este mensaje de forma segura.</p>" +
                           $"</div>",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(request.Email);
                await smtpClient.SendMailAsync(mailMessage);

>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
                return Ok(new { mensaje = "Token enviado correctamente" });
            }
            catch (Exception ex)
            {
<<<<<<< HEAD
                _logger.LogError(ex, "No se pudo enviar el correo de verificación.");
                return StatusCode(500, new { error = "No se pudo enviar el correo." });
            }
        }
    }
}
=======
                return StatusCode(500, new { error = "No se pudo enviar el correo", detalle = ex.Message });
            }
        }
    }
}
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
