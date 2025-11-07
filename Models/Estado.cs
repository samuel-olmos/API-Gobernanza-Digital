namespace API_Gobernanza_Digital.Models
{
    public class Estado
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!; // ej. "Pendiente", "Pagada", "Vencida", "Anulada"

        // Relación: Un estado puede estar en muchas boletas
        public virtual ICollection<Boleta> Boletas { get; set; } = new HashSet<Boleta>();
    }
}