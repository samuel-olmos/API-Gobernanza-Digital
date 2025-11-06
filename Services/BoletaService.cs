using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Context;
using Microsoft.EntityFrameworkCore;

namespace API_Gobernanza_Digital.Services;

public class BoletaService : IBoletaService
{
    private readonly GobernanzaDbContext _context;

    public BoletaService(GobernanzaDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Boleta> GetAll()
    {
        return _context.Boletas
            .Include(b => b.Contribuyente)
            .Include(b => b.Servicio)
            .AsNoTracking()
            .ToList();
    }

    public Boleta? GetById(int id)
    {
        return _context.Boletas
            .Include(b => b.Contribuyente)
            .Include(b => b.Servicio)
            .FirstOrDefault(b => b.Id == id);
    }

    public Boleta Create(Boleta boleta)
    {
        // Generar código de pago si no existe
        if (string.IsNullOrEmpty(boleta.CodigoPagoElectronico))
        {
            boleta.CodigoPagoElectronico = GenerarCodigoPago();
        }

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

    public int GenerarBoletasPeriodo(string periodoFiscal)
    {
        // Validar formato período (yyyy/MM)
        if (!DateTime.TryParseExact(periodoFiscal + "/01", "yyyy/MM/dd", 
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var periodo))
        {
            throw new ArgumentException("Formato de período inválido. Use yyyy/MM");
        }

        // Obtener todos los contribuyentes con sus servicios
        // Nota: se eliminó el filtro por FechaInicio/FechaFin porque
        // esos campos no existen en el modelo de ContribuyenteServicio
        // en este proyecto y no son necesarios para la generación.
        var contribuyentesConServicios = _context.ContribuyenteServicios
            .Include(cs => cs.Contribuyente)
            .Include(cs => cs.Servicio)
            .ToList();

        if (!contribuyentesConServicios.Any())
        {
            return 0;
        }

        // Verificar boletas existentes para este período
        var boletasExistentes = _context.Boletas
            .Where(b => b.Periodo.Year == periodo.Year && b.Periodo.Month == periodo.Month)
            .Select(b => new { b.ContribuyenteId, b.ServicioId })
            .ToHashSet();

        var nuevasBoletas = new List<Boleta>();

        foreach (var cs in contribuyentesConServicios)
        {
            // Verificar si debe generarse boleta según frecuencia
            if (!DebeGenerarBoleta(cs.ServicioId, periodo, cs.Servicio.Frecuencia))
            {
                continue;
            }

            // Evitar duplicados
            if (boletasExistentes.Contains(new { cs.ContribuyenteId, cs.ServicioId }))
            {
                continue;
            }

            var boleta = new Boleta
            {
                ContribuyenteId = cs.ContribuyenteId,
                ServicioId = cs.ServicioId,
                Periodo = periodo,
                FechaVencimiento = CalcularFechaVencimiento(periodo, cs.Servicio.Frecuencia),
                MontoTotal = cs.Servicio.MontoBase,
                CodigoPagoElectronico = GenerarCodigoPago(),
                Estado = EstadoBoleta.Pendiente
            };

            nuevasBoletas.Add(boleta);
        } 

        if (nuevasBoletas.Any())
        {
            _context.Boletas.AddRange(nuevasBoletas);
            _context.SaveChanges();
        }

        return nuevasBoletas.Count;
    }

    public bool MarcarComoPagada(int id, DateTime? fechaPago = null)
    {
        var boleta = _context.Boletas.Find(id);
        if (boleta == null) return false;

        boleta.Estado = EstadoBoleta.Pagada;
        boleta.FechaPago = fechaPago ?? DateTime.UtcNow;

        _context.SaveChanges();
        return true;
    }

    public int ActualizarBoletasVencidas()
    {
        var hoy = DateTime.UtcNow.Date;
        var boletasVencidas = _context.Boletas
            .Where(b => b.Estado == EstadoBoleta.Pendiente && 
                       b.FechaVencimiento < hoy)
            .ToList();

        foreach (var boleta in boletasVencidas)
        {
            boleta.Estado = EstadoBoleta.Vencida;
        }

        _context.SaveChanges();
        return boletasVencidas.Count;
    }

    public Task<int> GenerarBoletasAsync(DateTime? fechaReferencia = null)
    {
        // El controlador puede llamar con una fecha nullable; si no se
        // provee, usamos la fecha actual UTC. `GenerarBoletasPeriodo` espera
        // un string con formato yyyy/MM.
        var referencia = fechaReferencia ?? DateTime.UtcNow;
        var periodoFiscal = referencia.ToString("yyyy/MM");
        var cantidad = GenerarBoletasPeriodo(periodoFiscal);
        return Task.FromResult(cantidad);
    }

    public IEnumerable<Boleta> GetByContribuyente(int contribuyenteId)
    {
        return _context.Boletas
            .Include(b => b.Servicio)
            .Where(b => b.ContribuyenteId == contribuyenteId)
            .OrderByDescending(b => b.Periodo)
            .AsNoTracking()
            .ToList();
    }

    public IEnumerable<Boleta> GetByEstado(EstadoBoleta estado)
    {
        return _context.Boletas
            .Include(b => b.Contribuyente)
            .Include(b => b.Servicio)
            .Where(b => b.Estado == estado)
            .OrderBy(b => b.FechaVencimiento)
            .AsNoTracking()
            .ToList();
    }

    public Boleta? GetByCodigoPago(string codigo)
    {
        return _context.Boletas
            .Include(b => b.Contribuyente)
            .Include(b => b.Servicio)
            .FirstOrDefault(b => b.CodigoPagoElectronico == codigo);
    }

    // Métodos privados auxiliares

    private bool DebeGenerarBoleta(int servicioId, DateTime periodo, FrecuenciaCobro frecuencia)
    {
        var ultimaBoleta = _context.Boletas
            .Where(b => b.ServicioId == servicioId)
            .OrderByDescending(b => b.Periodo)
            .FirstOrDefault();

        if (ultimaBoleta == null)
        {
            return true; // Primera boleta del servicio
        }

        var mesesFrecuencia = ObtenerMesesPorFrecuencia(frecuencia);
        var siguientePeriodo = ultimaBoleta.Periodo.AddMonths(mesesFrecuencia);

        return periodo >= siguientePeriodo;
    }

    private static DateTime CalcularFechaVencimiento(DateTime periodo, FrecuenciaCobro frecuencia)
    {
        var meses = ObtenerMesesPorFrecuencia(frecuencia);
        // Vence el último día del período
        return periodo.AddMonths(meses).AddDays(-1);
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

    private static string GenerarCodigoPago()
    {
        // Genera código alfanumérico único de 12 caracteres
        return Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
    }
}
