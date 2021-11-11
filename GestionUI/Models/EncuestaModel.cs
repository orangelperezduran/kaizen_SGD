using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionUI.Models
{
    public class EncuestaModel
    {
        public string Entidad { get; set; }
        public string Oficina { get; set; }
        public int CodigOficina { get; set; }
        public string Encargado { get; set; }
        public string cargo { get; set; }
        public string Rol { get; set; }
        public string Proceso { get; set; }
        public string Serie { get; set; }
        public int CodigoSerie { get; set; }
        public string subserie { get; set; }
        public int CodigoSubserie { get; set; }
        public List<string> Tipologias { get; set; }
        public string tipologia { get; set; }
        public int Gestion { get; set; }
        public int Archivo { get; set; }
        public string Soporte { get; set; }
        public string Sistema { get; set; }
        public string Clasificacion { get; set; }
        public string DisposicionFinal { get; set; }
        [DataType(DataType.MultilineText)]
        public string Observaciones { get; set; }
        [DataType(DataType.MultilineText)]
        public string recomendaciones { get; set; }
        [DataType(DataType.MultilineText)]
        public string SeriesModificadas { get; set; }
        [DataType(DataType.MultilineText)]
        public string Funcion { get; set; }
        [DataType(DataType.MultilineText)]
        public string Procedimiento { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaInforme { get; set; }



    }
}