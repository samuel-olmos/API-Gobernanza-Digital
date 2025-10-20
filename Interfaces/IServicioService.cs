using API_Gobernanza_Digital.Models;

namespace API_Gobernanza_Digital.Interfaces;

public interface IServicioService
{
    IEnumerable<Servicio> GetAll();
    Servicio? GetById(int id);
    Servicio Create(Servicio servicio);
    Servicio? Update(int id, Servicio servicio);
    bool Delete(int id);
}