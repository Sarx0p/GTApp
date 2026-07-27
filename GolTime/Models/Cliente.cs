using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GolTime.Models
{
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("Nombre", TypeName = "varchar(80)")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(80, ErrorMessage = "El nombre debe tener entre 3 y 80 caracteres.", MinimumLength = 3)]
        public string? Nombre { get; set; }

        [Column("Numero", TypeName = "varchar(15)")]
        [Required(ErrorMessage = "El número es obligatorio.")]
        [Phone(ErrorMessage = "Ingrese un número válido.")]
        [StringLength(15)]
        public string? Numero { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<Reservacion> Reservaciones { get; set; } = new List<Reservacion>();
    }
}
