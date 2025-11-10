using System;

namespace API_Gobernanza_Digital.Models.Dtos
{
    public class ContribuyenteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Identificacion { get; set; } // e.g. RUT/CUIT
        public int TipoContribuyenteId { get; set; }
        public string? TipoContribuyenteNombre { get; set; }
        public string? Email { get; set; }
    }

    public class ContribuyenteCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Identificacion { get; set; }
        public int TipoContribuyenteId { get; set; }
        public string? Email { get; set; }
    }
}