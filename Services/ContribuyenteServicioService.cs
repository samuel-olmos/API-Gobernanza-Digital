using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Services.DbServices; 
using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Gobernanza_Digital.Services
{
    public class ContribuyenteServicioService : IContribuyenteServicioService
    {
        private readonly ContribuyenteServicioDbService _dbService;

        public ContribuyenteServicioService(ContribuyenteServicioDbService dbService)
        {
            _dbService = dbService;
        }

        // Mapper centralizado
        private static ContribuyenteServicioDto Map(ContribuyenteServicio cs) => new()
        {
            Id = cs.Id,
            ContribuyenteId = cs.ContribuyenteId,
            ContribuyenteNombre = cs.Contribuyente?.Nombre,
            ServicioId = cs.ServicioId,
            ServicioNombre = cs.Servicio?.Nombre,
            FechaInicio = cs.FechaInicio,
            FechaFin = cs.FechaFin
        };

        // --- MÉTODOS DE ESCRITURA (CON LÓGICA) ---
        public async Task<ContribuyenteServicioDto> CrearContribuyenteServicioAsync(ContribuyenteServicioCreateDto dto)
        {
            var nuevoContribuyenteServicio = new ContribuyenteServicio
            {
                ContribuyenteId = dto.ContribuyenteId,
                ServicioId = dto.ServicioId,
                FechaInicio = dto.FechaInicio,
                FechaFin = null
            };
            
            await _dbService.AddAsync(nuevoContribuyenteServicio);
            
            // Recargar con navegaciones para el DTO
            var created = await _dbService.GetByIdAsync(nuevoContribuyenteServicio.Id);
            return Map(created!);
        }

        public async Task<bool> CancelarContribuyenteServicioAsync(int contribuyenteServicioId)
        {
            var contribuyenteServicio = await _dbService.GetByIdAsync(contribuyenteServicioId);
            
            if (contribuyenteServicio == null || contribuyenteServicio.FechaFin != null)
            {
                return false;
            }

            contribuyenteServicio.FechaFin = DateTime.UtcNow;
            await _dbService.UpdateAsync(contribuyenteServicio);
            return true;
        }

        // --- MÉTODOS DE LECTURA (DEVUELVEN DTOs) ---
        public async Task<ContribuyenteServicioDto?> GetContribuyenteServicioByIdAsync(int id)
        {
            var entity = await _dbService.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task<IEnumerable<ContribuyenteServicioDto>> GetAllContribuyenteServiciosAsync()
        {
            var entities = await _dbService.GetAllAsync();
            return entities.Select(Map);
        }

        public async Task<IEnumerable<ContribuyenteServicioDto>> GetContribuyenteServiciosPorContribuyenteAsync(int contribuyenteId)
        {
            var entities = await _dbService.GetByContribuyenteIdAsync(contribuyenteId);
            return entities.Select(Map);
        }
    }
}