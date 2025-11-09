using API_Gobernanza_Digital.Models;

namespace API_Gobernanza_Digital.Interfaces;

public interface IContribuyenteService
{
    Task<IEnumerable<Contribuyente>> GetAllAsync();
    Task<Contribuyente?> GetByIdAsync(int id);
    Task<Contribuyente> CreateAsync(Contribuyente contribuyente);
    Task<Contribuyente?> UpdateAsync(int id, Contribuyente contribuyente);
    Task<bool> DeleteAsync(int id);
}