using API_Gobernanza_Digital.Context;
using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Services.DbServices;
using Microsoft.EntityFrameworkCore;

namespace API_Gobernanza_Digital.Services;

public class MontoService
{
    private readonly Random _random = new Random();


    public decimal CalcularMontoTotal(ContribuyenteServicio cs)
    {
        var montoBase = (decimal)cs.Servicio.MontoBase;

        if (cs.Contribuyente.Tipo.Nombre == "Sociedad")
        {
            montoBase *= 1.50m; // sufijo 'm' para literal decimal
        }

        var consumo = _random.Next(100, 200000);

        return montoBase * consumo * 0.10m;
    } 

}