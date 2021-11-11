using System;

namespace DataLibrary.Models
{
    public class VersionModel
    {
        public int version { get; set; }
        public DateTime creacion { get; set; }
        public string usuario { get; set; }
        public DateTime aprobado { get; set; }
    }
    public class AprobacionModel
    {
        public int version { get; set; }
        public string usuario { get; set; }
        public bool aprueba { get; set; }
        public DateTime fecha_aprobacion { get; set; }
    }

    public class OficinaModel
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int codigo { get; set; }
        public int versionTRD { get; set; }
    }
    public class SerieModel
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int codigo { get; set; }
        public int versionTRD { get; set; }

    }
    public class SubserieModel
    {
        public int trd_id { get; set; }
        public int id { get; set; }
        public string nombre { get; set; }
        public int codigo { get; set; }

        public int gestion { get; set; }
        public int versionTRD { get; set; }
        public int archivo { get; set; }
        public string d_final { get; set; }
        public string observaciones { get; set; }
        public string identificacion { get; set; }
        public int id_organizacion { get; set; }

    }

    public class organizacionmodel
    {

        public int id { get; set; }

        public string nombre { get; set; }

    }



    public class TRDModel
    {
        public int id { get; set; }
        public int id_oficina { get; set; }
        public int id_serie { get; set; }
        public int id_subserie { get; set; }
        public int version { get; set; }
    }
    public class TipologiaModel
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int id_trd { get; set; }
    }

    public class MostrarTRDmodel
    {
        public int version { get; set; }
        public string NombreOficina { get; set; }
        public int CodigoOficina { get; set; }
        public string NombreSerie { get; set; }
        public int CodigoSerie { get; set; }
        public string NombreSubserie { get; set; }
        public int CodigoSubserie { get; set; }

        public string NombreTipologia { get; set; }

        public int TiempoGestion { get; set; }

        public int TiempoArchivo { get; set; }
        public string DisposicionFinal { get; set; }
        public string Observaciones { get; set; }
        public int id_organizacion { get; set; }
        public string Identificacion { get; set; }
    }

    public class rol_trdModel
    {
        public int role_id { get; set; }
        public int trd_id { get; set; }
    }

    public class GetTRDModel
    {
        public int id { get; set; }
        public string UserId { get; set; }
        public int version { get; set; }
        public int id_oficina { get; set; }
        public int id_serie { get; set; }
        public int id_subserie { get; set; }
        public int rol { get; set; }
        public string busqueda { get; set; }
    }
}
