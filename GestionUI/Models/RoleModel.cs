using System.ComponentModel.DataAnnotations;

namespace GestionUI.Models
{
    public class RoleModel
    {
        public int id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio", AllowEmptyStrings = false)]
        [Display(Name = "Nombre de Rol:")]
        public string Name { get; set; }
        [Display(Name = "¿Puede aprobar TRD? (solo puede haber un rol que pueda aprobar TRD):")]
        public bool aprobarTRD { get; set; }
        [Display(Name = "¿Puede modificar TRD?:")]
        public bool modificarTRD { get; set; }
        [Display(Name = "¿Puede agregar y modificar roles?:")]
        public bool role_manager { get; set; }
        [Display(Name = "¿Puede agregar y modificar usuarios?:")]
        public bool user_manager { get; set; }
    }

    public class RoleManagerModel
    {
        public int idPrimario { get; set; }
        public int idSecundario { get; set; }
        public bool userManager { get; set; }
    }

    public class RolePage
    {
        public int RoleID { get; set; }
        public int PageID { get; set; }
    }

    public class Page
    {
        public int id { get; set; }
        public string controller { get; set; }
        public string action { get; set; }
    }
}