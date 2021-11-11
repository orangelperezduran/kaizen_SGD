using DataLibrary.DataAccess;
using DataLibrary.Models;
using System;
using System.Collections.Generic;

namespace DataLibrary.BusinessLogic
{
    public class NotificacionesLogic
    {

        public static NotificacionEmail GetNotificacionEmail()
        {
            string sql = @"select * from email_notification;";
            return MysqlDataAccess.LoadData<NotificacionEmail>(sql)[0];
        }
        public static void UpdatenotificacionEmail ()
        {
            NotificacionEmail data = new NotificacionEmail() { fecha = DateTime.Now.Date };
            string sql = @"update email_notification set fecha=@fecha where id=1;";
            MysqlDataAccess.SaveData(sql,data);
        }
        public static int Crearnotificacion(string userID, string notificacion, DateTime fecha, bool leido, string url)
        {
            NotificacionesModel data = new NotificacionesModel
            {
                userID = userID,
                notificacion = notificacion,
                fecha = fecha,
                leido = leido,
                url = url
            };
            string sql = @"insert into notificaciones(userID,notificacion,fecha,leido,url) values (@userID, @notificacion, @fecha, @leido,@url)";

            return MysqlDataAccess.SaveData(sql, data);
        }

        public static List<NotificacionesModel> CargarNotificaciones(string userID)
        {
            UserModel user = new UserModel() { id = userID };
            string sql = @"(select id,notificacion,fecha,leido,url from notificaciones where userID=@id order by id desc limit 50) order by id asc";
            return MysqlDataAccess.LoadData<NotificacionesModel>(sql, user);
        }

        public static int MarcarLeido(int id)
        {
            NotificacionesModel not = new NotificacionesModel() { id = id };
            string sql = @"update notificaciones set leido=1 where id=@id";
            return MysqlDataAccess.SaveData(sql, not);
        }

        public static NotificacionesModel GetNotificacion(int id)
        {
            NotificacionesModel not = new NotificacionesModel() { id = id };
            string sql = @"select id,notificacion,fecha,leido,url from notificaciones where id=@id";
            return MysqlDataAccess.LoadData<NotificacionesModel>(sql, not)[0];
        }

    }
}
