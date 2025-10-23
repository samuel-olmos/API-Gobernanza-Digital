namespace API_Gobernanza_Digital.Models;

public class Contribuyente
{
    public int Id { get; set; }
    public TipoContribuyente Tipo { get; set; } = null!;
    public string NombreRazonSocial { get; set; } = null!;
    public string Identificacion { get; set; } = null!; // DNI-CUIT-Identificacion
    public string Domicilio { get; set; } = null!;
    public string Email { get; set; } = null!;
    public List<Servicio> Servicios { get; set; } = new List<Servicio>();
    public List<Boleta> Boletas { get; set; } = new List<Boleta>();
}

enum class TipoContribuyente
{
    Persona,
    Sociedad
}