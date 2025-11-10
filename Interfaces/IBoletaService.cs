using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API_Gobernanza_Digital.Models.Dtos;

namespace API_Gobernanza_Digital.Interfaces;

public interface IBoletaService
{
    // Exposición (DTOs)
    IEnumerable<BoletaDto> GetAll();
    BoletaDto? GetById(int id);
    BoletaDto Create(BoletaCreateDto dto);
    BoletaDto? Update(int id, BoletaCreateDto dto);
    bool Delete(int id);

    IEnumerable<BoletaDto> GetByContribuyente(int contribuyenteId);
    IEnumerable<BoletaDto> ListarBoletasPorContribuyenteFiltradas(int contribuyenteId, int? periodoId = null, int? estadoId = null);
    IEnumerable<BoletaDto> GetByEstadoNombre(string nombreEstado);
    BoletaDto? GetByCodigoPago(string codigo);

    bool MarcarComoPagada(int id, DateTime? fechaPago = null);
    int ActualizarBoletasVencidas();
    Task<int> GenerarBoletasPeriodo(int idPeriodo);
}