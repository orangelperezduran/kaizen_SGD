using DataLibrary.BusinessLogic;
using DataLibrary.DataAccess;
using DataLibrary.Models;
using System;
using System.Collections.Generic;

namespace DataLibrary.BusinessLogic
{
    public class RadicacionLogic
    {
        
        public static bool ExpedienteExist(int ano, int id_trd, int? consecutivo)
        {
            ExpedienteModel data = new ExpedienteModel() { ano = ano, id_trd = id_trd, consecutivo_radicado = consecutivo };
            string sql = @"select * from expediente where id_trd=@id_trd and ano=@ano and consecutivo_radicado=@consecutivo_radicado";
            if (MysqlDataAccess.LoadData<ExpedienteModel>(sql, data).Count > 0)
                return true;
            else
                return false;
        }

        public static bool ExpedienteExist(int id_trd, long? id)
        {
            ExpedienteModel data = new ExpedienteModel() { identificacion = id, id_trd = id_trd };
            string sql = @"select * from expediente where id_trd=@id_trd and identificacion=@identificacion";
            if (MysqlDataAccess.LoadData<ExpedienteModel>(sql, data).Count > 0)
                return true;
            else
                return false;
        }

        public static bool ExpedienteExistAño(int id_trd, int ano)
        {
            ExpedienteModel data = new ExpedienteModel() { id_trd = id_trd, ano = ano };
            string sql = @"select * from expediente where id_trd=@id_trd and ano=@ano and identificacion is null and consecutivo_radicado is null";
            if (MysqlDataAccess.LoadData<ExpedienteModel>(sql, data).Count > 0)
                return true;
            else
                return false;
        }

        public static bool ExpedienteExistAñoID(int ano, int id_trd, long? id)
        {
            ExpedienteModel data = new ExpedienteModel() { ano = ano, id_trd = id_trd, identificacion = id };
            string sql = @"select * from expediente where id_trd=@id_trd and ano=@ano and identificacion=@identificacion";
            if (MysqlDataAccess.LoadData<ExpedienteModel>(sql, data).Count > 0)
                return true;
            else
                return false;
        }
        public static ExpedienteModel GetExpediente(int ano, int id_trd, int consecutivo)
        {
            ExpedienteModel data = new ExpedienteModel() { ano = ano, id_trd = id_trd, consecutivo_radicado = consecutivo };
            string sql = @"select * from expediente where id_trd=@id_trd and ano=@ano and consecutivo_radicado=@consecutivo_radicado";
            return MysqlDataAccess.LoadData<ExpedienteModel>(sql, data)[0];
        }
        public static ExpedienteModel GetExpedienteAñoID(int ano, int id_trd, long? id)
        {
            ExpedienteModel data = new ExpedienteModel() { ano = ano, id_trd = id_trd, identificacion = id };
            string sql = @"select * from expediente where id_trd=@id_trd and ano=@ano and identificacion=@identificacion";
            return MysqlDataAccess.LoadData<ExpedienteModel>(sql, data)[0];
        }
        public static ExpedienteModel GetExpediente(int id_trd, long? identificacion)
        {
            ExpedienteModel data = new ExpedienteModel() { id_trd = id_trd, identificacion = identificacion };
            string sql = @"select * from expediente where id_trd=@id_trd and identificacion=@identificacion and ano is null";
            return MysqlDataAccess.LoadData<ExpedienteModel>(sql, data)[0];
        }
        public static ExpedienteModel GetExpedienteAño(int id_trd, int ano)
        {
            ExpedienteModel data = new ExpedienteModel() { id_trd = id_trd, ano = ano };
            string sql = @"select * from expediente where id_trd=@id_trd and ano=@ano and identificacion is null and consecutivo_radicado is null";
            return MysqlDataAccess.LoadData<ExpedienteModel>(sql, data)[0];
        }

        public static radicadoModel CrearRadicado(int ano, string nombre, string email, long? identificacion,
            DateTime fecha, bool entrada, bool interno, DateTime? f_limite, string observaciones,bool respondido=false)
        {
            radicadoModel data = new radicadoModel
            {
                ano = ano,
                nombre = nombre,
                email = email,
                identificacion = identificacion,
                fecha = fecha,
                entrada = entrada,
                interno = interno,
                f_limite = f_limite,
                observaciones = observaciones,
                respondido=respondido
            };
            string sql = @"insert into radicado set ano=@ano,nombre=@nombre, email = @email, identificacion = @identificacion, 
fecha = @fecha, entrada = @entrada, 
interno = @interno, f_limite = @f_limite, observaciones=@observaciones,respondido=@respondido";
            MysqlDataAccess.SaveData(sql, data);

            sql = @"select * from radicado where ano=@ano and nombre=@nombre and ";
            if (data.email == null)
                sql = sql + @"email is @email ";
            else
                sql = sql + @"email = @email ";
            if (data.identificacion == null)
                sql = sql + @"and identificacion is @identificacion and entrada = @entrada and interno = @interno order by consecutivo desc;";
            else
                sql = sql + @"and identificacion = @identificacion and entrada = @entrada and interno = @interno order by consecutivo desc;";


            return MysqlDataAccess.LoadData<radicadoModel>(sql, data)[0];
        }

        public static void ResponderRadicado(string radicado_al_que_responde)
        {
            radicadoModel data = new radicadoModel() { numero_radicado = radicado_al_que_responde };
            string sql= @"UPDATE radicado SET respondido='1' WHERE numero_radicado=@numero_radicado; ";
            MysqlDataAccess.SaveData(sql, data);
        }

        public static void ModificarRadicado(int ano, int consecutivo, string radicado)
        {
            radicadoModel data = new radicadoModel { ano = ano, consecutivo = consecutivo, numero_radicado = radicado };
            string sql = @"update radicado set numero_radicado=@numero_radicado where consecutivo=@consecutivo and ano=@ano;";
            MysqlDataAccess.SaveData(sql, data);
        }

        public static void CrearExpediente(int id_trd, int? ano, int? consecutivo_radicado, DateTime f_creacion,
            string usuario, string observaciones, long? identificacion)
        {
            ExpedienteModel data = new ExpedienteModel
            {
                id_trd = id_trd,
                ano = ano,
                consecutivo_radicado = consecutivo_radicado,
                f_creacion = f_creacion,
                usuario_creacion = usuario,
                observaciones = observaciones,
                identificacion = identificacion
            };
            string sql = @"insert into expediente set  id_trd = @id_trd,
                ano = @ano,
                consecutivo_radicado = @consecutivo_radicado,
                f_creacion = @f_creacion,
                usuario_creacion = @usuario_creacion,
                observaciones = @observaciones,
                identificacion = @identificacion";
            MysqlDataAccess.SaveData(sql, data);
        }


        public static radicadoModel GetRadicado(int ano, int consecutivo)
        {
            radicadoModel data = new radicadoModel() { consecutivo = consecutivo, ano = ano };
            string sql = @"select * from radicado where consecutivo=@consecutivo and ano=@ano;";
            return MysqlDataAccess.LoadData<radicadoModel>(sql, data)[0];
        }

        public static List<radicadoModel> GetRadicadosAVencer()
        {
            radicadoModel data = new radicadoModel() { fecha = DateTime.Today };
            string sql = @"select * from radicado where respondido=0 and @fecha>DATE_SUB(vence, INTERVAL -2 MONTH);";
            return MysqlDataAccess.LoadData<radicadoModel>(sql);
        }

    }
}

public class DocumentoLogic
{
    
    public static void CrearDocumento(int id_tipologia, int ano, int? consecutivo, string observaciones, int folios, int id_expediente, string direccion, string respuesta
        , DateTime f_subida, string UsuarioID)
    {
        documentoModel data = new documentoModel
        {
            id_tipologia = id_tipologia,
            ano = ano,
            cons_radicado = consecutivo,
            folios = folios
            ,
            observaciones = observaciones,
            id_expediente = id_expediente,
            direccion = direccion,
            respuesta = respuesta,
            f_subida = f_subida,
            usuarioID = UsuarioID
        };
        string sql = @"insert into documento set   id_tipologia = @id_tipologia, ano_rad = @ano, cons_radicado = @cons_radicado, folios = @folios
            ,observaciones=@observaciones,id_expediente=@id_expediente,direccion=@direccion, respuesta=@respuesta,f_subida=@f_subida,usuarioID=@usuarioID;";
        MysqlDataAccess.SaveData(sql, data);
    }

    public static List<documentoModel> GetDocumentos(int exp_id)
    {
        documentoModel data = new documentoModel { id_expediente = exp_id };
        string sql = @"select documento.id,tipologia.nombre as tipologia, ano_rad as ano,cons_radicado,respuesta,observaciones,folios,direccion,f_subida,UserName 
from documento,tipologia,aspnetusers where id_expediente=@id_expediente and usuarioID=aspnetusers.ID and id_tipologia=tipologia.id group by documento.id;";
        return MysqlDataAccess.LoadData<documentoModel>(sql, data);
    }
}

public class ExpedienteLogic
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger("radicacionlogic");
    public static List<ExpedienteModel> GetExpediente(int id_expediente)
    {
        ExpedienteModel data = new ExpedienteModel() { id = id_expediente };
        string sql = @"select expediente.ano,consecutivo_radicado,f_creacion,f_cierre,us1.UserName as usuario_creacion,us2.UserName as usuario_cierre,
 expediente.observaciones,expediente.identificacion,sum(folios) as folios, central
from expediente left join documento on id_expediente=expediente.id 
left join aspnetusers as us1 on usuario_creacion=us1.Id
left join aspnetusers as us2 on usuario_cierre=us2.Id
where expediente.id=@id group by expediente.id;";
        return MysqlDataAccess.LoadData<ExpedienteModel>(sql, data);
    }
    public static List<ExpedienteModel> GetExpedientes(int id_trd, string busqueda = null, bool central = false)
    {
        List<ExpedienteModel> resultado = new List<ExpedienteModel>();
        ExpedienteModel data = new ExpedienteModel() { id_trd = id_trd, central = central };

        string sql = @"select expediente.id,id_trd,expediente.ano,consecutivo_radicado,
f_creacion,f_cierre,usuario_creacion,usuario_cierre,expediente.observaciones,identificacion,sum(folios) as folios
from expediente left join documento on id_expediente=expediente.id where id_trd=@id_trd and expediente.central=@central group by expediente.id;";

        if (!string.IsNullOrEmpty(busqueda) && busqueda.Length > 3)
        {
            data.observaciones = busqueda;
            sql = @"select expediente.id,id_trd,expediente.ano,consecutivo_radicado,
f_creacion,f_cierre,usuario_creacion,usuario_cierre,expediente.observaciones,identificacion,sum(folios) as folios
from (select * from expediente where match(observaciones) against(concat('*',@observaciones,'*'))) as expediente
 left join documento on id_expediente=expediente.id where id_trd=@id_trd and expediente.central=@central group by expediente.id;";
        }
        resultado = MysqlDataAccess.LoadData<ExpedienteModel>(sql, data);
        if (resultado.Count == 0)
        {
            sql = @"select expediente.id,id_trd,expediente.ano,consecutivo_radicado,
f_creacion,f_cierre,usuario_creacion,usuario_cierre,expediente.observaciones,identificacion,sum(folios) as folios
from expediente left join documento on id_expediente=expediente.id where id_trd=@id_trd and expediente.central=@central group by expediente.id;";
            resultado = MysqlDataAccess.LoadData<ExpedienteModel>(sql, data);
        }

        return resultado;
    }
    public static List<ExpedienteModel> GetExpedientesRadicado(int id_trd, int identificacion, DateTime desde, DateTime hasta, string busqueda = null, bool central = false)
    {
        List<ExpedienteModel> resultado = new List<ExpedienteModel>();
        List<radicadoModel> radicados = new List<radicadoModel>();
        ExpedienteModel data = new ExpedienteModel() { id_trd = id_trd, f_creacion = desde,f_cierre=hasta,id=identificacion, central = central };
        string sql;

        if (!string.IsNullOrEmpty(busqueda) && busqueda.Length > 3)
        {
            data.observaciones = busqueda;
             sql= @"SELECT * FROM radicado where (match(radicado.nombre,radicado.email,radicado.numero_radicado,radicado.observaciones) 
against(concat('""*',@observaciones,'*""') in boolean mode)) or identificacion like @id or (fecha<=@f_cierre and fecha >=@f_creacion);";
        }
        else
        {
            sql = @"SELECT * FROM radicado where identificacion like @id or (fecha<=@f_cierre and fecha >=@f_creacion);";
        }
        radicados = MysqlDataAccess.LoadData<radicadoModel>(sql, data);

        foreach(radicadoModel radicado in radicados)
        {
            data.ano = radicado.ano;
            data.consecutivo_radicado = radicado.consecutivo;
            sql = @"select expediente.id,id_trd,expediente.ano,consecutivo_radicado,radicado.fecha as f_creacion,
f_cierre,usuario_creacion,usuario_cierre,expediente.observaciones,radicado.identificacion,sum(folios) as folios, central
from radicado,expediente left join documento on id_expediente=expediente.id where expediente.consecutivo_radicado=
radicado.consecutivo and expediente.ano=radicado.ano and  
id_trd=@id_trd and expediente.ano=@ano and expediente.central=@central
and consecutivo_radicado=@consecutivo_radicado group by expediente.id;";
            resultado.AddRange(MysqlDataAccess.LoadData<ExpedienteModel>(sql, data));
        }
        

        return resultado;
    }
    public static List<ExpedienteModel> GetExpedientes(int id_trd, int ano, string busqueda = null, bool central = false)
    {
        List<ExpedienteModel> resultado = new List<ExpedienteModel>();
        ExpedienteModel data = new ExpedienteModel() { id_trd = id_trd, ano = ano, central = central };
        string sql = @"select expediente.id,id_trd,expediente.ano,consecutivo_radicado,
f_creacion,f_cierre,usuario_creacion,usuario_cierre,expediente.observaciones,identificacion,sum(folios) as folios, central
from expediente left join documento on id_expediente=expediente.id where id_trd=@id_trd and ano=@ano and expediente.central=@central group by expediente.id;";

        if (!string.IsNullOrEmpty(busqueda) && busqueda.Length > 3)
        {
            data.observaciones = busqueda;
            sql = @"select expediente.id,id_trd,expediente.ano,consecutivo_radicado,
f_creacion,f_cierre,usuario_creacion,usuario_cierre,expediente.observaciones,identificacion,sum(folios) as folios, central
from (select * from expediente where match(observaciones) against(concat('*',@observaciones,'*'))) as expediente
 left join documento on id_expediente=expediente.id where id_trd=@id_trd and ano=@ano and expediente.central=@central group by expediente.id;";
        }
        resultado = MysqlDataAccess.LoadData<ExpedienteModel>(sql, data);
        if (resultado.Count == 0)
        {
            sql = @"select expediente.id,id_trd,expediente.ano,consecutivo_radicado,
f_creacion,f_cierre,usuario_creacion,usuario_cierre,expediente.observaciones,identificacion,sum(folios) as folios, central
from expediente left join documento on id_expediente=expediente.id where id_trd=@id_trd and ano=@ano and expediente.central=@central group by expediente.id;";
            resultado = MysqlDataAccess.LoadData<ExpedienteModel>(sql, data);
        }

        return resultado;
    }

    public static List<ExpedienteModel> GetExpedientesAño(int id_trd)
    {
        ExpedienteModel data = new ExpedienteModel() { id_trd = id_trd };
        string sql = @"select expediente.id,id_trd,expediente.ano,consecutivo_radicado,
f_creacion,f_cierre,usuario_creacion,usuario_cierre,expediente.observaciones,identificacion,sum(folios) as folios, central
from expediente left join documento on id_expediente=expediente.id where id_trd=@id_trd group by expediente.ano;";
        return MysqlDataAccess.LoadData<ExpedienteModel>(sql, data);
    }

    public static void CerrarExpediente(int id, string userID, DateTime fecha_cierre)
    {
        ExpedienteModel data = new ExpedienteModel() { id = id, usuario_cierre = userID, f_cierre = fecha_cierre };
        string sql = @"update expediente set f_cierre=@f_cierre,usuario_cierre=@usuario_cierre where id=@id;";
        MysqlDataAccess.SaveData(sql, data);
    }
}
