using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VIANDRIVEFULL.Data;
using VIANDRIVEFULL.Models;
using VIANDRIVEFULL.Models.DTOs;

namespace VIANDRIVEFULL.Services
{
    public class SolicitudServicioService
    {
        private readonly AppDbContext _context;
        private readonly FirebaseService _firebase;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SolicitudServicioService(AppDbContext context, FirebaseService firebase, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _firebase = firebase;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<SolicitudServicio> CrearSolicitudAsync(SolicitudServicioDto dto, string usuario)
        {
            var precioSugerido = (dto.DistanciaKm > 0 && dto.TiempoEstimadoMin > 0)
                ? CalcularPrecioSugerido((decimal)dto.DistanciaKm, dto.TiempoEstimadoMin, dto.HoraPico)
                : (dto.OfertaPasajero > 0 ? dto.OfertaPasajero : 8000);

            var solicitud = new SolicitudServicio
            {
                ClienteId = dto.ClienteId > 0 ? dto.ClienteId : 1,
                ConductorId = dto.ConductorId,
                VehiculoId = dto.VehiculoId,
                Origen = string.IsNullOrWhiteSpace(dto.Origen) ? "Origen no especificado" : dto.Origen,
                Destino = string.IsNullOrWhiteSpace(dto.Destino) ? "Destino no especificado" : dto.Destino,

                LatOrigen = dto.LatOrigen,
                LngOrigen = dto.LngOrigen,
                LatDestino = dto.LatDestino,
                LngDestino = dto.LngDestino,

                FechaSolicitud = DateTime.Now,
                Estado = "Pendiente",
                PrecioSugerido = precioSugerido,
                OfertaPasajero = dto.OfertaPasajero > 0 ? dto.OfertaPasajero : precioSugerido,
                PrecioFinal = dto.OfertaPasajero > 0 ? dto.OfertaPasajero : precioSugerido,

                MedioPago = string.IsNullOrWhiteSpace(dto.MedioPago) ? "Efectivo" : dto.MedioPago,
                PagoConfirmado = false,
                OfertaAceptada = false,
                OfertaRechazada = false
            };

            _context.SolicitudesServicio.Add(solicitud);
            await _context.SaveChangesAsync();

            await RegistrarLogNegociacionAsync(solicitud.Id, "OfertaPasajero", solicitud.OfertaPasajero, usuario);

            try
            {
                if (dto.ConductorId > 0 && dto.VehiculoId > 0)
                {
                    var conductor = await _context.Conductores.FindAsync(dto.ConductorId);
                    var vehiculo = await _context.Vehiculos.FindAsync(dto.VehiculoId);

                    if (conductor?.PushToken != null && vehiculo?.Placa != null)
                    {
                        var titulo = "Nueva solicitud de servicio";
                        var cuerpo = $"Origen: {solicitud.Origen}\nDestino: {solicitud.Destino}\nOferta: ${solicitud.OfertaPasajero:N0}";

                        await _firebase.EnviarNotificacionAsync(
                            conductor.PushToken,
                            titulo,
                            cuerpo,
                            conductor.Id,
                            vehiculo.Placa
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Notificación omitida]: {ex.Message}");
            }

            return solicitud;
        }

        private decimal CalcularPrecioSugerido(decimal distanciaKm, int tiempoMinutos, bool horaPico)
        {
            decimal baseMinima = 5000;
            decimal tarifaKm = 100;
            decimal tarifaTiempo = 10;

            decimal precio = baseMinima + (distanciaKm * tarifaKm) + (tiempoMinutos * tarifaTiempo);

            if (horaPico)
                precio *= 1.2m;

            return Math.Round(precio, 0);
        }

        private async Task RegistrarLogNegociacionAsync(int solicitudId, string tipo, object valor, string usuario)
        {
            var ip = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Sin IP";

            if (decimal.TryParse(valor?.ToString(), out var valorDecimal))
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                INSERT INTO LogNegociacion (SolicitudId, Tipo, Valor, Usuario, IP)
                VALUES ({0}, {1}, {2}, {3}, {4})",
                    solicitudId, tipo, valorDecimal, usuario, ip);
            }
            else
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                INSERT INTO LogNegociacion (SolicitudId, Tipo, Valor, Detalle, Usuario, IP)
                VALUES ({0}, {1}, NULL, {2}, {3}, {4})",
                    solicitudId, tipo, valor?.ToString(), usuario, ip);
            }
        }

        public Task<(bool Exito, string Mensaje, SolicitudServicio Solicitud)> AceptarSolicitudAsync(int id)
        {
            return AceptarSolicitudAsync(id, 0, 0, 0);
        }

        public async Task<(bool Exito, string Mensaje, SolicitudServicio Solicitud)> AceptarSolicitudAsync(int id, int conductorIdParam, double latConductor = 0, double lngConductor = 0)
        {
            var solicitud = await _context.SolicitudesServicio.FindAsync(id);

            if (solicitud == null)
                return (false, "Solicitud no encontrada.", null);

            if (solicitud.Estado == "Cancelada")
                return (false, "No se puede aceptar una solicitud cancelada.", solicitud);

            var estadosValidos = new[] { "Pendiente", "Negociando", "ContraofertaPendiente" };

            if (!estadosValidos.Contains(solicitud.Estado))
                return (false, $"Estado actual no permite aceptación: {solicitud.Estado}", solicitud);

            int conductorIdFinal = conductorIdParam;

            if (conductorIdFinal <= 0)
            {
                var usuario = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Sistema";
                conductorIdFinal = await _context.Usuarios
                    .Where(u => u.Nombre == usuario)
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync();
            }

            var vehiculoId = await _context.Vehiculos
                .Where(v => v.ConductorId == conductorIdFinal)
                .Select(v => v.Id)
                .FirstOrDefaultAsync();

            solicitud.ConductorId = conductorIdFinal;
            solicitud.VehiculoId = vehiculoId;
            solicitud.Estado = "Aceptada";
            solicitud.OfertaAceptada = true;

            if (latConductor != 0) solicitud.LatConductor = latConductor;
            if (lngConductor != 0) solicitud.LngConductor = lngConductor;

            await _context.SaveChangesAsync();

            return (true, "Solicitud actualizada a 'Aceptada'.", solicitud);
        }

        public async Task<(bool Exito, string Mensaje, SolicitudServicio Solicitud)> CancelarSolicitudAsync(int id, string motivo, string usuario)
        {
            var solicitud = await _context.SolicitudesServicio.FindAsync(id);
            if (solicitud == null)
                return (false, "Solicitud no encontrada.", null);

            if (solicitud.Estado == "Cancelada")
                return (false, "La solicitud ya fue cancelada anteriormente.", solicitud);

            if (solicitud.Estado == "Finalizada")
                return (false, "No se puede cancelar una solicitud finalizada.", solicitud);

            solicitud.Estado = "Cancelada";
            solicitud.FechaCancelacion = DateTime.Now;
            solicitud.MotivoCancelacion = motivo;
            solicitud.UsuarioCancela = usuario;
            await _context.SaveChangesAsync();

            await RegistrarLogNegociacionAsync(solicitud.Id, "SolicitudCancelada", motivo, usuario);

            return (true, "Solicitud cancelada correctamente.", solicitud);
        }

        public async Task<(bool Exito, string Mensaje, SolicitudServicio Solicitud)> FinalizarSolicitudAsync(int id, string usuario)
        {
            var solicitud = await _context.SolicitudesServicio.FindAsync(id);

            if (solicitud == null)
                return (false, "Solicitud no encontrada.", null);

            if (solicitud.Estado == "Cancelada")
                return (false, "No se puede finalizar una solicitud cancelada.", solicitud);

            // 🟢 ESTADOS VÁLIDOS AMPLIADOS: Permite finalizar desde "Aceptada", "EnCamino" o "EnCurso"
            var estadosValidos = new[] { "Aceptada", "EnCamino", "EnCurso" };

            if (!estadosValidos.Contains(solicitud.Estado))
                return (false, $"No se puede finalizar desde el estado actual: {solicitud.Estado}", solicitud);

            var estadoAnterior = solicitud.Estado;
            solicitud.Estado = "Finalizada";
            solicitud.FechaFinalizacion = DateTime.Now;
            await _context.SaveChangesAsync();

            await RegistrarLogCambioAsync(solicitud.Id, estadoAnterior, "Finalizada", usuario);

            return (true, "Solicitud finalizada correctamente.", solicitud);
        }

        public async Task<(bool Exito, string Mensaje, SolicitudServicio Solicitud)> FinalizarSolicitudConCalificacionAsync(int solicitudId, FinalizarServicioDto dto, string usuario)
        {
            var resultadoFinalizar = await FinalizarSolicitudAsync(solicitudId, usuario);
            if (!resultadoFinalizar.Exito)
                return resultadoFinalizar;

            var solicitud = resultadoFinalizar.Solicitud;

            if (dto.TarifaFinal > 0)
            {
                solicitud.PrecioFinal = dto.TarifaFinal;
                await _context.SaveChangesAsync();
            }

            if (dto.CalificacionPasajero > 0)
            {
                solicitud.CalificacionPasajero = dto.CalificacionPasajero;
                await _context.SaveChangesAsync();

                var calificacionDto = new CalificacionClienteDTO
                {
                    Calificacion = dto.CalificacionPasajero,
                    Comentario = "Calificado automáticamente al finalizar el viaje"
                };

                await CalificarClienteAsync(solicitudId, calificacionDto, usuario);
            }

            if (dto.ConductorId > 0)
            {
                var conductor = await _context.Conductores.FindAsync(dto.ConductorId);
                if (conductor != null)
                {
                    conductor.ViajesCompletados += 1;
                    await _context.SaveChangesAsync();
                }
            }

            return (true, "Solicitud finalizada y calificada correctamente.", solicitud);
        }

        private async Task RegistrarLogCambioAsync(int solicitudId, string estadoAnterior, string estadoNuevo, string usuario)
        {
            var ip = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Sin IP";

            await _context.Database.ExecuteSqlRawAsync(@"
                INSERT INTO LogCambiosSolicitud (SolicitudId, EstadoAnterior, EstadoNuevo, Usuario, IP)
                VALUES ({0}, {1}, {2}, {3}, {4})",
                solicitudId, estadoAnterior, estadoNuevo, usuario ?? "Sistema", ip);
        }

        public async Task<(bool Exito, string Mensaje, SolicitudServicio Solicitud)> RechazarOfertaAsync(int id, string usuario)
        {
            var solicitud = await _context.SolicitudesServicio.FindAsync(id);

            if (solicitud == null)
                return (false, "Solicitud no encontrada.", null);

            if (solicitud.OfertaRechazada)
                return (false, "La oferta ya fue rechazada anteriormente.", solicitud);

            solicitud.OfertaRechazada = true;
            solicitud.FechaRechazoOferta = DateTime.Now;
            await _context.SaveChangesAsync();

            await RegistrarLogNegociacionAsync(solicitud.Id, "OfertaRechazada", solicitud.OfertaPasajero, usuario);

            return (true, "Oferta rechazada correctamente.", solicitud);
        }

        public async Task<(bool Exito, string Mensaje, SolicitudServicio Solicitud)> RegistrarContraofertaAsync(int id, decimal valor, string usuario, double latConductor = 0, double lngConductor = 0)
        {
            var solicitud = await _context.SolicitudesServicio.FindAsync(id);

            if (solicitud == null)
                return (false, "Solicitud no encontrada.", null);

            var estadosPermitidos = new[] { "Pendiente", "Negociando" };

            if (!estadosPermitidos.Contains(solicitud.Estado))
                return (false, $"No se puede negociar desde el estado actual: {solicitud.Estado}", solicitud);

            solicitud.ContraofertaConductor = valor;
            solicitud.Estado = "ContraofertaPendiente";

            if (latConductor != 0) solicitud.LatConductor = latConductor;
            if (lngConductor != 0) solicitud.LngConductor = lngConductor;

            await _context.SaveChangesAsync();

            await RegistrarLogNegociacionAsync(solicitud.Id, "ContraofertaConductor", valor, usuario);

            return (true, "Contraoferta registrada correctamente.", solicitud);
        }

        public async Task<(bool Exito, string Mensaje, SolicitudServicio Solicitud)> AceptarOfertaAsync(int id, string usuario)
        {
            var solicitud = await _context.SolicitudesServicio.FindAsync(id);

            if (solicitud == null)
                return (false, "Solicitud no encontrada.", null);

            if (solicitud.Estado == "Cancelada")
                return (false, "No se puede aceptar una oferta en una solicitud cancelada.", solicitud);

            if (solicitud.OfertaAceptada)
                return (false, "La oferta ya fue aceptada.", solicitud);

            solicitud.OfertaAceptada = true;
            solicitud.Estado = "Aceptada";
            solicitud.PrecioFinal = solicitud.ContraofertaConductor > 0 ? solicitud.ContraofertaConductor : solicitud.OfertaPasajero;

            await _context.SaveChangesAsync();

            await RegistrarLogNegociacionAsync(solicitud.Id, "OfertaAceptada", solicitud.PrecioFinal, usuario);

            return (true, "Oferta aceptada correctamente.", solicitud);
        }

        public async Task<(bool Exito, string Mensaje, SolicitudServicio Solicitud)> ConfirmarPagoAsync(int id, string usuario)
        {
            var solicitud = await _context.SolicitudesServicio.FindAsync(id);

            if (solicitud == null)
                return (false, "Solicitud no encontrada.", null);

            if (solicitud.Estado == "Cancelada")
                return (false, "No se puede confirmar el pago de una solicitud cancelada.", solicitud);

            if (solicitud.PagoConfirmado)
                return (false, "El pago ya fue confirmado anteriormente.", solicitud);

            solicitud.PagoConfirmado = true;
            solicitud.FechaPagoConfirmado = DateTime.Now;
            await _context.SaveChangesAsync();

            await RegistrarLogCambioAsync(solicitud.Id, solicitud.Estado, solicitud.Estado, usuario);

            return (true, "Pago confirmado correctamente.", solicitud);
        }

        public async Task<(bool Exito, string Mensaje, SolicitudServicio Solicitud)> CalificarConductorAsync(int id, CalificacionConductorDTO dto, string usuario)
        {
            var solicitud = await _context.SolicitudesServicio.FindAsync(id);
            if (solicitud == null)
                return (false, "Solicitud no encontrada.", null);

            if (solicitud.Estado != "Finalizada")
                return (false, "Solo se puede calificar una solicitud finalizada.", solicitud);

            if (solicitud.CalificacionConductor != null)
                return (false, "La solicitud ya fue calificada.", solicitud);

            solicitud.CalificacionConductor = dto.Calificacion;
            solicitud.ComentarioConductor = dto.Comentario;
            solicitud.FechaCalificacionConductor = DateTime.Now;
            solicitud.UsuarioCalificaConductor = usuario;

            _context.CalificacionesConductor.Add(new CalificacionConductor
            {
                SolicitudId = solicitud.Id,
                UsuarioCalificadorId = usuario,
                Comentario = dto.Comentario,
                FechaCalificacion = DateTime.Now,
                UsuarioCalificadoId = solicitud.ConductorId ?? 0
            });

            await _context.SaveChangesAsync();

            await RegistrarLogNegociacionAsync(solicitud.Id, "CalificacionConductor", dto.Calificacion, usuario);

            return (true, "Calificación registrada correctamente.", solicitud);
        }

        public async Task<(bool Exito, string Mensaje)> CalificarClienteAsync(int solicitudId, CalificacionClienteDTO dto, string usuario)
        {
            var solicitud = await _context.SolicitudesServicio.FindAsync(solicitudId);
            if (solicitud == null)
                return (false, "Solicitud no encontrada.");

            if (solicitud.Estado != "Finalizada")
                return (false, "Solo se puede calificar una solicitud finalizada.");

            var yaExiste = await _context.CalificacionesCliente
                .AnyAsync(c => c.SolicitudId == solicitudId);
            if (yaExiste)
                return (false, "La solicitud ya fue calificada.");

            var calificacion = new CalificacionCliente
            {
                SolicitudId = solicitudId,
                UsuarioAltaId = usuario,
                FechaAlta = DateTime.Now,
                FechaCalificacion = DateTime.Now,
                ClienteId = solicitud.ClienteId.ToString(),
                Calificacion = dto.Calificacion,
                Comentario = dto.Comentario
            };

            _context.CalificacionesCliente.Add(calificacion);
            await _context.SaveChangesAsync();

            await RegistrarLogNegociacionAsync(solicitudId, "CalificacionCliente", dto.Calificacion, usuario);

            return (true, "Calificación registrada correctamente.");
        }

        public async Task<(bool Exito, string Mensaje)> RegistrarPagoAsync(int solicitudId, PagoSolicitudDTO dto, string usuario)
        {
            var solicitud = await _context.SolicitudesServicio.FindAsync(solicitudId);
            if (solicitud == null)
                return (false, "Solicitud no encontrada.");

            var yaExiste = await _context.PagosSolicitud
                .AnyAsync(p => p.SolicitudId == solicitudId && p.EstadoPago == EstadoPago.Aprobado);
            if (yaExiste)
                return (false, "La solicitud ya tiene un pago aprobado.");

            var pago = new PagosSolicitud
            {
                SolicitudId = solicitudId,
                MedioPago = dto.MedioPago,
                EstadoPago = dto.EstadoPago,
                FechaPago = dto.FechaPago,
                Monto = dto.Monto,
                ReferenciaTransaccion = dto.ReferenciaTransaccion
            };

            _context.PagosSolicitud.Add(pago);
            await _context.SaveChangesAsync();

            await RegistrarLogNegociacionAsync(solicitudId, "PagoSolicitud", $"{dto.MedioPago} - {dto.Monto}", usuario);

            return (true, "Pago registrado correctamente.");
        }

        public async Task<bool> CreateSolicitudArchivos(SolicitudArchivoDto archivoDto, string filePath)
        {
            var archivo = new SolicitudArchivo
            {
                SolicitudId = archivoDto.SolicitudId,
                NombreArchivo = archivoDto.NombreArchivo,
                RutaArchivo = filePath,
                FechaRegistro = DateTime.Now
            };

            _context.SolicitudArchivos.Add(archivo);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}