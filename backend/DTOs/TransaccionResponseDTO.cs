namespace BilleteraCriptoProg3.DTOs
{
    public class TransaccionResponseDTO
    {
        public int Id { get; set; }
        public string CodigoCripto { get; set; }
        public string Metodo { get; set; }
        public double CantCripto { get; set; }
        public decimal Dinero { get; set; }
        public DateTime Datetime { get; set; }
        public string ClienteNombre { get; set; }
    }
}
