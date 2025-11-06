using System;
using System.Threading.Tasks;
using API_Gobernanza_Digital.Models;
namespace API_Gobernanza_Digital.Interfaces;

public interface IBoletaService
{
    IEnumerable<Boleta> GetAll();
    Boleta? GetById(int id);
    Boleta Create(Boleta boleta);
    Boleta? Update(int id, Boleta boleta);
    bool Delete(int id);
    // Generar boletas para un período de referencia. Se deja nullable
    // para que el servicio use la fecha actual si no se provee.
    Task<int> GenerarBoletasAsync(DateTime? fechaReferencia = null);
}
