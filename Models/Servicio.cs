namespace API_Gobernanza_Digital.Models;

public class Servicio
{
    public int Id { get; set; }
    public string NombreServicio { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string FrecuenciaCobro { get; set; } = string.Empty;
    public decimal MontoBase { get; set; }
}