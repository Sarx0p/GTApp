using System.ComponentModel.DataAnnotations;

namespace GolTime.Models
{
    public class InfoLogin
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        public string? Login { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string? Password { get; set; }
    }
}
