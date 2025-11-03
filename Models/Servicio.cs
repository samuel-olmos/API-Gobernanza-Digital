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
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public FrecuenciaCobro Frecuencia { get; set; }
    public decimal MontoBase { get; set; }

    // Relaciones
    public virtual ICollection<ContribuyenteServicio> ContribuyenteServicios { get; set; }
    public virtual ICollection<Boleta> Boletas { get; set; }

    public Servicio()
    {
        ContribuyenteServicios = new HashSet<ContribuyenteServicio>();
        Boletas = new HashSet<Boleta>();
    }
}

// public class Servicio
// {
//     public int Id { get; set; }
//     public string NombreServicio { get; set; } = string.Empty;
//     public string Descripcion { get; set; } = string.Empty;
//     public FrecuenciaCobro FrecuenciaDeCobro { get; set; } = FrecuenciaCobro.Mensual;
//     public float MontoBase { get; set; }
//     public List<Contribuyente> Contribuyentes { get; set; }
//     public List<Boleta> Boletas { get; set; }
// }