using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Services.DbServices; 
using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Models.Dtos; // <-- Importar el DTO
using System;
using System.Collections.Generic;
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

        // --- MÉTODOS DE ESCRITURA (CON LÓGICA) ---

        public async Task<ContribuyenteServicio> CrearContribuyenteServicioAsync(ContribuyenteServicioCreateDto dto)
        {
            var nuevoContribuyenteServicio = new ContribuyenteServicio
            {
                ContribuyenteId = dto.ContribuyenteId,
                ServicioId = dto.ServicioId,
                FechaInicio = DateTime.Now,
                FechaFin = null
            };
            
            await _dbService.AddAsync(nuevoContribuyenteServicio);
            return nuevoContribuyenteServicio;
        }

        public async Task<bool> CancelarContribuyenteServicioAsync(int contribuyenteServicioId)
        {
            var contribuyenteServicio = await _dbService.GetByIdAsync(contribuyenteServicioId);
            
            if (contribuyenteServicio == null || contribuyenteServicio.FechaFin != null)
            {
                return false; // No existe o ya estaba cancelado
            }

            contribuyenteServicio.FechaFin = DateTime.Now;
            
            await _dbService.UpdateAsync(contribuyenteServicio);
            return true;
        }

        // --- MÉTODOS DE LECTURA (PASAMANOS) ---

        public async Task<ContribuyenteServicio?> GetContribuyenteServicioByIdAsync(int id)
        {
            return await _dbService.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ContribuyenteServicio>> GetAllContribuyenteServiciosAsync()
        {
            return await _dbService.GetAllAsync();
        }

        public async Task<IEnumerable<ContribuyenteServicio>> GetContribuyenteServiciosPorContribuyenteAsync(int contribuyenteId)
        {
            return await _dbService.GetByContribuyenteIdAsync(contribuyenteId);
        }
    }
}