using Microsoft.AspNetCore.Mvc;
using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Models;

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
    public ActionResult<IEnumerable<Contribuyente>> GetAll()
    {
        return Ok(_service.GetAll());
    }

    [HttpGet("{id}")]
    public ActionResult<Contribuyente> GetById(int id)
    {
        var c = _service.GetById(id);
        if (c == null) return NotFound();
        return Ok(c);
    }

    [HttpPost]
    public ActionResult<Contribuyente> Create(Contribuyente contribuyente)
    {
        var created = _service.Create(contribuyente);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public ActionResult<Contribuyente> Update(int id, Contribuyente contribuyente)
    {
        var updated = _service.Update(id, contribuyente);
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
}
