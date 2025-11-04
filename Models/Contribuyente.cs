namespace API_Gobernanza_Digital.Models;
public enum TipoContribuyente
{
    Persona,
    Sociedad
}
public class Contribuyente
{
    public int Id { get; set; }
    public TipoContribuyente Tipo { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Apellido { get; set; } = null!;
    public string? RazonSocial { get; set; } = null!;
    public string Identificacion { get; set; } = null!; // DNI o CUIT
    public string Domicilio { get; set; } = null!;
    public string Email { get; set; } = null!;

    // Relaciones
    public virtual ICollection<ContribuyenteServicio> ContribuyenteServicios { get; set; }
    public virtual ICollection<Boleta> Boletas { get; set; }

    public Contribuyente()
    {
        ContribuyenteServicios = new HashSet<ContribuyenteServicio>();
        Boletas = new HashSet<Boleta>();
    }
}


// public class Contribuyente
// {
//     public int Id { get; set; }
//     public TipoContribuyente Tipo { get; set; }
//     public string Nombre { get; set; } = null!;
//     public string? Apellido { get; set; } = null!;
//     public string? RazonSocial { get; set; } = null!;
//     public string Identificacion { get; set; } = null!; // DNI-CUIT-Identificacion
//     public string Domicilio { get; set; } = null!;
//     public string Email { get; set; } = null!;
//     public List<Servicio> Servicios { get; set; } = new List<Servicio>();
//     public List<Boleta> Boletas { get; set; } = new List<Boleta>();
// }

