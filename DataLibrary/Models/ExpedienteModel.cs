using System;

namespace DataLibrary.Models
{
    public class ExpedienteModel
    {
        public int id { get; set; }
        public int id_trd { get; set; }
        public int? ano { get; set; }
        public int? consecutivo_radicado { get; set; }
        public DateTime? f_creacion { get; set; }
        public DateTime? f_cierre { get; set; }
        public string usuario_creacion { get; set; }
        public string usuario_cierre { get; set; }
        public string observaciones { get; set; }
        public long? identificacion { get; set; }
        public int folios { get; set; }
        public bool central { get; set; }
        
    }

    public class radicadoModel
    {
        public int consecutivo { get; set; }
        public int ano { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
        public long? identificacion { get; set; }
        public DateTime fecha { get; set; }
        public bool entrada { get; set; }
        public bool interno { get; set; }
        public DateTime? f_limite { get; set; }
        public string numero_radicado { get; set; }
        public string observaciones { get; set; }
        public bool respondido { get; set; }

    }

    public class documentoModel
    {
        public int id { get; set; }
        public int id_tipologia { get; set; }
        public int ano { get; set; }
        public int? cons_radicado { get; set; }
        public string respuesta { get; set; }
        public string observaciones { get; set; }
        public int folios { get; set; }
        public int id_expediente { get; set; }
        public string direccion { get; set; }
        public DateTime f_subida { get; set; }
        public string usuarioID { get; set; }
        public string tipologia { get; set; }
    }
}
