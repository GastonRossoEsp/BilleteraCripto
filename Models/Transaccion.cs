using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilleteraCriptoProg3.Models
{
    public class Transaccion
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CodigoCripto { get; set; }
        public string Metodo { get; set; }
        public int idCliente { get; set; }
        [ForeignKey("idCliente")]
        public Cliente? cliente { get; set; }
        public double CantCripto { get; set; }
        public double Dinero { get; set; } 
        public DateTime Datetime { get; set; }
    }
}
