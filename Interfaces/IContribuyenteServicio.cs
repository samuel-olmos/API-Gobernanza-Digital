using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Models.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API_Gobernanza_Digital.Interfaces
{
    public interface IContribuyenteServicioService
    {
        // Métodos que devuelven DTOs
        Task<ContribuyenteServicioDto> CrearContribuyenteServicioAsync(ContribuyenteServicioCreateDto dto);
        Task<bool> CancelarContribuyenteServicioAsync(int contribuyenteServicioId);
        Task<ContribuyenteServicioDto?> GetContribuyenteServicioByIdAsync(int id);
        Task<IEnumerable<ContribuyenteServicioDto>> GetAllContribuyenteServiciosAsync();
        Task<IEnumerable<ContribuyenteServicioDto>> GetContribuyenteServiciosPorContribuyenteAsync(int contribuyenteId);
    }
}