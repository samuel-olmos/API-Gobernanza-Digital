using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Services.DbServices; // El DbService CONCRETO
using API_Gobernanza_Digital.Models;
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

        public async Task<ContribuyenteServicio> CrearSuscripcionAsync(SuscripcionCreateDto dto)
        {
            var nuevaSuscripcion = new ContribuyenteServicio
            {
                ContribuyenteId = dto.ContribuyenteId,
                ServicioId = dto.ServicioId,
                FechaInicio = DateTime.Now,
                FechaFin = null
            };
            
            await _dbService.AddAsync(nuevaSuscripcion);
            return nuevaSuscripcion;
        }

        public async Task<bool> CancelarSuscripcionAsync(int suscripcionId)
        {
            var suscripcion = await _dbService.GetByIdAsync(suscripcionId);
            
            if (suscripcion == null || suscripcion.FechaFin != null)
            {
                return false;
            }

            suscripcion.FechaFin = DateTime.Now;
            
            await _dbService.UpdateAsync(suscripcion);
            return true;
        }

        // --- MÉTODOS DE LECTURA (SIN LÓGICA, SOLO PASAMANOS) ---

        public async Task<ContribuyenteServicio?> GetSuscripcionByIdAsync(int id)
        {
            return await _dbService.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ContribuyenteServicio>> GetAllSuscripcionesAsync()
        {
            return await _dbService.GetAllAsync();
        }

        public async Task<IEnumerable<ContribuyenteServicio>> GetSuscripcionesPorContribuyenteAsync(int contribuyenteId)
        {
            return await _dbService.GetByContribuyenteIdAsync(contribuyenteId);
        }
    }
}