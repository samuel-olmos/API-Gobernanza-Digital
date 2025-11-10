namespace API_Gobernanza_Digital.Models.Dtos
{
    /// <summary>
    /// DTO que representa la respuesta que la pasarela PayLink
    /// espera recibir cuando nos consulta por una factura (boleta).
    /// </summary>
    public class PayLinkBillResponseDto
    {
        // ID de transacción random (como pediste)
        public string TransactionId { get; set; } = null!;

        // Nuestro CodigoPagoElectronico
        public int FacturaId { get; set; }

        // El monto de la boleta
        public decimal Monto { get; set; }
    }
}