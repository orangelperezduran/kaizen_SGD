using DataLibrary.BusinessLogic;
using GestionUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GestionUI.Controllers
{
    [System.Web.Mvc.Authorize]
    public class TablaDeRetencionController : Controller
    {
        List<TablaDeRetencion> modelList = new List<TablaDeRetencion>();
        List<TablaDeRetencion> aprobacion = new List<TablaDeRetencion>();
        private void CargarModelList()
        {
            if (System.IO.File.ReadAllBytes(Server.MapPath("/TRDList.txt")).Length != 0)
            {
                string[] lines = System.IO.File.ReadAllLines(Server.MapPath("/TRDList.txt"));
                foreach (string line in lines)
                {
                    string[] TRDList = line.Split(';');
                    modelList.Add(new TablaDeRetencion
                    {
                        CodigoOficina = int.Parse(TRDList[0]),
                        CodigoSerie = int.Parse(TRDList[1]),
                        CodigoSubserie = int.Parse(TRDList[2]),
                        DisposicionFinal = TRDList[3],
                        NombreOficina = TRDList[4],
                        NombreSerie = TRDList[5],
                        NombreSubserie = TRDList[6],
                        NombreTipologia = TRDList[7],
                        TiempoGestion = int.Parse(TRDList[8]),
                        TiempoArchivo = int.Parse(TRDList[9]),
                        Observaciones = TRDList[10],
                        Organizacion = int.Parse(TRDList[11]),
                        Organizacion_text = TRDLogic.GetOrganizaciontext(int.Parse(TRDList[11])),
                        Identificacion = TRDList[12]
                    }); ;
                }
            }
        }

        private void CargarModelList(int version)
        {
            var x = TRDLogic.MostrarTRD(version);
            foreach (var line in TRDLogic.MostrarTRD(version))
            {
                modelList.Add(new TablaDeRetencion
                {
                    CodigoOficina = line.CodigoOficina,
                    CodigoSerie = line.CodigoSerie,
                    CodigoSubserie = line.CodigoSubserie,
                    DisposicionFinal = line.DisposicionFinal,
                    NombreOficina = line.NombreOficina,
                    NombreSerie = line.NombreSerie,
                    NombreSubserie = line.NombreSubserie,
                    NombreTipologia = line.NombreTipologia,
                    TiempoGestion = line.TiempoGestion,
                    TiempoArchivo = line.TiempoArchivo,
                    Observaciones = line.Observaciones,
                    Organizacion = line.id_organizacion,
                    Organizacion_text = TRDLogic.GetOrganizaciontext(line.id_organizacion)
                });
            }
            modelList = modelList.OrderBy(o => o.CodigoOficina).ThenBy(o => o.CodigoSerie).ThenBy(o => o.CodigoSubserie).ThenBy(o => o.NombreTipologia).ToList();

        }

        private void CargarAprobacion()
        {
            if (System.IO.File.ReadAllBytes(Server.MapPath("/TRDPendiente.txt")).Length != 0)
            {
                string[] lines = System.IO.File.ReadAllLines(Server.MapPath("/TRDPendiente.txt"));
                foreach (string line in lines)
                {
                    string[] TRDList = line.Split(';');
                    aprobacion.Add(new TablaDeRetencion
                    {
                        CodigoOficina = int.Parse(TRDList[0]),
                        CodigoSerie = int.Parse(TRDList[1]),
                        CodigoSubserie = int.Parse(TRDList[2]),
                        DisposicionFinal = TRDList[3],
                        NombreOficina = TRDList[4],
                        NombreSerie = TRDList[5],
                        NombreSubserie = TRDList[6],
                        NombreTipologia = TRDList[7],
                        TiempoGestion = int.Parse(TRDList[8]),
                        TiempoArchivo = int.Parse(TRDList[9]),
                        Observaciones = TRDList[10],
                        Organizacion = int.Parse(TRDList[11]),
                        Organizacion_text = TRDLogic.GetOrganizaciontext(int.Parse(TRDList[11])),
                        Identificacion = TRDList[12]
                    });
                }
            }
        }

        private void RehacerTexto()
        {

            if (System.IO.File.Exists(Server.MapPath("/TRDList.txt")))
                System.IO.File.Delete(Server.MapPath("/TRDList.txt"));
            using (System.IO.FileStream fs = System.IO.File.Create(Server.MapPath("/TRDList.txt")))
            {
            }
            using (System.IO.StreamWriter sr = new System.IO.StreamWriter(Server.MapPath("/TRDList.txt"), true))
            {
                foreach (var item in modelList)
                {
                    sr.WriteLine($"{item.CodigoOficina};{item.CodigoSerie};{item.CodigoSubserie};" +
                       $"{item.DisposicionFinal};{item.NombreOficina};" +
                       $"{item.NombreSerie};{item.NombreSubserie};" +
                       $"{item.NombreTipologia};{item.TiempoGestion};{item.TiempoArchivo};" +
                       $"{item.Observaciones};" +
                       $"{item.Organizacion};" +
                       $"{item.Identificacion}"); ;
                }
            }
        }

        private void CrearTRDaprobación()
        {

            if (System.IO.File.Exists(Server.MapPath("/TRDPendiente.txt")))
                System.IO.File.Delete(Server.MapPath("/TRDPendiente.txt"));

            using (System.IO.FileStream fs = System.IO.File.Create(Server.MapPath("/TRDPendiente.txt")))
            {
            }
            using (System.IO.StreamWriter sr = new System.IO.StreamWriter(Server.MapPath("/TRDPendiente.txt"), true))
            {
                foreach (var item in aprobacion)
                {
                    sr.WriteLine($"{item.CodigoOficina};{item.CodigoSerie};{item.CodigoSubserie};" +
                       $"{item.DisposicionFinal};{item.NombreOficina};" +
                       $"{item.NombreSerie};{item.NombreSubserie};" +
                       $"{item.NombreTipologia};{item.TiempoGestion};{item.TiempoArchivo};" +
                       $"{item.Observaciones};" +
                       $"{item.Organizacion};" +
                       $"{item.Identificacion}");
                }
            }

        }
        // GET: TablaDeRetencion

        public ActionResult Index(int id = 0)
        {
            int version;
            if (id != 0)
                version = id;
            else
                version = TRDLogic.GetVersion();
            if (TRDLogic.IsModificacion(version))
                version = version - 1;
            CargarModelList(version);
            TempData["organizacion"] = TRDLogic.GetOrganizacion();
            TempData["version"] = version;
            TempData["List"] = modelList;
            return View();
        }

        public ActionResult Aprobar()
        {
            if (!User.IsInRole(RoleManager.GetRolAprobador()))
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            int version = TRDLogic.GetVersion();
            if (version == 0)
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            if (!TRDLogic.IsModificacion(version))
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            TempData["version"] = version;
            CargarAprobacion();
            TempData["List"] = aprobacion;
            return View(version);
        }

        [HttpPost]
        public ActionResult AprobarTRD()
        {
            if (!User.IsInRole(RoleManager.GetRolAprobador()))
                return Content("<script language='javascript' type='text/javascript'>alert('No tiene autorización para aprobar la TRD'); window.location.replace('/');</script>");
            int version = TRDLogic.GetVersion();
            if (version == 0)
                return Content("<script language='javascript' type='text/javascript'>alert('No tiene autorización para aprobar la TRD'); window.location.replace('/');</script>");
            if (!TRDLogic.IsModificacion(version))
                return Content("<script language='javascript' type='text/javascript'>alert('No tiene autorización para aprobar la TRD'); window.location.replace('/');</script>");

            if (TRDLogic.AprobarTRD(User.Identity.GetUserId(), version))
            {

                CargarAprobacion();
                var oficinas = (from p in aprobacion select new { p.CodigoOficina, p.NombreOficina }).Distinct();
                var series = (from p in aprobacion select new { p.CodigoSerie, p.NombreSerie }).Distinct();
                var subseries = (from p in aprobacion
                                 select new
                                 {
                                     p.CodigoSubserie,
                                     p.NombreSubserie,
                                     p.DisposicionFinal,
                                     p.Observaciones,
                                     p.TiempoArchivo,
                                     p.TiempoGestion,
                                     p.Organizacion,
                                     p.Identificacion
                                 }).Distinct();
                var tipologias = (from p in aprobacion select new { p.CodigoOficina, p.CodigoSerie, p.CodigoSubserie, p.NombreTipologia }).Distinct();
                foreach (var item in oficinas)
                {
                    TRDLogic.insertOficina(item.NombreOficina, version, item.CodigoOficina);
                }
                foreach (var item in series)
                {
                    TRDLogic.insertSerie(item.NombreSerie, version, item.CodigoSerie);
                }
                foreach (var item in subseries)
                {
                    if (item.Identificacion == "")
                        TRDLogic.insertSubserie(item.NombreSubserie, version, item.CodigoSubserie, item.TiempoArchivo, item.DisposicionFinal,
                            item.TiempoGestion, item.Observaciones, item.Organizacion);
                    else
                        TRDLogic.insertSubserie(item.NombreSubserie, version, item.CodigoSubserie, item.TiempoArchivo, item.DisposicionFinal,
                           item.TiempoGestion, item.Observaciones, item.Organizacion, item.Identificacion);
                }
                foreach (var item in tipologias)
                {
                    TRDLogic.CrearTRDyTipologias(item.CodigoOficina, item.CodigoSerie, item.CodigoSubserie, item.NombreTipologia, version);
                }
                TRDLogic.AsignarTRDtoAdmin(version);
                if (System.IO.File.Exists(Server.MapPath("/TRDList.txt")))
                    System.IO.File.Delete(Server.MapPath("/TRDList.txt"));
                if (System.IO.File.Exists(Server.MapPath("/TRDPendiente.txt")))
                    System.IO.File.Delete(Server.MapPath("/TRDPendiente.txt"));
                return Json(new
                {
                    status = "success",
                    success = true,
                    alert = "Ha aprobado la TRD, de ahora en adelante todos los documentos se ingresarán bajo esta nueva versión."
                });
            }
            else
            {
                return Json(new
                {
                    status = "success",
                    success = true,
                    alert = "Ha aprobado la TRD, falta todavía la aprobación otros usuarios, " +
                    "para aprobarla del todo."
                });
            }
        }

        // GET: TablaDeRetencion/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TablaDeRetencion/Create

        public ActionResult Create()
        {
            if (!RoleManager.IsTRDModificador(User.Identity.GetUserId()))
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            if (!System.IO.File.Exists(Server.MapPath("/TRDList.txt")))
                using (System.IO.FileStream fs = System.IO.File.Create(Server.MapPath("/TRDList.txt")))
                {
                }
            else
                CargarModelList();
            TablaDeRetencion model = new TablaDeRetencion();
            TempData["organizacion"] = TRDLogic.GetOrganizacion();
            TempData["List"] = modelList;
            return View(model);
        }

        // POST: TablaDeRetencion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TablaDeRetencion TRD)
        {
            CargarModelList();
            TempData["List"] = modelList;
            TempData["organizacion"] = TRDLogic.GetOrganizacion();
            if (TRD.Organizacion == 0 || ((TRD.Organizacion == 2 || TRD.Organizacion == 2) && TRD.Identificacion == null))
            {
                ModelState.AddModelError("Identificacion", "Por favor para este tipo de organización ingrese un tipo de identificación");
                return View(TRD);
            }
            if (ModelState.IsValid == false)
            {
                return View(TRD);
            }
            else
            {
                if (TRD.Observaciones == null)
                    TRD.Observaciones = "";
                foreach (var item in modelList)
                {
                    if (item.CodigoOficina == TRD.CodigoOficina && item.CodigoSerie == TRD.CodigoSerie
                        && item.CodigoSubserie == TRD.CodigoSubserie
                        && item.NombreTipologia == TRD.NombreTipologia)
                    {
                        ModelState.AddModelError("", "Este conjunto ya existe en la TRD");
                        return View(TRD);
                    }
                    if (item.CodigoOficina == TRD.CodigoOficina)
                    {
                        if (TRD.NombreOficina != item.NombreOficina)
                        {
                            ModelState.AddModelError("NombreOficina", "Ese código de oficina ya está asignado y no corresponde al nombre guardado");
                            return View(TRD);
                        }
                    }
                    if (item.CodigoSerie == TRD.CodigoSerie)
                        if (TRD.NombreSerie != item.NombreSerie)
                        {
                            ModelState.AddModelError("NombreSerie", "Ese código de serie ya está asignado y no corresponde al nombre guardado");
                            return View(TRD);
                        }
                    if (item.CodigoSubserie == TRD.CodigoSubserie)
                    {
                        if (TRD.NombreSubserie != item.NombreSubserie)
                        {
                            ModelState.AddModelError("NombreSubserie", "Ese código de subserie ya está asignado y no corresponde al nombre guardado");
                            return View(TRD);
                        }
                        if (TRD.TiempoGestion != item.TiempoGestion)
                        {
                            ModelState.AddModelError("TiempoGestion", "El tiempo en gestión no corresponde a ese código de subserie");
                            return View(TRD);
                        }
                        if (TRD.TiempoArchivo != item.TiempoArchivo)
                        {
                            ModelState.AddModelError("TiempoArchivo", "El tiempo en Archivo no corresponde a ese código de subserie");
                            return View(TRD);
                        }
                        if (TRD.DisposicionFinal != item.DisposicionFinal)
                        {
                            ModelState.AddModelError("DisposicionFinal", "La disposición final no corresponde a ese código de subserie");
                            return View(TRD);
                        }
                        if (TRD.Observaciones != item.Observaciones)
                        {
                            ModelState.AddModelError("Observaciones", "Las Observaciones no corresponden a ese código de subserie");
                            return View(TRD);
                        }
                        if (TRD.Organizacion != item.Organizacion)
                        {
                            ModelState.AddModelError("Organizacion", "la organizacion no corresponden a ese código de subserie");
                            return View(TRD);
                        }
                        if (TRD.Identificacion != null && item.Identificacion != "")
                        {
                            if (TRD.Identificacion != item.Identificacion)
                            {
                                ModelState.AddModelError("Identificacion", "la identificacion no corresponden a ese código de subserie");
                                return View(TRD);
                            }
                        }


                    }
                }

                modelList.Add(TRD);
                if (TRD.Tipologias.Count != 0)
                {
                    foreach(var tipologia in TRD.Tipologias)
                    {
                        TRD.NombreTipologia = tipologia;
                        modelList.Add(TRD);
                    }
                }
                modelList = modelList.OrderBy(o => o.CodigoOficina).ThenBy(o => o.CodigoSerie).ThenBy(o => o.CodigoSubserie).ThenBy(o => o.NombreTipologia).ToList();
                RehacerTexto();

                TempData["List"] = modelList;
                return RedirectToAction("Create");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarTRD()
        {
            CargarModelList();
            aprobacion = modelList;
            CrearTRDaprobación();
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
                TRDLogic.ModificarVersion(version, User.Identity.GetUserId());
                TRDLogic.EliminarAprobacion(version);

            }
            else
                TRDLogic.CrearVersion(User.Identity.GetUserId());
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            List<DataLibrary.Models.UserModel> usuarios = UserProcessor.UsuariosAprobadores();
            foreach (var user in usuarios)
            {
                TRDLogic.CrearAprobacion(version, user.id);
            }

            foreach (var user in usuarios)
            {
                List<DataLibrary.Models.NotificacionesModel> notificaciones = new List<DataLibrary.Models.NotificacionesModel>();
                NotificacionesLogic.Crearnotificacion(user.id, "Se ha subido una nueva actualización de la TRD para su aprobación", DateTime.Now, false, "../TablaDeRetencion/Aprobar");
                notificaciones = NotificacionesLogic.CargarNotificaciones(user.id);
                int id = notificaciones[notificaciones.Count - 1].id;
                context.Clients.User(user.UserName).broadcastMessage("Se ha subido una nueva actualización de la TRD para su aprobación", DateTime.Now, id, false);
                context.Clients.User(user.UserName).contadorNotificaciones(1);
            }


            return Content("<script language='javascript' type='text/javascript'>alert('Se ha agregado la TRD para aprobación'); window.location.replace('/');</script>");
        }

        // GET: TablaDeRetencion/Edit/5
        public ActionResult Edit(int id)
        {
            CargarModelList();
            TempData["organizacion"] = TRDLogic.GetOrganizacion();
            TablaDeRetencion model = new TablaDeRetencion();
            model.CodigoOficina = modelList[id].CodigoOficina;
            model.CodigoSerie = modelList[id].CodigoSerie;
            model.CodigoSubserie = modelList[id].CodigoSubserie;
            model.DisposicionFinal = modelList[id].DisposicionFinal;
            model.NombreOficina = modelList[id].NombreOficina;
            model.NombreSerie = modelList[id].NombreSerie;
            model.NombreSubserie = modelList[id].NombreSubserie;
            model.NombreTipologia = modelList[id].NombreTipologia;
            model.Observaciones = modelList[id].Observaciones;
            model.TiempoArchivo = modelList[id].TiempoArchivo;
            model.TiempoGestion = modelList[id].TiempoGestion;
            model.Identificacion = modelList[id].Identificacion;
            model.Organizacion = modelList[id].Organizacion;

            return PartialView(model);
        }

        // POST: TablaDeRetencion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TablaDeRetencion TRD, int id)
        {
            CargarModelList();
            TablaDeRetencion model = modelList[id];

            if (ModelState.IsValid == false)
            {
                return Json(new
                {
                    status = "error",
                    success = false,
                    error = " No se ha podido modificar debido a que algún dato no se encuentra o está fuera de límites."
                });
            }

            else
            {
                bool val = false;
                if (model.NombreOficina != TRD.NombreOficina)
                {
                    val = true;
                    foreach (var item in modelList)
                    {
                        if (item.CodigoOficina == TRD.CodigoOficina)
                        {
                            item.NombreOficina = TRD.NombreOficina;
                        }
                    }

                }
                if (model.NombreSerie != TRD.NombreSerie)
                {
                    val = true;
                    foreach (var item in modelList)
                    {
                        if (item.CodigoSerie == TRD.CodigoSerie)
                        {
                            item.NombreSerie = TRD.NombreSerie;
                        }
                    }
                }
                if (model.NombreSubserie != TRD.NombreSubserie)
                {
                    val = true;
                    foreach (var item in modelList)
                    {
                        if (item.CodigoSubserie == TRD.CodigoSubserie)
                        {
                            item.NombreSubserie = TRD.NombreSubserie;
                        }
                    }
                }
                if (model.NombreTipologia != TRD.NombreTipologia)
                {
                    val = true;
                    modelList[id].NombreTipologia = TRD.NombreTipologia;
                }
                if (model.TiempoGestion != TRD.TiempoGestion)
                {
                    val = true;
                    foreach (var item in modelList)
                    {
                        if (item.CodigoSubserie == TRD.CodigoSubserie)
                        {
                            item.TiempoGestion = TRD.TiempoGestion;
                        }
                    }
                }
                if (model.TiempoArchivo != TRD.TiempoArchivo)
                {
                    val = true;
                    foreach (var item in modelList)
                    {
                        if (item.CodigoSubserie == TRD.CodigoSubserie)
                        {
                            item.TiempoArchivo = TRD.TiempoArchivo;
                        }
                    }
                }
                if (model.DisposicionFinal != TRD.DisposicionFinal)
                {
                    val = true;
                    foreach (var item in modelList)
                    {
                        if (item.CodigoSubserie == TRD.CodigoSubserie)
                        {
                            item.DisposicionFinal = TRD.DisposicionFinal;
                        }
                    }
                }
                if (model.Observaciones != TRD.Observaciones)
                {
                    val = true;
                    foreach (var item in modelList)
                    {
                        if (item.CodigoSubserie == TRD.CodigoSubserie)
                        {
                            item.Observaciones = TRD.Observaciones;
                        }
                    }
                }
                if (model.Organizacion != TRD.Organizacion)
                {
                    val = true;
                    foreach (var item in modelList)
                    {
                        if (item.CodigoSubserie == TRD.CodigoSubserie)
                        {
                            item.Organizacion = TRD.Organizacion;
                        }
                    }
                }
                if (model.Identificacion != TRD.Identificacion)
                {
                    val = true;
                    foreach (var item in modelList)
                    {
                        if (item.CodigoSubserie == TRD.CodigoSubserie)
                        {
                            item.Identificacion = TRD.Identificacion;
                        }
                    }
                }


                if (val)
                {
                    RehacerTexto();
                }

                TempData["List"] = modelList;
                return Json(new
                {
                    status = "success",
                    success = true
                });
            }
        }

        // GET: TablaDeRetencion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TablaDeRetencion/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                CargarModelList();
                modelList.RemoveAt(id);
                RehacerTexto();
                return RedirectToAction("Create");
            }
            catch
            {
                return View();
            }
        }

        public JsonResult Oficina(string Prefix)
        {
            CargarModelList();
            //Searching records from list using LINQ query  
            var NombreOficina = (from N in modelList
                                 where N.NombreOficina.Contains(Prefix)
                                 select new { N.NombreOficina, N.CodigoOficina }).Distinct();
            return Json(NombreOficina, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Serie(string Prefix)
        {
            CargarModelList();
            //Searching records from list using LINQ query  
            var NombreOficina = (from N in modelList
                                 where N.NombreSerie.Contains(Prefix)
                                 select new { N.NombreSerie, N.CodigoSerie }).Distinct();
            return Json(NombreOficina, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Subserie(string Prefix)
        {
            CargarModelList();
            //Searching records from list using LINQ query  
            var NombreOficina = (from N in modelList
                                 where N.NombreSubserie.Contains(Prefix)
                                 select new { N.NombreSubserie, N.CodigoSubserie, N.TiempoGestion, N.TiempoArchivo, N.DisposicionFinal, N.Organizacion, N.Identificacion }).Distinct();
            return Json(NombreOficina, JsonRequestBehavior.AllowGet);
        }
        public JsonResult codOficina(string Prefix)
        {
            CargarModelList();
            //Searching records from list using LINQ query  
            var NombreOficina = (from N in modelList
                                 where N.CodigoOficina.ToString().Contains(Prefix)
                                 select new { N.NombreOficina, N.CodigoOficina }).Distinct();
            return Json(NombreOficina, JsonRequestBehavior.AllowGet);
        }
        public JsonResult codSerie(string Prefix)
        {
            CargarModelList();
            //Searching records from list using LINQ query  
            var NombreOficina = (from N in modelList
                                 where N.CodigoSerie.ToString().Contains(Prefix)
                                 select new { N.NombreSerie, N.CodigoSerie }).Distinct();
            return Json(NombreOficina, JsonRequestBehavior.AllowGet);
        }
        public JsonResult codSubserie(string Prefix)
        {
            CargarModelList();
            //Searching records from list using LINQ query  
            var NombreOficina = (from N in modelList
                                 where N.CodigoSubserie.ToString().Contains(Prefix)
                                 select new { N.NombreSubserie, N.CodigoSubserie, N.TiempoGestion, N.TiempoArchivo, N.DisposicionFinal, N.Observaciones, N.Organizacion, N.Identificacion }).Distinct();
            return Json(NombreOficina, JsonRequestBehavior.AllowGet);
        }
    }
}
