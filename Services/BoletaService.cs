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
            .Include(b => b.ContribuyenteServicio).ThenInclude(cs => cs.Contribuyente)
            .Include(b => b.ContribuyenteServicio).ThenInclude(cs => cs.Servicio)
            .Include(b => b.Periodo)
            .Include(b => b.Estado)
            .AsNoTracking()
            .ToList();
    }

    public Boleta? GetById(int id)
    {
        return _context.Boletas
            .Include(b => b.ContribuyenteServicio).ThenInclude(cs => cs.Contribuyente)
            .Include(b => b.ContribuyenteServicio).ThenInclude(cs => cs.Servicio)
            .Include(b => b.Periodo)
            .Include(b => b.Estado)
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

        // Actualizar los campos relevantes según el nuevo modelo
        existing.ContribuyenteServicioId = boleta.ContribuyenteServicioId;
        existing.PeriodoId = boleta.PeriodoId;
        existing.EstadoId = boleta.EstadoId;
        existing.MontoTotal = boleta.MontoTotal;
        existing.CodigoPagoElectronico = boleta.CodigoPagoElectronico;
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

        // Simplified placeholder: por ahora no generamos boletas automáticamente.
        // Esto permite ejecutar la aplicación y correr migraciones sin depender
        // de lógica de negocio que aún puede cambiar. Implementar generación
        // real si se desea en un paso posterior.
        return 0;
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

    // Nota: se quitaron varios métodos auxiliares específicos de la versión
    // anterior (manejo por enums y cálculos de período). Para poder arrancar
    // rápidamente y ejecutar migraciones, la lógica avanzada se implementará
    // en una siguiente iteración según el nuevo diseño del modelo.

    private static string GenerarCodigoPago()
    {
        // Genera código alfanumérico único de 12 caracteres
        return Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
    }
}
