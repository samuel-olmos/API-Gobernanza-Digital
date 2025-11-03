namespace API_Gobernanza_Digital.Models;

public enum FrecuenciaCobro
{
    Mensual,
    Bimestral,
    Trimestral,
    Semestral,
    Anual
}

public class Servicio
{
    public int Id { get; set; }
    public string NombreServicio { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public FrecuenciaCobro FrecuenciaDeCobro { get; set; } = FrecuenciaCobro.Mensual;
    public float MontoBase { get; set; }
    public List<Contribuyente> Contribuyentes { get; set; }
    public List<Boleta> Boletas { get; set; }
}