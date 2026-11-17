namespace BilleteraCriptoProg3.DTOs
{
    public class EstadoCarteraDTO
    {
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public List<CarteraCriptoDTO> Criptomonedas { get; set; } = new();
        public decimal Total { get; set; }
        public decimal SaldoDisponible { get; set; }
        public decimal PatrimonioTotal { get; set; }
    }
}
