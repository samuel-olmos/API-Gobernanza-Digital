namespace API_Gobernanza_Digital.Models;
public enum EstadoBoleta
{
    Pendiente,
    Pagada,
    Vencida,
    Anulada
}
public class Boleta
{
    public int Id { get; set; }
    public int ContribuyenteId { get; set; } // FK
    public int ServicioId { get; set; } // FK
    public DateTime Periodo { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public decimal MontoTotal { get; set; }
    public string CodigoPagoElectronico { get; set; }
    public EstadoBoleta Estado { get; set; }
    public DateTime? FechaPago { get; set; } // Nullable

    // Propiedades de Navegación
    public virtual Contribuyente Contribuyente { get; set; }
    public virtual Servicio Servicio { get; set; }
}

// public class Boleta
// {
//     public int Id { get; set; }
//     public int ContribuyenteId { get; set; }
//     public int ServicioId { get; set; }
//     public string PeriodoFiscal { get; set; } = string.Empty; // Formato: "2025/01"
//     public DateTime FechaVencimiento { get; set; }
//     public decimal MontoTotal { get; set; }
//     public string CodigoPagoElectronico { get; set; } = string.Empty; // Código único
//     public EstadoBoleta Estado { get; set; } = EstadoBoleta.Pendiente;

//     // Propiedades de navegación
//     public Contribuyente? Contribuyente { get; set; }
//     public Servicio? Servicio { get; set; }
// }