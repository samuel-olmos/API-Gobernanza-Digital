using System;
using Microsoft.AspNetCore.Mvc;
using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Models;

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

	[HttpGet]
	public ActionResult<IEnumerable<Boleta>> GetAll()
	{
		return Ok(_service.GetAll());
	}

	[HttpGet("{id}")]
	public ActionResult<Boleta> GetById(int id)
	{
		var b = _service.GetById(id);
		if (b == null) return NotFound();
		return Ok(b);
	}

	[HttpPost]
	public ActionResult<Boleta> Create(Boleta boleta)
	{
		var created = _service.Create(boleta);
		return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
	}

	[HttpPut("{id}")]
	public ActionResult<Boleta> Update(int id, Boleta boleta)
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
}