namespace API_Gobernanza_Digital.Models
{
    public class Periodo
    {
        public int Id { get; set; }
        public string PeriodoFiscal { get; set; } = null!; // ej. "2025/01"
        public int Anio { get; set; }
        public int Mes { get; set; }

        public bool Generadas { get; set; } // Indica si ya se generaron boletas para este período

        // Relación: Un período puede tener muchas boletas
        public virtual ICollection<Boleta> Boletas { get; set; } = new HashSet<Boleta>();
    }
}