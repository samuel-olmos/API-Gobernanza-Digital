using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace API_Gobernanza_Digital.Controllers
{
    [ApiController]
    [Route("api/contribuyente-servicio")]
    public class ContribuyenteServicioController : ControllerBase
    {
        private readonly IContribuyenteServicioService _servicioService;

        public ContribuyenteServicioController(IContribuyenteServicioService servicioService)
        {
            _servicioService = servicioService;
        }

        // --- ENDPOINTS DE ESCRITURA ---
        [HttpPost]
        [ProducesResponseType(typeof(ContribuyenteServicioDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CrearContribuyenteServicio([FromBody] ContribuyenteServicioCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var nuevoServicio = await _servicioService.CrearContribuyenteServicioAsync(dto);
            
            return CreatedAtAction(nameof(GetContribuyenteServicioById), new { id = nuevoServicio.Id }, nuevoServicio);
        }

        [HttpPut("{id}/cancelar")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CancelarContribuyenteServicio(int id)
        {
            var resultado = await _servicioService.CancelarContribuyenteServicioAsync(id);
            if (!resultado)
            {
                return NotFound("No se encontró el ContribuyenteServicio o ya estaba cancelado.");
            }
            
            return NoContent();
        }
        
        // --- ENDPOINTS DE LECTURA ---
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ContribuyenteServicioDto>), 200)]
        public async Task<ActionResult<IEnumerable<ContribuyenteServicioDto>>> GetAllContribuyenteServicios()
        {
            var lista = await _servicioService.GetAllContribuyenteServiciosAsync();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ContribuyenteServicioDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ContribuyenteServicioDto>> GetContribuyenteServicioById(int id)
        {
            var item = await _servicioService.GetContribuyenteServicioByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpGet("contribuyente/{contribuyenteId}")]
        [ProducesResponseType(typeof(IEnumerable<ContribuyenteServicioDto>), 200)]
        public async Task<ActionResult<IEnumerable<ContribuyenteServicioDto>>> GetContribuyenteServiciosPorContribuyente(int contribuyenteId)
        {
            var lista = await _servicioService.GetContribuyenteServiciosPorContribuyenteAsync(contribuyenteId);
            return Ok(lista);
        }
    }
}