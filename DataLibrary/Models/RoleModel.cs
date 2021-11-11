namespace DataLibrary.Models
{
    public class RoleModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool aprobarTRD { get; set; }
        public bool modificarTRD { get; set; }
        public bool role_manager { get; set; }
        public bool user_manager { get; set; }

    }

    public class Role_manager
    {
        public int primario { get; set; }
        public int secundario { get; set; }
        public bool agregarUsuario { get; set; }
    }

}
