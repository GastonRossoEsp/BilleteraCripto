namespace BilleteraCriptoProg3.DTOs
{
    public class UpdateTransaccionDto
    {
        public string CodigoCripto { get; set; }
        public string Metodo { get; set; }
        public double CantCripto { get; set; }
        public DateTime Datetime { get; set; }
    }
}
