using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Models.Dtos;

namespace API_Gobernanza_Digital.Interfaces;

public interface IContribuyenteService
{
    Task<IEnumerable<ContribuyenteDto>> GetAllAsync();
    Task<ContribuyenteDto?> GetByIdAsync(int id);
    Task<ContribuyenteDto> CreateAsync(ContribuyenteCreateDto contribuyente);
    Task<ContribuyenteDto?> UpdateAsync(int id, ContribuyenteCreateDto contribuyente);
    Task<bool> DeleteAsync(int id);
}