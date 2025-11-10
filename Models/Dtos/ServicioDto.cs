namespace API_Gobernanza_Digital.Models.Dtos
{
    public class ServicioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal MontoBase { get; set; }
        public int FrecuenciaId { get; set; }
        public string? FrecuenciaNombre { get; set; }
    }

    public class ServicioCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal MontoBase { get; set; }
        public int FrecuenciaId { get; set; }
    }
}