using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace GestionUI.Models
{
    public class ExpedienteModel
    {
        public int id { get; set; }
        [Display(Name = "Oficina:")]
        [Required(ErrorMessage = "La oficina es obligatoria")]
        public int oficina { get; set; }
        [Display(Name = "Serie:")]
        [Required(ErrorMessage = "La serie es obligatoria")]
        public int serie { get; set; }
        [Display(Name = "Subserie:")]
        [Required(ErrorMessage = "La subserie es obligatoria")]
        public int subserie { get; set; }
        [Display(Name = "Tipologia:")]
        [Required(ErrorMessage = "La tipologia es obligatoria")]
        public int id_tipologia { get; set; }

        public string busqueda { get; set; }

        public int identificacion { get; set; }
        public int ano { get; set; }
        [Display(Name = "Fecha inicial")]
        [DataType(DataType.Date)]
        public DateTime desde { get; set; }
        [Display(Name = "Fecha final")]
        [DataType(DataType.Date)]
        public DateTime hasta { get; set; }
    }

    public class radicadoModel
    {
        public int consecutivo { get; set; }
        public int ano { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
        public int identificacion { get; set; }
        public DateTime fecha { get; set; }
        public bool entrada { get; set; }
        public bool interno { get; set; }
        public string observaciones { get; set; }
        public string busqueda { get; set; }

    }

    public class radicarModel
    {
        [Display(Name = "Ingrese radicado al cual se le responde:")]
        public string respuesta_a { get; set; }
        public long? identificacion { get; set; }
        public int id { get; set; }
        [Display(Name = "Oficina:")]
        [Required(ErrorMessage = "La oficina es obligatoria")]
        public int oficina { get; set; }
        [Display(Name = "Serie:")]
        [Required(ErrorMessage = "La serie es obligatoria")]
        public int serie { get; set; }
        [Display(Name = "Subserie:")]
        [Required(ErrorMessage = "La subserie es obligatoria")]
        public int subserie { get; set; }
        [Display(Name = "Tipologia:")]
        [Required(ErrorMessage = "La tipologia es obligatoria")]
        public int id_tipologia { get; set; }
        [Display(Name = "Remitente:")]
        [Required(ErrorMessage = "El remitente es obligatorio")]
        public string remitente { get; set; }
        [Display(Name = "Identificación del remitente:")]
        [Required(ErrorMessage = "La identificación es obligatoria")]
        public int id_remitente { get; set; }
        [Display(Name = "Año:")]        
        public int ano { get; set; }
        [Display(Name = "Número de folios:")]
        [Required(ErrorMessage = "El número de folios es obligatorio")]
        [Range(1, 1000, ErrorMessage = "Por favor ponga un número válido")]
        public int folios { get; set; }


        public DateTime fecha { get; set; }
        [Display(Name = "¿Es documento de entrada?:")]
        public bool entrada { get; set; }
        [Display(Name = "¿Es documento interno?:")]
        public bool interno { get; set; }



        [Display(Name = "Documento:")]
        [Required(ErrorMessage = "El documento es obligatorio")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase file { get; set; }
        [Display(Name = "Observaciones:")]
        [DataType(DataType.MultilineText)]
        public string observaciones { get; set; }
        [Display(Name = "Correo electrónico del remitente")]
        [EmailAddress]
        public string email_remitente { get; set; }

        [Required]
        [Display(Name = "Fecha límite de respuesta:")]
        [DataType(DataType.Date)]
        public DateTime f_limite { get; set; }
    }

    public class VirtualizarModel
    {
        public int? identificacion { get; set; }
        public int id { get; set; }
        [Display(Name = "Oficina:")]
        [Required(ErrorMessage = "La oficina es obligatoria")]
        public int oficina { get; set; }
        [Display(Name = "Serie:")]
        [Required(ErrorMessage = "La serie es obligatoria")]
        public int serie { get; set; }
        [Display(Name = "Subserie:")]
        [Required(ErrorMessage = "La subserie es obligatoria")]
        public int subserie { get; set; }
        [Display(Name = "Tipologia:")]
        [Required(ErrorMessage = "La tipologia es obligatoria")]
        public int id_tipologia { get; set; }

        [Display(Name = "Año:")]
        public int ano { get; set; }
        [Display(Name = "Número de folios:")]
        [Required(ErrorMessage = "El número de folios es obligatorio")]
        [Range(1, 200, ErrorMessage = "Por favor ponga un número válido")]
        public int folios { get; set; }

        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime fecha { get; set; }

        [Display(Name = "¿Es documento interno?:")]
        public bool interno { get; set; }



        [Display(Name = "Documento:")]
        [Required(ErrorMessage = "El documento es obligatorio")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase file { get; set; }
        [Display(Name = "Observaciones:")]
        [DataType(DataType.MultilineText)]
        public string observaciones { get; set; }
        [Display(Name = "Observaciones para expedientes (solo válido si se abre expediente con este documento):")]
        [DataType(DataType.MultilineText)]
        public string observacionesexp { get; set; }

    }
}