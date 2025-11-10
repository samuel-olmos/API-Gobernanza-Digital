using System;
using Microsoft.AspNetCore.Mvc;
using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Models.Dtos;

namespace API_Gobernanza_Digital.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoletaController : ControllerBase
{
    private readonly IBoletaService _service;

    public BoletaController(IBoletaService service)
    {
        _service = service;
    }

    // GET: api/boleta
    [HttpGet]
    public ActionResult<IEnumerable<BoletaDto>> GetAll()
    {
        return Ok(_service.GetAll());
    }

    // GET: api/boleta/{id}
    [HttpGet("{id}")]
    public ActionResult<BoletaDto> GetById(int id)
    {
        var b = _service.GetById(id);
        if (b == null) return NotFound();
        return Ok(b);
    }

    // GET: api/boleta/contribuyente/{contribuyenteId}?periodoId=1&estadoId=2
    [HttpGet("contribuyente/{contribuyenteId}")]
    public ActionResult<IEnumerable<BoletaDto>> GetByContribuyente(
        int contribuyenteId, 
        [FromQuery] int? periodoId = null, 
        [FromQuery] int? estadoId = null)
    {
        var boletas = _service.ListarBoletasPorContribuyenteFiltradas(contribuyenteId, periodoId, estadoId);
        return Ok(boletas);
    }

    // GET: api/boleta/estado/{nombreEstado}
    [HttpGet("estado/{nombreEstado}")]
    public ActionResult<IEnumerable<BoletaDto>> GetByEstado(string nombreEstado)
    {
        var boletas = _service.GetByEstadoNombre(nombreEstado);
        return Ok(boletas);
    }

    // GET: api/boleta/codigo/{codigo}
    [HttpGet("codigo/{codigo}")]
    public ActionResult<BoletaDto> GetByCodigoPago(string codigo)
    {
        var boleta = _service.GetByCodigoPago(codigo);
        if (boleta == null) return NotFound();
        return Ok(boleta);
    }

    [HttpPost]
    public ActionResult<BoletaDto> Create(BoletaCreateDto boleta)
    {
        var created = _service.Create(boleta);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public ActionResult<BoletaDto> Update(int id, BoletaCreateDto boleta)
    {
        var updated = _service.Update(id, boleta);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var ok = _service.Delete(id);
        if (!ok) return NotFound();
        return NoContent();
    }

    // POST: api/boleta/generar?idPeriodo=5
    [HttpPost("generar")]
    public async Task<IActionResult> Generar([FromQuery] int idPeriodo)
    {
        var generadas = await _service.GenerarBoletasPeriodo(idPeriodo);
        if (generadas == 0)
        {
            return Ok(new { message = "No se generaron boletas nuevas." });
        }
        return Ok(new { message = $"Se generaron {generadas} boletas.", cantidad = generadas });
    }

    // PUT: api/boleta/{id}/pagar
    [HttpPut("{id}/pagar")]
    public IActionResult MarcarComoPagada(int id, [FromQuery] DateTime? fechaPago = null)
    {
        var ok = _service.MarcarComoPagada(id, fechaPago);
        if (!ok) return NotFound();
        return Ok(new { message = "Boleta marcada como pagada." });
    }

    // PUT: api/boleta/actualizar-vencidas
    [HttpPut("actualizar-vencidas")]
    public IActionResult ActualizarVencidas()
    {
        var actualizadas = _service.ActualizarBoletasVencidas();
        return Ok(new { message = $"Se actualizaron {actualizadas} boletas a estado 'Vencida'.", cantidad = actualizadas });
    }
}