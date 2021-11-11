using DataLibrary.BusinessLogic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace GestionUI
{
    public class User
    {

        public string Name { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }
    [Authorize]
    public class NotificationHub : Hub
    {
        private static readonly ConcurrentDictionary<string, User> Users
       = new ConcurrentDictionary<string, User>();


        public override Task OnConnected()
        {

            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;
            string userId = Context.User.Identity.GetUserId();
            var user = Users.GetOrAdd(userName, _ => new User
            {
                Name = userName,
                ConnectionIds = new HashSet<string>()
            });

            lock (user.ConnectionIds)
            {

                user.ConnectionIds.Add(connectionId);


                List<DataLibrary.Models.NotificacionesModel> notificaciones = new List<DataLibrary.Models.NotificacionesModel>();
                notificaciones = NotificacionesLogic.CargarNotificaciones(userId);
                int cont = 0;
                foreach (var notification in notificaciones)
                {
                    Clients.Client(connectionId).broadcastMessage(notification.notificacion, notification.fecha, notification.id, notification.leido);
                    if (!notification.leido)
                        cont++;
                }
                Clients.Client(connectionId).contadorNotificaciones(cont);

                // TODO: Broadcast the connected user
            }

            return base.OnConnected();
        }


        public void Send(string name, string message)
        {
            Clients.All.broadcastMessage(name, message);
        }

        public override Task OnDisconnected(bool stopCalled)
        {

            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            User user;
            Users.TryGetValue(userName, out user);

            if (user != null)
            {

                lock (user.ConnectionIds)
                {

                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));

                    if (!user.ConnectionIds.Any())
                    {

                        User removedUser;
                        Users.TryRemove(userName, out removedUser);
                    }
                }
            }

            return base.OnDisconnected(stopCalled);
        }

    }
}