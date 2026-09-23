using BilleteraCriptoProg3.Models.CriptoYa;
using System.Text.Json;
using System.Net.Http.Json;
using System.Linq;

namespace BilleteraCriptoProg3.Services
{
    public class CriptoYaService
    {
        private readonly IHttpClientFactory _factory;

        public CriptoYaService(IHttpClientFactory factory)
        {
            _factory = factory;
        }
        // este get queda para la logica de transacciones
        public async Task<double> GetPrecioAsync(string codigo)
        {
            var client = _factory.CreateClient();

            var markets = await client.GetFromJsonAsync<CriptoYaRespuesta>(
                $"https://criptoya.com/api/{codigo}/ars/1",
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            var market = markets?.Values.FirstOrDefault()
                ?? throw new Exception("No hay mercados disponibles para esta cripto.");

            return market.ask > 0 ? market.ask :
                   market.totalAsk > 0 ? market.totalAsk :
                   market.bid > 0 ? market.bid :
                   market.totalBid > 0 ? market.totalBid :
                   throw new Exception("No se pudo obtener un precio válido.");
        }
        // y este queda para darle valores a la cartera del cliente
        public async Task<double> GetPrecioActualAsync(string codigo)
        {
            var client = _factory.CreateClient();
            var markets = await client.GetFromJsonAsync<CriptoYaRespuesta>(
                $"https://criptoya.com/api/{codigo}/ars/1",
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
            if (markets == null || !markets.Any()) throw new Exception($"No se encontraron datos para la criptomoneda {codigo}.");

            var validBids = markets.Values
                .Where(x => x.bid > 0)
                .Select(x => x.bid)
                .ToList();

            if (!validBids.Any()) throw new Exception($"No se encontró un precio válido para {codigo}.");

            return validBids.Average();
        }
    }
}
