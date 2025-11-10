using System;

namespace API_Gobernanza_Digital.Models.Dtos
{
    public class ContribuyenteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Apellido { get; set; }
        public string? RazonSocial { get; set; }
        public string Identificacion { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TipoId { get; set; }
        public string? TipoNombre { get; set; }              // agregado
    }

    public class ContribuyenteCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Apellido { get; set; }
        public string? RazonSocial { get; set; }
        public string Identificacion { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TipoId { get; set; }
    }
}