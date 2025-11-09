using API_Gobernanza_Digital.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using API_Gobernanza_Digital.Models; // Necesario para ActionResult<...>

namespace API_Gobernanza_Digital.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuscripcionesController : ControllerBase
    {
        private readonly IContribuyenteServicioService _servicioService;

        public SuscripcionesController(IContribuyenteServicioService servicioService)
        {
            _servicioService = servicioService;
        }

        // --- ENDPOINTS DE ESCRITURA ---

        [HttpPost]
        [ProducesResponseType(typeof(ContribuyenteServicio), 201)] // Tipo de dato que devuelve
        [ProducesResponseType(400)]
        public async Task<IActionResult> CrearSuscripcion([FromBody] SuscripcionCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var nuevaSuscripcion = await _servicioService.CrearSuscripcionAsync(dto);
            
            // Devolvemos el objeto creado usando el nuevo endpoint GetSuscripcionById
            return CreatedAtAction(nameof(GetSuscripcionById), new { id = nuevaSuscripcion.Id }, nuevaSuscripcion);
        }

        [HttpPut("{id}/cancelar")]
        [ProducesResponseType(204)] // Éxito sin contenido
        [ProducesResponseType(404)] // No encontrado
        public async Task<IActionResult> CancelarSuscripcion(int id)
        {
            var resultado = await _servicioService.CancelarSuscripcionAsync(id);
            if (!resultado)
            {
                return NotFound("No se encontró la suscripción o ya estaba cancelada.");
            }
            
            return NoContent();
        }
        
        // --- ENDPOINTS DE LECTURA (NUEVOS) ---

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ContribuyenteServicio>), 200)]
        public async Task<ActionResult<IEnumerable<ContribuyenteServicio>>> GetAllSuscripciones()
        {
            var suscripciones = await _servicioService.GetAllSuscripcionesAsync();
            return Ok(suscripciones);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ContribuyenteServicio), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ContribuyenteServicio>> GetSuscripcionById(int id)
        {
            var suscripcion = await _servicioService.GetSuscripcionByIdAsync(id);
            if (suscripcion == null)
            {
                return NotFound();
            }
            return Ok(suscripcion);
        }

        [HttpGet("contribuyente/{contribuyenteId}")]
        [ProducesResponseType(typeof(IEnumerable<ContribuyenteServicio>), 200)]
        public async Task<ActionResult<IEnumerable<ContribuyenteServicio>>> GetSuscripcionesPorContribuyente(int contribuyenteId)
        {
            var suscripciones = await _servicioService.GetSuscripcionesPorContribuyenteAsync(contribuyenteId);
            return Ok(suscripciones);
        }
    }
}