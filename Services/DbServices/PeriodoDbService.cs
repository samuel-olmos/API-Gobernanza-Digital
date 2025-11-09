using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Context;

namespace API_Gobernanza_Digital.Services.DbServices
{
    public class PeriodoDbService
    {
        private readonly GobernanzaDbContext _context;

        public PeriodoDbService(GobernanzaDbContext context)
        {
            _context = context;
        }

        public async Task<Periodo?> GetByIdAsync(int id)
        {
            return await _context.Periodos.FindAsync(id);
        }

        public async Task<IEnumerable<Periodo>> GetAllAsync()
        {
            return await _context.Periodos.ToListAsync();
        }

        public async Task AddAsync(Periodo periodo)
        {
            await _context.Periodos.AddAsync(periodo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Periodo periodo)
        {
            var existing = await _context.Periodos.FindAsync(periodo.Id);
            if (existing == null) return;
            _context.Entry(existing).CurrentValues.SetValues(periodo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.Periodos.FindAsync(id);
            if (existing == null) return;
            _context.Periodos.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}
