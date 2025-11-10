using System;

namespace API_Gobernanza_Digital.Models.Dtos
{
    public class ContribuyenteServicioDto
    {
        public int Id { get; set; }
        public int ContribuyenteId { get; set; }
        public string? ContribuyenteNombre { get; set; }
        public int ServicioId { get; set; }
        public string? ServicioNombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public decimal? MontoTotal { get; set; }
    }
}
public class ContribuyenteServicioCreateDto
{
    public int ContribuyenteId { get; set; }
    public int ServicioId { get; set; }
    public DateTime FechaInicio { get; set; }
}