using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilleteraCriptoProg3.Entities
{
    public class Transaccion
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CodigoCripto { get; set; } = string.Empty;
        public string Metodo { get; set; } = string.Empty;
        public int ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }
        public double CantCripto { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Dinero { get; set; }
        public DateTime Datetime { get; set; }
    }
}
