using System.ComponentModel.DataAnnotations;

namespace GestionUI.Models
{
    public class UserModel
    {
        public string id { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required(ErrorMessage = "El userName es obligatorio")]
        [StringLength(60, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 4)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string nombre { get; set; }
        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string apellido { get; set; }
        [Required(ErrorMessage = "La identificacion es obligatoria")]
        public int identificacion { get; set; }
    }
}
