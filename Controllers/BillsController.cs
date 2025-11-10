using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Models.Dtos; // <-- Importamos el DTO
using Microsoft.AspNetCore.Mvc;
using System;

namespace API_Gobernanza_Digital.Controllers
{
    /// <summary>
    /// Este controlador es EXCLUSIVO para que la pasarela de pagos (PayLink)
    /// pueda consultarnos, como requiere su documentación.
    /// </summary>
    [ApiController]
    public class BillsController : ControllerBase
    {
        private readonly IBoletaService _boletaService;

        public BillsController(IBoletaService boletaService)
        {
            _boletaService = boletaService;
        }

        /// <summary>
        /// Endpoint para que PayLink consulte una factura (boleta)
        /// antes de procesar un pago.
        /// </summary>
        [HttpGet("/api/bills/{id}")] // <-- Ruta absoluta (como pide PayLink)
        [ProducesResponseType(typeof(PayLinkBillResponseDto), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetBillForGateway(int id, [FromQuery] int businessId)
        {
            // (Ignoramos businessId por ahora, pero podríamos usarlo para validar)

            // 1. Usamos el método existente de IBoletaService
            var boleta = _boletaService.GetById(id);

            if (boleta == null)
            {
                return NotFound(new { message = "Factura no encontrada." });
            }

            // 2. Generamos el 'transactionId' random (como pediste)
            var randomTransactionId = Guid.NewGuid().ToString("N")[..10].ToUpper();

            // 3. Creamos el DTO de respuesta que PayLink espera
            var respuesta = new PayLinkBillResponseDto
            {
                TransactionId = randomTransactionId,
                FacturaId = boleta.Id, // Nuestro código es su FacturaId
                Monto = boleta.MontoTotal
            };

            return Ok(respuesta);
        }
    }
}