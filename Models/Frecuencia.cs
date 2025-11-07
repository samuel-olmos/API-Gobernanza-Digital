namespace API_Gobernanza_Digital.Models
{
    public class Frecuencia
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!; // ej. "Mensual", "Bimestral"
        public int MesesIntervalo { get; set; } // ej. 1, 2, 3, 12

        // Relación: Una frecuencia puede estar en muchos servicios
        public virtual ICollection<Servicio> Servicios { get; set; } = new HashSet<Servicio>();
    }
}