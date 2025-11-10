using System.Threading.Tasks;

namespace API_Gobernanza_Digital.Interfaces
{
    /// <summary>
    /// Define la lógica de negocio para interactuar con la
    /// pasarela de pagos externa (PayLink).
    /// </summary>
    public interface IPagoService
    {
        /// <summary>
        /// Inicia un pago en la pasarela externa (POST /api/payments).
        /// </summary>
        /// <param name="codigoPagoElectronico">Nuestro CodigoPagoElectronico (su facturaId)</param>
        /// <returns>True si el pago fue "Confirmado", False si fue rechazado.</returns>
        Task<bool> PagarBoletaAsync(string codigoPagoElectronico);
    }
}