using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_Gobernanza_Digital.Context;
using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Models.Dtos;
using API_Gobernanza_Digital.Services.DbServices;
using Microsoft.EntityFrameworkCore;

namespace API_Gobernanza_Digital.Services;

public class BoletaService : IBoletaService
{
    private readonly GobernanzaDbContext _context;
    private readonly BoletaDbService _boletaDb;
    private readonly PeriodoDbService _periodoDb;
    private readonly MontoService _montoService;

    public BoletaService(GobernanzaDbContext context)
    {
        _context = context;
        _boletaDb = new BoletaDbService(context);
        _periodoDb = new PeriodoDbService(context);
        _montoService = new MontoService();
    }

    // -------------------------------- Mapeo --------------------------------
    // Aca se mapea de la entidad al DTO para que el servicio devuelva solo los datos necesarios
    private static BoletaDto Map(Boleta b) => new()
    {
        Id = b.Id,
        ContribuyenteServicioId = b.ContribuyenteServicioId,
        ContribuyenteId = b.ContribuyenteServicio?.ContribuyenteId ?? 0,
        ContribuyenteNombre = b.ContribuyenteServicio?.Contribuyente?.Nombre,
        ServicioId = b.ContribuyenteServicio?.ServicioId ?? 0,
        ServicioNombre = b.ContribuyenteServicio?.Servicio?.Nombre,
        PeriodoId = b.PeriodoId,
        PeriodoFiscal = b.Periodo?.PeriodoFiscal,
        EstadoId = b.EstadoId,
        EstadoNombre = b.Estado?.Nombre,
        MontoTotal = b.MontoTotal,
        CodigoPagoElectronico = b.CodigoPagoElectronico,
        FechaEmision = b.FechaEmision,
        FechaVencimiento = b.FechaVencimiento,
        FechaPago = b.FechaPago
    };

    // -------------------------------- CRUD DTO --------------------------------
    // Metodos Crud que llama el Controlador, usando el DbService interno y mapeando a DTO
    public IEnumerable<BoletaDto> GetAll() =>
        _boletaDb.GetAll().Select(Map); // mapeo a DTO

    public BoletaDto? GetById(int id)
    {
        var entity = _boletaDb.GetById(id);
        return entity == null ? null : Map(entity); // mapeo a DTO
    }

    public BoletaDto Create(BoletaCreateDto dto)
    {
        var estadoPend = dto.EstadoId ??
            _context.Estados.AsNoTracking().First(e => e.Nombre == "Pendiente").Id;

        var entity = new Boleta
        {
            ContribuyenteServicioId = dto.ContribuyenteServicioId,
            PeriodoId = dto.PeriodoId,
            EstadoId = estadoPend,
            MontoTotal = dto.MontoTotal ?? CalcularMontoDesdeSuscripcion(dto.ContribuyenteServicioId),
            CodigoPagoElectronico = GenerarCodigoPago(),
            FechaEmision = DateTime.UtcNow,
            FechaVencimiento = dto.FechaVencimiento ?? DateTime.UtcNow.Date.AddDays(10)
        };

        var created = _boletaDb.Add(entity);
        var result = _boletaDb.GetById(created.Id);
        return Map(result!);
    }

    public BoletaDto? Update(int id, BoletaCreateDto dto)
    {
        var existing = _context.Boletas.Find(id);
        if (existing == null) return null;

        existing.ContribuyenteServicioId = dto.ContribuyenteServicioId;
        existing.PeriodoId = dto.PeriodoId;
        if (dto.MontoTotal.HasValue) existing.MontoTotal = dto.MontoTotal.Value;
        if (dto.EstadoId.HasValue) existing.EstadoId = dto.EstadoId.Value;
        if (dto.FechaVencimiento.HasValue) existing.FechaVencimiento = dto.FechaVencimiento.Value;

        var updated = _boletaDb.Update(id, existing);
        return updated == null ? null : Map(_boletaDb.GetById(id)!);
    }

    public bool Delete(int id) => _boletaDb.Delete(id);

    // ---------------------------- Consultas filtradas ----------------------------
    public IEnumerable<BoletaDto> GetByContribuyente(int contribuyenteId) =>
        _boletaDb.GetByContribuyente(contribuyenteId).Select(Map);

    public IEnumerable<BoletaDto> ListarBoletasPorContribuyenteFiltradas(int contribuyenteId, int? periodoId = null, int? estadoId = null)
    {
        var boletas = _boletaDb.GetByContribuyente(contribuyenteId);

        if (periodoId.HasValue)
            boletas = boletas.Where(b => b.PeriodoId == periodoId.Value);
        if (estadoId.HasValue)
            boletas = boletas.Where(b => b.EstadoId == estadoId.Value);

        return boletas.OrderByDescending(b => b.FechaVencimiento).Select(Map);
    }

    public IEnumerable<BoletaDto> GetByEstadoNombre(string nombreEstado)
    {
        if (string.IsNullOrWhiteSpace(nombreEstado)) return Enumerable.Empty<BoletaDto>();
        var estado = _context.Estados.AsNoTracking().FirstOrDefault(e => e.Nombre == nombreEstado);
        if (estado == null) return Enumerable.Empty<BoletaDto>();
        return _boletaDb.GetByEstado(estado.Id).Select(Map);
    }

    public BoletaDto? GetByCodigoPago(string codigo)
    {
        var entity = _boletaDb.GetByCodigoPago(codigo);
        return entity == null ? null : Map(entity);
    }

    // ---------------------------- Operaciones de estado ----------------------------
    public bool MarcarComoPagada(int id, DateTime? fechaPago = null)
    {
        var entity = _context.Boletas.Find(id);
        if (entity == null) return false;

        var estadoPagada = _context.Estados.FirstOrDefault(e => e.Nombre == "Pagada")
            ?? throw new InvalidOperationException("Estado 'Pagada' no encontrado.");

        entity.EstadoId = estadoPagada.Id;
        entity.FechaPago = fechaPago ?? DateTime.UtcNow;
        _context.SaveChanges();
        return true;
    }

    public int ActualizarBoletasVencidas()
    {
        var hoy = DateTime.UtcNow.Date;
        var estadoPend = _context.Estados.AsNoTracking().First(e => e.Nombre == "Pendiente");
        var estadoVenc = _context.Estados.AsNoTracking().First(e => e.Nombre == "Vencida");

        var vencidas = _context.Boletas
            .Where(b => b.EstadoId == estadoPend.Id && b.FechaVencimiento.Date < hoy)
            .ToList();

        foreach (var b in vencidas)
            b.EstadoId = estadoVenc.Id;

        _context.SaveChanges();
        return vencidas.Count;
    }

    // ---------------------------- Generación por período ----------------------------
    public async Task<int> GenerarBoletasPeriodo(int idPeriodo)
    {
        var periodo = await _periodoDb.GetByIdAsync(idPeriodo)
            ?? throw new InvalidOperationException("Periodo no encontrado.");
        var periodoFecha = new DateTime(periodo.Anio, periodo.Mes, 1);

        if (periodoFecha > DateTime.UtcNow ||
           (periodoFecha.Year == DateTime.UtcNow.Year && periodoFecha.Month >= DateTime.UtcNow.Month))
            throw new InvalidOperationException("No se generan boletas para período futuro o mes actual.");

        var estadoPendiente = _context.Estados.First(e => e.Nombre == "Pendiente");

        var suscripciones = await _context.ContribuyenteServicios
            .Include(cs => cs.Servicio).ThenInclude(s => s.Frecuencia)
            .Include(cs => cs.Contribuyente).ThenInclude(c => c.Tipo)
            .Where(cs => cs.FechaInicio <= periodoFecha &&
                        (cs.FechaFin == null || cs.FechaFin >= periodoFecha))
            .AsNoTracking()
            .ToListAsync();

        if (suscripciones.Count == 0) return 0;

        var nuevas = new List<Boleta>();

        foreach (var cs in suscripciones)
        {
            if (!EsMesCobro(periodoFecha.Month, cs.Servicio.Frecuencia.MesesIntervalo))
                continue;

            if (_boletaDb.ExisteBoleta(cs.Id, periodo.Id))
                continue;

            nuevas.Add(new Boleta
            {
                ContribuyenteServicioId = cs.Id,
                PeriodoId = periodo.Id,
                EstadoId = estadoPendiente.Id,
                MontoTotal = _montoService.CalcularMontoTotal(cs),
                CodigoPagoElectronico = GenerarCodigoPago(),
                FechaEmision = DateTime.UtcNow,
                FechaVencimiento = DateTime.UtcNow.Date.AddDays(10)
            });
        }

        if (nuevas.Count == 0) return 0;

        _boletaDb.AddRange(nuevas);
        _boletaDb.SaveChanges();
        periodo.Generadas = true;
        _context.Periodos.Update(periodo);
        await _context.SaveChangesAsync();
        return nuevas.Count;
    }

    private bool EsMesCobro(int mes, int intervalo) =>
        intervalo <= 1 || ((mes - 1) % intervalo) == 0;

    private decimal CalcularMontoDesdeSuscripcion(int contribuyenteServicioId)
    {
        var cs = _context.ContribuyenteServicios
            .Include(x => x.Servicio)
            .Include(x => x.Contribuyente).ThenInclude(c => c.Tipo)
            .FirstOrDefault(x => x.Id == contribuyenteServicioId)
            ?? throw new InvalidOperationException("Suscripción no encontrada.");
        return _montoService.CalcularMontoTotal(cs);
    }

    private static string GenerarCodigoPago()
    {
        var random = new Random();
        return string.Join("", Enumerable.Range(0, 12).Select(_ => random.Next(0, 10)));
    }
}
