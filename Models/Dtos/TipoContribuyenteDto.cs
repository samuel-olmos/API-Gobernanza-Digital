namespace API_Gobernanza_Digital.Models.Dtos
{
    public class TipoContribuyenteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class TipoContribuyenteCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
    }
}