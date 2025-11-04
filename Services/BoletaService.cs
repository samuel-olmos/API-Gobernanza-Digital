using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Data;
using Microsoft.EntityFrameworkCore;

namespace API_Gobernanza_Digital.Services;

public class BoletaService : IBoletaService
{
    private readonly ApplicationDbContext _db;

    public BoletaService(ApplicationDbContext db)
    {
        _db = db;
    }

    public IEnumerable<Boleta> GetAll()
    {
        return _db.Boletas
            .Include(b => b.Contribuyente)
            .Include(b => b.Servicio)
            .AsNoTracking()
            .ToList();
    }

    public Boleta? GetById(int id)
    {
        return _db.Boletas
            .Include(b => b.Contribuyente)
            .Include(b => b.Servicio)
            .FirstOrDefault(b => b.Id == id);
    }

    public Boleta Create(Boleta boleta)
    {
        _db.Boletas.Add(boleta);
        _db.SaveChanges();
        return boleta;
    }

    public Boleta? Update(int id, Boleta boleta)
    {
        var existing = _db.Boletas.Find(id);
        if (existing == null) return null;

        existing.ContribuyenteId = boleta.ContribuyenteId;
        existing.ServicioId = boleta.ServicioId;
        existing.PeriodoFiscal = boleta.PeriodoFiscal;
        existing.FechaVencimiento = boleta.FechaVencimiento;
        existing.MontoTotal = boleta.MontoTotal;
        existing.CodigoPagoElectronico = boleta.CodigoPagoElectronico;
        existing.Estado = boleta.Estado;

        _db.SaveChanges();
        return existing;
    }

    public bool Delete(int id)
    {
        var existing = _db.Boletas.Find(id);
        if (existing == null) return false;
        _db.Boletas.Remove(existing);
        _db.SaveChanges();
        return true;
    }

    public async Task<int> GenerarBoletasAsync(DateTime? fechaReferencia = null)
    {
        var referencia = (fechaReferencia ?? DateTime.UtcNow).Date;
        var periodoInicio = new DateTime(referencia.Year, referencia.Month, 1);
        var periodoFiscal = periodoInicio.ToString("yyyy/MM", CultureInfo.InvariantCulture);

        var contribuyentes = await _db.Contribuyentes
            .Include(c => c.Servicios)
            .ToListAsync()
            .ConfigureAwait(false);

        if (contribuyentes.Count == 0)
        {
            return 0;
        }

        var idsContribuyentes = contribuyentes.Select(c => c.Id).ToList();

        var boletasPrevias = await _db.Boletas
            .Where(b => idsContribuyentes.Contains(b.ContribuyenteId))
            .ToListAsync()
            .ConfigureAwait(false);

        var boletasPorPeriodoActual = new HashSet<(int, int)>(
            boletasPrevias
                .Where(b => string.Equals(b.PeriodoFiscal, periodoFiscal, StringComparison.OrdinalIgnoreCase))
                .Select(b => (b.ContribuyenteId, b.ServicioId)));

        var ultimaPorSuscripcion = boletasPrevias
            .GroupBy(b => (b.ContribuyenteId, b.ServicioId))
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(b => b.FechaVencimiento).First());

        var nuevasBoletas = new List<Boleta>();

        foreach (var contribuyente in contribuyentes)
        {
            var servicios = contribuyente.Servicios ?? Enumerable.Empty<Servicio>();

            foreach (var servicio in servicios)
            {
                var key = (contribuyente.Id, servicio.Id);

                if (boletasPorPeriodoActual.Contains(key))
                {
                    continue;
                }

                ultimaPorSuscripcion.TryGetValue(key, out var ultimaBoleta);

                if (!DebeGenerarse(periodoInicio, ultimaBoleta, servicio.FrecuenciaDeCobro))
                {
                    continue;
                }

                var nuevaBoleta = new Boleta
                {
                    ContribuyenteId = contribuyente.Id,
                    ServicioId = servicio.Id,
                    PeriodoFiscal = periodoFiscal,
                    FechaVencimiento = CalcularFechaVencimiento(periodoInicio, servicio.FrecuenciaDeCobro),
                    MontoTotal = (decimal)Math.Round(servicio.MontoBase, 2, MidpointRounding.AwayFromZero),
                    CodigoPagoElectronico = GenerarCodigoPago(),
                    Estado = EstadoBoleta.Pendiente
                };

                nuevasBoletas.Add(nuevaBoleta);
            }
        }

        if (nuevasBoletas.Count == 0)
        {
            return 0;
        }

        await _db.Boletas.AddRangeAsync(nuevasBoletas).ConfigureAwait(false);
        await _db.SaveChangesAsync().ConfigureAwait(false);

        return nuevasBoletas.Count;
    }

    private static bool DebeGenerarse(DateTime periodoInicio, Boleta? ultima, FrecuenciaCobro frecuencia)
    {
        if (ultima == null)
        {
            return true;
        }

        var meses = ObtenerMesesPorFrecuencia(frecuencia);
        var periodoUltima = ObtenerInicioDePeriodo(ultima) ?? periodoInicio;
        var siguientePeriodo = periodoUltima.AddMonths(meses);

        return periodoInicio >= siguientePeriodo;
    }

    private static DateTime CalcularFechaVencimiento(DateTime periodoInicio, FrecuenciaCobro frecuencia)
    {
        var meses = ObtenerMesesPorFrecuencia(frecuencia);
        return periodoInicio.AddMonths(meses).AddDays(-1);
    }

    private static int ObtenerMesesPorFrecuencia(FrecuenciaCobro frecuencia)
    {
        return frecuencia switch
        {
            FrecuenciaCobro.Mensual => 1,
            FrecuenciaCobro.Bimestral => 2,
            FrecuenciaCobro.Trimestral => 3,
            FrecuenciaCobro.Semestral => 6,
            FrecuenciaCobro.Anual => 12,
            _ => 1
        };
    }

    private static DateTime? ObtenerInicioDePeriodo(Boleta boleta)
    {
        if (!string.IsNullOrWhiteSpace(boleta.PeriodoFiscal)
            && DateTime.TryParseExact(
                boleta.PeriodoFiscal + "/01",
                "yyyy/MM/dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var periodo))
        {
            return periodo;
        }

        return new DateTime(boleta.FechaVencimiento.Year, boleta.FechaVencimiento.Month, 1);
    }

    private static string GenerarCodigoPago()
    {
        return Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)[..12].ToUpperInvariant();
    }
}
