using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_Gobernanza_Digital.Context;
using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Gobernanza_Digital.Services;

public class BoletaService : IBoletaService
{
    private readonly GobernanzaDbContext _context;
    private readonly BoletaDbService _boletaDb;

    public BoletaService(GobernanzaDbContext context)
    {
        _context = context;
        _boletaDb = new BoletaDbService(context);
    }

    // CRUD
    public IEnumerable<Boleta> GetAll() => _boletaDb.GetAll();
    public Boleta? GetById(int id) => _boletaDb.GetById(id);

    public Boleta Create(Boleta boleta)
    {
        if (string.IsNullOrEmpty(boleta.CodigoPagoElectronico))
            boleta.CodigoPagoElectronico = GenerarCodigoPago();
        return _boletaDb.Add(boleta);
    }

    public Boleta? Update(int id, Boleta boleta) => _boletaDb.Update(id, boleta);
    public bool Delete(int id) => _boletaDb.Delete(id);

    public IEnumerable<Boleta> GetByContribuyente(int contribuyenteId) => _boletaDb.GetByContribuyente(contribuyenteId);
    public IEnumerable<Boleta> GetByEstado(EstadoBoleta estado) => _boletaDb.GetByEstado(estado);
    public Boleta? GetByCodigoPago(string codigo) => _boletaDb.GetByCodigoPago(codigo);

    public bool MarcarComoPagada(int id, DateTime? fechaPago = null)
    {
        var boleta = _boletaDb.GetById(id);
        if (boleta == null) return false;
        boleta.Estado = EstadoBoleta.Pagada;
        boleta.FechaPago = fechaPago ?? DateTime.UtcNow;
        _boletaDb.Update(id, boleta);
        return true;
    }

    public int ActualizarBoletasVencidas()
    {
        var hoy = DateTime.UtcNow.Date;
        return _boletaDb.UpdateEstadosVencidos(hoy);
    }

    public async Task<int> GenerarBoletasPeriodo(DateTime? fechaReferencia = null)
    {
        // Normalizar fechaReferencia al día 10 del mes
        var refDate = (fechaReferencia ?? DateTime.UtcNow).Date;
        var periodoActual = new DateTime(refDate.Year, refDate.Month, 10);

        // Obtener todos los servicios activos en el periodoActual
        var activos = await _context.ContribuyenteServicios
            .Include(cs=>cs.Servicio)
            .Where(cs=>cs.FechaInicio <= periodoActual && (cs.FechaFin == null || cs.FechaFin >= periodoActual))
            .AsNoTracking()
            .ToListAsync();

        if (activos.Count == 0) return 0;

        var ultimas = _boletaDb.GetUltimasBoletasPorPar();
        var nuevas = new List<Boleta>();

        foreach (var cs in activos)
        {
            var key = (cs.ContribuyenteId, cs.ServicioId);
            var periodos = DeterminarPeriodosAGenerar(cs.Servicio.Frecuencia, cs.FechaInicio, periodoActual, ultimas.GetValueOrDefault(key));

            foreach (var periodo in periodos)
            {
                if (_boletaDb.ExisteBoleta(cs.ContribuyenteId, cs.ServicioId, periodo)) continue;

                nuevas.Add(new Boleta {
                    ContribuyenteId = cs.ContribuyenteId,
                    ServicioId = cs.ServicioId,
                    Periodo = periodo,
                    FechaVencimiento = periodo.AddMonths(1),
                    MontoTotal = cs.Servicio.MontoBase,
                    CodigoPagoElectronico = GenerarCodigoPago(),
                    Estado = EstadoBoleta.Pendiente
                });
            }
        }

        if (nuevas.Count == 0) return 0;
        _boletaDb.AddRange(nuevas);
        _boletaDb.SaveChanges();
        return nuevas.Count;
    }

    private List<DateTime> DeterminarPeriodosAGenerar(FrecuenciaCobro frecuencia, DateTime fechaInicio, DateTime periodoActual, DateTime? ultimoPeriodo)
    {
        var periodos = new List<DateTime>();
        var mesesFrecuencia = ObtenerMesesPorFrecuencia(frecuencia);
        var mesesCobro = CalcularMesesCobro(frecuencia);
        var inicio = new DateTime(fechaInicio.Year, fechaInicio.Month, 10);

        DateTime primer;
        if (ultimoPeriodo == null)
            primer = ObtenerPrimerPeriodoCobro(inicio, mesesCobro);
        else
            primer = ultimoPeriodo.Value.AddMonths(mesesFrecuencia);

        var p = primer;
        while (p <= periodoActual)
        {
            if (mesesCobro.Contains(p.Month))
                periodos.Add(p);
            p = p.AddMonths(mesesFrecuencia);
        }
        return periodos;
    }

    private List<int> CalcularMesesCobro(FrecuenciaCobro frecuencia) =>
        frecuencia switch {
            FrecuenciaCobro.Mensual => new() {1,2,3,4,5,6,7,8,9,10,11,12},
            FrecuenciaCobro.Bimestral => new() {1,3,5,7,9,11},
            FrecuenciaCobro.Trimestral => new() {1,4,7,10},
            FrecuenciaCobro.Semestral => new() {1,7},
            FrecuenciaCobro.Anual => new() {1},
            _ => new() {1}
        };

    private DateTime ObtenerPrimerPeriodoCobro(DateTime inicio, List<int> mesesCobro)
    {
        var mes = inicio.Month;
        var año = inicio.Year;
        var primerMes = mesesCobro.FirstOrDefault(m => m >= mes);
        if (primerMes == 0)
        {
            primerMes = mesesCobro.First();
            año++;
        }
        return new DateTime(año, primerMes, 10);
    }

    private static int ObtenerMesesPorFrecuencia(FrecuenciaCobro frecuencia) =>
        frecuencia switch {
            FrecuenciaCobro.Mensual => 1,
            FrecuenciaCobro.Bimestral => 2,
            FrecuenciaCobro.Trimestral => 3,
            FrecuenciaCobro.Semestral => 6,
            FrecuenciaCobro.Anual => 12,
            _ => 1
        };

    private static string GenerarCodigoPago() =>
        Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
}
