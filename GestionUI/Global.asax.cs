using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DataLibrary.BusinessLogic;

namespace GestionUI
{
    public class Listado
    {
        string id { get; set; }
        string corrreo { get; set; }
        List<string> radicados { get; set; }
    }
    public class WebApiApplication : System.Web.HttpApplication
    {
        
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //if (NotificacionesLogic.GetNotificacionEmail().fecha < DateTime.Today.Date)
            //{
            //    NotificacionesLogic.UpdatenotificacionEmail();
            //    foreach ()
            //}

        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //Pregunta si tenemos o no ya una conexion segura(SSL), sino redirige
            //string otro = Request.ServerVariables["HTTP_HOST"];
            //if (HttpContext.Current.Request.IsSecureConnection.Equals(false))
            //{
            //    if (Request.ServerVariables["HTTP_HOST"] != "localhost:57792")
            //        // redireccion de HTTP a HTTPS
            //        Response.Redirect("https://" + Request.ServerVariables["HTTP_HOST"] + HttpContext.Current.Request.RawUrl);
            //    else
            //        Response.Redirect("https://" + "localhost:44396" + HttpContext.Current.Request.RawUrl);
            //}
        }
    }
}
