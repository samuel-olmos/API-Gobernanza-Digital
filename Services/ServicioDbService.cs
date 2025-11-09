using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Context;
using Microsoft.EntityFrameworkCore;

namespace API_Gobernanza_Digital.Services
{
    public class ServicioDbService : IServicioService
    {
        private readonly GobernanzaDbContext _context;

        public ServicioDbService(GobernanzaDbContext context)
        {
            _context = context;
        }

        public async Task<Servicio?> GetByIdAsync(int id)
        {
            return await _context.Servicios
                .Include(s => s.Frecuencia)
                .Include(s => s.ContribuyenteServicios)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Servicio>> GetAllAsync()
        {
            return await _context.Servicios
                .Include(s => s.Frecuencia)
                .ToListAsync();
        }

        public async Task<Servicio> CreateAsync(Servicio servicio)
        {
            await _context.Servicios.AddAsync(servicio);
            await _context.SaveChangesAsync();
            return servicio;
        }

        public async Task<Servicio?> UpdateAsync(int id, Servicio servicio)
        {
            var existing = await _context.Servicios.FindAsync(id);
            if (existing == null) return null;
            
            _context.Entry(existing).CurrentValues.SetValues(servicio);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Servicios.FindAsync(id);
            if (existing == null) return false;
            
            _context.Servicios.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}