using DataLibrary.BusinessLogic;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using GestionUI.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GestionUI.Controllers
{
    [Authorize]
    public class BusquedaController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("BusquedaController");

        public ActionResult Radicado(int ver = 0)
        {
            int version;
            if (ver != 0)
                version = ver;
            else
                version = TRDLogic.GetVersion();
            if (TRDLogic.IsModificacion(version))
                version = version - 1;
            TempData["version"] = version;
            return View();
        }
        [HttpPost]
        public ActionResult Radicado(ExpedienteModel model, int version)
        {
            TempData["version"] = version;
            if ((!string.IsNullOrEmpty(model.busqueda) && model.busqueda.Length > 3) || model.identificacion!=0  || (model.desde.Year>1900 && model.hasta.Year>1900))
            {
                
                if (model.oficina != 0)
                {
                    TempData["oficina"] = TRDLogic.GetOficinas(User.Identity.GetUserId(), version, model.oficina, true);
                }
                else
                {
                    TempData["oficina"] = TRDLogic.GetOficinas(User.Identity.GetUserId(), version, 0, true);
                }

                
                ModelState.Clear();
                return View(model);
            }
            else
            {
                ModelState.Clear();
                ModelState.AddModelError("busqueda", "en caso de que no se haya escogido subserie, la busqueda es obligatoria y tiene que ser más de 3 caracteres");
                return View(model);
            }

            //if (ModelState.IsValid)
            //{
            //    int i = TRDLogic.GetTrdID(model.serie, model.oficina, model.subserie);
            //    return RedirectToAction("Busqueda", new { id = i });
            //}
            //else
            //{
            //    TempData["version"] = version;
            //    return View(model);
            //}
        }

        public ActionResult IndexCentral(int ver = 0)
        {
            int version;
            if (ver != 0)
                version = ver;
            else
                version = TRDLogic.GetVersion();
            if (TRDLogic.IsModificacion(version))
                version = version - 1;
            TempData["version"] = version;
            return View();
        }
        [HttpPost]
        public ActionResult IndexCentral(ExpedienteModel model, int version)
        {
            TempData["version"] = version;
            if ((!string.IsNullOrEmpty(model.busqueda) && model.busqueda.Length > 3) || model.subserie != 0)
            {

                if (model.oficina != 0)
                {
                    TempData["oficina"] = TRDLogic.GetOficinas(User.Identity.GetUserId(), version, model.oficina, true);
                }
                else
                {
                    TempData["oficina"] = TRDLogic.GetOficinas(User.Identity.GetUserId(), version, 0, true);
                }
                TempData["serie"] = model.serie;
                TempData["subserie"] = model.subserie;
                ModelState.Clear();
                return View(model);
            }
            else
            {
                ModelState.AddModelError("busqueda", "en caso de que no se haya escogido subserie, la busqueda es obligatoria y tiene que ser más de 3 caracteres");
                return View(model);
            }

            //if (ModelState.IsValid)
            //{
            //    int i = TRDLogic.GetTrdID(model.serie, model.oficina, model.subserie);
            //    return RedirectToAction("Busqueda", new { id = i });
            //}
            //else
            //{
            //    TempData["version"] = version;
            //    return View(model);
            //}
        }

        // GET: Busqueda
        public ActionResult Index(int ver = 0)
        {
            int version;
            if (ver != 0)
                version = ver;
            else
                version = TRDLogic.GetVersion();
            if (TRDLogic.IsModificacion(version))
                version = version - 1;
            TempData["version"] = version;
            return View();
        }
        [HttpPost]
        public ActionResult Index(ExpedienteModel model, int version)
        {
            TempData["version"] = version;
            if ((!string.IsNullOrEmpty(model.busqueda) && model.busqueda.Length > 3) || model.subserie != 0)
            {

                if (model.oficina != 0)
                {
                    TempData["oficina"] = TRDLogic.GetOficinas(User.Identity.GetUserId(), version, model.oficina);
                }
                else
                {
                    TempData["oficina"] = TRDLogic.GetOficinas(User.Identity.GetUserId(), version);
                }
                TempData["serie"] = model.serie;
                TempData["subserie"] = model.subserie;
                ModelState.Clear();
                return View(model);
            }
            else
            {
                ModelState.AddModelError("busqueda", "en caso de que no se haya escogido subserie, la busqueda es obligatoria y tiene que ser más de 3 caracteres");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                int i = TRDLogic.GetTrdID(model.serie, model.oficina, model.subserie);
                return RedirectToAction("Busqueda", new { id = i });
            }
            else
            {
                TempData["version"] = version;
                return View(model);
            }
        }

        public ActionResult Busqueda(int id = 0, int ano = 0, int exp_id = 0)
        {
            if (id == 0)
                return RedirectToAction("Index");
            bool central = false;
            if (exp_id != 0)
            {
                var x = ExpedienteLogic.GetExpediente(exp_id);
                if (x.Count != 0 && x[0].central)
                    central = true;
            }
            if (TRDLogic.AuthorizedTRDid(User.Identity.GetUserId(), id) || central)
            {
                if (ano == 0 && exp_id == 0)
                    return RedirectToAction("Index");
                if (ano == 0)
                {
                    TempData["id_trd"] = id;
                    TempData["identificacion"] = exp_id;
                    return View();
                }
                else
                {
                    TempData["id_trd"] = id;
                    TempData["ano"] = ano;
                    return View();
                }
            }
            else
                return RedirectToAction("Index");
            //if (id == 0)
            //    return RedirectToAction("Index");
            //if (TRDLogic.AuthorizedTRDid(User.Identity.GetUserId(), id))
            //{
            //    TempData["id_orgranizacion"] = TRDLogic.GetOrganizacion(id);
            //    TempData["id_trd"] = id;
            //    return View();
            //}
            //else
            //    return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Busqueda(int id = 0, int ano = 0, int exp_id = 0, int dummie = 0)
        {
            if (id == 0)
                return RedirectToAction("Index");
            if (TRDLogic.AuthorizedTRDid(User.Identity.GetUserId(), id))
            {
                if (ano == 0 && exp_id == 0)
                    return RedirectToAction("Index");
                if (ano == 0)
                {
                    TempData["id_trd"] = id;
                    TempData["identificacion"] = exp_id;
                    return View();
                }
                else
                {
                    TempData["id_trd"] = id;
                    TempData["ano"] = ano;
                    return View();
                }
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                ExpedienteLogic.CerrarExpediente(id, User.Identity.GetUserId(), DateTime.Now);
                return RedirectToAction("Create");
            }
            catch (Exception e)
            {
                return View();
            }
        }

    }
}