namespace API_Gobernanza_Digital.Models
{
    public class Servicio
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal MontoBase { get; set; }

        // --- Relaciones (ACTUALIZADAS) ---
        public int FrecuenciaId { get; set; } // FK a la tabla Frecuencia
        public virtual Frecuencia Frecuencia { get; set; } = null!;
        
        public virtual ICollection<ContribuyenteServicio> ContribuyenteServicios { get; set; } = new HashSet<ContribuyenteServicio>();
    }
}