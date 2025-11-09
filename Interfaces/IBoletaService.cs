using System;
using System.Threading.Tasks;
using API_Gobernanza_Digital.Models;
namespace API_Gobernanza_Digital.Interfaces;

public interface IBoletaService
{
    // CRUD
    IEnumerable<Boleta> GetAll();
    Boleta? GetById(int id);
    Boleta Create(Boleta boleta);
    Boleta? Update(int id, Boleta boleta);
    bool Delete(int id);

    // Métodos específicos
    Task<int> GenerarBoletasPeriodo(DateTime? fechaReferencia = null);
    bool MarcarComoPagada(int id, DateTime? fechaPago = null);
    int ActualizarBoletasVencidas();
    IEnumerable<Boleta> GetByContribuyente(int contribuyenteId);
    IEnumerable<Boleta> GetByEstado(EstadoBoleta estado);
    Boleta? GetByCodigoPago(string codigo);
}

//