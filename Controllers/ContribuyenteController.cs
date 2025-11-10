using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Models;
using API_Gobernanza_Digital.Models.Dtos;

namespace API_Gobernanza_Digital.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContribuyenteController : ControllerBase
{
    private readonly IContribuyenteService _service;

    public ContribuyenteController(IContribuyenteService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContribuyenteDto>>> GetAll()
    {
        var all = await _service.GetAllAsync();
        return Ok(all);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContribuyenteDto>> GetById(int id)
    {
        var c = await _service.GetByIdAsync(id);
        if (c == null) return NotFound();
        return Ok(c);
    }

    [HttpPost]
    public async Task<ActionResult<ContribuyenteDto>> Create(ContribuyenteCreateDto contribuyente)
    {
        var created = await _service.CreateAsync(contribuyente);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ContribuyenteDto>> Update(int id, ContribuyenteCreateDto contribuyente)
    {
        var updated = await _service.UpdateAsync(id, contribuyente);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}
