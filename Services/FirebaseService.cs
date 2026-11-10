<<<<<<< HEAD
using FirebaseAdmin;
=======
﻿using FirebaseAdmin;
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace VIANDRIVEFULL.Services
{
    public class FirebaseService
    {
        private static bool _initialized = false;
<<<<<<< HEAD
        private static readonly object InitializationLock = new();
        private readonly bool _firebaseAvailable;
=======
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
        private readonly ILogger<FirebaseService> _logger;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FirebaseService(ILogger<FirebaseService> logger, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _config = config;
            _httpContextAccessor = httpContextAccessor;

<<<<<<< HEAD
            var configuredPath = _config["Firebase:CredentialsPath"];
            var path = string.IsNullOrWhiteSpace(configuredPath)
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Services", "firebase-key.json")
                : configuredPath;

            lock (InitializationLock)
            {
                if (!_initialized && File.Exists(path))
                {
                    try
                    {
                        var credential = GoogleCredential.FromFile(path);
                        FirebaseApp.Create(new AppOptions { Credential = credential });
                        _initialized = true;
                        _logger.LogInformation("FirebaseApp inicializado correctamente.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "No se pudo inicializar Firebase. Las notificaciones push quedarán deshabilitadas.");
                    }
                }
                else if (!_initialized)
                {
                    _logger.LogWarning("Firebase no está configurado. Las notificaciones push quedarán deshabilitadas; el resto de la API seguirá funcionando.");
                }
            }
            _firebaseAvailable = _initialized;
=======
            if (!_initialized)
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Services", "firebase-key.json");

                if (!File.Exists(path))
                {
                    _logger.LogError($"Archivo firebase-key.json no encontrado en: {path}");
                    throw new FileNotFoundException("No se encontró el archivo firebase-key.json en: " + path);
                }

                var credential = GoogleCredential.FromFile(path);
                FirebaseApp.Create(new AppOptions { Credential = credential });

                _initialized = true;
                _logger.LogInformation("FirebaseApp inicializado correctamente.");
            }
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
        }

        public async Task<string> EnviarNotificacionAsync(string token, string titulo, string cuerpo, int conductorId, string placa)
        {
            string resultado;

<<<<<<< HEAD
            if (!_firebaseAvailable)
                return "Firebase no está configurado en el servidor.";

            if (string.IsNullOrWhiteSpace(token) || token.Length < 100 || !token.StartsWith("e"))
            {
                resultado = "Token inválido. Notificación no enviada.";
                _logger.LogWarning("Token push inválido. Notificación omitida.");
=======
            if (string.IsNullOrWhiteSpace(token) || token.Length < 100 || !token.StartsWith("e"))
            {
                resultado = "Token inválido. Notificación no enviada.";
                _logger.LogWarning($"Token inválido. Notificación omitida. Token recibido: '{token}'");
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
                await RegistrarLogNotificacionAsync(conductorId, placa, token, resultado);
                return resultado;
            }

            var mensaje = new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = titulo,
                    Body = cuerpo
                }
            };

            try
            {
                var respuesta = await FirebaseMessaging.DefaultInstance.SendAsync(mensaje);
                resultado = $"Notificación enviada correctamente: {respuesta}";
                _logger.LogInformation(resultado);
                await RegistrarLogNotificacionAsync(conductorId, placa, token, resultado);
                return respuesta;
            }
            catch (FirebaseMessagingException ex)
            {
                resultado = $"Error al enviar notificación: {ex.Message}";
<<<<<<< HEAD
                _logger.LogError(ex, "Error al enviar notificación Firebase.");
=======
                _logger.LogError($"Error al enviar notificación Firebase. Token: '{token}'. Error: {ex.Message}");
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
                await RegistrarLogNotificacionAsync(conductorId, placa, token, resultado);
                return resultado;
            }
            catch (Exception ex)
            {
                resultado = $"Error inesperado: {ex.Message}";
                _logger.LogError($"Error inesperado en FirebaseService: {ex}");
                await RegistrarLogNotificacionAsync(conductorId, placa, token, resultado);
                return resultado;
            }
        }

        private async Task RegistrarLogNotificacionAsync(int conductorId, string placa, string token, string resultado)
        {
            string ip = GetIpAddress();

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                var query = @"INSERT INTO LogNotificaciones (ConductorId, Placa, Token, Resultado, IP, FechaEnvio)
                              VALUES (@ConductorId, @Placa, @Token, @Resultado, @IP, GETDATE())";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ConductorId", conductorId);
                    command.Parameters.AddWithValue("@Placa", placa ?? "");
                    command.Parameters.AddWithValue("@Token", token ?? "");
                    command.Parameters.AddWithValue("@Resultado", resultado ?? "");
                    command.Parameters.AddWithValue("@IP", ip ?? "Sin IP");

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private string GetIpAddress()
        {
            return _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Sin IP";
        }
    }
}
