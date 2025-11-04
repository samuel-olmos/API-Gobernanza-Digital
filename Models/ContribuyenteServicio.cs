namespace API_Gobernanza_Digital.Models;
public class ContribuyenteServicio
{
    // Claves Foráneas que forman la Clave Primaria Compuesta
    public int ContribuyenteId { get; set; }
    public int ServicioId { get; set; }

    public DateTime FechaAlta { get; set; }

    // Propiedades de Navegación
    public virtual Contribuyente Contribuyente { get; set; }
    public virtual Servicio Servicio { get; set; }
}