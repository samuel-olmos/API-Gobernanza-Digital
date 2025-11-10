using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace API_Gobernanza_Digital.Services.DbServices
{
    public class ContribuyenteServicioDbService
    {
        private readonly GobernanzaDbContext _context;

        public ContribuyenteServicioDbService(GobernanzaDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ContribuyenteServicio contribuyenteServicio)
        {
            await _context.ContribuyenteServicios.AddAsync(contribuyenteServicio);
            await _context.SaveChangesAsync();
        }

        public async Task<ContribuyenteServicio?> GetByIdAsync(int id)
        {
            return await _context.ContribuyenteServicios
                .Include(cs => cs.Contribuyente)
                .Include(cs => cs.Servicio)
                .FirstOrDefaultAsync(cs => cs.Id == id);
        }
        
        public async Task<IEnumerable<ContribuyenteServicio>> GetAllAsync()
        {
            return await _context.ContribuyenteServicios
                .Include(cs => cs.Contribuyente)
                .Include(cs => cs.Servicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<ContribuyenteServicio>> GetByContribuyenteIdAsync(int contribuyenteId)
        {
            return await _context.ContribuyenteServicios
                .Where(cs => cs.ContribuyenteId == contribuyenteId)
                .Include(cs => cs.Contribuyente)
                .Include(cs => cs.Servicio)
                .ToListAsync();
        }

        public async Task UpdateAsync(ContribuyenteServicio contribuyenteServicio)
        {
            _context.Entry(contribuyenteServicio).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}