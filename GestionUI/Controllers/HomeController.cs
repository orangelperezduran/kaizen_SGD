using DataLibrary.BusinessLogic;
using Microsoft.AspNet.SignalR;
using System.Web.Mvc;

namespace GestionUI.Controllers
{
    [System.Web.Mvc.Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Start()
        {
            return View();
        }
        public ActionResult Index()
        {
            var r = this.ControllerContext.RouteData.Values;
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Notificaciones(int id, bool leido)
        {
            if (leido == false)
            {
                NotificacionesLogic.MarcarLeido(id);
                var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                context.Clients.User(User.Identity.Name).contadorNotificaciones(-1);
            }
            return RedirectToAction(NotificacionesLogic.GetNotificacion(id).url);
        }

    }
}
