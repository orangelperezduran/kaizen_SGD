using DataLibrary.DataAccess;
using DataLibrary.Models;
using System.Collections.Generic;

namespace DataLibrary.BusinessLogic
{
    public static class RoleManager
    {
        public static string GetRolAprobador()
        {
            string sql = @"select Name from aspnetroles where aprobarTRD=true;";
            return MysqlDataAccess.LoadData<string>(sql)[0];
        }
        public static int CrearRoleManagerRow(int primario, int secundario)
        {
            Role_manager data = new Role_manager
            {
                primario = primario,
                secundario = secundario,
                agregarUsuario = true
            };
            string sql = @"insert into role_manager(primario,secundario,agregarUsuario) values (@primario, @secundario, @agregarUsuario)";
            return MysqlDataAccess.SaveData(sql, data);
        }
        public static void EliminarRoleManager(int id, string UserID)
        {
            Role_manager data = new Role_manager
            {
                primario = id
            };
            var roles = CargarRoles();
            foreach (var rol in roles)
            {
                data.secundario = rol.id;
                string sql;
                sql = @"select primario from role_manager where primario=@primario and secundario=@secundario";
                if (MysqlDataAccess.LoadData<int>(sql, data).Count != 0)
                {
                    if (RoleManager.AutorizedRole(UserID, rol.id.ToString()))
                    {
                        sql = @"Delete from role_manager where primario=@primario and secundario=@secundario";
                        MysqlDataAccess.SaveData(sql, data);
                    }

                }
            }
        }
        public static int UpdateRoleManager(int primario, int secundario)
        {
            Role_manager data = new Role_manager
            {
                primario = primario,
                secundario = secundario,
                agregarUsuario = true
            };
            string sql;
            sql = @"select primario from role_manager where primario=@primario and secundario=@secundario";
            if (MysqlDataAccess.LoadData<int>(sql, data).Count == 0)
            {
                sql = @"insert into role_manager(primario,secundario,agregarUsuario) values (@primario, @secundario, @agregarUsuario)";
                return MysqlDataAccess.SaveData(sql, data);
            }
            else
            {
                return 0;
            }

        }
        public static int CrearRole(string nombre, bool AprobarTRD, bool ModificarTRD, bool isRoleManager, bool userManager)
        {
            RoleModel data = new RoleModel
            {
                name = nombre,
                aprobarTRD = AprobarTRD,
                modificarTRD = ModificarTRD,
                role_manager = isRoleManager,
                user_manager = userManager
            };
            if (AprobarTRD == true)
            {
                string temsql = @"select id from aspnetroles where aprobarTRD=true;";
                List<int> i = MysqlDataAccess.LoadData<int>(temsql);
                if (i.Count != 0)
                {
                    RoleModel rol = new RoleModel() { id = i[0] };
                    temsql = @"update aspnetroles set AprobarTRD=0 where id=@id";
                    MysqlDataAccess.SaveData(temsql, rol);
                }
            }
            string sql = @"insert into aspnetroles(name,aprobarTRD,modificarTRD,role_manager,user_manager) values (@name, @aprobarTRD, @modificarTRD,@role_manager,@user_manager)";

            int result = MysqlDataAccess.SaveData(sql, data);
            int version = TRDLogic.GetVersion();
            bool modificacion = true;
            if (version == 0)
            {
                modificacion = false;
                version = 1;
            }
            else
            {
                if (!TRDLogic.IsModificacion(version))
                    modificacion = false;
            }

            if (modificacion)
            {
                AprobacionModel apr = new AprobacionModel()
                {
                    version = version
                };
                sql = @"delete from aprobacion_trd where version=@version";
                result = +MysqlDataAccess.SaveData(sql, apr);
            }
            return result;
        }

        public static int EditarRol(string nombre, bool AprobarTRD, bool ModificarTRD, bool isRoleManager, bool userManager, int id, bool isUserManager)
        {
            RoleModel data = new RoleModel
            {
                id = id,
                name = nombre,
                aprobarTRD = AprobarTRD,
                modificarTRD = ModificarTRD,
                role_manager = isRoleManager,
                user_manager = userManager
            };
            if (AprobarTRD)
            {
                string temsql = @"select id from aspnetroles where aprobarTRD=true;";
                List<int> i = MysqlDataAccess.LoadData<int>(temsql);
                if (i.Count != 0)
                {
                    RoleModel rol = new RoleModel() { id = i[0] };
                    temsql = @"update aspnetroles set AprobarTRD=0 where id=@id";
                    MysqlDataAccess.SaveData(temsql, rol);
                }
            }
            else
            {
                string temsql = @"select id from aspnetroles where aprobarTRD=true;";
                List<int> i = MysqlDataAccess.LoadData<int>(temsql);
                if (i.Count == 0 || id==i[0])
                {
                    RoleModel rol = new RoleModel() { id = 1 };
                    temsql = @"update aspnetroles set AprobarTRD=1 where id=@id";
                    MysqlDataAccess.SaveData(temsql, rol);
                }

            }
            string sql;
            if (isUserManager)
            {
                sql = @"update aspnetroles set name=@name,aprobarTRD=@aprobarTRD,modificarTRD=@modificarTRD,role_manager=@role_manager,user_manager=@user_manager where id=@id;";
            }
            else
                sql = @"update aspnetroles set name=@name,aprobarTRD=@aprobarTRD,modificarTRD=@modificarTRD,role_manager=@role_manager where id=@id;";


            int result = MysqlDataAccess.SaveData(sql, data);
            int version = TRDLogic.GetVersion();
            bool modificacion = true;
            if (version == 0)
            {
                modificacion = false;
                version = 1;
            }
            else
            {
                if (!TRDLogic.IsModificacion(version))
                    modificacion = false;
            }

            if (modificacion)
            {
                AprobacionModel apr = new AprobacionModel()
                {
                    version = version
                };
                sql = @"delete from aprobacion_trd where version=@version";
                result = +MysqlDataAccess.SaveData(sql, apr);
                List<UserModel> usuarios = UserProcessor.UsuariosAprobadores();
                foreach (var user in usuarios)
                {
                    TRDLogic.CrearAprobacion(version, user.id);
                }
            }
            return result;
        }

        public static bool IsRoleManager(string UserID)
        {
            UserModel user = new UserModel { id = UserID };
            RoleModel role = new RoleModel();
            List<bool> RoleManager = new List<bool> { false };
            string sql = @"select RoleId from aspnetuserroles where UserId=@id";
            List<int> i = MysqlDataAccess.LoadData<int>(sql, user);
            foreach (int x in i)
            {
                role.id = x;
                sql = @"select role_manager from aspnetroles where id=@id";
                RoleManager = MysqlDataAccess.LoadData<bool>(sql, role);
                if (RoleManager[0])
                    break;
            }
            return RoleManager[0];
        }

        public static bool IsUserManager(string UserID)
        {
            UserModel user = new UserModel { id = UserID };
            RoleModel role = new RoleModel();
            List<bool> UserManager = new List<bool> { false };
            string sql = @"select RoleId from aspnetuserroles where UserId=@id";
            List<int> i = MysqlDataAccess.LoadData<int>(sql, user);
            foreach (int x in i)
            {
                role.id = x;
                sql = @"select user_manager from aspnetroles where Id=@id";
                UserManager = MysqlDataAccess.LoadData<bool>(sql, role);
                if (UserManager[0])
                    break;
            }
            return UserManager[0];
        }

        public static bool IsAprobador(int RoleID)
        {
            RoleModel data = new RoleModel() { id = RoleID };
            List<bool> UserManager = new List<bool> { false };
            string sql = @"select aprobarTRD from aspnetroles where Id=@id";
            return MysqlDataAccess.LoadData<bool>(sql, data)[0];
        }

        public static bool IsTRDModificador(string UserID)
        {
            UserModel user = new UserModel { id = UserID };
            RoleModel role = new RoleModel();
            List<bool> ModificarTRD = new List<bool> { false };
            string sql = @"select RoleId from aspnetuserroles where UserId=@id";
            List<int> i = MysqlDataAccess.LoadData<int>(sql, user);
            foreach (int x in i)
            {
                role.id = x;
                sql = @"select modificarTRD from aspnetroles where Id=@id";
                ModificarTRD = MysqlDataAccess.LoadData<bool>(sql, role);
                if (ModificarTRD[0])
                    break;
            }
            return ModificarTRD[0];
        }

        public static bool IsUserManager(int RoleID)
        {
            RoleModel role = new RoleModel() { id = RoleID };
            List<bool> UserManager = new List<bool>();
            string sql = @"select user_manager from aspnetroles where Id=@id";
            UserManager = MysqlDataAccess.LoadData<bool>(sql, role);
            return UserManager[0];
        }

        public static bool AutorizedRole(string UserID, string secRoleID)
        {
            UserModel user = new UserModel { id = UserID };
            RoleModel role = new RoleModel();
            List<bool> UserManager = new List<bool> { false };
            string sql = @"select RoleId from aspnetuserroles where UserId=@id";
            List<int> i = MysqlDataAccess.LoadData<int>(sql, user);
            foreach (int x in i)
            {
                role.id = x;
                sql = @"select agregarUsuario from role_manager where primario=@id and secundario=" + secRoleID;
                UserManager = MysqlDataAccess.LoadData<bool>(sql, role);
                if (UserManager.Count > 0 && UserManager[0])
                    break;
            }
            if (UserManager.Count == 0)
                return false;
            else
                return UserManager[0];
        }

        public static bool RoleIsUserManager(int RoleID, string secRoleID)
        {
            RoleModel role = new RoleModel() { id = RoleID };
            bool UserManager = false;
            string sql = @"select agregarUsuario from role_manager where primario=@id and secundario=" + secRoleID;
            try
            {
                UserManager = MysqlDataAccess.LoadData<bool>(sql, role)[0];
            }
            catch { }
            return UserManager;
        }


        public static List<RoleModel> CargarRoles()
        {
            string sql = @"select id,Name,aprobarTRD,modificarTRD,role_manager,user_manager from aspnetroles";
            return MysqlDataAccess.LoadData<RoleModel>(sql);
        }
        public static List<int> CargarRolesSecundarios(int RolePrimario)
        {
            RoleModel role = new RoleModel() { id = RolePrimario };
            string sql = @"select id from role_manager where id=@id";
            return MysqlDataAccess.LoadData<int>(sql, role);
        }

        public static RoleModel CargarRol(int id)
        {
            RoleModel data = new RoleModel() { id = id };
            string sql = @"select id,Name,aprobarTRD,modificarTRD,role_manager,user_manager from aspnetroles where id=@id";
            return MysqlDataAccess.LoadData<RoleModel>(sql, data)[0];
        }


        public static List<int> IdRoles(string UserID)
        {
            UserModel user = new UserModel() { id = UserID };
            List<int> Roles = new List<int>();
            string sql = @"select RoleId from aspnetuserroles where UserId=@id";
            return MysqlDataAccess.LoadData<int>(sql, user);
        }

        public static int CantidadDeUsuariosEnRol(int RoleID)
        {
            RoleModel user = new RoleModel() { id = RoleID };
            string sql = @"Select UserId from aspnetuserroles where RoleId=@id;";
            return MysqlDataAccess.LoadData<string>(sql, user).Count;
        }

        public static int EliminarRol(int id)
        {
            RoleModel data = new RoleModel
            {
                id = id
            };
            string sql = @"delete from aspnetroles where Id=@id";
            return MysqlDataAccess.SaveData(sql, data);
        }
    }
}
