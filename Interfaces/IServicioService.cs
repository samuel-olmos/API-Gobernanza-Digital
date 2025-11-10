using System.Collections.Generic;
using System.Threading.Tasks;
using API_Gobernanza_Digital.Models.Dtos;

namespace API_Gobernanza_Digital.Interfaces;

public interface IServicioService
{
    Task<IEnumerable<ServicioDto>> GetAllAsync();
    Task<ServicioDto?> GetByIdAsync(int id);
    Task<ServicioDto> CreateAsync(ServicioCreateDto dto);
    Task<ServicioDto?> UpdateAsync(int id, ServicioCreateDto dto);
    Task<bool> DeleteAsync(int id);

}