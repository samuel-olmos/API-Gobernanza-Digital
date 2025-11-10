using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Models.Dtos; // <-- Importar el DTO
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API_Gobernanza_Digital.Interfaces
{
    public interface IContribuyenteServicioService
    {
        // Métodos de Escritura
        Task<ContribuyenteServicio> CrearContribuyenteServicioAsync(ContribuyenteServicioCreateDto dto);
        Task<bool> CancelarContribuyenteServicioAsync(int contribuyenteServicioId);

        // Métodos de Lectura
        Task<ContribuyenteServicio?> GetContribuyenteServicioByIdAsync(int id);
        Task<IEnumerable<ContribuyenteServicio>> GetAllContribuyenteServiciosAsync();
        Task<IEnumerable<ContribuyenteServicio>> GetContribuyenteServiciosPorContribuyenteAsync(int contribuyenteId);
    }
}