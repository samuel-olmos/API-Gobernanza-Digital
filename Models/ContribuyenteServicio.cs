namespace API_Gobernanza_Digital.Models
{
    public class ContribuyenteServicio
    {
        public int Id { get; set; } // PK Simple
        public int ContribuyenteId { get; set; } // FK
        public int ServicioId { get; set; } // FK

        // --- NUEVOS DATOS AÑADIDOS ---
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; } // La hacemos Nullable (con '?')
                                               // por si la suscripción no tiene fecha de fin.

        // Propiedades de Navegación
        public virtual Contribuyente Contribuyente { get; set; } = null!;
        public virtual Servicio Servicio { get; set; } = null!;
        
        // Relación: Una suscripción genera muchas boletas
        public virtual ICollection<Boleta> Boletas { get; set; } = new HashSet<Boleta>();
    }
}