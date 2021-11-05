using System.ComponentModel.DataAnnotations;

namespace LoginUsuario.Models.Dto
{
    public class UsuarioLoginDto
    {
        [Required(ErrorMessage = "El Usuario es Obligatorio")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "El Password es Obligatorio")]
        public string Password { get; set; }
    }
}
