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
        _context.Boletas.Include(b=>b.Contribuyente).Include(b=>b.Servicio).AsNoTracking().ToList();

    public Boleta? GetById(int id) =>
        _context.Boletas.Include(b=>b.Contribuyente).Include(b=>b.Servicio).FirstOrDefault(b=>b.Id==id);

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
        existing.ContribuyenteId = boleta.ContribuyenteId;
        existing.ServicioId = boleta.ServicioId;
        existing.Periodo = boleta.Periodo;
        existing.FechaVencimiento = boleta.FechaVencimiento;
        existing.MontoTotal = boleta.MontoTotal;
        existing.Estado = boleta.Estado;
        existing.FechaPago = boleta.FechaPago;
        existing.CodigoPagoElectronico = boleta.CodigoPagoElectronico;
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
        _context.Boletas.Include(b=>b.Servicio)
            .Where(b=>b.ContribuyenteId==contribuyenteId)
            .OrderByDescending(b=>b.Periodo).AsNoTracking().ToList();

    public IEnumerable<Boleta> GetByEstado(EstadoBoleta estado) =>
        _context.Boletas.Include(b=>b.Contribuyente).Include(b=>b.Servicio)
            .Where(b=>b.Estado==estado).OrderBy(b=>b.FechaVencimiento).AsNoTracking().ToList();

    public Boleta? GetByCodigoPago(string codigo) =>
        _context.Boletas.Include(b=>b.Contribuyente).Include(b=>b.Servicio)
            .FirstOrDefault(b=>b.CodigoPagoElectronico==codigo);

    public Dictionary<(int ContribuyenteId,int ServicioId), DateTime> GetUltimasBoletasPorPar() =>
        _context.Boletas
            .GroupBy(b=>new { b.ContribuyenteId, b.ServicioId })
            .Select(g=>new {
                g.Key.ContribuyenteId,
                g.Key.ServicioId,
                Ultimo = g.Max(b=>b.Periodo)
            })
            .AsEnumerable()
            .ToDictionary(x=> (x.ContribuyenteId,x.ServicioId), x=> x.Ultimo);

    public bool ExisteBoleta(int contribuyenteId,int servicioId,DateTime periodo) =>
        _context.Boletas.Any(b=>b.ContribuyenteId==contribuyenteId && b.ServicioId==servicioId && b.Periodo==periodo);

    public void AddRange(IEnumerable<Boleta> boletas)
    {
        _context.Boletas.AddRange(boletas);
    }

    public int UpdateEstadosVencidos(DateTime hoy)
    {
        var vencidas = _context.Boletas
            .Where(b=>b.Estado==EstadoBoleta.Pendiente && b.FechaVencimiento < hoy)
            .ToList();
        foreach (var b in vencidas) b.Estado = EstadoBoleta.Vencida;
        _context.SaveChanges();
        return vencidas.Count;
    }

    public void SaveChanges() => _context.SaveChanges();
}
