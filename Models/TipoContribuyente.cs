namespace API_Gobernanza_Digital.Models
{
    public class TipoContribuyente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!; // ej. "Persona", "Sociedad"

        // Relación: Un tipo puede tener muchos contribuyentes
        public virtual ICollection<Contribuyente> Contribuyentes { get; set; } = new HashSet<Contribuyente>();
    }
}