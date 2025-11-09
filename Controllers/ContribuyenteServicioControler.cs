using API_Gobernanza_Digital.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API_Gobernanza_Digital.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuscripcionesController : ControllerBase
    {
        // Depende solo de la interfaz de LÓGICA (esto está perfecto)
        private readonly IContribuyenteServicioService _servicioService;

        public SuscripcionesController(IContribuyenteServicioService servicioService)
        {
            _servicioService = servicioService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearSuscripcion([FromBody] SuscripcionCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var nuevaSuscripcion = await _servicioService.CrearSuscripcionAsync(dto);
            
            // (Sería mejor devolver un DTO, pero funciona)
            return CreatedAtAction(nameof(GetSuscripcion), new { id = nuevaSuscripcion.Id }, nuevaSuscripcion);
        }

        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> CancelarSuscripcion(int id)
        {
            var resultado = await _servicioService.CancelarSuscripcionAsync(id);
            if (!resultado)
            {
                return NotFound("No se encontró la suscripción o ya estaba cancelada.");
            }
            
            return NoContent(); // Éxito
        }
        
        [HttpGet("{id}")]
        public IActionResult GetSuscripcion(int id)
        {
            return Ok("Endpoint GetSuscripcion pendiente de implementación");
        }
    }
}