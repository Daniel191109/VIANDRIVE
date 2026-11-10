<<<<<<< HEAD
using System;
=======
﻿using System;
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VIANDRIVEFULL.Models
{
    public class SolicitudServicio
    {
        [Key]
        public int Id { get; set; }

        public int ClienteId { get; set; }
        public int? ConductorId { get; set; }
        public int? VehiculoId { get; set; }

        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;

        public double LatOrigen { get; set; }
        public double LngOrigen { get; set; }
        public double LatDestino { get; set; }
        public double LngDestino { get; set; }

        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; } = "Pendiente";

        public decimal PrecioSugerido { get; set; }
        public decimal TarifaSugerida => PrecioSugerido;

        public decimal OfertaPasajero { get; set; }
        public decimal? ContraofertaConductor { get; set; }
        public decimal? PrecioFinal { get; set; }

        public string MedioPago { get; set; } = "Efectivo";
        public bool PagoConfirmado { get; set; }
        public DateTime? FechaPagoConfirmado { get; set; }
        public bool OfertaAceptada { get; set; }
        public bool OfertaRechazada { get; set; }
        public DateTime? FechaRechazoOferta { get; set; }

        public DateTime? FechaCancelacion { get; set; }
        public string? MotivoCancelacion { get; set; }
        public string? UsuarioCancela { get; set; }
        public DateTime? FechaFinalizacion { get; set; }

        public int? CalificacionPasajero { get; set; }
        public int? CalificacionConductor { get; set; }
        public string? ComentarioConductor { get; set; }
        public DateTime? FechaCalificacionConductor { get; set; }
        public string? UsuarioCalificaConductor { get; set; }

        public double? LatConductor { get; set; }
        public double? LngConductor { get; set; }

        // ==========================================
        // PROPIEDADES DE NAVEGACIÓN (RELACIONES EF CORE)
        // ==========================================

<<<<<<< HEAD
        // Relación con el pasajero registrado en la tabla Clientes.
        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }
        
        // Relación con el Conductor
        [ForeignKey("ConductorId")]
        public virtual Conductor? Conductor { get; set; }

        // Relación con el Vehículo
        [ForeignKey("VehiculoId")]
        public virtual Vehiculo? Vehiculo { get; set; }
=======
        // Relación con el Pasajero (Asumiendo que tu tabla de usuarios se llama 'Usuario' o 'Cliente')
        [ForeignKey("ClienteId")]
        public virtual Usuario Cliente { get; set; }
        // Nota: Si tu modelo de base de datos para el cliente se llama "Cliente" en vez de "Usuario", 
        // cambia la palabra "Usuario" por "Cliente" en la línea de arriba.

        // Relación con el Conductor
        [ForeignKey("ConductorId")]
        public virtual Conductor Conductor { get; set; }

        // Relación con el Vehículo
        [ForeignKey("VehiculoId")]
        public virtual Vehiculo Vehiculo { get; set; }
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
    }
}