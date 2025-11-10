using System;

namespace API_Gobernanza_Digital.Models.Dtos
{
    public class BoletaDto
    {
        public int Id { get; set; }
        public int ContribuyenteId { get; set; }
        public string? ContribuyenteNombre { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaEmision { get; set; }
        public int PeriodoId { get; set; }
        public string? PeriodoNombre { get; set; }
        public string? Estado { get; set; }
    }

    public class BoletaCreateDto
    {
        public int ContribuyenteId { get; set; }
        public decimal Monto { get; set; }
        public int PeriodoId { get; set; }
        public DateTime? FechaEmision { get; set; }
    }
}