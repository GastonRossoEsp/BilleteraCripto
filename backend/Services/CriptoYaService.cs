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
    }
}
