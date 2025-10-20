using API_Gobernanza_Digital.Models;

namespace API_Gobernanza_Digital.Interfaces;

public interface IContribuyenteService
{
    IEnumerable<Contribuyente> GetAll();
    Contribuyente? GetById(int id);
    Contribuyente Create(Contribuyente contribuyente);
    Contribuyente? Update(int id, Contribuyente contribuyente);
    bool Delete(int id);
    Contribuyente? GetByIdentificacion(string identificacion);
}