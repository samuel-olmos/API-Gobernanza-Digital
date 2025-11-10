using System;

namespace API_Gobernanza_Digital.Models.Dtos
{
    public class BoletaDto
    {
        public int Id { get; set; }
        public int ContribuyenteServicioId { get; set; }
        public int ContribuyenteId { get; set; }
        public string? ContribuyenteNombre { get; set; }
        public int ServicioId { get; set; }
        public string? ServicioNombre { get; set; }
        public int PeriodoId { get; set; }
        public string? PeriodoFiscal { get; set; }
        public int EstadoId { get; set; }
        public string? EstadoNombre { get; set; }
        public decimal MontoTotal { get; set; }
        public string CodigoPagoElectronico { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaPago { get; set; }
    }

    public class BoletaCreateDto
    {
        public int ContribuyenteServicioId { get; set; }
        public int PeriodoId { get; set; }
        public decimal? MontoTotal { get; set; }          // si null se calcula
        public DateTime? FechaVencimiento { get; set; }    // si null +10 días
        public int? EstadoId { get; set; }                 // si null = 'Pendiente'
    }
}