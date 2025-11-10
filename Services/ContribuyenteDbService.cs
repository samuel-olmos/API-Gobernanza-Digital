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

        public ContribuyenteDbService(GobernanzaDbContext context)
        {
            _context = context;
        }

        public async Task<ContribuyenteDto?> GetByIdAsync(int id)
        {
            var contribuyente = await _context.Contribuyentes
                .Include(c => c.Tipo)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contribuyente == null) return null;

            return new ContribuyenteDto
            {
                Id = contribuyente.Id,
                Nombre = contribuyente.Nombre,
                Apellido = contribuyente.Apellido,
                RazonSocial = contribuyente.RazonSocial,
                Identificacion = contribuyente.Identificacion,
                Domicilio = contribuyente.Domicilio,
                Email = contribuyente.Email,
                TipoId = contribuyente.TipoId
            };
        }

        public async Task<IEnumerable<ContribuyenteDto>> GetAllAsync()
        {
            return await _context.Contribuyentes
                .Include(c => c.Tipo)
                .Select(contribuyente => new ContribuyenteDto
                {
                    Id = contribuyente.Id,
                    Nombre = contribuyente.Nombre,
                    Apellido = contribuyente.Apellido,
                    RazonSocial = contribuyente.RazonSocial,
                    Identificacion = contribuyente.Identificacion,
                    Domicilio = contribuyente.Domicilio,
                    Email = contribuyente.Email,
                    TipoId = contribuyente.TipoId
                })
                .ToListAsync();
        }

        public async Task<ContribuyenteDto> CreateAsync(ContribuyenteCreateDto contribuyente)
        {
            var entity = new Contribuyente
            {
                Nombre = contribuyente.Nombre,
                Apellido = contribuyente.Apellido,
                RazonSocial = contribuyente.RazonSocial,
                Identificacion = contribuyente.Identificacion,
                Domicilio = contribuyente.Domicilio,
                Email = contribuyente.Email,
                TipoId = contribuyente.TipoId
            };

            await _context.Contribuyentes.AddAsync(entity);
            await _context.SaveChangesAsync();

            return new ContribuyenteDto
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Apellido = entity.Apellido,
                RazonSocial = entity.RazonSocial,
                Identificacion = entity.Identificacion,
                Domicilio = entity.Domicilio,
                Email = entity.Email,
                TipoId = entity.TipoId
            };
        }

        public async Task<ContribuyenteDto?> UpdateAsync(int id, ContribuyenteCreateDto contribuyente)
        {
            var existing = await _context.Contribuyentes.FindAsync(id);
            if (existing == null) return null;

            existing.Nombre = contribuyente.Nombre;
            existing.Apellido = contribuyente.Apellido;
            existing.RazonSocial = contribuyente.RazonSocial;
            existing.Identificacion = contribuyente.Identificacion;
            existing.Domicilio = contribuyente.Domicilio;
            existing.Email = contribuyente.Email;
            existing.TipoId = contribuyente.TipoId;

            _context.Entry(existing).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new ContribuyenteDto
            {
                Id = existing.Id,
                Nombre = existing.Nombre,
                Apellido = existing.Apellido,
                RazonSocial = existing.RazonSocial,
                Identificacion = existing.Identificacion,
                Domicilio = existing.Domicilio,
                Email = existing.Email,
                TipoId = existing.TipoId
            };
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