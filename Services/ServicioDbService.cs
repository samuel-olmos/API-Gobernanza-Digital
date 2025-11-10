using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Models.Dtos;
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

        // Mapper simple a DTO
        private static ServicioDto MapToDto(Servicio s) => new()
        {
            Id = s.Id,
            Nombre = s.Nombre,
            Descripcion = s.Descripcion,
            MontoBase = s.MontoBase,
            FrecuenciaId = s.FrecuenciaId,
            FrecuenciaNombre = s.Frecuencia?.Nombre
        };

        public async Task<ServicioDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Servicios
                .Include(s => s.Frecuencia)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            return entity == null ? null : MapToDto(entity);
        }

        public async Task<IEnumerable<ServicioDto>> GetAllAsync()
        {
            return await _context.Servicios
                .Include(s => s.Frecuencia)
                .AsNoTracking()
                .Select(s => MapToDto(s))
                .ToListAsync();
        }

        public async Task<ServicioDto> CreateAsync(ServicioCreateDto dto)
        {
            var entity = new Servicio
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                MontoBase = dto.MontoBase,
                FrecuenciaId = dto.FrecuenciaId
            };

            await _context.Servicios.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Recargar frecuencia para el DTO
            await _context.Entry(entity).Reference(s => s.Frecuencia).LoadAsync();
            return MapToDto(entity);
        }

        public async Task<ServicioDto?> UpdateAsync(int id, ServicioCreateDto dto)
        {
            var existing = await _context.Servicios.FirstOrDefaultAsync(s => s.Id == id);
            if (existing == null) return null;

            existing.Nombre = dto.Nombre;
            existing.Descripcion = dto.Descripcion;
            existing.MontoBase = dto.MontoBase;
            existing.FrecuenciaId = dto.FrecuenciaId;

            await _context.SaveChangesAsync();

            // Recargar frecuencia para el DTO
            await _context.Entry(existing).Reference(s => s.Frecuencia).LoadAsync();
            return MapToDto(existing);
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