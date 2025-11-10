using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Context;
using Microsoft.EntityFrameworkCore;
using API_Gobernanza_Digital.Models.Dtos;

namespace API_Gobernanza_Digital.Services.DbServices
{
    public class ContribuyenteDbService : IContribuyenteService
    {
        private readonly GobernanzaDbContext _context;

        public ContribuyenteDbService(GobernanzaDbContext context) => _context = context;

        // Mapper central
        private static ContribuyenteDto Map(Contribuyente c) => new()
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Apellido = c.Apellido,
            RazonSocial = c.RazonSocial,
            Identificacion = c.Identificacion,
            Domicilio = c.Domicilio,
            Email = c.Email,
            TipoId = c.TipoId,
            TipoNombre = c.Tipo?.Nombre
        };

        public async Task<ContribuyenteDto?> GetByIdAsync(int id)
        {
            var c = await _context.Contribuyentes
                .Include(x => x.Tipo)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            return c == null ? null : Map(c);
        }

        public async Task<IEnumerable<ContribuyenteDto>> GetAllAsync()
        {
            return await _context.Contribuyentes
                .Include(x => x.Tipo)
                .AsNoTracking()
                .Select(x => Map(x))
                .ToListAsync();
        }

        public async Task<ContribuyenteDto> CreateAsync(ContribuyenteCreateDto dto)
        {
            var entity = new Contribuyente
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                RazonSocial = dto.RazonSocial,
                Identificacion = dto.Identificacion,
                Domicilio = dto.Domicilio,
                Email = dto.Email,
                TipoId = dto.TipoId
            };
            await _context.Contribuyentes.AddAsync(entity);
            await _context.SaveChangesAsync();
            await _context.Entry(entity).Reference(e => e.Tipo).LoadAsync();
            return Map(entity);
        }

        public async Task<ContribuyenteDto?> UpdateAsync(int id, ContribuyenteCreateDto dto)
        {
            var existing = await _context.Contribuyentes.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return null;

            existing.Nombre = dto.Nombre;
            existing.Apellido = dto.Apellido;
            existing.RazonSocial = dto.RazonSocial;
            existing.Identificacion = dto.Identificacion;
            existing.Domicilio = dto.Domicilio;
            existing.Email = dto.Email;
            existing.TipoId = dto.TipoId;

            await _context.SaveChangesAsync();
            await _context.Entry(existing).Reference(e => e.Tipo).LoadAsync();
            return Map(existing);
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