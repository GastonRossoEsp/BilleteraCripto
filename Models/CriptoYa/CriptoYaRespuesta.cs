using System.Text.Json.Serialization;

namespace BilleteraCriptoProg3.Models.CriptoYa
{
    // Representa cada exchange (ripio, binance, lemon, etc.)
    public class CriptoExchange
    {
        public double ask { get; set; }
        public double totalAsk { get; set; }
        public double bid { get; set; }
        public double totalBid { get; set; }
    }

    // Representa la respuesta completa: un diccionario de exchanges
    public class CriptoYaRespuesta : Dictionary<string, CriptoExchange>
    {
    }
}
