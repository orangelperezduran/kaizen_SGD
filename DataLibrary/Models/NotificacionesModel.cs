using System;

namespace DataLibrary.Models
{
    public class NotificacionesModel
    {
        public int id { get; set; }
        public string userID { get; set; }
        public string notificacion { get; set; }
        public DateTime fecha { get; set; }
        public bool leido { get; set; }
        public string url { get; set; }
    }
    public class NotificacionEmail
    {
        public int id { get; set; }
        public DateTime fecha { get; set; }
    }
}
