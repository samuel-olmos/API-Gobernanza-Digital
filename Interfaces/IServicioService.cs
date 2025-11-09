using API_Gobernanza_Digital.Models;

namespace API_Gobernanza_Digital.Interfaces;

public interface IServicioService
{
    Task<IEnumerable<Servicio>> GetAllAsync();
    Task<Servicio?> GetByIdAsync(int id);
    Task<Servicio> CreateAsync(Servicio servicio);
    Task<Servicio?> UpdateAsync(int id, Servicio servicio);
    Task<bool> DeleteAsync(int id);
}