namespace API_Gobernanza_Digital.Models
{
    public class Contribuyente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Apellido { get; set; } = null!;
        public string? RazonSocial { get; set; } = null!;
        public string Identificacion { get; set; } = null!; // DNI o CUIT
        public string Domicilio { get; set; } = null!;
        public string Email { get; set; } = null!;

        // --- Relaciones (ACTUALIZADAS) ---
        public int TipoId { get; set; } // FK a la tabla TipoContribuyente
        public virtual TipoContribuyente Tipo { get; set; } = null!;
        
        public virtual ICollection<ContribuyenteServicio> ContribuyenteServicios { get; set; } = new HashSet<ContribuyenteServicio>();
    }
}