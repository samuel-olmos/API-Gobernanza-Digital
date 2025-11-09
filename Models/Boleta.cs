namespace API_Gobernanza_Digital.Models
{
    public class Boleta
    {
        public int Id { get; set; }
        public decimal MontoTotal { get; set; }
        public string CodigoPagoElectronico { get; set; } = null!;
        public DateTime? FechaPago { get; set; } // Nullable
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }

        // --- Relaciones (ACTUALIZADAS) ---
        public int ContribuyenteServicioId { get; set; } // FK a la "suscripción"
        public virtual ContribuyenteServicio ContribuyenteServicio { get; set; } = null!;

        public int PeriodoId { get; set; } // FK al período
        public virtual Periodo Periodo { get; set; } = null!;

        public int EstadoId { get; set; } // FK al estado
        public virtual Estado Estado { get; set; } = null!;
    }
}