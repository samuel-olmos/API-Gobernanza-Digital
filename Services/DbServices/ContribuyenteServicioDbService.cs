using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace API_Gobernanza_Digital.Services.DbServices
{
    // NO HAY INTERFAZ
    public class ContribuyenteServicioDbService
    {
        private readonly GobernanzaDbContext _context;

        public ContribuyenteServicioDbService(GobernanzaDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ContribuyenteServicio suscripcion)
        {
            await _context.ContribuyenteServicios.AddAsync(suscripcion);
            await _context.SaveChangesAsync();
        }

        public async Task<ContribuyenteServicio?> GetByIdAsync(int id)
        {
            return await _context.ContribuyenteServicios.FindAsync(id);
        }

        public async Task UpdateAsync(ContribuyenteServicio suscripcion)
        {
            _context.Entry(suscripcion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}