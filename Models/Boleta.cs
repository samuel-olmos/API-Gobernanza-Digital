namespace API_Gobernanza_Digital.Models;

public class Boleta
{
    public int Id { get; set; }
    public int ContribuyenteId { get; set; }
    public int ServicioId { get; set; }
    public string PeriodoFiscal { get; set; } = string.Empty; // Formato: "2025/01"
    public DateTime FechaVencimiento { get; set; }
    public decimal MontoTotal { get; set; }
    public string CodigoPagoElectronico { get; set; } = string.Empty; // Código único
    public EstadoBoleta Estado { get; set; } = EstadoBoleta.Pendiente;
    
    // Propiedades de navegación
    public Contribuyente? Contribuyente { get; set; }
    public Servicio? Servicio { get; set; }
}

public enum EstadoBoleta
{
    Pendiente,
    Pagada,
    Vencida
}