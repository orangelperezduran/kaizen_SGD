using DataLibrary.DataAccess;
using DataLibrary.Models;
using System;
using System.Collections.Generic;

namespace DataLibrary.BusinessLogic
{
    public static class UserProcessor
    {
        public static void CrearUsuario(string id, string nombre, string apellido, int identificacion, int[] roles)
        {
            UserModel data = new UserModel
            {
                id = id,
                nombre = nombre,
                apellido = apellido,
                identificacion = identificacion
            };
            string sql = @"insert into user_details(id,nombre,apellido,identificacion) values (@id, @nombre, @apellido, @identificacion)";

            MysqlDataAccess.SaveData(sql, data);
            if (roles != null)
            {
                foreach (int i in roles)
                {
                    sql = @"insert into aspnetuserroles(UserId,RoleId) values(@id," + i + ")";
                    MysqlDataAccess.SaveData(sql, data);
                }
            }

        }

        public static void UpdateUsuario(UserModel user)
        {
            string sql = @"update user_details set nombre=@nombre,apellido=@apellido,identificacion=@identificacion where id=@id;";
            MysqlDataAccess.SaveData(sql, user);
        }

        public static void AsignarRoles(string UserId, int RoleId)
        {
            UserRolModel data = new UserRolModel() { UserId = UserId, RoleId = RoleId };
            string sql = @"insert into aspnetuserroles(UserId,RoleId) values(@UserId,@RoleId)";
            MysqlDataAccess.SaveData(sql, data);
        }

        public static List<UserModel> CargarUsuarios()
        {
            string sql = @"select id,nombre,apellido,identificacion from user_details";
            return MysqlDataAccess.LoadData<UserModel>(sql);
        }
        public static UserModel CargarUsuario(string id)
        {
            UserModel data = new UserModel() { id = id };
            string sql = @"select aspnetusers.id,aspnetusers.Email,aspnetusers.UserName,nombre,apellido,identificacion 
from user_details,aspnetusers where aspnetusers.id=user_details.id and aspnetusers.id=@id";
            return MysqlDataAccess.LoadData<UserModel>(sql, data)[0];
        }

        public static List<UserModel> CargarUsuarios(int rol)
        {
            RoleModel data = new RoleModel() { id = rol };
            string sql = @"select user_details.id,UserName,Email,nombre,apellido,identificacion 
from user_details,aspnetuserroles,aspnetusers where aspnetusers.Id=user_details.id and UserId=user_details.id and RoleId=@id and (LockoutEndDateUtc is null or LockoutEndDateUtc<'" + DateTime.UtcNow + "')";
            return MysqlDataAccess.LoadData<UserModel>(sql, data);
        }

        public static List<UserModel> CargarInhabilitados()
        {
            string sql = @"select user_details.id,UserName,Email,nombre,apellido,identificacion 
from user_details,aspnetusers where aspnetusers.Id=user_details.id  and LockoutEndDateUtc > '3000-01-30 05:00:00'";
            return MysqlDataAccess.LoadData<UserModel>(sql);
        }

        public static List<UserModel> CargarSinGrupo()
        {
            string sql = @"select user_details.id,UserName,Email,nombre,apellido,identificacion 
from user_details,aspnetuserroles,aspnetusers where aspnetusers.Id=user_details.id and  aspnetusers.id not in(select UserId from aspnetuserroles) and (LockoutEndDateUtc is null or LockoutEndDateUtc<'" + DateTime.UtcNow + "') group by id";
            return MysqlDataAccess.LoadData<UserModel>(sql);
        }

        public static List<UserModel> UsuariosAprobadores()
        {
            string sql = @"select aspnetusers.Id,UserName,Email from aspnetusers,aspnetuserroles,aspnetroles where UserId=aspnetusers.Id and RoleId=aspnetroles.Id and aprobarTRD=1 and (LockoutEndDateUtc is null or LockoutEndDateUtc<='" + DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)) + "');";
            return MysqlDataAccess.LoadData<UserModel>(sql);
        }

        public static bool IsInRole(string id, int rol)
        {
            UserModel data = new UserModel() { email = id };
            string sql = @"select id from aspnetusers where username=@email";
            data.id = MysqlDataAccess.LoadData<string>(sql, data)[0];
            sql = @"select RoleId from aspnetuserroles where UserId=@id";
            List<int> i = MysqlDataAccess.LoadData<int>(sql, data);
            return i.Contains(rol);

        }

        public static int EliminarUserRol(int RoleId, string UserID)
        {
            UserRolModel data = new UserRolModel { UserId = UserID, RoleId = RoleId };
            string sql = @"delete from aspnetuserroles where UserId=@UserId and RoleId=@RoleId;";
            return MysqlDataAccess.SaveData(sql, data);

        }

        public static List<string> CargarEmails(int oficina, int serie, int subserie)
        {
            TRDModel data = new TRDModel() { id_oficina = oficina, id_serie = serie, id_subserie = subserie };
            string sql = @"select aspnetusers.email from aspnetusers,aspnetuserroles,aspnetroles,role_trd,trd 
where aspnetusers.EmailConfirmed=1 and (LockoutEndDateUtc is null or LockoutEndDateUtc<utc_timestamp()) and
aspnetusers.Id=aspnetuserroles.UserId and aspnetuserroles.RoleId=aspnetroles.Id and
aspnetroles.Id=role_trd.role_id and role_trd.trd_id=trd.id and trd.id_oficina=@id_oficina and
trd.id_serie=@id_serie and trd.id_subserie=@id_subserie;";
            return MysqlDataAccess.LoadData<string>(sql, data);
        }
    }
}
