using GolTime.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GolTime.Models
{
    public class Reservacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        public DateOnly Fecha { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria.")]
        public TimeOnly HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de finalización es obligatoria.")]
        public TimeOnly HoraFin { get; set; }

        [Required(ErrorMessage = "El estado de la reservación es obligatorio.")]
        public EstadoReservaEnum EstadoReserva { get; set; }

        [Required(ErrorMessage = "El estado del pago es obligatorio.")]
        public EstadoPagoEnum EstadoPago { get; set; }

        public int UserId { get; set; }

        public int ClientId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        [ForeignKey("ClientId")]
        public virtual Cliente? Client { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime UpdatedAt { get; set; }
    }
}