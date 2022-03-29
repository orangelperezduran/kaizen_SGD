using DataLibrary.BusinessLogic;
using GestionUI.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GestionUI.Controllers
{
    
    [Authorize]
    public class DocumentoController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("DocumentoController");
        List<TablaDeRetencion> modelList = new List<TablaDeRetencion>();
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

        // GET: Documento
        public ActionResult Inventario(int ver = 0)
        {
            int version;
            if (ver != 0)
                version = ver;
            else
                version = TRDLogic.GetVersion();
            if (TRDLogic.IsModificacion(version))
                version = version - 1;
            CargarModelList(version);
            TempData["version"] = version;
            TempData["List"] = modelList;
            return View();
        }


        public ActionResult Digitalizar(int id = 0)
        {
            int version;
            if (id != 0)
                version = id;
            else
                version = TRDLogic.GetVersion();
            if (TRDLogic.IsModificacion(version))
                version = version - 1;
            TempData["version"] = version;
            return View();
        }

        public ActionResult DigitalizarCopia(int id = 0)
        {
            int version;
            if (id != 0)
                version = id;
            else
                version = TRDLogic.GetVersion();
            if (TRDLogic.IsModificacion(version))
                version = version - 1;
            TempData["version"] = version;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Digitalizar(VirtualizarModel model, int version)
        {

            try
            {

                if (ModelState.IsValid)
                {
                    int temp = TRDLogic.GetSubserie(model.subserie)[0].id_organizacion;
                    if ((temp == 2 || temp == 4 || temp == 3) && model.identificacion == null)
                    {
                        ModelState.AddModelError("identificacion", "Para la subserie seleccionada la identificación es obligatoria");
                        TempData["version"] = version;
                        return View(model);
                    }

                    if (model.file.ContentType != "application/pdf")
                    {
                        ModelState.AddModelError("file", "El documento tiene que ser un pdf");
                        TempData["version"] = version;
                        return View(model);
                    }
                    int i = TRDLogic.GetTrdID(model.serie, model.oficina, model.subserie);
                    string mensaje;
                    DataLibrary.Models.ExpedienteModel y = new DataLibrary.Models.ExpedienteModel();
                    string direccion, nombre;
                    bool z = false;
                    int cont = 0;
                    switch (temp)
                    {
                        case 1:
                            if (RadicacionLogic.ExpedienteExistAño(i, model.ano) == false || model.interno == true)
                            {
                                RadicacionLogic.CrearExpediente(i, model.ano, null, DateTime.Now, User.Identity.GetUserId(), model.observacionesexp, null);
                                Directory.CreateDirectory(Server.MapPath("/TRD/" + version + "/" + TRDLogic.GetOficina(model.oficina)[0].codigo
                                + "/" + TRDLogic.GetSerie(model.serie)[0].codigo + "/" + TRDLogic.GetSubserie(model.subserie)[0].codigo + "/" + model.ano));
                                mensaje = "Se ha creado un nuevo expediente y se ha añadido el radicado al mismo";
                            }
                            else
                                mensaje = "Se ha creado un nuevo documento y se ha subido al expediente";
                            y = RadicacionLogic.GetExpedienteAño(i, model.ano);
                            direccion = Server.MapPath("/TRD/" + version + "/" + TRDLogic.GetOficina(model.oficina)[0].codigo
                                + "/" + TRDLogic.GetSerie(model.serie)[0].codigo + "/" + TRDLogic.GetSubserie(model.subserie)[0].codigo + "/" + model.ano);
                            nombre = TRDLogic.GetTipologia(model.id_tipologia)[0].nombre;
                            cont = 0;
                            z = false;
                            while (z == false)
                            {
                                if (System.IO.File.Exists(direccion + "\\" + nombre + cont + ".pdf"))
                                    cont++;
                                else
                                {
                                    model.file.SaveAs(direccion + "\\" + nombre + cont + ".pdf");
                                    z = true;
                                    break;
                                }
                            }
                            DocumentoLogic.CrearDocumento(model.id_tipologia, model.ano, null, model.observaciones,
                                model.folios, y.id, direccion + "\\" + nombre + cont + ".pdf", null, DateTime.Now, User.Identity.GetUserId());
                            TempData["SuccessMessage"] = mensaje;

                            return RedirectToAction("Digitalizar");
                        case 3:


                            if (RadicacionLogic.ExpedienteExist(model.ano, i, int.Parse(model.identificacion.ToString().Substring(model.identificacion.ToString().Length - 4))) == false || model.interno == true)
                            {
                                ModelState.AddModelError("Identificacion", "El radicado de entrada al, cual trata de anexar, el documento no existe");
                                return View(model);
                            }

                            mensaje = "Se ha creado un nuevo documento y se ha subido al expediente";
                            y = RadicacionLogic.GetExpediente(model.ano, i, int.Parse(model.identificacion.ToString().Substring(model.identificacion.ToString().Length - 4)));
                            direccion = Server.MapPath("/TRD/" + version + "/" + TRDLogic.GetOficina(model.oficina)[0].codigo
                                + "/" + TRDLogic.GetSerie(model.serie)[0].codigo + "/" + TRDLogic.GetSubserie(model.subserie)[0].codigo + "/" + model.ano + "/" + model.identificacion);
                            nombre = TRDLogic.GetTipologia(model.id_tipologia)[0].nombre;
                            cont = 0;
                            z = false;
                            while (z == false)
                            {
                                if (System.IO.File.Exists(direccion + "\\" + nombre + cont + ".pdf"))
                                    cont++;
                                else
                                {
                                    model.file.SaveAs(direccion + "\\" + nombre + cont + ".pdf");
                                    z = true;
                                    break;
                                }
                            }
                            DocumentoLogic.CrearDocumento(model.id_tipologia, model.ano, null, model.observaciones,
                                model.folios, y.id, direccion + "\\" + nombre + cont + ".pdf", null, DateTime.Now, User.Identity.GetUserId());
                            TempData["SuccessMessage"] = mensaje;

                            return RedirectToAction("Digitalizar");
                        case 2:
                            if (RadicacionLogic.ExpedienteExist(i, model.identificacion) == false || model.interno == true)
                            {
                                RadicacionLogic.CrearExpediente(i, null, null, DateTime.Now, User.Identity.GetUserId(), model.observacionesexp, model.identificacion);
                                Directory.CreateDirectory(Server.MapPath("/TRD/" + version + "/" + TRDLogic.GetOficina(model.oficina)[0].codigo
                                    + "/" + TRDLogic.GetSerie(model.serie)[0].codigo + "/" + TRDLogic.GetSubserie(model.subserie)[0].codigo + "/" + model.identificacion));
                                mensaje = "Se ha creado un nuevo expediente y se ha añadido el radicado al mismo";
                            }
                            else
                                mensaje = "Se ha añadido el radicado al expediente con identificacion=" + model.identificacion;
                            y = RadicacionLogic.GetExpediente(i, model.identificacion);
                            direccion = Server.MapPath("/TRD/" + version + "/" + TRDLogic.GetOficina(model.oficina)[0].codigo
                                    + "/" + TRDLogic.GetSerie(model.serie)[0].codigo + "/" + TRDLogic.GetSubserie(model.subserie)[0].codigo + "/" + model.identificacion);
                            nombre = TRDLogic.GetTipologia(model.id_tipologia)[0].nombre;
                            cont = 0;
                            z = false;
                            while (z == false)
                            {
                                if (System.IO.File.Exists(direccion + "\\" + nombre + cont + ".pdf"))
                                    cont++;
                                else
                                {
                                    model.file.SaveAs(direccion + "\\" + nombre + cont + ".pdf");
                                    z = true;
                                    break;
                                }
                            }
                            DocumentoLogic.CrearDocumento(model.id_tipologia, model.ano, null, model.observaciones,
                                model.folios, y.id, direccion + "\\" + nombre + cont + ".pdf", null, DateTime.Now, User.Identity.GetUserId());
                            TempData["SuccessMessage"] = mensaje;

                            return RedirectToAction("Digitalizar");

                        case 4:
                            if (RadicacionLogic.ExpedienteExistAñoID(i, model.ano, model.identificacion) == false || model.interno == true)
                            {
                                RadicacionLogic.CrearExpediente(i, model.ano, null, DateTime.Now, User.Identity.GetUserId(), model.observacionesexp, model.identificacion);
                                Directory.CreateDirectory(Server.MapPath("/TRD/" + version + "/" + TRDLogic.GetOficina(model.oficina)[0].codigo
                                + "/" + TRDLogic.GetSerie(model.serie)[0].codigo + "/" + TRDLogic.GetSubserie(model.subserie)[0].codigo + "/" + model.ano
                                + "/" + model.identificacion));
                                mensaje = "Se ha creado un nuevo expediente y se ha añadido el radicado al mismo";
                            }
                            else
                                mensaje = "Se ha añadido el radicado al expediente con ano =" + model.ano + " e identificacion=" + model.identificacion;
                            y = RadicacionLogic.GetExpedienteAñoID(model.ano, i, model.identificacion);
                            direccion = Server.MapPath("/TRD/" + version + "/" + TRDLogic.GetOficina(model.oficina)[0].codigo
                                    + "/" + TRDLogic.GetSerie(model.serie)[0].codigo + "/" + TRDLogic.GetSubserie(model.subserie)[0].codigo + "/" + model.ano
                                    + "/" + model.identificacion);
                            nombre = TRDLogic.GetTipologia(model.id_tipologia)[0].nombre;
                            cont = 0;
                            z = false;
                            while (z == false)
                            {
                                if (System.IO.File.Exists(direccion + "\\" + nombre + cont + ".pdf"))
                                    cont++;
                                else
                                {
                                    model.file.SaveAs(direccion + "\\" + nombre + cont + ".pdf");
                                    z = true;
                                    break;
                                }
                            }
                            DocumentoLogic.CrearDocumento(model.id_tipologia, model.ano, null, model.observaciones,
                                model.folios, y.id, direccion + "\\" + nombre + cont + ".pdf", null, DateTime.Now, User.Identity.GetUserId());
                            TempData["SuccessMessage"] = mensaje;
                            return RedirectToAction("Digitalizar");
                    }

                }
                else
                {
                    TempData["version"] = version;
                    return View(model);
                }
                //TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["version"] = version;
                return View(model);
            }
        }


        // GET: Documento/Create
        public ActionResult Radicar(int ver = 0,string radicado=null)
        {
            radicarModel model = new radicarModel();
            if (radicado != null)
            {
                model.respuesta_a = radicado;
            }
            int version;
            if (ver != 0)
                version = ver;
            else
                version = TRDLogic.GetVersion();
            if (TRDLogic.IsModificacion(version))
                version = version - 1;
            TempData["version"] = version;
            return View(model);
        }



        // POST: Documento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Radicar(radicarModel model, int version)
        {
            model.ano = DateTime.Now.Year;
            
            try
            {
                if (model.entrada == false && model.respuesta_a == null)
                {
                    ModelState.AddModelError("respuesta_a", "Si no es un radicado de entrada por favor ingrese el número del radicado al cual responde");
                    TempData["version"] = version;
                    return View(model);
                }

                if (ModelState.IsValid)
                {
                    int temp = TRDLogic.GetSubserie(model.subserie)[0].id_organizacion;
                    int i = TRDLogic.GetTrdID(model.serie, model.oficina, model.subserie);
                    if ((temp == 2 || temp == 5) && model.identificacion == null)
                    {
                        ModelState.AddModelError("identificacion", "Para la subserie seleccionada la identificación es obligatoria");
                        TempData["version"] = version;
                        return View(model);
                    }
                    if (temp == 1 || temp == 3)
                    {
                        if (model.entrada == false)
                        {
                            if (RadicacionLogic.ExpedienteExist(model.ano, i, int.Parse(model.respuesta_a.Substring(model.respuesta_a.Length - 4))) == false)
                            {
                                ModelState.AddModelError("respuesta_a", "El radicado de entrada al cual responde no existe");
                                TempData["version"] = version;
                                return View(model);
                            }
                        }
                    }
                    if (temp == 2)
                    {
                        if (model.entrada == false)
                        {
                            if (RadicacionLogic.ExpedienteExist(i, model.identificacion) == false)
                            {
                                ModelState.AddModelError("respuesta_a", "El Expediente no existe y no puede ser creado con un radicado de salida");
                                TempData["version"] = version;
                                return View(model);
                            }
                            else
                            {
                                try
                                {
                                    var flag = RadicacionLogic.GetRadicado(model.ano, int.Parse(model.respuesta_a.Substring(model.respuesta_a.Length - 4)));
                                }
                                catch
                                {
                                    ModelState.AddModelError("respuesta_a", "El radicado al cual responde no existe");
                                    TempData["version"] = version;
                                    return View(model);
                                }
                            }
                        }
                    }
                    if (temp == 4)
                    {
                        if (model.entrada == false)
                        {
                            if (RadicacionLogic.ExpedienteExistAñoID(model.ano, i, model.identificacion) == false)
                            {
                                ModelState.AddModelError("respuesta_a", "El Expediente no existe y no puede ser creado con un radicado de salida");
                                TempData["version"] = version;
                                return View(model);
                            }
                            else
                            {
                                try
                                {
                                    var flag = RadicacionLogic.GetRadicado(model.ano, int.Parse(model.respuesta_a.ToString().Substring(model.respuesta_a.ToString().Length - 4)));
                                }
                                catch
                                {
                                    ModelState.AddModelError("respuesta_a", "El radicado al cual responde no existe");
                                    TempData["version"] = version;
                                    return View(model);
                                }
                            }
                        }
                    }
                    if (model.file.ContentType != "application/pdf")
                    {
                        ModelState.AddModelError("file", "El documento tiene que ser un pdf");
                        TempData["version"] = version;
                        return View(model);
                    }
                    DataLibrary.Models.radicadoModel x;
                    if (model.respuesta_a != null)
                    {
                        x = RadicacionLogic.CrearRadicado(model.ano, model.remitente, model.email_remitente, model.id_remitente, DateTime.Now, model.entrada,
                            model.interno, null, model.observaciones,true);
                    }
                    else
                    {
                        x = RadicacionLogic.CrearRadicado(model.ano, model.remitente, model.email_remitente, model.id_remitente, DateTime.Now,
                            model.entrada, model.interno, model.f_limite, model.observaciones);
                    }

                    string radicado;
                    if (model.entrada)
                    {
                        if (model.interno)
                        {
                            radicado = version.ToString("00") + TRDLogic.GetOficina(model.oficina)[0].codigo.ToString()
                                + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString()
                                + "0" + "0" + x.consecutivo.ToString("0000");
                        }
                        else
                            radicado = version.ToString("00") + TRDLogic.GetOficina(model.oficina)[0].codigo.ToString()
                                + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00")
                                + "0" + "1" + x.consecutivo.ToString("0000");
                    }
                    else
                    {
                        if (model.interno)
                        {
                            radicado = version.ToString("00") + TRDLogic.GetOficina(model.oficina)[0].codigo.ToString()
                                + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00")
                                + "1" + "0" + x.consecutivo.ToString("0000");
                        }
                        else
                            radicado = version.ToString("00") + TRDLogic.GetOficina(model.oficina)[0].codigo.ToString()
                                + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00")
                                + "1" + "1" + x.consecutivo.ToString("0000");
                    }

                    RadicacionLogic.ModificarRadicado(x.ano, x.consecutivo, radicado);

                    string mensaje;
                    IdentityMessage message;
                    DataLibrary.Models.radicadoModel a = new DataLibrary.Models.radicadoModel();
                    switch (temp)
                    {
                        case 1:
                        case 3:
                            DataLibrary.Models.ExpedienteModel y = new DataLibrary.Models.ExpedienteModel();
                            string direccion, nombre;
                            if (model.entrada)
                            {
                                if (RadicacionLogic.ExpedienteExist(model.ano, i, x.consecutivo) == false)
                                {
                                    RadicacionLogic.CrearExpediente(i, model.ano, x.consecutivo, DateTime.Now, User.Identity.GetUserId(), model.observaciones, model.identificacion);
                                    Directory.CreateDirectory(Server.MapPath("/TRD/" + version + "/" + TRDLogic.GetOficina(model.oficina)[0].codigo
                                    + "/" + TRDLogic.GetSerie(model.serie)[0].codigo + "/" + TRDLogic.GetSubserie(model.subserie)[0].codigo + "/" + model.ano + "/" + radicado));
                                    mensaje = "Se ha creado un nuevo expediente y se ha añadido el radicado al mismo";
                                }
                                else
                                    mensaje = "Se ha creado un nuevo documento y se ha subido al expediente";
                                y = RadicacionLogic.GetExpediente(model.ano, i, x.consecutivo);
                                direccion = Server.MapPath("/TRD/" + version + "/" + TRDLogic.GetOficina(model.oficina)[0].codigo
                                    + "/" + TRDLogic.GetSerie(model.serie)[0].codigo + "/" + TRDLogic.GetSubserie(model.subserie)[0].codigo + "/" + model.ano + "/" + radicado);
                                nombre = TRDLogic.GetTipologia(model.id_tipologia)[0].nombre;
                            }
                            else
                            {

                                y = RadicacionLogic.GetExpediente(model.ano, i, int.Parse(model.respuesta_a.ToString().Substring(model.respuesta_a.ToString().Length - 4)));
                                mensaje = "Se ha creado un nuevo documento y se ha subido al expediente";
                                direccion = Server.MapPath("/TRD/" + version + "/" + TRDLogic.GetOficina(model.oficina)[0].codigo
                                    + "/" + TRDLogic.GetSerie(model.serie)[0].codigo + "/" + TRDLogic.GetSubserie(model.subserie)[0].codigo + "/" + model.ano + "/" + model.respuesta_a);
                                nombre = TRDLogic.GetTipologia(model.id_tipologia)[0].nombre + model.ano;
                                a = RadicacionLogic.GetRadicado(model.ano, int.Parse(model.respuesta_a.ToString().Substring(model.respuesta_a.ToString().Length - 4)));
                            }
                            CrearStamp(model.file, direccion, radicado, nombre);
                            if (model.respuesta_a == null)
                                DocumentoLogic.CrearDocumento(model.id_tipologia, model.ano, x.consecutivo, model.observaciones,
                                model.folios, y.id, direccion + "\\" + nombre + radicado + ".pdf", null, DateTime.Now, User.Identity.GetUserId());
                            else
                            {
                                DocumentoLogic.CrearDocumento(model.id_tipologia, model.ano, x.consecutivo, model.observaciones,
                               model.folios, y.id, direccion + "\\" + nombre + radicado + ".pdf", model.respuesta_a
                               , DateTime.Now, User.Identity.GetUserId());
                                RadicacionLogic.ResponderRadicado(model.respuesta_a);
                            }
                                
                            TempData["SuccessMessage"] = mensaje;
                            TempData["file"] = Encript.Crypt(direccion + "\\" + radicado + "sticker.pdf");
                            foreach (var email in UserProcessor.CargarEmails(model.oficina, model.serie, model.subserie).Distinct())
                            {
                                if (model.entrada)
                                {
                                    message = new IdentityMessage()
                                    {
                                        Body = "Hola " + email + " se ha radicado con éxito un documento bajo el número:"
                                                                     + radicado + ".\n" +
                                                                     "Al mismo tiempo se adjunta al correo el documento radicado con el sticker de radicación. " +
                                                                     "\n" +
                                                                     "Quedamos atentos a cualquier nueva solicitud.",
                                        Destination = email,
                                        Subject = "Radicado número:" + radicado
                                    };
                                    SendAsync(message, direccion + "/" + nombre + radicado + ".pdf");
                                }
                                else
                                {
                                    message = new IdentityMessage()
                                    {
                                        Body = "Hola " + email + " se ha radicado con éxito una respuesta al número de radicado " + a.numero_radicado + "bajo el número:"
                                                                     + radicado + ".\n" +
                                                                     "Al mismo tiempo se adjunta al correo el documento radicado con el sticker de radicación. " +
                                                                     "\n" +
                                                                     "Quedamos atentos a cualquier nueva solicitud.",
                                        Destination = email,
                                        Subject = "Radicado número:" + radicado
                                    };
                                    SendAsync(message, direccion + "/" + nombre + radicado + ".pdf");
                                }

                            }
                            if (model.email_remitente != null)
                            {

                                message = new IdentityMessage()
                                {
                                    Body = "Hola " + model.remitente + " se ha radicado con éxito el documento enviado bajo el número:"
                                    + radicado + ".\nAl mismo tiempo adjuntamos al correo el documento radicado con el sticker de radicación. " +
                                    "\nQuedamos atentos a cualquier nueva solicitud.",
                                    Destination = model.email_remitente,
                                    Subject = "Radicado número:" + radicado
                                };
                                SendAsync(message, direccion + "/" + nombre + radicado + ".pdf");
                            }

                            if (a.email != null)
                            {
                                message = new IdentityMessage()
                                {
                                    Body = "Hola " + a.nombre + " se ha radicado con éxito la respuesta bajo el número:"
                                    + radicado + ".\nAl mismo tiempo adjuntamos al correo el documento radicado con el sticker de radicación. " +
                                    "\nQuedamos atentos a cualquier nueva solicitud.",
                                    Destination = a.email,
                                    Subject = "Radicado número:" + radicado
                                };
                                SendAsync(message, direccion + "/" + nombre + radicado + ".pdf");
                            }
                            log.Info("El usuario " + User.Identity.Name + "ha creado el radicado: " + radicado);
                            return RedirectToAction("Radicar");
                        case 2:
                            if (model.entrada)
                            {
                                if (RadicacionLogic.ExpedienteExist(i, model.identificacion) == false)
                                {
                                    RadicacionLogic.CrearExpediente(i, null, null, DateTime.Now, User.Identity.GetUserId(), model.observaciones, model.identificacion);
                                    Directory.CreateDirectory(Server.MapPath("/TRD/" + version + "/" + TRDLogic.GetOficina(model.oficina)[0].codigo
                                    + "/" + TRDLogic.GetSerie(model.serie)[0].codigo + "/" + TRDLogic.GetSubserie(model.subserie)[0].codigo + "/" + model.identificacion));
                                    mensaje = "Se ha creado un nuevo expediente y se ha añadido el radicado al mismo";
                                }
                                else
                                    mensaje = "Se ha añadido el radicado al expediente con identificacion=" + model.identificacion;
                            }
                            else
                            {

                                mensaje = "Se ha añadido el radicado al expediente con identificacion=" + model.identificacion;
                                a = RadicacionLogic.GetRadicado(model.ano, int.Parse(model.respuesta_a.ToString().Substring(model.respuesta_a.ToString().Length - 4)));
                            }
                            y = RadicacionLogic.GetExpediente(i, model.identificacion);
                            direccion = Server.MapPath("/TRD/" + version + "/" + TRDLogic.GetOficina(model.oficina)[0].codigo
                                    + "/" + TRDLogic.GetSerie(model.serie)[0].codigo + "/" + TRDLogic.GetSubserie(model.subserie)[0].codigo + "/" + model.identificacion);
                            nombre = TRDLogic.GetTipologia(model.id_tipologia)[0].nombre + model.ano;
                            CrearStamp(model.file, direccion, radicado, nombre);
                            if (model.respuesta_a == null)
                                DocumentoLogic.CrearDocumento(model.id_tipologia, model.ano, x.consecutivo, model.observaciones,
                                model.folios, y.id, direccion + "\\" + nombre + radicado + ".pdf", null, DateTime.Now, User.Identity.GetUserId());
                            else
                            {
                                DocumentoLogic.CrearDocumento(model.id_tipologia, model.ano, x.consecutivo, model.observaciones,
                               model.folios, y.id, direccion + "\\" + nombre + radicado + ".pdf", model.respuesta_a
                               , DateTime.Now, User.Identity.GetUserId());
                                RadicacionLogic.ResponderRadicado(model.respuesta_a);
                            }
                                
                            TempData["SuccessMessage"] = mensaje;
                            TempData["file"] = Encript.Crypt(direccion + "\\" + radicado + "sticker.pdf");
                            foreach (var email in UserProcessor.CargarEmails(model.oficina, model.serie, model.subserie))
                            {
                                if (model.entrada)
                                {
                                    message = new IdentityMessage()
                                    {
                                        Body = "Hola " + email + " se ha radicado con éxito un documento bajo el número:"
                                                                     + radicado + ".\n" +
                                                                     "Al mismo tiempo se adjunta al correo el documento radicado con el sticker de radicación. " +
                                                                     "\n" +
                                                                     "Quedamos atentos a cualquier nueva solicitud.",
                                        Destination = email,
                                        Subject = "Radicado número:" + radicado
                                    };
                                    SendAsync(message, direccion + "/" + nombre + radicado + ".pdf");
                                }
                                else
                                {
                                    message = new IdentityMessage()
                                    {
                                        Body = "Hola " + email + " se ha radicado con éxito una respuesta al número de radicado " + a.numero_radicado + "bajo el número:"
                                                                     + radicado + ".\n" +
                                                                     "Al mismo tiempo se adjunta al correo el documento radicado con el sticker de radicación. " +
                                                                     "\n" +
                                                                     "Quedamos atentos a cualquier nueva solicitud.",
                                        Destination = email,
                                        Subject = "Radicado número:" + radicado
                                    };
                                    SendAsync(message, direccion + "/" + nombre + radicado + ".pdf");
                                }

                            }
                            if (model.email_remitente != null)
                            {
                                message = new IdentityMessage()
                                {
                                    Body = "Hola " + model.remitente + " se ha radicado con éxito el documento enviado bajo el número:"
                                    + radicado + ".\nAl mismo tiempo adjuntamos al correo el documento radicado con el sticker de radicación. " +
                                    "\nQuedamos atentos a cualquier nueva solicitud.",
                                    Destination = model.email_remitente,
                                    Subject = "Radicado número:" + radicado
                                };
                                SendAsync(message, direccion + "/" + nombre + radicado + ".pdf");
                            }
                            if (a.email != null)
                            {
                                message = new IdentityMessage()
                                {
                                    Body = "Hola " + a.nombre + " se ha radicado con éxito la respuesta bajo el número:"
                                    + radicado + ".\nAl mismo tiempo adjuntamos al correo el documento radicado con el sticker de radicación. " +
                                    "\nQuedamos atentos a cualquier nueva solicitud.",
                                    Destination = a.email,
                                    Subject = "Radicado número:" + radicado
                                };
                                SendAsync(message, direccion + "/" + nombre + radicado + ".pdf");
                            }

                            log.Info("El usuario " + User.Identity.Name + "ha creado el radicado: " + radicado);
                            return RedirectToAction("Radicar");

                        case 4:
                            if (model.entrada)
                            {
                                if (RadicacionLogic.ExpedienteExistAñoID(i, model.ano, model.identificacion) == false)
                                {
                                    RadicacionLogic.CrearExpediente(i, model.ano, null, DateTime.Now, User.Identity.GetUserId(), model.observaciones, model.identificacion);
                                    Directory.CreateDirectory(Server.MapPath("/TRD/" + version + "/" + TRDLogic.GetOficina(model.oficina)[0].codigo
                                    + "/" + TRDLogic.GetSerie(model.serie)[0].codigo + "/" + TRDLogic.GetSubserie(model.subserie)[0].codigo + "/" + model.ano
                                    + "/" + model.identificacion));
                                    mensaje = "Se ha creado un nuevo expediente y se ha añadido el radicado al mismo";
                                }
                                else
                                    mensaje = "Se ha añadido el radicado al expediente con ano =" + model.ano + " e identificacion=" + model.identificacion;
                            }
                            else
                            {

                                mensaje = "Se ha añadido el radicado al expediente con identificacion=" + model.identificacion;
                                a = RadicacionLogic.GetRadicado(model.ano, int.Parse(model.respuesta_a.ToString().Substring(model.respuesta_a.ToString().Length - 4)));
                            }
                            y = RadicacionLogic.GetExpedienteAñoID(model.ano, i, model.identificacion);
                            direccion = Server.MapPath("/TRD/" + version + "/" + TRDLogic.GetOficina(model.oficina)[0].codigo
                                    + "/" + TRDLogic.GetSerie(model.serie)[0].codigo + "/" + TRDLogic.GetSubserie(model.subserie)[0].codigo + "/" + model.ano
                                    + "/" + model.identificacion);
                            nombre = TRDLogic.GetTipologia(model.id_tipologia)[0].nombre + model.ano;
                            CrearStamp(model.file, direccion, radicado, nombre);
                            if (model.respuesta_a == null)
                                DocumentoLogic.CrearDocumento(model.id_tipologia, model.ano, x.consecutivo, model.observaciones,
                                model.folios, y.id, direccion + "\\" + nombre + radicado + ".pdf", null, DateTime.Now, User.Identity.GetUserId());
                            else
                            {
                                DocumentoLogic.CrearDocumento(model.id_tipologia, model.ano, x.consecutivo, model.observaciones,
                               model.folios, y.id, direccion + "\\" + nombre + radicado + ".pdf",model.respuesta_a
                               , DateTime.Now, User.Identity.GetUserId());
                                RadicacionLogic.ResponderRadicado(model.respuesta_a);
                            }
                                
                            TempData["SuccessMessage"] = mensaje;
                            TempData["file"] = Encript.Crypt(direccion + "\\" + radicado + "sticker.pdf");
                            foreach (var email in UserProcessor.CargarEmails(model.oficina, model.serie, model.subserie))
                            {
                                if (model.entrada)
                                {
                                    message = new IdentityMessage()
                                    {
                                        Body = "Hola " + email + " se ha radicado con éxito un documento bajo el número:"
                                                                     + radicado + ".\n" +
                                                                     "Al mismo tiempo se adjunta al correo el documento radicado con el sticker de radicación. " +
                                                                     "\n" +
                                                                     "Quedamos atentos a cualquier nueva solicitud.",
                                        Destination = email,
                                        Subject = "Radicado número:" + radicado
                                    };
                                    SendAsync(message, direccion + "/" + nombre + radicado + ".pdf");
                                }
                                else
                                {
                                    message = new IdentityMessage()
                                    {
                                        Body = "Hola " + email + " se ha radicado con éxito una respuesta al número de radicado " + a.numero_radicado + "bajo el número:"
                                                                     + radicado + ".\n" +
                                                                     "Al mismo tiempo se adjunta al correo el documento radicado con el sticker de radicación. " +
                                                                     "\n" +
                                                                     "Quedamos atentos a cualquier nueva solicitud.",
                                        Destination = email,
                                        Subject = "Radicado número:" + radicado
                                    };
                                    SendAsync(message, direccion + "/" + nombre + radicado + ".pdf");
                                }

                            }
                            if (model.email_remitente != null)
                            {
                                message = new IdentityMessage()
                                {
                                    Body = "Hola " + model.remitente + " se ha radicado con éxito el documento enviado bajo el número:"
                                   + radicado + ".\nAl mismo tiempo adjuntamos al correo el documento radicado con el sticker de radicación. " +
                                   "\nQuedamos atentos a cualquier nueva solicitud.",
                                    Destination = model.email_remitente,
                                    Subject = "Radicado número:" + radicado
                                };
                                SendAsync(message, direccion + "/" + nombre + radicado + ".pdf");
                            }
                            if (a.email != null)
                            {
                                message = new IdentityMessage()
                                {
                                    Body = "Hola " + a.nombre + " se ha radicado con éxito la respuesta bajo el número:"
                                    + radicado + ".\nAl mismo tiempo adjuntamos al correo el documento radicado con el sticker de radicación. " +
                                    "\nQuedamos atentos a cualquier nueva solicitud.",
                                    Destination = a.email,
                                    Subject = "Radicado número:" + radicado
                                };
                                SendAsync(message, direccion + "/" + nombre + radicado + ".pdf");
                            }
                            log.Info("El usuario " + User.Identity.Name + "ha creado el radicado: " + radicado);
                            return RedirectToAction("Radicar");
                    }

                }
                else
                {
                    TempData["version"] = version;
                    return View(model);
                }
                //TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                log.Error(e);
                TempData["version"] = version;
                return View();
            }
        }

        public ActionResult Doc(string id)
        {
            return File(Encript.Decrypt(id), "application/pdf");
        }

        public void CrearStamp(HttpPostedFileBase file, string direccion, string radicado, string tipologia)
        {
            using (MemoryStream ms = new MemoryStream())
            {

                //The Image is drawn based on length of Barcode text.
                using (Bitmap bitMap = new Bitmap(radicado.Length * 30, 150))
                {
                    //The Graphics library object is generated for the Image.
                    using (Graphics graphics = Graphics.FromImage(bitMap))
                    {
                        //The installed Barcode font.
                        System.Drawing.Font oFont = new System.Drawing.Font("IDAutomationHC39M", 16);
                        PointF point = new PointF(2f, 2f);

                        //White Brush is used to fill the Image with white color.
                        SolidBrush whiteBrush = new SolidBrush(Color.White);
                        graphics.FillRectangle(whiteBrush, 0, 0, bitMap.Width, bitMap.Height);

                        //Black Brush is used to draw the Barcode over the Image.
                        SolidBrush blackBrush = new SolidBrush(Color.Black);
                        graphics.DrawString(radicado, oFont, blackBrush, point);
                        graphics.DrawString("\nFecha:" + DateTime.Now + "\nUsuario: " + User.Identity.GetUserName(), new System.Drawing.Font("Calibri", 16), blackBrush, 0, 50);
                    }

                    //The Bitmap is saved to Memory Stream.
                    bitMap.Save(direccion + "\\" + radicado + ".png", ImageFormat.Png);
                }
            }
            byte[] pdfbytes = null;
            BinaryReader rdr = new BinaryReader(file.InputStream);
            pdfbytes = rdr.ReadBytes((int)file.ContentLength);

            using (Stream inputImageStream = new FileStream(direccion + "/" + radicado + ".png", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream outputPdfStream = new FileStream(direccion + "/" + tipologia + radicado + ".pdf", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var reader = new PdfReader(pdfbytes);
                var stamper = new PdfStamper(reader, outputPdfStream);
                var pdfContentByte = stamper.GetOverContent(1);
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(inputImageStream);
                image.ScaleAbsolute(150, 70);
                image.SetAbsolutePosition(reader.GetPageSize(1).Width - 150, reader.GetPageSize(1).Height - 70);
                pdfContentByte.AddImage(image);
                stamper.Close();
            }
            var PageSize = new iTextSharp.text.Rectangle((float)170.0784, (float)85.0392);
            Document doc = new Document(PageSize);
            PdfWriter writer = PdfWriter.GetInstance(doc,
                new FileStream(direccion + "/" + radicado + "sticker.pdf", FileMode.Create));
            doc.Open();
            iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(direccion + "/" + radicado + ".png");
            imagen.ScaleAbsolute(doc.PageSize.Width, doc.PageSize.Height);
            imagen.SetAbsolutePosition(0, 0);
            doc.Add(imagen);
            doc.Close();

        }

        public JsonResult GetSeries(int id, int version, int serie = 0)
        {
            var temp = TRDLogic.GetSeries(User.Identity.GetUserId(), version, id);
            if (serie != 0)
            {
                foreach (var item in temp)
                {
                    if (item.Value == serie.ToString())
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
            return Json(temp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubSeries(int id, int version, int oficina, int subserie = 0)
        {
            List<SelectListItem> temp = new List<SelectListItem>(TRDLogic.GetSubseries(User.Identity.GetUserId(), version, oficina, id).Select(m => new SelectListItem()
            { Text = m.nombre, Value = m.id.ToString(), Selected = false }));
            if (subserie != 0)
            {
                foreach (var item in temp)
                {
                    if (item.Value == subserie.ToString())
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
            return Json(temp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTipologias(int id, int version, int oficina, int serie, int tipologia = 0)
        {
            List<SelectListItem> temp = new List<SelectListItem>(TRDLogic.GetTipologias(User.Identity.GetUserId(),
                version, oficina, serie, id).Select(m => new SelectListItem() { Text = m.nombre, Value = m.id.ToString() }));
            if (tipologia != 0)
            {
                foreach (var item in temp)
                {
                    if (item.Value == tipologia.ToString())
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
            return Json(temp, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOrganizacionText(int id, int serie, int oficina,int version, string ano)
        {
            var temp = TRDLogic.GetSubserie(id)[0];
            if (!int.TryParse(ano, out int year))
            {
                year = 0;
            }
            temp.archivo = TRDLogic.GetCantidadDocumentos(serie,id,oficina,version,year);
            temp.gestion = TRDLogic.GetUltimoConsecutivo(serie, id, oficina, version,year);
            return Json(temp, JsonRequestBehavior.AllowGet);
        }


        public Task SendAsync(IdentityMessage message, string file)
        {
            var smtp = new SmtpClient();
            var mail = new MailMessage();
            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string username = smtpSection.Network.UserName;

            mail.IsBodyHtml = true;
            mail.From = new MailAddress(username);
            mail.To.Add(message.Destination);
            mail.Subject = message.Subject;
            mail.Body = message.Body;
            if (file.Length > 0)
            {
                mail.Attachments.Add(new Attachment(file));
            }
            smtp.EnableSsl = true;
            smtp.Timeout = 1000;

            var t = Task.Run(() => smtp.SendAsync(mail, null));

            return t;
        }

    }
    public static class Encript
    {
        public static string Crypt(this string text)
        {
            return Convert.ToBase64String(
                ProtectedData.Protect(
                    Encoding.Unicode.GetBytes(text), null, DataProtectionScope.CurrentUser));
        }

        public static string Decrypt(this string text)
        {
            return Encoding.Unicode.GetString(
                ProtectedData.Unprotect(Convert.FromBase64String(text), null, DataProtectionScope.CurrentUser));
        }


    }
}
