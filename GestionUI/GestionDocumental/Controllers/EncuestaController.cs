using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionUI.Models;

namespace GestionUI.Controllers
{
    public class EncuestaController : Controller
    {
        List<EncuestaModel> modelList = new List<EncuestaModel>();
        private void CargarModelList()
        {
            if (System.IO.File.ReadAllBytes(Server.MapPath("/Entidades.txt")).Length != 0)
            {
                string[] lines = System.IO.File.ReadAllLines(Server.MapPath("/Entidades.txt"));
                foreach (string line in lines)
                {
                    string[] TRDList = line.Split(';');
                    modelList.Add(new EncuestaModel
                    {
                        Entidad = TRDList[0],
                        Oficina = TRDList[1],
                        Encargado = TRDList[2],
                        Serie = TRDList[3],
                        subserie = TRDList[4],
                        Gestion = int.Parse(TRDList[5]),
                        Archivo = int.Parse(TRDList[6]),
                        tipologia = TRDList[7],
                        Soporte = TRDList[8],
                        DisposicionFinal = TRDList[9],
                        Observaciones = TRDList[10],
                        recomendaciones = TRDList[11],
                        SeriesModificadas = TRDList[12],
                        Funcion = TRDList[13],
                        Original = bool.Parse(TRDList[14]),
                        Copia = bool.Parse(TRDList[15]),
                    }); ;
                }
            }
        }
        private void RehacerTexto()
        {

            if (System.IO.File.Exists(Server.MapPath("/Entidades.txt")))
                System.IO.File.Delete(Server.MapPath("/Entidades.txt"));
            using (System.IO.FileStream fs = System.IO.File.Create(Server.MapPath("/Entidades.txt")))
            {
            }
            using (System.IO.StreamWriter sr = new System.IO.StreamWriter(Server.MapPath("/Entidades.txt"), true))
            {
                foreach (var item in modelList)
                {
                    sr.WriteLine($"{item.Entidad};{item.Oficina};{item.Encargado};{item.Serie};" +
                       $"{item.subserie};{item.Gestion};" +
                       $"{item.Archivo};{item.tipologia};" +
                       $"{item.Soporte};{item.DisposicionFinal};" +
                       $"{item.Observaciones};{item.recomendaciones};{item.SeriesModificadas};{item.Funcion};{item.Original};{item.Copia};");
                }
            }
        }

        // GET: Encuesta
        public ActionResult Index()
        {
            if (!System.IO.File.Exists(Server.MapPath("/Entidades.txt")))
                using (System.IO.FileStream fs = System.IO.File.Create(Server.MapPath("/Entidades.txt")))
                {
                }
            else
                CargarModelList();
            TempData["List"] = modelList;
            return View();
        }

        public ActionResult AgregarEntidad()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AgregarEntidad(EncuestaModel model)
        {
            if (model.Entidad != null)
            {
                if (!System.IO.File.Exists(Server.MapPath("/Entidades.txt")))
                    using (System.IO.FileStream fs = System.IO.File.Create(Server.MapPath("/Entidades.txt")))
                    {
                    }
                else
                    CargarModelList();
                foreach (var item in modelList) {
                    if (item.Entidad == model.Entidad)
                    {
                        TempData["ErrorMessage"] = "Esta entidad ya existe";
                        return Json(new
                        {
                            status = "error",
                            success = false
                        });
                    }
                }
                modelList.Add(model);
                modelList = modelList.OrderBy(o => o.Entidad).ThenBy(o => o.Oficina).ThenBy(o => o.Serie).ThenBy(o => o.subserie).ThenBy(o => o.tipologia).ToList();
                RehacerTexto();
                TempData["List"] = modelList;
                TempData["SuccessMessage"] = "Se ha agregado la entidad con éxito";
                return Json(new
                {
                    status = "success",
                    success = true
                });
            }
            else
            {
                TempData["ErrorMessage"] = "Error al agregar entidad, vuélvalo a intenar";
                return Json(new
                {
                    status = "error",
                    success = false
                });
            }

        }


        public ActionResult Oficina(string entidad)
        {
            if (entidad == null )
            {
                return RedirectToAction("Index");
            }

            if (!System.IO.File.Exists(Server.MapPath("/Entidades.txt")))
                using (System.IO.FileStream fs = System.IO.File.Create(Server.MapPath("/Entidades.txt")))
                {
                }
            else
                CargarModelList();
            bool flag = true;
            foreach (var item in modelList)
            {
                if (entidad == item.Entidad)
                {
                    flag = false;
                }
            }
            if (flag)
            {
                return RedirectToAction("Index");
            }
            TempData["List"] = modelList;
            EncuestaModel model = new EncuestaModel();
            model.Entidad = entidad;
            return View(model);
        }

        public ActionResult AgregarOficina(string entidad)
        {
            EncuestaModel model = new EncuestaModel();
            model.Entidad = entidad;
            return View(model);
        }
        [HttpPost]
        public ActionResult AgregarOficina(EncuestaModel model)
        {
            if (model.Oficina != null && model.Encargado != null)
            {
                CargarModelList();
                int index = -1;
                for (int i = 0; i == modelList.Count() - 1; i++)
                {
                    if (modelList[i].Oficina == model.Oficina && model.Entidad == modelList[i].Entidad)
                    {
                        TempData["ErrorMessage"] = "Esta oficina ya existe";
                        return RedirectToAction("Index");
                    }
                    if (model.Entidad == modelList[i].Entidad && modelList[i].Oficina == "")
                    {
                        index = i;
                    }

                }
                if (index != -1)
                {
                    modelList[index].Oficina = model.Oficina;
                    modelList[index].Encargado = model.Encargado;
                }
                else
                {
                    modelList.Add(model);
                }
                modelList = modelList.OrderBy(o => o.Entidad).ThenBy(o => o.Oficina).ThenBy(o => o.Serie).ThenBy(o => o.subserie).ThenBy(o => o.tipologia).ToList();
                RehacerTexto();
                TempData["List"] = modelList;
                TempData["SuccessMessage"] = "Se ha agregado la entidad con éxito";
                return Json(new
                {
                    status = "success",
                    success = true
                });
            }
            else
            {
                TempData["ErrorMessage"] = "Error al agregar oficina productora, vuélvalo a intenar";
                return RedirectToAction("index");
            }

        }

        public ActionResult Encuesta(string entidad, string oficina)
        {
            if (entidad == null || oficina== null)
            {
                return RedirectToAction("Index");
            }

            if (!System.IO.File.Exists(Server.MapPath("/Entidades.txt")))
                using (System.IO.FileStream fs = System.IO.File.Create(Server.MapPath("/Entidades.txt")))
                {
                }
            else
                CargarModelList();
            bool flag = true;
            string encargado = "";
            foreach (var item in modelList)
            {
                if (entidad == item.Entidad && oficina== item.Oficina)
                {
                    encargado = item.Encargado;
                    flag = false;
                }
            }
            if (flag)
            {
                return RedirectToAction("Index");
            }
            TempData["List"] = modelList;
            EncuestaModel model = new EncuestaModel();
            model.Entidad = entidad;
            model.Oficina = oficina;
            model.Encargado =encargado;
            return View(model);
        }

        [HttpPost]
        public ActionResult Encuesta(EncuestaModel model)
        {
            try
            {
                if (model.Serie == null)
                {
                    ModelState.AddModelError("Serie", "La serie es obligatoria");
                    return View(model);
                }                
                if (model.Gestion <=0)
                {
                    ModelState.AddModelError("Gestion", "Los años en gestión no pueden ser negativos ni 0");
                    return View(model);
                }
                if (model.Archivo <= 0)
                {
                    ModelState.AddModelError("Archivo", "Los años en central no pueden ser negativos ni 0");
                    return View(model);
                }
                if (model.Soporte == null)
                {
                    ModelState.AddModelError("Soporte", "El soporte es obligatorio");
                    return View(model);
                }
                if (model.Tipologias[0] == "")
                {
                    ModelState.AddModelError("Tipologias", "por lo menos tiene que existir una tipología");
                    return View(model);
                }
                CargarModelList();
                int index = -1;
                for (int i = 0; i == modelList.Count() - 1; i++)
                {                    
                    if (model.Entidad == modelList[i].Entidad && modelList[i].Oficina == model.Oficina && modelList[i].Serie=="")
                    {
                        index = i;
                    }

                }
                if (index != -1)
                {
                    modelList[index].Serie = model.Serie;
                    modelList[index].subserie = model.subserie;
                    modelList[index].Gestion = model.Gestion;
                    modelList[index].Archivo = model.Archivo;
                    modelList[index].tipologia = model.Tipologias[0];
                    modelList[index].Soporte = model.Soporte;
                    modelList[index].DisposicionFinal = model.DisposicionFinal; 
                    modelList[index].Observaciones = model.Observaciones; 
                    modelList[index].recomendaciones = model.recomendaciones;
                    modelList[index].SeriesModificadas = model.SeriesModificadas;
                    modelList[index].Funcion = model.Funcion;
                    modelList[index].Original = model.Original;
                    modelList[index].Copia = model.Copia;
                    if (model.Tipologias.Count > 1)
                    {
                        for(int x = 1; x < model.Tipologias.Count; x++)
                        {
                            modelList.Add(new EncuestaModel
                            {
                                Entidad=model.Entidad,
                                Oficina=model.Oficina,
                                Encargado=model.Encargado,
                                Serie = model.Serie,
                                subserie = model.subserie,
                                Gestion = model.Gestion,
                                Archivo = model.Archivo,
                                tipologia = model.Tipologias[x],
                                Soporte = model.Soporte,
                                DisposicionFinal = model.DisposicionFinal,
                                Observaciones = model.Observaciones,
                                recomendaciones = model.recomendaciones,
                                SeriesModificadas = model.SeriesModificadas,
                                Funcion = model.Funcion,
                                Original = model.Original,
                                Copia = model.Copia,
                            });
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < model.Tipologias.Count; x++)
                    {
                        modelList.Add(new EncuestaModel
                        {
                            Entidad = model.Entidad,
                            Oficina = model.Oficina,
                            Encargado = model.Encargado,
                            Serie = model.Serie,
                            subserie = model.subserie,
                            Gestion = model.Gestion,
                            Archivo = model.Archivo,
                            tipologia = model.Tipologias[x],
                            Soporte = model.Soporte,
                            DisposicionFinal = model.DisposicionFinal,
                            Observaciones = model.Observaciones,
                            recomendaciones = model.recomendaciones,
                            SeriesModificadas = model.SeriesModificadas,
                            Funcion = model.Funcion,
                            Original = model.Original,
                            Copia = model.Copia,
                        });
                    }
                }
                modelList = modelList.OrderBy(o => o.Entidad).ThenBy(o => o.Oficina).ThenBy(o => o.Serie).ThenBy(o => o.subserie).ThenBy(o => o.tipologia).ToList();
                RehacerTexto();
                TempData["List"] = modelList;

                TempData["SuccessMessage"] = "Se ha agregado exitosamente el registro";
                EncuestaModel encuesta = new EncuestaModel();
                encuesta.Entidad = model.Entidad;
                encuesta.Oficina = model.Oficina;
                encuesta.Encargado = model.Encargado;
                return View(model);
            }
            catch
            {
                TempData["ErrorMessage"] = "Error, inténtelo de nuevo";
                return View(model);
            }
        }
        public JsonResult Serie(string Prefix)
        {
            CargarModelList();
            //Searching records from list using LINQ query  
            var NombreOficina = (from N in modelList
                                 where N.Serie.Contains(Prefix)
                                 select new { N.Serie}).Distinct();
            return Json(NombreOficina, JsonRequestBehavior.AllowGet);
        }
    }

    
}