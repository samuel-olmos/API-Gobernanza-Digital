namespace API_Gobernanza_Digital.Models.Dtos
{
    public class FrecuenciaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int DiasIntervalo { get; set; }
    }

    public class FrecuenciaCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public int DiasIntervalo { get; set; }
    }
}