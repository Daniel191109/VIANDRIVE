using System.Threading.Tasks;
using Google;
using Microsoft.EntityFrameworkCore;
using VIANDRIVEFULL.Data;

//using VIANDRIVEFULL.DTO;
using VIANDRIVEFULL.Models;
using VIANDRIVEFULL.Models.DTOs;

namespace VIANDRIVEFULL.Services
{
    public class RegistroPagoService
    {
        private readonly AppDbContext _context;

        public RegistroPagoService(AppDbContext context)
        {
            _context = context;
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

            // Trazabilidad opcional
            await RegistrarLogAsync(solicitudId, "Pago registrado", usuario);

            return (true, "Pago registrado correctamente.");
        }

        private async Task RegistrarLogAsync(int solicitudId, string accion, string usuario)
        {
            var log = new AuditoriaSolicitud
            {
                SolicitudId = solicitudId,
                Accion = accion,
                Usuario = usuario,
                Fecha = DateTime.Now
            };

            _context.AuditoriaSolicitud.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
