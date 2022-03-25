using DataLibrary.DataAccess;
using DataLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace DataLibrary.BusinessLogic
{
    public class TRDLogic
    {
        public static void AsignarTRDtoAdmin(int version)
        {
            VersionModel data = new VersionModel() { version = version };
            string sql = @"select id from trd where version=@version;";
            List<int> trdIds = MysqlDataAccess.LoadData<int>(sql, data);
            rol_trdModel temp = new rol_trdModel() { role_id = 1 };
            foreach (var id in trdIds)
            {
                temp.trd_id = id;
                sql = "insert ignore into role_trd set role_id=@role_id,trd_id=@trd_id";
                MysqlDataAccess.SaveData(sql, temp);
            }
        }

        public static void EliminarRol_trd(int rol_id, string UserId, int version)
        {
            rol_trdModel data = new rol_trdModel
            {
                role_id = rol_id
            };
            var Ids = CargarRol_TRD(version);
            foreach (var id in Ids)
            {
                if (AuthorizedTRDid(UserId, id.id))
                    data.trd_id = id.id;
                {
                    string sql = @"Delete from role_trd where role_id=@role_id and trd_id=@trd_id";
                    MysqlDataAccess.SaveData(sql, data);
                }
            }
        }

        public static List<TRDModel> CargarRol_TRD(int version)
        {
            string sql = @"select id,id_oficina,id_serie,id_subserie from trd where version='" + version + "';";
            return MysqlDataAccess.LoadData<TRDModel>(sql);
        }
        public static void Insertrol_trd(int rol_id, int trd_id)
        {
            rol_trdModel data = new rol_trdModel { role_id = rol_id, trd_id = trd_id };
            string sql = @"insert ignore into role_trd set role_id=@role_id,trd_id=@trd_id;";
            MysqlDataAccess.SaveData(sql, data);
        }
        public static bool IsSubserieCheck(int rol, int id_serie, int id_subserie, int id_oficina)
        {
            GetTRDModel data = new GetTRDModel { id_oficina = id_oficina, id_serie = id_serie, id_subserie = id_subserie, rol = rol };
            string sql = @"select trd.id from trd,role_trd where trd_id=trd.id and role_id=@rol and id_serie=@id_serie and id_subserie=@id_subserie and id_oficina=@id_oficina";
            if (MysqlDataAccess.LoadData<int>(sql, data).Count > 0)
                return true;
            else
                return false;
        }
        public static List<SelectListItem> GetOficinas(string User_Id, int version, int oficina = 0, bool central = false)
        {
            GetTRDModel data = new GetTRDModel() { UserId = User_Id, version = version };
            string sql;
            if (central)
            {
                sql = @"select oficina.id as value,oficina.nombre as text from oficina,trd
where trd.version=@version and oficina.id=trd.id_oficina group by value;";
                if (oficina != 0)
                {
                    data.id_oficina = oficina;
                    sql = @"select oficina.id as value,oficina.nombre as text from oficina,trd 
where trd.version=@version and oficina.id=trd.id_oficina and oficina.id =@id_oficina group by value;";
                }
            }
            else
            {
                sql = @"select oficina.id as value,oficina.nombre as text from oficina,trd,aspnetroles,role_trd,aspnetuserroles 
where UserId=@UserId and RoleId=aspnetroles.Id and aspnetroles.id=role_trd.role_id and role_trd.trd_id=trd.id and trd.version=@version and oficina.id=trd.id_oficina group by value;";
                if (oficina != 0)
                {
                    data.id_oficina = oficina;
                    sql = @"select oficina.id as value,oficina.nombre as text from oficina,trd,aspnetroles,role_trd,aspnetuserroles 
where UserId=@UserId and RoleId=aspnetroles.Id and aspnetroles.id=role_trd.role_id and 
role_trd.trd_id=trd.id and trd.version=@version and oficina.id=trd.id_oficina and oficina.id =@id_oficina group by value;";
                }
            }


            return MysqlDataAccess.LoadData<SelectListItem>(sql, data);
        }

        public static List<SelectListItem> GetSeries(string User_Id, int version, int id_oficina, int serie = 0, bool central = false)
        {
            GetTRDModel data = new GetTRDModel() { UserId = User_Id, version = version, id_oficina = id_oficina };
            string sql;
            if (central)
            {
                sql = @"select serie.id as value,serie.nombre as text from serie,trd
where trd.version=@version and trd.id_oficina=@id_oficina and serie.id=trd.id_serie group by value;";

                if (serie != 0)
                {
                    data.id_serie = serie;
                    sql = @"select serie.id as value,serie.nombre as text from serie,trd
where  trd.version=@version and trd.id_oficina=@id_oficina and serie.id=trd.id_serie and serie.id=@id_serie group by value;";
                }
            }
            else
            {
                sql = @"select serie.id as value,serie.nombre as text from serie,trd,aspnetroles,role_trd,aspnetuserroles
where UserId=@UserId and RoleId=aspnetroles.Id and aspnetroles.id=role_trd.role_id and role_trd.trd_id=trd.id and trd.version=@version 
and trd.id_oficina=@id_oficina and serie.id=trd.id_serie group by value;";

                if (serie != 0)
                {
                    data.id_serie = serie;
                    sql = @"select serie.id as value,serie.nombre as text from serie,trd,aspnetroles,role_trd,aspnetuserroles
where UserId=@UserId and RoleId=aspnetroles.Id and aspnetroles.id=role_trd.role_id and role_trd.trd_id=trd.id and trd.version=@version 
and trd.id_oficina=@id_oficina and serie.id=trd.id_serie and serie.id=@id_serie group by value;";
                }
            }

            return MysqlDataAccess.LoadData<SelectListItem>(sql, data);
        }

        public static List<SubserieModel> GetSubseries(string User_Id, int version, int id_oficina, int id_serie, int subserie = 0, string busqueda = null, bool central = false)
        {
            List<SubserieModel> resultado = new List<SubserieModel>();
            GetTRDModel data = new GetTRDModel() { UserId = User_Id, version = version, id_oficina = id_oficina, id_serie = id_serie };
            string sql;
            if (central)
            {
                sql = @"select trd.id as trd_id,subserie.id as id,subserie.nombre,subserie.id_organizacion,subserie.identificacion from subserie,trd 
where trd.version=@version and trd.id_oficina=@id_oficina and trd.id_serie=@id_serie and subserie.id=trd.id_subserie group by id;";

                if (subserie != 0)
                {
                    data.id_subserie = subserie;
                    sql = @"select trd.id as trd_id,subserie.id as id,subserie.nombre,subserie.id_organizacion,subserie.identificacion from subserie,trd 
where trd.version=@version and trd.id_oficina=@id_oficina and trd.id_serie=@id_serie and subserie.id=trd.id_subserie and subserie.id=@id_subserie group by id;";
                }
                else
                {
                    if (!string.IsNullOrEmpty(busqueda) && busqueda.Length > 3)
                    {
                        data.busqueda = busqueda;
                        sql = @"select trd.id as trd_id,subserie.id as id,subserie.nombre,subserie.id_organizacion,subserie.identificacion 
from trd,(select * from subserie where match(nombre,identificacion) against(concat('*',@busqueda,'*') in boolean mode)) as subserie
where trd.version=@version and trd.id_oficina=@id_oficina and trd.id_serie=@id_serie and subserie.id=trd.id_subserie group by id;";
                    }
                }
            }
            else
            {
                sql = @"select trd.id as trd_id,subserie.id as id,subserie.nombre,subserie.id_organizacion,subserie.identificacion from subserie,trd,aspnetroles,role_trd,aspnetuserroles
where UserId=@UserId and RoleId=aspnetroles.Id and aspnetroles.id=role_trd.role_id and role_trd.trd_id=trd.id and trd.version=@version 
and trd.id_oficina=@id_oficina and trd.id_serie=@id_serie and subserie.id=trd.id_subserie group by id;";

                if (subserie != 0)
                {
                    data.id_subserie = subserie;
                    sql = @"select trd.id as trd_id,subserie.id as id,subserie.nombre,subserie.id_organizacion,subserie.identificacion from subserie,trd,aspnetroles,role_trd,aspnetuserroles
where UserId=@UserId and RoleId=aspnetroles.Id and aspnetroles.id=role_trd.role_id and role_trd.trd_id=trd.id and trd.version=@version 
and trd.id_oficina=@id_oficina and trd.id_serie=@id_serie and subserie.id=trd.id_subserie and subserie.id=@id_subserie group by id;";
                }
                else
                {
                    if (!string.IsNullOrEmpty(busqueda) && busqueda.Length > 3)
                    {
                        data.busqueda = busqueda;
                        sql = @"select trd.id as trd_id,subserie.id as id,subserie.nombre,subserie.id_organizacion,subserie.identificacion 
from trd,aspnetroles,role_trd,aspnetuserroles,(select * from subserie where match(nombre,identificacion) against(concat('*',@busqueda,'*') in boolean mode)) as subserie
where UserId=@UserId and RoleId=aspnetroles.Id and aspnetroles.id=role_trd.role_id and role_trd.trd_id=trd.id and trd.version=@version 
and trd.id_oficina=@id_oficina and trd.id_serie=@id_serie and subserie.id=trd.id_subserie group by id;";
                    }
                }
            }

            resultado = MysqlDataAccess.LoadData<SubserieModel>(sql, data);


            return resultado;
        }

        public static List<SubserieModel> GetSubserie(int subserie)
        {
            GetTRDModel data = new GetTRDModel() { id_subserie = subserie };
            string sql = @"select * from subserie where id=@id_subserie";
            return MysqlDataAccess.LoadData<SubserieModel>(sql, data);
        }
        public static List<OficinaModel> GetOficina(int oficina)
        {
            GetTRDModel data = new GetTRDModel() { id_oficina = oficina };
            string sql = @"select * from oficina where id=@id_oficina";
            return MysqlDataAccess.LoadData<OficinaModel>(sql, data);
        }
        public static List<SerieModel> GetSerie(int serie)
        {
            GetTRDModel data = new GetTRDModel() { id_serie = serie };
            string sql = @"select * from serie where id=@id_serie";
            return MysqlDataAccess.LoadData<SerieModel>(sql, data);
        }

        public static List<TipologiaModel> GetTipologia(int tipologia)
        {
            TipologiaModel data = new TipologiaModel() { id = tipologia };
            string sql = @"select * from tipologia where id=@id";
            return MysqlDataAccess.LoadData<TipologiaModel>(sql, data);
        }

        public static List<TipologiaModel> GetTipologias(string User_Id, int version, int id_oficina, int id_serie, int id_subserie)
        {
            GetTRDModel data = new GetTRDModel() { UserId = User_Id, version = version, id_oficina = id_oficina, id_serie = id_serie, id_subserie = id_subserie };
            string sql = @"select tipologia.id as id,tipologia.nombre from tipologia,trd,aspnetroles,role_trd,aspnetuserroles 
where UserId=@UserId and RoleId=aspnetroles.Id and aspnetroles.id=role_trd.role_id and role_trd.trd_id=trd.id and trd.version=@version 
and trd.id_oficina=@id_oficina and trd.id_serie=@id_serie and trd.id_subserie=@id_subserie and trd.id=tipologia.id_trd group by id;";
            return MysqlDataAccess.LoadData<TipologiaModel>(sql, data);
        }


        public static List<SelectListItem> GetOrganizacion()
        {
            string sql = @"select id as value,nombre as text from organizacion";
            return MysqlDataAccess.LoadData<SelectListItem>(sql);
        }

        public static string GetOrganizaciontext(int id)
        {
            organizacionmodel data = new organizacionmodel() { id = id };
            string sql = @"select nombre from organizacion where id=@id";
            return MysqlDataAccess.LoadData<string>(sql, data)[0];
        }


        public static bool AuthorizedTRDid(string User_Id, int id_trd)
        {
            GetTRDModel data = new GetTRDModel() { UserId = User_Id, id = id_trd };
            string sql = @"select role_id as id from aspnetroles,role_trd,aspnetuserroles 
where UserId=@UserId and RoleId=aspnetroles.Id and aspnetroles.id=role_trd.role_id and role_trd.trd_id=@id;";
            if (MysqlDataAccess.LoadData<int>(sql, data).Count > 0)
            {
                return true;
            }
            else
                return false;
        }


        public static void insertOficina(string nombre, int version, int codigo)
        {
            OficinaModel data = new OficinaModel { codigo = codigo, nombre = nombre, versionTRD = version };
            string sql = @"insert ignore into oficina set nombre=@nombre,codigo=@codigo,version=@versionTRD";
            MysqlDataAccess.SaveData(sql, data);

        }
        public static void insertSerie(string nombre, int version, int codigo)
        {
            SerieModel data = new SerieModel { codigo = codigo, nombre = nombre, versionTRD = version };
            string sql = @"insert ignore into serie set nombre=@nombre,codigo=@codigo,versiontrd=@versionTRD";
            MysqlDataAccess.SaveData(sql, data);

        }
        public static void insertSubserie(string nombre, int version, int codigo, int archivo, string d_final, int gestion, string observaciones, int organizacion)
        {
            SubserieModel data = new SubserieModel
            {
                codigo = codigo,
                nombre = nombre,
                versionTRD = version,
                archivo = archivo,
                d_final = d_final,
                gestion = gestion,
                observaciones = observaciones,
                id_organizacion = organizacion
            };
            string sql = @"insert ignore into subserie set nombre=@nombre,codigo=@codigo,versiontrd=@versionTRD,
archivo=@archivo,d_final=@d_final,gestion=@gestion,observaciones=@observaciones,id_organizacion=@id_organizacion";
            MysqlDataAccess.SaveData(sql, data);

        }

        public static void insertSubserie(string nombre, int version, int codigo, int archivo, string d_final, int gestion, string observaciones, int organizacion, string identificacion)
        {
            SubserieModel data = new SubserieModel
            {
                codigo = codigo,
                nombre = nombre,
                versionTRD = version,
                archivo = archivo,
                d_final = d_final,
                gestion = gestion,
                observaciones = observaciones,
                id_organizacion = organizacion,
                identificacion = identificacion
            };
            string sql = @"insert ignore into subserie set nombre=@nombre,codigo=@codigo,versiontrd=@versionTRD,
archivo=@archivo,d_final=@d_final,gestion=@gestion,observaciones=@observaciones,id_organizacion=@id_organizacion,identificacion=@identificacion";
            MysqlDataAccess.SaveData(sql, data);

        }

        public static void CrearTRDyTipologias(int cod_oficina, int cod_serie, int cod_subserie, string tipologia, int version)
        {
            TRDModel data = new TRDModel { id_oficina = cod_oficina, id_serie = cod_serie, id_subserie = cod_subserie, version = version };
            string sql = @"select id from oficina where codigo=@id_oficina and version=@version;";
            data.id_oficina = MysqlDataAccess.LoadData<int>(sql, data)[0];
            sql = @"select id from serie where codigo=@id_serie and versiontrd=@version;";
            data.id_serie = MysqlDataAccess.LoadData<int>(sql, data)[0];
            sql = @"select id from subserie where codigo=@id_subserie and versiontrd=@version;";
            data.id_subserie = MysqlDataAccess.LoadData<int>(sql, data)[0];
            sql = @"insert ignore into trd set id_oficina=@id_oficina, id_serie=@id_serie,id_subserie=@id_subserie,version=@version";
            MysqlDataAccess.SaveData(sql, data);
            sql = @"select id from trd where id_oficina=@id_oficina and id_serie=@id_serie and id_subserie=@id_subserie and version=@version";
            data.id = MysqlDataAccess.LoadData<int>(sql, data)[0];
            TipologiaModel tip = new TipologiaModel { id_trd = data.id, nombre = tipologia };
            sql = @"insert ignore into tipologia set id_trd=@id_trd,nombre=@nombre";
            MysqlDataAccess.SaveData(sql, tip);
            VersionModel ver = new VersionModel { aprobado = DateTime.Now, version = version };
            sql = @"update versiontrd set aprobado=@aprobado where version=@version";
            MysqlDataAccess.SaveData(sql, ver);
            string projectPath = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(projectPath + "/TRD/" + version))
                Directory.CreateDirectory(projectPath + "/TRD/" + version);
            if (!Directory.Exists(projectPath + "/TRD/" + version + "/" + cod_oficina))
                Directory.CreateDirectory(projectPath + "/TRD/" + version + "/" + cod_oficina);
            if (!Directory.Exists(projectPath + "/TRD/" + version + "/" + cod_oficina + "/" + cod_serie))
                Directory.CreateDirectory(projectPath + "/TRD/" + version + "/" + cod_oficina + "/" + cod_serie);
            if (!Directory.Exists(projectPath + "/TRD/" + version + "/" + cod_oficina + "/" + cod_serie))
                Directory.CreateDirectory(projectPath + "/TRD/" + version + "/" + cod_oficina + "/" + cod_serie + "/" + cod_subserie);

        }
        public static int GetVersion()
        {
            string sql = @"select version from versiontrd order by version desc limit 1";
            List<int> cod = MysqlDataAccess.LoadData<int>(sql);
            if (cod.Count == 0)
                return 0;
            else
                return cod[0];
        }

        public static List<int> GetAllVersiones()
        {
            string sql = @"select version from versiontrd order by version desc;";
            return MysqlDataAccess.LoadData<int>(sql);
        }

        public static bool IsModificacion(int id)
        {
            VersionModel data = new VersionModel()
            {
                version = id
            };
            string sql = @"select usuario from aprobacion_trd where version=@version and aprueba=false";
            List<AprobacionModel> apr = MysqlDataAccess.LoadData<AprobacionModel>(sql, data);
            if (apr.Count == 0)
            {
                sql = @"select usuario from aprobacion_trd where version=@version";
                apr = MysqlDataAccess.LoadData<AprobacionModel>(sql, data);
                if (apr.Count != 0)
                    return false;
                else
                    return true;
            }
            else
                return true;

        }

        public static bool PendienteDeAprobacion(string UserId, int version)
        {
            AprobacionModel data = new AprobacionModel { usuario = UserId, version = version };
            string sql = @"select aprueba from aprobacion_trd where usuario=@usuario and version=@version";
            List<AprobacionModel> result = MysqlDataAccess.LoadData<AprobacionModel>(sql, data);
            if (result.Count != 0)
            {
                if (result[0].aprueba)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }
        public static int CrearVersion(string userID)
        {

            VersionModel data = new VersionModel()
            {
                usuario = userID,
                creacion = DateTime.Now
            };
            string sql = @"insert into versiontrd(fecha_creacion,usuario_modificador) values (@creacion, @usuario)";

            return MysqlDataAccess.SaveData(sql, data);
        }
        public static int ModificarVersion(int version, string userID)
        {

            VersionModel data = new VersionModel()
            {
                version = version,
                usuario = userID,
                creacion = DateTime.Now
            };
            string sql = @"update versiontrd set fecha_creacion=@creacion,usuario_modificador=@usuario where version=@version";

            return MysqlDataAccess.SaveData(sql, data);
        }

        public static int CrearAprobacion(int version, string userID)
        {

            AprobacionModel data = new AprobacionModel()
            {
                version = version,
                usuario = userID,
            };
            string sql = @"insert into aprobacion_trd(version,usuario) values (@version, @usuario)";

            return MysqlDataAccess.SaveData(sql, data);
        }

        public static bool AprobarTRD(string UserId, int version)
        {
            AprobacionModel data = new AprobacionModel { aprueba = true, fecha_aprobacion = DateTime.Now, usuario = UserId, version = version };
            string sql = @"update aprobacion_trd set aprueba=@aprueba,fecha_aprobacion=@fecha_aprobacion where version=@version and usuario=@usuario";
            MysqlDataAccess.SaveData(sql, data);
            sql = @"select usuario from aprobacion_trd where version=@version and aprueba=false";
            List<AprobacionModel> apr = MysqlDataAccess.LoadData<AprobacionModel>(sql, data);
            if (apr.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int EliminarAprobacion(int version, string userID)
        {

            AprobacionModel data = new AprobacionModel()
            {
                version = version,
                usuario = userID,
            };
            string sql = @"Delete from aprobacion_trd where version=@version and usuario=@usuario;";

            return MysqlDataAccess.SaveData(sql, data);
        }

        public static int EliminarAprobacion(int version)
        {

            AprobacionModel data = new AprobacionModel()
            {
                version = version
            };
            string sql = @"delete from aprobacion_trd where version=@version";

            return MysqlDataAccess.SaveData(sql, data);
        }


        public static List<MostrarTRDmodel> MostrarTRD(int version)
        {
            MostrarTRDmodel data = new MostrarTRDmodel { version = version };
            string sql = @"select oficina.nombre as NombreOficina,oficina.codigo as CodigoOficina,
serie.nombre as NombreSerie,serie.codigo as CodigoSerie, subserie.nombre as NombreSubserie, subserie.codigo as CodigoSubserie,
tipologia.nombre as NombreTipologia, subserie.gestion as TiempoGestion, subserie.archivo as TiempoArchivo, d_final as DisposicionFinal, observaciones,
id_organizacion from oficina,serie,subserie,trd,tipologia where id_oficina=oficina.id and id_serie=serie.id and id_subserie=subserie.id and tipologia.id_trd=trd.id and trd.version=@version;";
            return MysqlDataAccess.LoadData<MostrarTRDmodel>(sql, data);
        }

        public static int GetTrdID(int serie, int oficina, int subserie)
        {
            GetTRDModel data = new GetTRDModel() { id_subserie = subserie, id_serie = serie, id_oficina = oficina };
            string sql = @"select id from trd where id_subserie=@id_subserie and id_serie=@id_serie and id_oficina=@id_oficina";
            return MysqlDataAccess.LoadData<int>(sql, data)[0];
        }

        public static int GetOrganizacion(int trd_id)
        {
            GetTRDModel data = new GetTRDModel() { id = trd_id };
            string sql = @"select organizacion.id from organizacion,subserie,trd where trd.id=@id and id_subserie=subserie.id and id_organizacion=organizacion.id";
            return MysqlDataAccess.LoadData<int>(sql, data)[0];
        }



    }
}
