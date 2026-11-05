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

        public EmailApiController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("enviar-token")]
        public async Task<IActionResult> EnviarToken([FromBody] TokenRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Token))
            {
                return BadRequest(new { error = "El correo y el token son obligatorios." });
            }

            try
            {
                var senderEmail = _config["Gmail:Correo"];
                var senderPassword = _config["Gmail:ClaveApp"];

                using var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                };

                // DIFERENCIAR EL CONTENIDO SEGÚN EL TIPO
                string asunto = "Código de Verificación - Viandrive";
                string mensajeEncabezado = "Tu código de verificación es:";
                string mensajeDetalle = "Estás a un paso de completar tu cuenta.";

                if (request.Tipo == "LoginPage")
                {
                    asunto = "Código de Ingreso - Viandrive";
                    mensajeEncabezado = "Tu código de acceso para iniciar sesión es:";
                    mensajeDetalle = "Alguien intentó iniciar sesión en tu cuenta de Viandrive.";
                }
                else if (request.Tipo == "RegistroConductorPage")
                {
                    asunto = "Código de Verificación - Registro de Conductor";
                    mensajeEncabezado = "Tu código para registrarte como conductor es:";
                    mensajeDetalle = "Completa tu registro para empezar a conducir.";
                }
                else if (request.Tipo == "MainPage")
                {
                    asunto = "Código de Verificación - Registro de Usuario";
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

                return Ok(new { mensaje = "Token enviado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "No se pudo enviar el correo", detalle = ex.Message });
            }
        }
    }
}