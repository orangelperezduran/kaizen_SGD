using DataLibrary.BusinessLogic;
using GestionUI.Models;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GestionUI.Controllers
{
    [System.Web.Mvc.Authorize]
    public class RoleController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger("RoleController");
        // GET: Role
        public ActionResult Index(int ver = 0)
        {
            if (!RoleManager.IsRoleManager(User.Identity.GetUserId()))
                return RedirectToRoute(new { controller = "Home", action = "Index" });

            int version;
            if (ver != 0)
                version = ver;
            else
                version = TRDLogic.GetVersion();
            if (TRDLogic.IsModificacion(version))
                version = version - 1;
            TempData["version"] = version;
            return View(RoleManager.CargarRoles());

        }

        // GET: Role/Details/5
        public ActionResult Details(int id)
        {
            if (!RoleManager.IsRoleManager(User.Identity.GetUserId()))
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            return View();
        }

        // GET: Role/Create
        public ActionResult Create(int id = 0)
        {

            if (!RoleManager.IsRoleManager(User.Identity.GetUserId()))
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            int version;
            if (id != 0)
                version = id;
            else
                version = TRDLogic.GetVersion();
            if (TRDLogic.IsModificacion(version))
                version = version - 1;
            TempData["version"] = version;
            RoleModel role = new RoleModel();
            return PartialView(role);
        }

        // POST: Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoleModel Role, int[] roles, int[] subseries)
        {
            if (!RoleManager.IsRoleManager(User.Identity.GetUserId()))
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            try
            {
                RoleManager.CrearRole(Role.Name, Role.aprobarTRD, Role.modificarTRD, Role.role_manager, Role.user_manager);
                List<DataLibrary.Models.RoleModel> data = new List<DataLibrary.Models.RoleModel>();
                data = RoleManager.CargarRoles();
                if (roles != null)
                    foreach (int item in roles)
                    {
                        RoleManager.CrearRoleManagerRow(data[data.Count - 1].id, item);
                    }

                if (RoleManager.IsUserManager(User.Identity.GetUserId()))
                {

                    foreach (int i in RoleManager.IdRoles(User.Identity.GetUserId()))
                    {
                        if (RoleManager.IsUserManager(i))
                            RoleManager.CrearRoleManagerRow(i, data[data.Count - 1].id);
                    }
                }
                if (!User.IsInRole("Admin"))
                {
                    RoleManager.CrearRoleManagerRow(1, data[data.Count - 1].id);
                }
                if (subseries != null)
                {
                    foreach (var i in subseries)
                    {
                        TRDLogic.Insertrol_trd(data[data.Count - 1].id, i);
                    }
                }


                log.Info("El usuario " + User.Identity.Name + " ha creado el nuevo rol "+Role.Name);
                return Json(new
                {
                    status = "success",
                    success = true
                });
            }
            catch (Exception e)
            {
                log.Error(e);
                return Json(new
                {
                    status = "error",
                    success = false,
                    error = " No se ha podido agregar grupo, por favor vuelvalo a intentar"
                });
            }
        }

        // GET: Role/Edit/5
        public ActionResult Edit(int id)
        {
            if (!RoleManager.IsRoleManager(User.Identity.GetUserId()))
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            DataLibrary.Models.RoleModel rol = RoleManager.CargarRol(id);
            RoleModel model = new RoleModel()
            {
                id = id,
                aprobarTRD = rol.aprobarTRD,
                modificarTRD = rol.modificarTRD,
                Name = rol.name,
                role_manager = rol.role_manager,
                user_manager = rol.user_manager
            };
            return View(model);
        }

        // POST: Role/Edit/5
        [HttpPost]
        public ActionResult Edit(RoleModel Role, int[] roles, int[] subseries, int id)
        {
            if (!RoleManager.IsRoleManager(User.Identity.GetUserId()))
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            try
            {
                if (id != 1)
                {
                    if (RoleManager.IsUserManager(User.Identity.GetUserId()))
                    {
                        RoleManager.EditarRol(Role.Name, Role.aprobarTRD, Role.modificarTRD, Role.role_manager, Role.user_manager, id, true);
                        RoleManager.EliminarRoleManager(id, User.Identity.GetUserId());
                        if (roles != null)
                            foreach (int item in roles)
                            {
                                RoleManager.UpdateRoleManager(id, item);
                            }
                    }
                    else
                        RoleManager.EditarRol(Role.Name, Role.aprobarTRD, Role.modificarTRD, Role.role_manager, Role.user_manager, id, false);

                    var version = TRDLogic.GetVersion();
                    if (TRDLogic.IsModificacion(version))
                    {
                        version = version - 1;
                    }
                    TRDLogic.EliminarRol_trd(id, User.Identity.GetUserId(), version);
                    if (subseries != null)
                    {
                        foreach (var i in subseries)
                        {
                            TRDLogic.Insertrol_trd(id, i);
                        }
                    }

                    var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    List<DataLibrary.Models.UserModel> users = new List<DataLibrary.Models.UserModel>();
                    users = UserProcessor.UsuariosAprobadores();
                    foreach (var user in users)
                    {
                        List<DataLibrary.Models.NotificacionesModel> notificaciones = new List<DataLibrary.Models.NotificacionesModel>();
                        NotificacionesLogic.Crearnotificacion(user.id, "El rol " + Role.Name + ", al que usted pertenece, ahora aprueba las TRD", DateTime.Now, false, "../");
                        notificaciones = NotificacionesLogic.CargarNotificaciones(user.id);
                        int iduser = notificaciones[notificaciones.Count - 1].id;
                        context.Clients.User(user.UserName).broadcastMessage("El rol " + Role.Name + ", al que usted pertenece, ahora aprueba las TRD", DateTime.Now, iduser, false);
                        context.Clients.User(user.UserName).contadorNotificaciones(1);
                    }

                    log.Info("El usuario " + User.Identity.Name + " ha editado el rol " + Role.Name);
                    return Json(new
                    {
                        status = "success",
                        success = true
                    });
                }
                else
                {
                    return Json(new
                    {
                        status = "error",
                        success = false,
                        error = "No se puede editar el grupo administrador."
                    });
                }

            }
            catch (Exception e)
            {
                log.Error(e);
                return Json(new
                {
                    status = "error",
                    success = false,
                    error = " No se ha podido editar grupo, por favor, vuélvalo a intentar"
                });
            }
        }

        // GET: Role/Delete/5
        public ActionResult Delete(int id)
        {
            if (!RoleManager.IsRoleManager(User.Identity.GetUserId()))
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            return View();
        }

        // POST: Role/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            if (!RoleManager.IsRoleManager(User.Identity.GetUserId()))
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            try
            {
                if (id != 1)
                {
                    RoleManager.EliminarRol(id);
                    log.Info("El usuario " + User.Identity.Name + " Eliminó un rol ");
                    return Json(new
                    {
                        status = "success",
                        success = true
                    });
                }
                else
                {
                    return Json(new
                    {
                        status = "error",
                        success = false,
                        error = "No se puede eliminar el grupo administrador."
                    });
                }

            }
            catch (Exception e)
            {

                return Json(new
                {
                    status = "error",
                    success = false,
                    error = " No se ha podido eliminar grupo, por favor, vuélvalo a intentar"
                });
            }
        }
    }
}
