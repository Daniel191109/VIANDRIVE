<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;
=======
﻿using Microsoft.EntityFrameworkCore;
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
using VIANDRIVEFULL.Models;

namespace VIANDRIVEFULL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Conductor> Conductores { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<SolicitudServicio> SolicitudesServicio { get; set; }
        public DbSet<Viaje> Viajes { get; set; }
        public DbSet<CalificacionConductor> CalificacionesConductor { get; set; }
        public DbSet<CalificacionCliente> CalificacionesCliente { get; set; }
        public DbSet<PagosSolicitud> PagosSolicitud { get; set; }
        public DbSet<AuditoriaSolicitud> AuditoriaSolicitud { get; set; }
        public DbSet<EstadoConductor> EstadosConductor { get; set; }
        public DbSet<UbicacionConductor> UbicacionesConductor { get; set; }
        public DbSet<CancelacionViaje> Cancelaciones { get; set; }
        public DbSet<Cupon> Cupones { get; set; }
        public DbSet<Mensaje> Mensajes { get; set; }
        public DbSet<Tarifa> Tarifas { get; set; }
        public DbSet<ContactList> ContactList { get; set; }
        public DbSet<SolicitudArchivo> SolicitudArchivos { get; set; }
        public DbSet<NotificacionLog> NotificacionLog { get; set; }
        

        // ✅ AGREGAR ESTA LÍNEA
        public DbSet<PostulacionSolicitud> Postulaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SolicitudServicio>()
                .ToTable("SolicitudesServicio");

<<<<<<< HEAD
            // Las solicitudes deben apuntar a los registros reales de cada tabla del dominio.
            modelBuilder.Entity<SolicitudServicio>()
                .HasOne(s => s.Cliente)
                .WithMany()
                .HasForeignKey(s => s.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SolicitudServicio>()
                .HasOne(s => s.Conductor)
                .WithMany()
                .HasForeignKey(s => s.ConductorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SolicitudServicio>()
                .HasOne(s => s.Vehiculo)
                .WithMany()
                .HasForeignKey(s => s.VehiculoId)
                .OnDelete(DeleteBehavior.Restrict);

=======
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
            // Configuración de precisión para la tarifa
            modelBuilder.Entity<PostulacionSolicitud>()
                .Property(p => p.TarifaOfrecida)
                .HasColumnType("decimal(18,2)");
        }
    }
}