using System.ComponentModel.DataAnnotations;

namespace BilleteraCriptoProg3.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
