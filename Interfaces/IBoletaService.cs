using API_Gobernanza_Digital.Models;
namespace API_Gobernanza_Digital.Interfaces;

public interface IBoletaService
{
    IEnumerable<Boleta> GetAll();
    Boleta? GetById(int id);
    Boleta Create(Boleta boleta); //
    Boleta? Update(int id, Boleta boleta);
    bool Delete(int id);
}
