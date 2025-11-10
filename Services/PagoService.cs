using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json; // Requiere el paquete NuGet: System.Net.Http.Json
using System.Threading.Tasks;

namespace API_Gobernanza_Digital.Services
{
    // --- DTOs Auxiliares (Solo para este servicio) ---

    // DTO para ENVIAR a POST /api/payments
    public class PayLinkPaymentRequestDto
    {
        public string TransactionId { get; set; } = null!;
        public string FacturaId { get; set; } = null!;
        public decimal Monto { get; set; }
    }

    // DTO para RECIBIR de PayLink
    public class PayLinkPaymentResponseDto
    {
        public int Id { get; set; }
        public string TransactionId { get; set; } = null!;
        public string FacturaId { get; set; } = null!;
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = null!; // <-- "Confirmado"
        public int BusinessId { get; set; }
    }

    // --- El Servicio ---

    public class PagoService : IPagoService
    {
        // --- Variables "Hardcodeadas" (como pediste) ---
        
        // 1. URL base de la OTRA API (la de PayLink)
        private readonly string _pasarelaApiUrl = "http://localhost:5105"; // <-- CAMBIA ESTE PUERTO
        
        // 2. API Key que PayLink te dio al registrarte
        private readonly string _apiKey = "14427b31c1fe49d7abed"; 
        
        // ---

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IBoletaService _boletaService; // Tu servicio de boletas existente

        public PagoService(IHttpClientFactory httpClientFactory, IBoletaService boletaService)
        {
            _httpClientFactory = httpClientFactory;
            _boletaService = boletaService;
        }

        /// <summary>
        /// Implementación del Flujo 2: Pagar una Boleta.
        /// </summary>
        public async Task<bool> PagarBoletaAsync(string codigoPagoElectronico)
        {
            // 1. Buscar la boleta en NUESTRA BD
            var boleta = _boletaService.GetByCodigoPago(codigoPagoElectronico);
            if (boleta == null)
            {
                throw new InvalidOperationException("La boleta con ese código de pago no existe.");
            }
            if (boleta.Estado.Nombre == "Pagada") // Asumiendo que el Estado está cargado
            {
                throw new InvalidOperationException("La boleta ya fue pagada.");
            }

            // 2. Crear el payload para PayLink (como pediste)
            var payload = new PayLinkPaymentRequestDto
            {
                TransactionId = Guid.NewGuid().ToString("N")[..10].ToUpper(), // ID random
                FacturaId = boleta.CodigoPagoElectronico, // Nuestro código
                Monto = boleta.MontoTotal
            };

            // 3. Crear el cliente HTTP y llamar a PayLink
            var httpClient = _httpClientFactory.CreateClient();
            var url = $"{_pasarelaApiUrl}/api/payments";
            
            // Añadir la API Key
            httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);

            // 4. Enviar la petición POST
            var response = await httpClient.PostAsJsonAsync(url, payload);

            if (!response.IsSuccessStatusCode)
            {
                // El pago falló en la pasarela (ej. 400, 404)
                return false;
            }

            // 5. Leer la respuesta
            var gatewayResponse = await response.Content.ReadFromJsonAsync<PayLinkPaymentResponseDto>();

            // 6. Verificar el estado (como pediste)
            if (gatewayResponse?.Estado == "Confirmado")
            {
                // 7. Actualizar NUESTRA BD usando tu método existente
                _boletaService.MarcarComoPagada(boleta.Id, gatewayResponse.Fecha);
                return true;
            }

            // El pago fue recibido pero quedó "Pendiente" o "Rechazado"
            return false;
        }
    }
}