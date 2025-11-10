using System;
using System.Collections.Generic;
using System.Linq;
using API_Gobernanza_Digital.Context;
using API_Gobernanza_Digital.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Gobernanza_Digital.Services;

public class BoletaDbService
{
    private readonly GobernanzaDbContext _context;
    public BoletaDbService(GobernanzaDbContext context) => _context = context;

    // Query completo con todas las navegaciones
    private IQueryable<Boleta> QueryFull() =>
        _context.Boletas
            .Include(b => b.ContribuyenteServicio).ThenInclude(cs => cs.Contribuyente)
            .Include(b => b.ContribuyenteServicio).ThenInclude(cs => cs.Servicio)
            .Include(b => b.Periodo)
            .Include(b => b.Estado);

    // Obtener todas (con navegaciones)
    public IEnumerable<Boleta> GetAll() =>
        QueryFull()
            .AsNoTracking()
            .OrderByDescending(b => b.FechaEmision)
            .ToList();

    // Obtener por ID (con navegaciones)
    public Boleta? GetById(int id) =>
        QueryFull()
            .AsNoTracking()
            .FirstOrDefault(b => b.Id == id);

    // Crear boleta
    public Boleta Add(Boleta boleta)
    {
        if (boleta.FechaEmision == default)
            boleta.FechaEmision = DateTime.UtcNow;
        _context.Boletas.Add(boleta);
        _context.SaveChanges();
        return boleta;
    }

    // Actualizar boleta
    public Boleta? Update(int id, Boleta updated)
    {
        var existing = _context.Boletas.Find(id);
        if (existing == null) return null;

        existing.ContribuyenteServicioId = updated.ContribuyenteServicioId;
        existing.PeriodoId = updated.PeriodoId;
        existing.EstadoId = updated.EstadoId;
        existing.MontoTotal = updated.MontoTotal;
        existing.FechaVencimiento = updated.FechaVencimiento;

        _context.SaveChanges();
        return existing;
    }

    // Eliminar boleta
    public bool Delete(int id)
    {
        var entity = _context.Boletas.Find(id);
        if (entity == null) return false;
        _context.Boletas.Remove(entity);
        _context.SaveChanges();
        return true;
    }

    // Filtros específicos
    public IEnumerable<Boleta> GetByContribuyente(int contribuyenteId) =>
        QueryFull()
            .Where(b => b.ContribuyenteServicio!.ContribuyenteId == contribuyenteId)
            .AsNoTracking()
            .OrderByDescending(b => b.FechaVencimiento)
            .ToList();

    public IEnumerable<Boleta> GetByEstado(int estadoId) =>
        QueryFull()
            .Where(b => b.EstadoId == estadoId)
            .AsNoTracking()
            .OrderBy(b => b.FechaVencimiento)
            .ToList();

    public Boleta? GetByCodigoPago(string codigo) =>
        QueryFull()
            .AsNoTracking()
            .FirstOrDefault(b => b.CodigoPagoElectronico == codigo);

    // Métodos auxiliares
    public void AddRange(IEnumerable<Boleta> boletas)
    {
        foreach (var b in boletas)
            if (b.FechaEmision == default) b.FechaEmision = DateTime.UtcNow;
        _context.Boletas.AddRange(boletas);
    }

    public bool ExisteBoleta(int contribuyenteServicioId, int periodoId) =>
        _context.Boletas.Any(b => 
            b.ContribuyenteServicioId == contribuyenteServicioId && 
            b.PeriodoId == periodoId);

    public void SaveChanges() => _context.SaveChanges();
}
