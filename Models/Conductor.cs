public class Conductor
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Email { get; set; }
    public string? Documento { get; set; }
    public string? PasswordHash { get; set; }

    public string? Telefono { get; set; }
    public int Edad { get; set; }
    public string? TipoLicencia { get; set; }
    public string? NumeroLicencia { get; set; }

    public string? Placa { get; set; }
    public string? Modelo { get; set; }
    public string? Color { get; set; }
    public int AnioVehiculo { get; set; }

    public string? SelfieBase64 { get; set; }
    public string? TarjetaPropiedadBase64 { get; set; }
    public string? SoatBase64 { get; set; }
    public string? FotosVehiculoBase64 { get; set; }
    public string? PushToken { get; set; }

    public bool Disponible { get; set; }
    public decimal Calificacion { get; set; } = 5.0m;
    public int ViajesCompletados { get; set; } = 0;
    public decimal GananciasHoy { get; set; } = 0.0m;
    public decimal Saldo { get; set; } = 0.0m;
}