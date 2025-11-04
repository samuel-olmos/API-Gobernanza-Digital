using Microsoft.AspNetCore.Mvc;
using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Models;

namespace API_Gobernanza_Digital.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicioController : ControllerBase
{
    private readonly IServicioService _service;

    public ServicioController(IServicioService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Servicio>> GetAll()
    {
        return Ok(_service.GetAll());
    }

    [HttpGet("{id}")]
    public ActionResult<Servicio> GetById(int id)
    {
        var s = _service.GetById(id);
        if (s == null) return NotFound();
        return Ok(s);
    }

    [HttpPost]
    public ActionResult<Servicio> Create(Servicio servicio)
    {
        var created = _service.Create(servicio);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public ActionResult<Servicio> Update(int id, Servicio servicio)
    {
        var updated = _service.Update(id, servicio);
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
