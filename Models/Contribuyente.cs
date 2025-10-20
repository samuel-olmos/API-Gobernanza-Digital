namespace API_Gobernanza_Digital.Models;

public abstract class Contribuyente
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Domicilio { get; set; }
    public string Email { get; set; }
}

public class Persona : Contribuyente
{
    public string Apellido { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string Dni { get; set; }
}

public class Empresa : Contribuyente
{
    public string NombreContacto { get; set; }
    public string CargoContacto { get; set; }
    public string RazonSocial { get; set; }

}