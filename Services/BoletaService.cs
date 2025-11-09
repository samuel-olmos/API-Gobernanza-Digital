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

    // Elimina el uso del enum. Usa el nombre de la entidad Estado.
    public IEnumerable<Boleta> GetByEstadoNombre(string nombreEstado)
    {
        if (string.IsNullOrWhiteSpace(nombreEstado)) return Enumerable.Empty<Boleta>();
        var estado = _context.Estados.AsNoTracking().FirstOrDefault(e => e.Nombre == nombreEstado);
        if (estado == null) return Enumerable.Empty<Boleta>();
        return _boletaDb.GetByEstado(estado.Id);
    }

    public Boleta? GetByCodigoPago(string codigo) => _boletaDb.GetByCodigoPago(codigo);

    public bool MarcarComoPagada(int id, DateTime? fechaPago = null)
    {
        var boleta = _boletaDb.GetById(id);
        if (boleta == null) return false;

        var estadoPagada = _context.Estados.FirstOrDefault(e => e.Nombre == "Pagada");
        if (estadoPagada == null) throw new InvalidOperationException("Estado 'Pagada' no encontrado.");

        // Puedes setear por Id o asignar la navegación; con Id es suficiente.
        boleta.EstadoId = estadoPagada.Id;
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
        var refDate = (fechaReferencia ?? DateTime.UtcNow).Date;
        var periodoActualFecha = new DateTime(refDate.Year, refDate.Month, 10);

        var activos = await _context.ContribuyenteServicios
            .Include(cs => cs.Servicio)
                .ThenInclude(s => s.Frecuencia)
            .Where(cs => cs.FechaInicio <= periodoActualFecha &&
                        (cs.FechaFin == null || cs.FechaFin >= periodoActualFecha))
            .AsNoTracking()
            .ToListAsync();

        if (activos.Count == 0) return 0;

        var ultimas = _boletaDb.GetUltimasBoletasPorPar();

        var estadoPendiente = _context.Estados.FirstOrDefault(e => e.Nombre == "Pendiente")
            ?? throw new InvalidOperationException("Estado 'Pendiente' no encontrado.");

        var nuevas = new List<Boleta>();

        foreach (var cs in activos)
        {
            var ultimoPeriodoFecha = ultimas.GetValueOrDefault(cs.Id);
            DateTime? ultimo = ultimoPeriodoFecha == default ? null : ultimoPeriodoFecha;

            var mesesIntervalo = cs.Servicio.Frecuencia.MesesIntervalo;
            var periodos = DeterminarPeriodosAGenerar(mesesIntervalo, cs.FechaInicio, periodoActualFecha, ultimo);

            foreach (var fechaPeriodo in periodos)
            {
                var periodoEntidad = await GetOrCreatePeriodo(fechaPeriodo);
                if (_boletaDb.ExisteBoleta(cs.Id, periodoEntidad.Id)) continue;

                nuevas.Add(new Boleta
                {
                    ContribuyenteServicioId = cs.Id,
                    PeriodoId = periodoEntidad.Id,
                    EstadoId = estadoPendiente.Id,
                    MontoTotal = cs.Servicio.MontoBase,
                    CodigoPagoElectronico = GenerarCodigoPago()
                });
            }
        }

        if (nuevas.Count == 0) return 0;

        _boletaDb.AddRange(nuevas);
        _boletaDb.SaveChanges();
        return nuevas.Count;
    }

    private List<DateTime> DeterminarPeriodosAGenerar(int mesesIntervalo, DateTime fechaInicio, DateTime periodoActual, DateTime? ultimoPeriodo)
    {
        var periodos = new List<DateTime>();
        var mesesCobro = CalcularMesesCobro(mesesIntervalo);
        var inicioNormalizado = new DateTime(fechaInicio.Year, fechaInicio.Month, 10);

        DateTime primer = ultimoPeriodo == null
            ? ObtenerPrimerPeriodoCobro(inicioNormalizado, mesesCobro)
            : new DateTime(ultimoPeriodo.Value.Year, ultimoPeriodo.Value.Month, 10).AddMonths(mesesIntervalo);

        var cursor = primer;
        while (cursor <= periodoActual)
        {
            if (mesesCobro.Contains(cursor.Month))
                periodos.Add(cursor);
            cursor = cursor.AddMonths(mesesIntervalo);
        }
        return periodos;
    }

    private List<int> CalcularMesesCobro(int intervalo) =>
        intervalo switch
        {
            <= 0 => new() { 1 },
            1 => Enumerable.Range(1, 12).ToList(),
            2 => new() { 1, 3, 5, 7, 9, 11 },
            3 => new() { 1, 4, 7, 10 },
            6 => new() { 1, 7 },
            >= 12 => new() { 1 },
            _ => Enumerable.Range(1, 12).Where(m => ((m - 1) % intervalo) == 0).ToList()
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

    private async Task<Periodo> GetOrCreatePeriodo(DateTime fechaPeriodo)
    {
        var anio = fechaPeriodo.Year;
        var mes = fechaPeriodo.Month;
        var fecha10 = new DateTime(anio, mes, 10);

        var existente = await _context.Periodos.FirstOrDefaultAsync(p => p.Anio == anio && p.Mes == mes);
        if (existente != null) return existente;

        var nuevo = new Periodo
        {
            Anio = anio,
            Mes = mes,
            PeriodoFiscal = $"{anio:D4}/{mes:D2}",
            FechaVencimiento = fecha10
        };
        _context.Periodos.Add(nuevo);
        await _context.SaveChangesAsync();
        return nuevo;
    }

    private static string GenerarCodigoPago() =>
        Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
}
