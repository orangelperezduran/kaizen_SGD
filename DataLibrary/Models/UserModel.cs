namespace DataLibrary.Models
{
    public class UserModel
    {
        public string id { get; set; }
        public string email { get; set; }
        public string UserName { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public int identificacion { get; set; }
    }

    public class UserRolModel
    {
        public string UserId { get; set; }
        public int RoleId { get; set; }
    }
}
