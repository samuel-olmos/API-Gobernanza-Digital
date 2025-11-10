namespace API_Gobernanza_Digital.Models.Dtos
{
    public class PasarelaPagoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Url { get; set; }
    }

    public class PasarelaPagoCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Url { get; set; }
    }
}