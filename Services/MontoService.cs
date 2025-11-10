using API_Gobernanza_Digital.Context;
using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Services.DbServices;
using Microsoft.EntityFrameworkCore;
using System;

namespace API_Gobernanza_Digital.Services;

public class MontoService
{
    private readonly Random _random = new Random();

    public MontoService()
    {
    }

    public decimal CalcularMontoTotal(ContribuyenteServicio cs)
    {
        // Validaciones defensivas
        if (cs == null) 
            throw new ArgumentNullException(nameof(cs));
        if (cs.Servicio == null) 
            throw new InvalidOperationException("El servicio no está cargado. Use .Include(cs => cs.Servicio)");
        if (cs.Contribuyente == null) 
            throw new InvalidOperationException("El contribuyente no está cargado. Use .Include(cs => cs.Contribuyente).ThenInclude(c => c.Tipo)");
        if (cs.Contribuyente.Tipo == null) 
            throw new InvalidOperationException("El tipo de contribuyente no está cargado. Use .ThenInclude(c => c.Tipo)");

        var montoBase = (decimal)cs.Servicio.MontoBase;

        if (cs.Contribuyente.Tipo.Nombre == "Sociedad")
        {
            montoBase *= 1.50m;
        }

        var consumo = _random.Next(100, 200000);

        return montoBase * consumo * 0.10m;
    } 
}