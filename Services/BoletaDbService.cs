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

    public IEnumerable<Boleta> GetAll() =>
        _context.Boletas
            .Include(b => b.ContribuyenteServicio)
                .ThenInclude(cs => cs.Contribuyente)
            .Include(b => b.ContribuyenteServicio)
                .ThenInclude(cs => cs.Servicio)
            .Include(b => b.Periodo)
            .Include(b => b.Estado)
            .AsNoTracking()
            .ToList();

    public Boleta? GetById(int id) =>
        _context.Boletas
            .Include(b => b.ContribuyenteServicio)
                .ThenInclude(cs => cs.Contribuyente)
            .Include(b => b.ContribuyenteServicio)
                .ThenInclude(cs => cs.Servicio)
            .Include(b => b.Periodo)
            .Include(b => b.Estado)
            .FirstOrDefault(b => b.Id == id);

    public Boleta Add(Boleta boleta)
    {
        _context.Boletas.Add(boleta);
        _context.SaveChanges();
        return boleta;
    }

    public Boleta? Update(int id, Boleta boleta)
    {
        var existing = _context.Boletas.Find(id);
        if (existing == null) return null;

        existing.MontoTotal = boleta.MontoTotal;
        existing.CodigoPagoElectronico = boleta.CodigoPagoElectronico;
        existing.FechaPago = boleta.FechaPago;

        existing.ContribuyenteServicioId = boleta.ContribuyenteServicioId;
        existing.PeriodoId = boleta.PeriodoId;
        existing.EstadoId = boleta.EstadoId;

        _context.SaveChanges();
        return existing;
    }

    public bool Delete(int id)
    {
        var existing = _context.Boletas.Find(id);
        if (existing == null) return false;
        _context.Boletas.Remove(existing);
        _context.SaveChanges();
        return true;
    }

    public IEnumerable<Boleta> GetByContribuyente(int contribuyenteId) =>
        _context.Boletas
            .Include(b => b.ContribuyenteServicio)
                .ThenInclude(cs => cs.Servicio)
            .Include(b => b.Periodo)
            .Where(b => b.ContribuyenteServicio.ContribuyenteId == contribuyenteId)
            .OrderByDescending(b => b.Periodo.FechaVencimiento)
            .AsNoTracking()
            .ToList();

    public IEnumerable<Boleta> GetByEstado(int estadoId) =>
        _context.Boletas
            .Include(b => b.ContribuyenteServicio)
                .ThenInclude(cs => cs.Contribuyente)
            .Include(b => b.ContribuyenteServicio)
                .ThenInclude(cs => cs.Servicio)
            .Include(b => b.Periodo)
            .Where(b => b.EstadoId == estadoId)
            .OrderBy(b => b.Periodo.FechaVencimiento)
            .AsNoTracking()
            .ToList();

    public Boleta? GetByCodigoPago(string codigo) =>
        _context.Boletas
            .Include(b => b.ContribuyenteServicio)
                .ThenInclude(cs => cs.Contribuyente)
            .Include(b => b.ContribuyenteServicio)
                .ThenInclude(cs => cs.Servicio)
            .Include(b => b.Periodo)
            .Include(b => b.Estado)
            .FirstOrDefault(b => b.CodigoPagoElectronico == codigo);

    // Devuelve la última fecha de periodo (Periodo.FechaVencimiento) por ContribuyenteServicioId
    public Dictionary<int, DateTime> GetUltimasBoletasPorPar() =>
        _context.Boletas
            .Include(b => b.Periodo)
            .GroupBy(b => b.ContribuyenteServicioId)
            .Select(g => new {
                ContribuyenteServicioId = g.Key,
                Ultimo = g.Max(b => b.Periodo.FechaVencimiento)
            })
            .AsEnumerable()
            .ToDictionary(x => x.ContribuyenteServicioId, x => x.Ultimo);

    // Comprueba existencia por ContribuyenteServicioId + PeriodoId
    public bool ExisteBoleta(int contribuyenteServicioId, int periodoId) =>
        _context.Boletas.Any(b => b.ContribuyenteServicioId == contribuyenteServicioId && b.PeriodoId == periodoId);

    public void AddRange(IEnumerable<Boleta> boletas)
    {
        _context.Boletas.AddRange(boletas);
    }

    // Actualiza estados vencidos buscando los ids reales de los estados "Pendiente" y "Vencida"
    public int UpdateEstadosVencidos(DateTime hoy)
    {
        var pendiente = _context.Estados.AsNoTracking().FirstOrDefault(e => e.Nombre == "Pendiente");
        var vencida = _context.Estados.AsNoTracking().FirstOrDefault(e => e.Nombre == "Vencida");

        if (pendiente == null || vencida == null)
            throw new InvalidOperationException("Los estados 'Pendiente' y/o 'Vencida' no existen en la tabla Estados.");

        var vencidas = _context.Boletas
            .Include(b => b.Periodo)
            .Where(b => b.EstadoId == pendiente.Id && b.Periodo.FechaVencimiento < hoy)
            .ToList();

        foreach (var b in vencidas) b.EstadoId = vencida.Id;

        _context.SaveChanges();
        return vencidas.Count;
    }

    public void SaveChanges() => _context.SaveChanges();
}
