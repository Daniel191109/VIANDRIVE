using System.ComponentModel.DataAnnotations;

namespace VIANDRIVEFULL.Models.DTOs
{
    public class ConductorRegistroDto
    {
        // Datos personales
        public string Name { get; set; }              // Nombre completo
        public int Age { get; set; }                  // Edad
        public string Phone { get; set; }             // Teléfono
        public string Email { get; set; }             // Correo electrónico
        public string Document { get; set; }          // Cédula o documento

        // Datos del vehículo
        public string LicenseType { get; set; }       // Tipo de licencia (B1, C2...)
        public string LicenseNumber { get; set; }     // Número de licencia
        public string Plate { get; set; }             // Placa del vehículo
        public string Color { get; set; }             // Color seleccionado
        public string Model { get; set; }             // Modelo seleccionado
        public int Year { get; set; }                 // Año del vehículo

        // Seguridad
        public string Password { get; set; }          // Contraseña

        // Archivos en base64
        public string SelfieBase64 { get; set; }           // Foto con documento
        public string PropertyCardBase64 { get; set; }     // Tarjeta de propiedad
        public string SoatBase64 { get; set; }             // SOAT o seguro
        public string VehiclePhotosBase64 { get; set; }    // Fotos del vehículo
    }
}

