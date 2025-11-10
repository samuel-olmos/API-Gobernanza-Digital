using API_Gobernanza_Digital.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API_Gobernanza_Digital.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagosController : ControllerBase
    {
        private readonly IPagoService _pagoService;

        public PagosController(IPagoService pagoService)
        {
            _pagoService = pagoService;
        }

        /// <summary>
        /// Endpoint para simular el pago de una boleta.
        /// Llama al POST /api/payments de PayLink.
        /// </summary>
        /// <param name="codigoPago">El CodigoPagoElectronico de la boleta.</param>
        [HttpPost("pagar/{codigoPago}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PagarBoleta(string codigoPago)
        {
            try
            {
                var exito = await _pagoService.PagarBoletaAsync(codigoPago);
                if (exito)
                {
                    return Ok(new { message = "Pago confirmado y registrado." });
                }
                return BadRequest(new { message = "El pago fue rechazado por la pasarela." });
            }
            catch (InvalidOperationException ex)
            {
                // Capturamos excepciones de lógica de negocio (ej. "Boleta ya pagada")
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Captura errores de conexión, etc.
                return StatusCode(500, new { message = "Error al conectar con la pasarela.", error = ex.Message });
            }
        }
    }
}