using System.ComponentModel.DataAnnotations;

namespace GestionUI.Models
{
    public class TablaDeRetencion
    {
        public int version { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio", AllowEmptyStrings = false)]
        [Display(Name = "Nombre de oficina:")]
        public string NombreOficina { get; set; }
        [Required(ErrorMessage = "El código es obligatorio")]
        [Range(1, 999, ErrorMessage = "El rango es de 1 a 999")]
        [Display(Name = "Codigo oficina:")]
        public int CodigoOficina { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio", AllowEmptyStrings = false)]
        [Display(Name = "Nombre de serie:")]
        public string NombreSerie { get; set; }
        [Required(ErrorMessage = "El código es obligatorio")]
        [Range(1, 999, ErrorMessage = "El rango es de 1 a 999")]
        [Display(Name = "Codigo de serie:")]
        public int CodigoSerie { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre de subserie:")]
        public string NombreSubserie { get; set; }
        [Required(ErrorMessage = "El código es obligatorio")]
        [Range(1, 999, ErrorMessage = "El rango es de 1 a 999")]
        [Display(Name = "Codigo de subserie:")]
        public int CodigoSubserie { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre de tipología:")]
        public string NombreTipologia { get; set; }
        [Required(ErrorMessage = "Digite los anos en gestión")]
        [Range(1, 10, ErrorMessage = "El rango es de 1 a 10")]
        [Display(Name = "Tiempo en gestión:")]
        public int TiempoGestion { get; set; }
        [Required(ErrorMessage = "Digite los anos en archivo")]
        [Range(1, 100, ErrorMessage = "El rango es de 1 a 100")]
        [Display(Name = "Tiempo en archivo:")]
        public int TiempoArchivo { get; set; }

        [Display(Name = "Disposición final:")]
        public string DisposicionFinal { get; set; }
        [Display(Name = "Observaciones:")]
        [DataType(DataType.MultilineText)]
        public string Observaciones { get; set; }

        [Display(Name = "Tipo de identificacion requerida:")]
        public string Identificacion { get; set; }
        [Display(Name = "Escoja tipo de Organización:")]
        public int Organizacion { get; set; }
        public string Organizacion_text { get; set; }


    }

}