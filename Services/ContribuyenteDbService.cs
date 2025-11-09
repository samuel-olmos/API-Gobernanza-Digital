using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Context;
using Microsoft.EntityFrameworkCore;

namespace API_Gobernanza_Digital.Services.DbServices
{
    public class ContribuyenteDbService : IContribuyenteService
    {
        private readonly GobernanzaDbContext _context;

        public ContribuyenteDbService(GobernanzaDbContext context)
        {
            _context = context;
        }

        public async Task<Contribuyente?> GetByIdAsync(int id)
        {
            return await _context.Contribuyentes
                .Include(c => c.ContribuyenteServicios)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Contribuyente>> GetAllAsync()
        {
            return await _context.Contribuyentes
                .ToListAsync();
        }

        public async Task<Contribuyente> CreateAsync(Contribuyente contribuyente)
        {
            await _context.Contribuyentes.AddAsync(contribuyente);
            await _context.SaveChangesAsync();
            return contribuyente;
        }

        public async Task<Contribuyente?> UpdateAsync(int id, Contribuyente contribuyente)
        {
            var existing = await _context.Contribuyentes.FindAsync(id);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(contribuyente);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Contribuyentes.FindAsync(id);
            if (existing == null) return false;

            _context.Contribuyentes.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}