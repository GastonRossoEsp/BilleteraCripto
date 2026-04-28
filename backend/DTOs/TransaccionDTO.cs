namespace BilleteraCriptoProg3.DTOs
{
    public class TransaccionDTO
    {
        public string CodigoCripto { get; set; }
        public string Metodo { get; set; }
        public int ClienteId { get; set; }
        public double CantCripto { get; set; }
        public DateTime Datetime { get; set; }

    }
}
