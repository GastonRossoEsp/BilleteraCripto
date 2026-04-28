using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilleteraCriptoProg3.Entities
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo { get; set; } = 0;
        public List<Transaccion> Transacciones { get; set; }
    }
}
