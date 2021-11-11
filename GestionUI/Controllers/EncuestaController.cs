using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Packaging;
using GestionUI.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace GestionUI.Controllers
{
    [System.Web.Mvc.Authorize]
    public class EncuestaController : Controller
    {
        List<EncuestaModel> modelList = new List<EncuestaModel>();

        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }


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
                        CodigOficina = int.Parse(TRDList[2]),
                        Encargado = TRDList[3],
                        cargo = TRDList[4],
                        Serie = TRDList[5],
                        CodigoSerie = int.Parse(TRDList[6]),
                        subserie = TRDList[7],
                        CodigoSubserie = int.Parse(TRDList[8]),
                        Gestion = int.Parse(TRDList[9]),
                        Archivo = int.Parse(TRDList[10]),
                        tipologia = TRDList[11],
                        Soporte = TRDList[12],
                        DisposicionFinal = TRDList[13],
                        Observaciones = TRDList[14],
                        recomendaciones = TRDList[15],
                        SeriesModificadas = TRDList[16],
                        Funcion = TRDList[17],
                        Procedimiento = TRDList[18],
                        Sistema = TRDList[19],
                        Clasificacion = TRDList[20],
                        Rol = TRDList[21],
                        Proceso = TRDList[22],
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
                    sr.WriteLine($"{item.Entidad};{item.Oficina};{item.CodigOficina};{item.Encargado};{item.cargo};{item.Serie};{item.CodigoSerie};" +
                       $"{item.subserie};{item.CodigoSubserie};{item.Gestion};" +
                       $"{item.Archivo};{item.tipologia};" +
                       $"{item.Soporte};{item.DisposicionFinal};" +
                       $"{item.Observaciones};{item.recomendaciones};{item.SeriesModificadas};{item.Funcion};{item.Procedimiento};{item.Sistema};{item.Clasificacion};{item.Rol};" +
                       $"{item.Proceso};");
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
                foreach (var item in modelList)
                {
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
                modelList = modelList.OrderBy(o => o.Entidad).ThenBy(o => o.Oficina).ThenBy(o => o.Serie).ThenBy(o => o.subserie).ToList();
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

        [HttpPost]
        public ActionResult Eliminar(string entidad, string oficina, string serie, string subserie, string tipologia)
        {
            CargarModelList();
            modelList.RemoveAll(x => x.Entidad == entidad && x.Oficina == oficina && x.Serie == serie && x.subserie == subserie && x.tipologia == tipologia);
            modelList = modelList.OrderBy(o => o.Entidad).ThenBy(o => o.Oficina).ThenBy(o => o.Serie).ThenBy(o => o.subserie).ToList();
            RehacerTexto();
            TempData["List"] = modelList;
            TempData["SuccessMessage"] = "Se ha eliminado el campo con éxito";
            return Json(new
            {
                status = "success",
                success = true
            });
        }

        public ActionResult Oficina(string entidad)
        {
            if (entidad == null)
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
            EncuestaModel model = new EncuestaModel() { Entidad = entidad };
            return View(model);
        }

        public ActionResult AgregarOficina(string entidad)
        {
            EncuestaModel model = new EncuestaModel();
            model.Entidad = entidad;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarOficina(EncuestaModel model)
        {
            if (model.Oficina != null && model.Encargado != null)
            {
                CargarModelList();
                int index = -1;
                for (int i = 0; i < modelList.Count - 1; i++)
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
                    modelList[index].CodigOficina = model.CodigOficina;
                    modelList[index].Encargado = model.Encargado;
                    modelList[index].cargo = model.cargo;
                }
                else
                {
                    modelList.Add(model);
                }
                modelList = modelList.OrderBy(o => o.Entidad).ThenBy(o => o.Oficina).ThenBy(o => o.Serie).ThenBy(o => o.subserie).ToList();
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

        public ActionResult Editar(string entidad, string oficina, string serie, string subserie)
        {
            CargarModelList();
            var encuesta = (from N in modelList
                            where N.Entidad == entidad && N.Oficina == oficina && N.Serie == serie && N.subserie == subserie
                            select N
                        );
            List<EncuestaModel> temp = new List<EncuestaModel>();
            string seriet = "", subseriet = "asdfghjkl";
            int cont = -1;
            foreach (var x in encuesta)
            {
                if (seriet == "" || seriet != x.Serie)
                {
                    cont++;
                    temp.Add(x);
                    if (temp[cont].Tipologias == null)
                        temp[cont].Tipologias = new List<string>();
                    temp[cont].Tipologias.Add(x.tipologia);

                }
                else
                {
                    if (subseriet != x.subserie)
                    {
                        cont++;
                        temp.Add(x);
                        if (temp[cont].Tipologias == null)
                            temp[cont].Tipologias = new List<string>();
                        temp[cont].Tipologias.Add(x.tipologia);
                    }
                    else
                    {
                        temp[cont].Tipologias.Add(x.tipologia);
                    }
                }
                seriet = x.Serie;
                subseriet = x.subserie;
            }
            TempData["model"] = temp[0];
            return View(temp[0]);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(EncuestaModel model, List<string> Orig, List<string> Cop, List<string> formato)
        {
            if (model.Serie == null)
            {
                ModelState.AddModelError("Serie", "La serie es obligatoria");
                return View(model);
            }
            if (model.Gestion < 0)
            {
                ModelState.AddModelError("Gestion", "Los anos en gestión no pueden ser negativos");
                return View(model);
            }
            if (model.Archivo < 0)
            {
                ModelState.AddModelError("Archivo", "Los anos en central no pueden ser negativos");
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
            if (model.SeriesModificadas == null)
                model.SeriesModificadas = "";
            if (model.Observaciones == null)
                model.Observaciones = "";
            if (model.recomendaciones == null)
                model.recomendaciones = "";
            if (model.Procedimiento == null)
                model.Procedimiento = "";
            if (model.Funcion == null)
                model.Funcion = "";
            EncuestaModel anterior = (EncuestaModel)TempData["model"];
            CargarModelList();
            if (anterior.Oficina != model.Oficina)
            {
                foreach (var item in modelList)
                {
                    if (item.Oficina == anterior.Oficina && item.Entidad == anterior.Entidad)
                    {
                        item.Oficina = model.Oficina;
                    }
                }
            }
            if (anterior.CodigOficina != model.CodigOficina)
            {
                foreach (var item in modelList)
                {
                    if (item.CodigOficina == anterior.CodigOficina && item.Entidad == anterior.Entidad)
                    {
                        item.CodigOficina = model.CodigOficina;
                    }
                }
            }
            if (anterior.Encargado != model.Encargado)
            {
                foreach (var item in modelList)
                {
                    if (item.Encargado == anterior.Encargado && item.Entidad == anterior.Entidad && item.Oficina == model.Oficina)
                    {
                        item.Encargado = model.Encargado;
                    }
                }
            }
            if (anterior.cargo != model.cargo)
            {
                foreach (var item in modelList)
                {
                    if (item.cargo == anterior.cargo && item.Entidad == anterior.Entidad && item.Oficina == model.Oficina)
                    {
                        item.cargo = model.cargo;
                    }
                }
            }
            if (anterior.Serie != model.Serie)
            {
                foreach (var item in modelList)
                {
                    if (item.Serie == anterior.Serie && item.Entidad == anterior.Entidad && item.Oficina == model.Oficina)
                    {
                        item.Serie = model.Serie;
                    }
                }
            }
            modelList.RemoveAll(x => x.subserie == anterior.subserie && x.Entidad == anterior.Entidad && x.Oficina == model.Oficina && x.Serie == model.Serie);
            int index = -1;
            for (int i = 0; i < modelList.Count; i++)
            {
                if (model.Entidad == modelList[i].Entidad && modelList[i].Oficina == model.Oficina && modelList[i].Serie == "")
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
                if (Orig != null)
                    if (Orig[0] == 0.ToString())
                    {
                        modelList[index].tipologia = modelList[index].tipologia + ", O,";
                    }
                if (Cop != null)
                    if (Cop[0] == 0.ToString())
                    {
                        modelList[index].tipologia = modelList[index].tipologia + ", C,";
                    }
                modelList[index].tipologia = modelList[index].tipologia + formato[0];
                modelList[index].Soporte = model.Soporte;
                modelList[index].DisposicionFinal = model.DisposicionFinal;
                modelList[index].Observaciones = model.Observaciones.Replace(Environment.NewLine, @" \n ");
                modelList[index].recomendaciones = model.recomendaciones.Replace(Environment.NewLine, @" \n ");
                modelList[index].SeriesModificadas = model.SeriesModificadas.Replace(Environment.NewLine, @" \n ");
                modelList[index].Funcion = model.Funcion.Replace(Environment.NewLine, @" \n ");
                modelList[index].Procedimiento = model.Procedimiento.Replace(Environment.NewLine, @" \n ");
                modelList[index].Sistema = model.Sistema.Replace(Environment.NewLine, @" \n ");
                modelList[index].Rol = model.Rol.Replace(Environment.NewLine, @" \n ");
                modelList[index].Clasificacion = model.Clasificacion.Replace(Environment.NewLine, @" \n ");
                modelList[index].Proceso = model.Proceso.Replace(Environment.NewLine, @" \n ");
                if (model.Tipologias.Count > 1)
                {
                    for (int x = 1; x < model.Tipologias.Count; x++)
                    {
                        string original = "", copia = "";
                        bool o = false;
                        if (Orig != null)
                            o = Orig.Any(s => s == x.ToString());
                        if (o)
                            original = " O,";
                        o = false;
                        if (Cop != null)
                            o = Cop.Any(s => s == x.ToString());
                        if (o)
                            copia = " C,";
                        string tip = model.Tipologias[x] + "," + original + copia + formato[x];
                        if (model.SeriesModificadas == null)
                            model.SeriesModificadas = "";
                        if (model.Observaciones == null)
                            model.Observaciones = "";
                        if (model.recomendaciones == null)
                            model.recomendaciones = "";
                        if (model.Procedimiento == null)
                            model.Procedimiento = "";
                        if (model.Funcion == null)
                            model.Funcion = "";
                        modelList.Add(new EncuestaModel
                        {
                            Entidad = model.Entidad,
                            Oficina = model.Oficina,
                            CodigOficina = model.CodigOficina,
                            Encargado = model.Encargado,
                            cargo = model.cargo,
                            Serie = model.Serie,
                            subserie = model.subserie,
                            Gestion = model.Gestion,
                            Archivo = model.Archivo,
                            tipologia = tip,
                            Soporte = model.Soporte,
                            DisposicionFinal = model.DisposicionFinal,
                            Observaciones = model.Observaciones.Replace(Environment.NewLine, @" \n "),
                            recomendaciones = model.recomendaciones.Replace(Environment.NewLine, @" \n "),
                            SeriesModificadas = model.SeriesModificadas.Replace(Environment.NewLine, @" \n "),
                            Funcion = model.Funcion.Replace(Environment.NewLine, @" \n "),
                            Procedimiento = model.Procedimiento.Replace(Environment.NewLine, @" \n "),
                            Sistema = model.Sistema.Replace(Environment.NewLine, @" \n "),
                            Rol = model.Rol.Replace(Environment.NewLine, @" \n "),
                            Clasificacion = model.Clasificacion.Replace(Environment.NewLine, @" \n "),
                            Proceso = model.Proceso.Replace(Environment.NewLine, @" \n "),
                        });
                    }
                }
            }
            else
            {
                for (int x = 0; x < model.Tipologias.Count; x++)
                {
                    string original = "", copia = "";
                    bool o = false;
                    if (Orig != null)
                        o = Orig.Any(s => s == x.ToString());
                    if (o)
                        original = " O,";
                    o = false;
                    if (Cop != null)
                        o = Cop.Any(s => s == x.ToString());
                    if (o)
                        copia = " C,";
                    string tip = model.Tipologias[x] + "," + original + copia + formato[x];

                    modelList.Add(new EncuestaModel
                    {
                        Entidad = model.Entidad,
                        Oficina = model.Oficina,
                        CodigOficina = model.CodigOficina,
                        Encargado = model.Encargado,
                        cargo = model.cargo,
                        Serie = model.Serie,
                        subserie = model.subserie,
                        Gestion = model.Gestion,
                        Archivo = model.Archivo,
                        tipologia = tip,
                        Soporte = model.Soporte,
                        DisposicionFinal = model.DisposicionFinal,
                        Observaciones = model.Observaciones.Replace(Environment.NewLine, @" \n "),
                        recomendaciones = model.recomendaciones.Replace(Environment.NewLine, @" \n "),
                        SeriesModificadas = model.SeriesModificadas.Replace(Environment.NewLine, @" \n "),
                        Funcion = model.Funcion.Replace(Environment.NewLine, @" \n "),
                        Procedimiento = model.Procedimiento.Replace(Environment.NewLine, @" \n "),
                        Sistema = model.Sistema.Replace(Environment.NewLine, @" \n "),
                        Rol = model.Rol.Replace(Environment.NewLine, @" \n "),
                        Clasificacion = model.Clasificacion.Replace(Environment.NewLine, @" \n "),
                        Proceso = model.Proceso.Replace(Environment.NewLine, @" \n "),
                    });
                }
            }
            modelList = modelList.OrderBy(o => o.Entidad).ThenBy(o => o.Oficina).ThenBy(o => o.Serie).ThenBy(o => o.subserie).ToList();
            RehacerTexto();
            TempData["List"] = modelList;


            TempData["SuccessMessage"] = "Se ha Editado exitosamente el registro";
            EncuestaModel encuesta = new EncuestaModel();
            encuesta.Entidad = model.Entidad;
            encuesta.Oficina = model.Oficina;
            encuesta.Encargado = model.Encargado;
            return RedirectToAction("Encuesta", new { entidad = model.Entidad, oficina = model.Oficina });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TRD(EncuestaModel model)
        {
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Hoja1");

            workSheet.PrinterSettings.Orientation = eOrientation.Landscape;
            workSheet.PrinterSettings.FitToWidth = 1;
            workSheet.Cells["A1:S2"].Style.Font.Name = "Arial";
            workSheet.Cells["A1:S2"].Style.Font.Size = 7;
            workSheet.Cells["A1:S2"].Style.WrapText = false;
            workSheet.Cells["A1:S4"].Style.Font.Bold = true;
            workSheet.Cells["A1:C2"].Merge = true;
            workSheet.Cells["A1:C2"].Value = "OFICINA PRODUCTORA";
            workSheet.Cells["A1:C2"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["A1:C2"].Style.VerticalAlignment=ExcelVerticalAlignment.Center;
            workSheet.Cells["A1:C2"].Style.HorizontalAlignment=ExcelHorizontalAlignment.Center;
            workSheet.Cells["D1:R2"].Merge = true;
            workSheet.Cells["D1:R2"].Value = model.CodigOficina+" "+model.Oficina;
            workSheet.Cells["D1:R2"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["D1:R2"].Style.VerticalAlignment=ExcelVerticalAlignment.Center;
            workSheet.Cells["D1:R2"].Style.HorizontalAlignment=ExcelHorizontalAlignment.Center;
            workSheet.Cells["A3:S4"].Style.Font.Name = "Arial";
            workSheet.Cells["A3:S4"].Style.Font.Size = 6;
            workSheet.Cells["A3:S4"].Style.WrapText = false;
            workSheet.Cells["A3:S4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells["A3:S4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            
            workSheet.Cells["A3:C3"].Merge = true;
            workSheet.Cells["A3:C3"].Value = "CÓDIGO";
            workSheet.Cells["A3:C3"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["A4"].Value = "D";            
            workSheet.Cells["A4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["B4"].Value = "S";
            workSheet.Cells["B4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["c4"].Value = "Sb";
            workSheet.Cells["C4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["D3:E4"].Merge = true;
            workSheet.Cells["D3:E4"].Value = "SERIES Y TIPOS DOCUMENTALES";
            workSheet.Cells["D3:E4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["F3:G3"].Merge = true;
            workSheet.Cells["F3:G3"].Value = "RETENCIÓN EN AÑOS";
            workSheet.Cells["F3:G3"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["F4"].Value = "Gestión";
            workSheet.Cells["F4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["F4"].Style.TextRotation = 90;
            workSheet.Cells["G4"].Value = "Central";
            workSheet.Cells["G4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["G4"].Style.TextRotation = 90;
            workSheet.Cells["H3:I3"].Merge = true;
            workSheet.Cells["H3:I3"].Value = "SOPORTE/FORMATO";
            workSheet.Cells["H3:I3"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["H4"].Value = "Papel";
            workSheet.Cells["H4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["I4"].Merge = true;
            workSheet.Cells["I4"].Value = "Electrónico";
            workSheet.Cells["I4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["J3:K3"].Merge = true;
            workSheet.Cells["J3:K3"].Value = "TRADICIÓN DOC";
            workSheet.Cells["J3:K3"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["J4"].Value = "ORIGINAL";
            workSheet.Cells["J4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["J4"].Style.TextRotation = 90;
            workSheet.Cells["K4"].Value = "COPIA";
            workSheet.Cells["K4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["K4"].Style.TextRotation = 90;
            workSheet.Cells["L3:O3"].Merge = true;
            workSheet.Cells["L3:O3"].Value = "DISPOSICIÓN FINAL";
            workSheet.Cells["L3:O3"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["L4"].Value = "CT";
            workSheet.Cells["L4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["M4"].Value = "E";
            workSheet.Cells["M4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["N4"].Value = "M";
            workSheet.Cells["N4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["O4"].Value = "S";
            workSheet.Cells["O4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["P3:P4"].Merge = true;
            workSheet.Cells["P3:P4"].Value = "PROCEDIMIENTO";
            workSheet.Cells["P3:P4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["Q3:Q4"].Merge = true;
            workSheet.Cells["Q3:Q4"].Value = "FUNCIÓN";
            workSheet.Cells["Q3:Q4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["R3:R4"].Merge = true;
            workSheet.Cells["R3:R4"].Value = "PROCESO";
            workSheet.Cells["R3:R4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            workSheet.Cells["A3:R4"].AutoFitColumns();
            CargarModelList();
            var encuesta = (from N in modelList
                            where N.Entidad == model.Entidad && N.Oficina == model.Oficina
                            select N
                        );
            List<EncuestaModel> temp = new List<EncuestaModel>();

            var conseries = (from N in modelList
                             where N.Entidad == model.Entidad
                             select new { N.Serie }).Distinct().ToList();
            int cont;
            conseries = conseries.OrderBy(o => o.Serie).ToList();
            List<EncuestaModel> codseries = new List<EncuestaModel>();
            for (cont = 1; cont <= conseries.Count(); cont++)
            {
                codseries.Add(new EncuestaModel() { CodigoSerie = cont, Serie = conseries[cont - 1].Serie });
            }
            var workSheet2 = excel.Workbook.Worksheets.Add("Hoja2");
            workSheet2.Cells[1, 1].Value = "Código Serie";
            workSheet2.Cells[1, 2].Value = "Nombre Serie";
            workSheet2.Cells[1, 4].Value = "Nombre Serie";
            workSheet2.Cells[1, 5].Value = "Código Subserie";
            workSheet2.Cells[1, 6].Value = "Nombre Subserie";
            for (int i = 0; i < codseries.Count(); i++)
            {
                workSheet2.Cells[i + 2, 1].Value = codseries[i].CodigoSerie;
                workSheet2.Cells[i + 2, 2].Value = codseries[i].Serie;
                workSheet2.Cells[i + 2, 1, i + 1, 2].AutoFitColumns();
            }
            var consubseries = (from N in modelList
                                where N.Entidad == model.Entidad
                                select new { N.subserie, N.Serie }).Distinct().ToList();
            consubseries = consubseries.OrderBy(o => o.Serie).ThenBy(o => o.subserie).ToList();
            List<EncuestaModel> codsubseries = new List<EncuestaModel>();
            int contsub = 1;
            for (cont = 1; cont <= consubseries.Count(); cont++)
            {
                if (cont - 2 >= 0)
                    if (consubseries[cont - 1].Serie != consubseries[cont - 2].Serie)
                        contsub = 1;
                codsubseries.Add(new EncuestaModel() { CodigoSubserie = contsub , subserie = consubseries[cont - 1].subserie,Serie= consubseries[cont - 1].Serie });
                contsub++;
            }

            for (int i = 0; i < codsubseries.Count(); i++)
            {
                workSheet2.Cells[i + 2, 5].Value = codsubseries[i].CodigoSubserie;
                workSheet2.Cells[i + 2, 6].Value = codsubseries[i].subserie;
                workSheet2.Cells[i + 2, 4].Value = codsubseries[i].Serie;
                workSheet2.Cells[i + 2, 4, i + 1, 6].AutoFitColumns();
            }

            string serie = "", subserie = "asdfghjkl";
            cont = -1;
            foreach (var x in encuesta)
            {
                if (serie == "" || serie != x.Serie)
                {
                    cont++;
                    temp.Add(x);
                    if (temp[cont].Tipologias == null)
                        temp[cont].Tipologias = new List<string>();
                    temp[cont].Tipologias.Add(x.tipologia);

                }
                else
                {
                    if (subserie != x.subserie)
                    {
                        cont++;
                        temp.Add(x);
                        if (temp[cont].Tipologias == null)
                            temp[cont].Tipologias = new List<string>();
                        temp[cont].Tipologias.Add(x.tipologia);
                    }
                    else
                    {
                        temp[cont].Tipologias.Add(x.tipologia);
                    }
                }
                serie = x.Serie;
                subserie = x.subserie;
            }
            serie = "";
            int RowCount = 5;
            for (int i = 0; i < temp.Count; i++)
            {
                string[] subs;
                if (serie == "" || serie != temp[i].Serie)
                {
                    RowCount = RowCount + 1;
                    int inicio = RowCount;
                    workSheet.Cells[(RowCount), 1].Value = temp[i].CodigOficina;
                    workSheet.Cells[(RowCount), 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[(RowCount), 1].Style.Font.Bold = true;
                    workSheet.Cells[(RowCount), 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    workSheet.Cells[(RowCount), 2].Value =
                        (from N in codseries where N.Serie == temp[i].Serie select N.CodigoSerie).ToList()[0];
                    workSheet.Cells[(RowCount), 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[(RowCount), 2].Style.Font.Bold = true;
                    workSheet.Cells[(RowCount), 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    workSheet.Cells[(RowCount), 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    workSheet.Cells[(RowCount), 4, (RowCount), 18].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    workSheet.Cells[(RowCount), 4, (RowCount), 18].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells[(RowCount), 4, (RowCount), 18].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                    workSheet.Cells[(RowCount), 4, (RowCount), 18].Merge = true;
                    subs = temp[i].Serie.Split('-');
                    if (subs.Count() > 1)
                    {
                        workSheet.Cells[(RowCount), 4, (RowCount), 15].Value = subs[1].Trim();
                    }
                    else
                        workSheet.Cells[(RowCount), 4, (RowCount), 18].Value = subs[0].Trim();

                    workSheet.Cells[(RowCount), 4, (RowCount), 16].Style.Font.Bold = true;
                    workSheet.Cells[(RowCount), 4, (RowCount), 17].Style.Font.Bold = true;
                    workSheet.Cells[(RowCount), 4, (RowCount), 18].Style.Font.Bold = true;
                    workSheet.Cells[(RowCount), 4, (RowCount), 18].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    serie = temp[i].Serie;
                    RowCount = RowCount + 1;
                }
                workSheet.Cells[(RowCount), 1, (RowCount), 18].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[(RowCount), 1].Value = temp[i].CodigOficina;
                workSheet.Cells[(RowCount), 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[(RowCount), 1].Style.Font.Bold = true;
                workSheet.Cells[(RowCount), 1, (temp[i].Tipologias.Count + RowCount), 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[(RowCount), 2].Value =
                       (from N in codseries where N.Serie == temp[i].Serie select N.CodigoSerie).ToList()[0];
                workSheet.Cells[(RowCount), 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[(RowCount), 2].Style.Font.Bold = true;
               
                workSheet.Cells[(RowCount), 2, (temp[i].Tipologias.Count + RowCount), 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[(RowCount), 3, (temp[i].Tipologias.Count + RowCount), 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                subs = temp[i].subserie.Split('-');
                workSheet.Cells[(RowCount), 4, (RowCount), 5].Merge = true;
                workSheet.Cells[(RowCount), 4, (RowCount), 5].Style.Font.Bold = true;
                workSheet.Cells[(RowCount), 4, (RowCount), 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[(RowCount), 4, (RowCount), 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                if (subs.Count() > 1)
                {
                    workSheet.Cells[(RowCount), 4, (RowCount), 5].Value = subs[1].Trim();

                    if (!string.IsNullOrEmpty(subs[1]))
                    {
                        
                        workSheet.Cells[(RowCount), 3].Value =
                      (from N in codsubseries where (N.Serie == temp[i].Serie & N.subserie == temp[i].subserie) select N.CodigoSubserie).ToList()[0];
                    }
                    else
                    {
                        workSheet.Cells[(RowCount), 4, (RowCount), 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                        workSheet.Cells[(RowCount), 4, (RowCount), 5].Value = temp[i].Serie ;
                    }
                }
                else
                {
                    workSheet.Cells[(RowCount), 4, (RowCount), 5].Value = subs[0].Trim();
                    if (!string.IsNullOrEmpty(subs[0]))
                    {                        
                        workSheet.Cells[(RowCount), 3].Value =
                     (from N in codsubseries where (N.Serie == temp[i].Serie & N.subserie == temp[i].subserie) select N.CodigoSubserie).ToList()[0];
                    }
                    else
                    {
                        workSheet.Cells[(RowCount), 4, (RowCount), 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                        workSheet.Cells[(RowCount), 4, (RowCount), 5].Value = temp[i].Serie;
                    }
                }
                
                
                
                workSheet.Cells[(RowCount), 4, (temp[i].Tipologias.Count + RowCount), 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[(RowCount), 6].Value = temp[i].Gestion;
                workSheet.Cells[(RowCount), 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[(RowCount), 6].Style.Font.Bold = true;
                workSheet.Cells[(RowCount), 6, (temp[i].Tipologias.Count + RowCount), 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[(RowCount), 7].Value = temp[i].Archivo;
                workSheet.Cells[(RowCount), 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[(RowCount), 7].Style.Font.Bold = true;
                workSheet.Cells[(RowCount), 7, (temp[i].Tipologias.Count + RowCount), 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                workSheet.Cells[(RowCount), 8, (RowCount), 9].Merge = true;
                workSheet.Cells[(RowCount), 8, (RowCount), 9].Value = temp[i].Soporte;
                workSheet.Cells[(RowCount), 8, (RowCount), 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[(RowCount), 8, (RowCount), 9].Style.Font.Bold = true;
                workSheet.Cells[(RowCount), 10, (temp[i].Tipologias.Count + RowCount), 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[(RowCount), 11, (temp[i].Tipologias.Count + RowCount), 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[(RowCount), 12, (temp[i].Tipologias.Count + RowCount), 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[(RowCount), 13, (temp[i].Tipologias.Count + RowCount), 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[(RowCount), 14, (temp[i].Tipologias.Count + RowCount), 15].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[(RowCount), 15, (temp[i].Tipologias.Count + RowCount), 16].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                if (temp[i].DisposicionFinal.Contains("CT"))
                {
                    workSheet.Cells[(RowCount), 12].Value = "X";
                    workSheet.Cells[(RowCount), 12].Style.Font.Bold = true;
                    workSheet.Cells[(RowCount), 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                if (temp[i].DisposicionFinal.Contains("E"))
                {
                    workSheet.Cells[(RowCount), 13].Value = "X";
                    workSheet.Cells[(RowCount), 13].Style.Font.Bold = true;
                    workSheet.Cells[(RowCount), 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                if (temp[i].DisposicionFinal.Contains("M"))
                {
                    workSheet.Cells[(RowCount), 14].Value = "X";
                    workSheet.Cells[(RowCount), 14].Style.Font.Bold = true;
                    workSheet.Cells[(RowCount), 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                if (temp[i].DisposicionFinal.Contains("S"))
                {
                    workSheet.Cells[(RowCount), 15].Value = "X";
                    workSheet.Cells[(RowCount), 15].Style.Font.Bold = true;
                    workSheet.Cells[(RowCount), 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                workSheet.Cells[(RowCount), 16, (temp[i].Tipologias.Count + RowCount), 16].Merge = true;
                workSheet.Cells[(RowCount), 16, (temp[i].Tipologias.Count + RowCount), 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Justify;
                workSheet.Cells[(RowCount), 16, (temp[i].Tipologias.Count + RowCount), 16].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                workSheet.Cells[(RowCount), 17, (temp[i].Tipologias.Count + RowCount), 17].Merge = true;
                workSheet.Cells[(RowCount), 17, (temp[i].Tipologias.Count + RowCount), 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Justify;
                workSheet.Cells[(RowCount), 17, (temp[i].Tipologias.Count + RowCount), 17].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                workSheet.Cells[(RowCount), 18, (temp[i].Tipologias.Count + RowCount), 18].Merge = true;
                workSheet.Cells[(RowCount), 16, (temp[i].Tipologias.Count + RowCount), 16].Value = temp[i].Procedimiento;
                workSheet.Cells[(RowCount), 17, (temp[i].Tipologias.Count + RowCount), 17].Value = temp[i].Funcion;
                workSheet.Cells[(RowCount), 18, (temp[i].Tipologias.Count + RowCount), 18].Value = temp[i].Proceso;
                workSheet.Cells[(RowCount), 18, (temp[i].Tipologias.Count + RowCount), 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Justify;
                workSheet.Cells[(RowCount), 18, (temp[i].Tipologias.Count + RowCount), 18].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[(RowCount), 16, (temp[i].Tipologias.Count + RowCount), 16].Style.WrapText = true;
                workSheet.Cells[(RowCount), 17, (temp[i].Tipologias.Count + RowCount), 17].Style.WrapText = true;
                workSheet.Cells[(RowCount), 18, (temp[i].Tipologias.Count + RowCount), 18].Style.WrapText = true;
                workSheet.Cells[(RowCount), 16, (temp[i].Tipologias.Count + RowCount), 16].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[(RowCount), 17, (temp[i].Tipologias.Count + RowCount), 17].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                workSheet.Cells[(RowCount), 18, (temp[i].Tipologias.Count + RowCount), 18].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                RowCount = RowCount + 1;
                for (int y = 0; y < temp[i].Tipologias.Count; y++)
                {
                    subs = temp[i].Tipologias[y].Split(',');
                    workSheet.Cells[(RowCount), 4, (RowCount), 5].Merge = true;
                    workSheet.Cells[(RowCount), 4, (RowCount), 5].Value = subs[0].Trim();
                    workSheet.Cells[(RowCount), 8, (RowCount), 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[(RowCount), 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    if (subs[subs.Count() - 1].Trim().Contains("Papel"))
                    {
                        workSheet.Cells[(RowCount), 8].Value = "X";
                    }
                    if (subs[subs.Count() - 1].Trim().Contains("Electrónico"))
                    {
                        
                        workSheet.Cells[(RowCount), 9].Value = "X";
                    }

                    //workSheet.Cells[(RowCount), 8, (RowCount), 10].Merge = true;
                    //workSheet.Cells[(RowCount), 8, (RowCount), 10].Value = subs[subs.Count() - 1].Trim();
                    if (y == temp[i].Tipologias.Count - 1)
                        workSheet.Cells[(RowCount), 8, (RowCount), 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    if (subs.Count() > 3)
                    {
                        workSheet.Cells[(RowCount), 10].Value = "X";
                        workSheet.Cells[(RowCount), 11].Value = "X";
                        workSheet.Cells[(RowCount), 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[(RowCount), 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    }
                    else
                    {
                        if (subs.Count() > 2)
                        {
                            if (subs[1].Contains("O"))
                            {
                                workSheet.Cells[(RowCount), 10].Value = "X";
                                workSheet.Cells[(RowCount), 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }
                            else
                            {
                                workSheet.Cells[(RowCount), 11].Value = "X";
                                workSheet.Cells[(RowCount), 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }
                        }


                    }
                    RowCount = RowCount + 1;
                }

            }
            workSheet.Cells["A3:S1000"].Style.Font.Name = "Arial";
            workSheet.Cells["A3:S1000"].Style.Font.Size = 9;
            workSheet.Column(1).Width = 6;
            workSheet.Column(2).Width = 6;
            workSheet.Column(3).Width = 6;
            workSheet.Column(6).Width = 5;
            workSheet.Column(7).Width = 5;
            workSheet.Column(8).Width = 5;
            workSheet.Column(9).Width = 5;
            workSheet.Column(10).Width = 5;
            workSheet.Column(11).Width = 5;
            workSheet.Column(12).Width = 5;
            workSheet.Column(13).Width = 5;
            workSheet.Column(14).Width = 5;
            workSheet.Column(15).Width = 5;
            workSheet.Column(16).Width = 37;
            workSheet.Column(17).Width = 20;
            workSheet.Column(18).Width = 20;
            workSheet.Column(19).Width = 20;
            using (var memoryStream = new MemoryStream())
            {
                string nombre = string.Format("attachment; filename=TRD " + Regex.Replace(model.Oficina, @"[^0-9a-zA-Z]+", " ") + ".xlsx");
                Response.ClearContent();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", nombre);
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }

            return RedirectToAction("Encuesta", new { entidad = model.Entidad, oficina = model.Oficina });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CCD(EncuestaModel model)
        {
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Hoja1");

            workSheet.PrinterSettings.Orientation = eOrientation.Landscape;
            workSheet.PrinterSettings.FitToWidth = 1;
            workSheet.Cells["A1:S1"].Style.Font.Name = "Calibri";
            workSheet.Cells["A1:S1"].Style.Font.Size = 12;
            workSheet.Cells["A1:S1"].Style.WrapText = true;
            workSheet.Cells["A1:S1"].Style.Font.Bold = true;
            workSheet.Cells["A1:S1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells["A1:S1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells["A1"].Value = "Código de oficina productora";
            workSheet.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["B1"].Value = "Oficina productora";
            workSheet.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["c1"].Value = "Código Serie documental";
            workSheet.Cells["C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["D1"].Value = "Nombre de serie documental";
            workSheet.Cells["D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["E1"].Value = "Código de subserie documental";
            workSheet.Cells["E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["F1"].Value = "Nombre de subserie documental";
            workSheet.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            workSheet.Cells["G1"].Value = "Tipologías documentals";
            workSheet.Cells["G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            workSheet.Column(2).Width = 30;
            workSheet.Column(4).Width = 30;
            workSheet.Column(6).Width = 30;
            workSheet.Column(7).Width = 30;
            CargarModelList();
            var encuesta = (from N in modelList
                            where N.Entidad == model.Entidad && N.Oficina == model.Oficina
                            select N
                        );
            List<EncuestaModel> temp = new List<EncuestaModel>();

            var conseries = (from N in modelList
                             where N.Entidad == model.Entidad
                             select new { N.Serie }).Distinct().ToList();
            int cont;
            conseries = conseries.OrderBy(o => o.Serie).ToList();
            List<EncuestaModel> codseries = new List<EncuestaModel>();
            for (cont = 1; cont <= conseries.Count(); cont++)
            {
                codseries.Add(new EncuestaModel() { CodigoSerie = cont, Serie = conseries[cont - 1].Serie });
            }
            var workSheet2 = excel.Workbook.Worksheets.Add("Hoja2");
            workSheet2.Cells[1, 1].Value = "Código Serie";
            workSheet2.Cells[1, 2].Value = "Nombre Serie";
            workSheet2.Cells[1, 4].Value = "Código Subserie";
            workSheet2.Cells[1, 5].Value = "Nombre Subserie";
            for (int i = 0; i < codseries.Count(); i++)
            {
                workSheet2.Cells[i + 2, 1].Value = codseries[i].CodigoSerie;
                workSheet2.Cells[i + 2, 2].Value = codseries[i].Serie;
                workSheet2.Cells[i + 2, 1, i + 1, 2].AutoFitColumns();
            }
            var consubseries = (from N in modelList
                                where N.Entidad == model.Entidad
                                select new { N.subserie, N.Serie }).Distinct().ToList();
            consubseries = consubseries.OrderBy(o => o.Serie).ThenBy(o => o.subserie).ToList();
            
            List<EncuestaModel> codsubseries = new List<EncuestaModel>();
            int contsub = 1;
            for (cont = 1; cont <= consubseries.Count(); cont++)
            {
                if (cont - 2 >= 0)
                    if (consubseries[cont - 1].Serie != consubseries[cont - 2].Serie)
                        contsub = 1;
                codsubseries.Add(new EncuestaModel() { CodigoSubserie = contsub, subserie = consubseries[cont - 1].subserie, Serie = consubseries[cont - 1].Serie });
                contsub++;
            }

            for (int i = 0; i < codsubseries.Count(); i++)
            {
                workSheet2.Cells[i + 2, 5].Value = codsubseries[i].CodigoSubserie;
                workSheet2.Cells[i + 2, 6].Value = codsubseries[i].subserie;
                workSheet2.Cells[i + 2, 4].Value = codsubseries[i].Serie;
                workSheet2.Cells[i + 2, 4, i + 1, 6].AutoFitColumns();
            }
            
            string serie = "", subserie = "asdfghjkl";
            cont = -1;
            foreach (var x in encuesta)
            {
                if (serie == "" || serie != x.Serie)
                {
                    cont++;
                    temp.Add(x);
                    if (temp[cont].Tipologias == null)
                        temp[cont].Tipologias = new List<string>();
                    temp[cont].Tipologias.Add(x.tipologia);

                }
                else
                {
                    if (subserie != x.subserie)
                    {
                        cont++;
                        temp.Add(x);
                        if (temp[cont].Tipologias == null)
                            temp[cont].Tipologias = new List<string>();
                        temp[cont].Tipologias.Add(x.tipologia);
                    }
                    else
                    {
                        temp[cont].Tipologias.Add(x.tipologia);
                    }
                }
                serie = x.Serie;
                subserie = x.subserie;
            }
            serie = "";
            int RowCount = 2;
            string[] tipos;
            for (int i = 0; i < temp.Count; i++)
            {

                workSheet.Cells[(RowCount), 1, (temp[i].Tipologias.Count + RowCount - 1), 1].Merge = true;
                workSheet.Cells[(RowCount), 1, (temp[i].Tipologias.Count + RowCount - 1), 1].Value = temp[i].CodigOficina;
                workSheet.Cells[(RowCount), 1, (temp[i].Tipologias.Count + RowCount - 1), 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[(RowCount), 1, (temp[i].Tipologias.Count + RowCount - 1), 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[(RowCount), 1, (temp[i].Tipologias.Count + RowCount - 1), 6].Style.WrapText = true;
                workSheet.Cells[(RowCount), 2, (temp[i].Tipologias.Count + RowCount - 1), 2].Value = temp[i].Oficina;
                workSheet.Cells[(RowCount), 2, (temp[i].Tipologias.Count + RowCount - 1), 2].Merge = true;
                workSheet.Cells[(RowCount), 2, (temp[i].Tipologias.Count + RowCount - 1), 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[(RowCount), 2, (temp[i].Tipologias.Count + RowCount - 1), 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[(RowCount), 2, (temp[i].Tipologias.Count + RowCount - 1), 2].Style.WrapText = true;
                workSheet.Cells[(RowCount), 3, (temp[i].Tipologias.Count + RowCount - 1), 3].Value =
                    (from N in codseries where N.Serie == temp[i].Serie select N.CodigoSerie).ToList()[0];
                workSheet.Cells[(RowCount), 3, (temp[i].Tipologias.Count + RowCount - 1), 3].Merge = true;
                workSheet.Cells[(RowCount), 3, (temp[i].Tipologias.Count + RowCount - 1), 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[(RowCount), 3, (temp[i].Tipologias.Count + RowCount - 1), 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[(RowCount), 4, (temp[i].Tipologias.Count + RowCount - 1), 4].Value = temp[i].Serie;
                workSheet.Cells[(RowCount), 4, (temp[i].Tipologias.Count + RowCount - 1), 4].Merge = true;
                workSheet.Cells[(RowCount), 4, (temp[i].Tipologias.Count + RowCount - 1), 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[(RowCount), 4, (temp[i].Tipologias.Count + RowCount - 1), 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[(RowCount), 5, (temp[i].Tipologias.Count + RowCount - 1), 5].Value =
                    (from N in codsubseries where (N.Serie == temp[i].Serie & N.subserie == temp[i].subserie) select N.CodigoSubserie).ToList()[0];
                workSheet.Cells[(RowCount), 5, (temp[i].Tipologias.Count + RowCount - 1), 5].Merge = true;
                workSheet.Cells[(RowCount), 5, (temp[i].Tipologias.Count + RowCount - 1), 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[(RowCount), 5, (temp[i].Tipologias.Count + RowCount - 1), 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[(RowCount), 6, (temp[i].Tipologias.Count + RowCount - 1), 6].Value = temp[i].subserie;
                workSheet.Cells[(RowCount), 6, (temp[i].Tipologias.Count + RowCount - 1), 6].Merge = true;
                workSheet.Cells[(RowCount), 6, (temp[i].Tipologias.Count + RowCount - 1), 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[(RowCount), 6, (temp[i].Tipologias.Count + RowCount - 1), 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                for (int y = 0; y < temp[i].Tipologias.Count; y++)
                {
                    tipos = temp[i].Tipologias[y].Split(',');
                    workSheet.Cells[(RowCount), 7].Value = tipos[0];
                    workSheet.Cells[(RowCount), 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Justify;
                    workSheet.Cells[(RowCount), 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    RowCount = RowCount + 1;
                }

            }


            using (var memoryStream = new MemoryStream())
            {
                string nombre = string.Format("attachment; filename=CCD " + Regex.Replace(model.Oficina, @"[^0-9a-zA-Z]+", " ") + ".xlsx");
                Response.ClearContent();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", nombre);
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }

            return RedirectToAction("Encuesta", new { entidad = model.Entidad, oficina = model.Oficina });
        }

        public ActionResult Encuesta(string entidad, string oficina)
        {
            if (entidad == null || oficina == null)
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
            string encargado = "", Cargo = "";
            int CodigoOficina = 0;
            foreach (var item in modelList)
            {
                if (entidad == item.Entidad && oficina == item.Oficina)
                {
                    encargado = item.Encargado;
                    CodigoOficina = item.CodigOficina;
                    Cargo = item.cargo;
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
            model.Encargado = encargado;
            model.CodigOficina = CodigoOficina;
            model.cargo = Cargo;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Encuesta(EncuestaModel model, List<string> Orig, List<string> Cop, List<string> formato)
        {

            try
            {
                if (model.Serie == null)
                {
                    ModelState.AddModelError("Serie", "La serie es obligatoria");
                    return View(model);
                }
                if (model.Gestion < 0)
                {
                    ModelState.AddModelError("Gestion", "Los anos en gestión no pueden ser negativos");
                    return View(model);
                }
                if (model.Archivo < 0)
                {
                    ModelState.AddModelError("Archivo", "Los anos en central no pueden ser negativos");
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
                if (model.SeriesModificadas == null)
                    model.SeriesModificadas = "";
                if (model.Observaciones == null)
                    model.Observaciones = "";
                if (model.recomendaciones == null)
                    model.recomendaciones = "";
                if (model.Procedimiento == null)
                    model.Procedimiento = "";
                if (model.Funcion == null)
                    model.Funcion = "";
                CargarModelList();
                int index = -1;
                for (int i = 0; i < modelList.Count; i++)
                {
                    if (model.Entidad == modelList[i].Entidad && modelList[i].Oficina == model.Oficina && modelList[i].Serie == "")
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
                    if (Orig != null)
                        if (Orig[0] == 0.ToString())
                        {
                            modelList[index].tipologia = modelList[index].tipologia + ", O,";
                        }
                    if (Cop != null)
                        if (Cop[0] == 0.ToString())
                        {
                            modelList[index].tipologia = modelList[index].tipologia + ", C,";
                        }
                    modelList[index].tipologia = modelList[index].tipologia + formato[0];
                    modelList[index].Soporte = model.Soporte;
                    modelList[index].DisposicionFinal = model.DisposicionFinal;
                    modelList[index].Observaciones = model.Observaciones.Replace(Environment.NewLine, @" \n ");
                    modelList[index].recomendaciones = model.recomendaciones.Replace(Environment.NewLine, @" \n ");
                    modelList[index].SeriesModificadas = model.SeriesModificadas.Replace(Environment.NewLine, @" \n ");
                    modelList[index].Funcion = model.Funcion.Replace(Environment.NewLine, @" \n ");
                    modelList[index].Procedimiento = model.Procedimiento.Replace(Environment.NewLine, @" \n ");
                    modelList[index].Sistema = model.Sistema.Replace(Environment.NewLine, @" \n ");
                    modelList[index].Rol = model.Rol.Replace(Environment.NewLine, @" \n ");
                    modelList[index].Clasificacion = model.Clasificacion.Replace(Environment.NewLine, @" \n ");
                    modelList[index].Proceso = model.Proceso.Replace(Environment.NewLine, @" \n ");
                    if (model.Tipologias.Count > 1)
                    {
                        for (int x = 1; x < model.Tipologias.Count; x++)
                        {
                            string original = "", copia = "";
                            bool o = false;
                            if (Orig != null)
                                o = Orig.Any(s => s == x.ToString());
                            if (o)
                                original = " O,";
                            o = false;
                            if (Cop != null)
                                o = Cop.Any(s => s == x.ToString());
                            if (o)
                                copia = " C,";
                            string tip = model.Tipologias[x] + "," + original + copia + formato[x];
                            if (model.SeriesModificadas == null)
                                model.SeriesModificadas = "";
                            if (model.Observaciones == null)
                                model.Observaciones = "";
                            if (model.recomendaciones == null)
                                model.recomendaciones = "";
                            if (model.Procedimiento == null)
                                model.Procedimiento = "";
                            if (model.Funcion == null)
                                model.Funcion = "";
                            modelList.Add(new EncuestaModel
                            {
                                Entidad = model.Entidad,
                                Oficina = model.Oficina,
                                CodigOficina = model.CodigOficina,
                                Encargado = model.Encargado,
                                cargo = model.cargo,
                                Serie = model.Serie,
                                subserie = model.subserie,
                                Gestion = model.Gestion,
                                Archivo = model.Archivo,
                                tipologia = tip,
                                Soporte = model.Soporte,
                                DisposicionFinal = model.DisposicionFinal,
                                Observaciones = model.Observaciones.Replace(Environment.NewLine, @" \n "),
                                recomendaciones = model.recomendaciones.Replace(Environment.NewLine, @" \n "),
                                SeriesModificadas = model.SeriesModificadas.Replace(Environment.NewLine, @" \n "),
                                Funcion = model.Funcion.Replace(Environment.NewLine, @" \n "),
                                Procedimiento = model.Procedimiento.Replace(Environment.NewLine, @" \n "),
                                Sistema = model.Sistema.Replace(Environment.NewLine, @" \n "),
                                Rol = model.Rol.Replace(Environment.NewLine, @" \n "),
                                Clasificacion = model.Clasificacion.Replace(Environment.NewLine, @" \n "),
                                Proceso = model.Proceso.Replace(Environment.NewLine, @" \n "),
                            });
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < model.Tipologias.Count; x++)
                    {
                        string original = "", copia = "";
                        bool o = false;
                        if (Orig != null)
                            o = Orig.Any(s => s == x.ToString());
                        if (o)
                            original = " O,";
                        o = false;
                        if (Cop != null)
                            o = Cop.Any(s => s == x.ToString());
                        if (o)
                            copia = " C,";
                        string tip = model.Tipologias[x] + "," + original + copia + formato[x];

                        modelList.Add(new EncuestaModel
                        {
                            Entidad = model.Entidad,
                            Oficina = model.Oficina,
                            CodigOficina = model.CodigOficina,
                            Encargado = model.Encargado,
                            cargo = model.cargo,
                            Serie = model.Serie,
                            subserie = model.subserie,
                            Gestion = model.Gestion,
                            Archivo = model.Archivo,
                            tipologia = tip,
                            Soporte = model.Soporte,
                            DisposicionFinal = model.DisposicionFinal,
                            Observaciones = model.Observaciones.Replace(Environment.NewLine, @" \n "),
                            recomendaciones = model.recomendaciones.Replace(Environment.NewLine, @" \n "),
                            SeriesModificadas = model.SeriesModificadas.Replace(Environment.NewLine, @" \n "),
                            Funcion = model.Funcion.Replace(Environment.NewLine, @" \n "),
                            Procedimiento = model.Procedimiento.Replace(Environment.NewLine, @" \n "),
                            Sistema = model.Sistema.Replace(Environment.NewLine, @" \n "),
                            Rol = model.Rol.Replace(Environment.NewLine, @" \n "),
                            Clasificacion = model.Clasificacion.Replace(Environment.NewLine, @" \n "),
                            Proceso = model.Proceso.Replace(Environment.NewLine, @" \n "),
                        });
                    }
                }
                modelList = modelList.OrderBy(o => o.Entidad).ThenBy(o => o.Oficina).ThenBy(o => o.Serie).ThenBy(o => o.subserie).ToList();
                RehacerTexto();
                TempData["List"] = modelList;

                TempData["SuccessMessage"] = "Se ha agregado exitosamente el registro";
                EncuestaModel encuesta = new EncuestaModel();
                encuesta.Entidad = model.Entidad;
                encuesta.Oficina = model.Oficina;
                encuesta.Encargado = model.Encargado;
                return RedirectToAction("Encuesta", new { entidad = model.Entidad, oficina = model.Oficina });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error, inténtelo de nuevo";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Informe(EncuestaModel model)
        {
            try
            {
                               
                CargarModelList();
                var encuesta = (from N in modelList
                                where N.Entidad == model.Entidad && N.Oficina == model.Oficina
                                select N
                            );

                Document doc = new Document(PageSize.LETTER, iTextSharp.text.Utilities.MillimetersToPoints(30),
                    iTextSharp.text.Utilities.MillimetersToPoints(30), iTextSharp.text.Utilities.MillimetersToPoints(25),
                    iTextSharp.text.Utilities.MillimetersToPoints(10));
                if (!Directory.Exists(Server.MapPath("/Informes")))
                {
                    Directory.CreateDirectory(Server.MapPath("/Informes"));
                }

                string temporal = ("/Informes/informe " + model.Entidad + " " + model.Oficina.Replace('-', ' ').Replace('.', ' ').Replace(',', ' ').Trim());
                if (temporal.Length > 155)
                    temporal = temporal.Substring(0, 155) + ".pdf";
                else
                    temporal = temporal + ".pdf";
                //temporal= temporal.Substring()
                PdfWriter writer = PdfWriter.GetInstance(doc,
                    new FileStream(Server.MapPath(temporal), FileMode.Create));
                doc.AddTitle("informe " + model.Entidad + " " + model.Oficina);
                doc.AddCreator("SARA");
                var page = new PDFFooter();
                page.FechaInforme = model.FechaInforme;
                writer.PageEvent = page;


                doc.Open();




                iTextSharp.text.Font Arial14 = new iTextSharp.text.Font
                    (iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                iTextSharp.text.Font Arial12 = new iTextSharp.text.Font
                    (iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font Arial10 = new iTextSharp.text.Font
                    (iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font pie = new iTextSharp.text.Font
                    (iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.NORMAL, BaseColor.GRAY);


                //añade el texto
                PdfPTable table = new PdfPTable(1);
                table.WidthPercentage = 100;

                PdfPCell anchorCell = new PdfPCell();
                Paragraph titulo = new Paragraph("ENCUESTA DOCUMENTAL\n \"" + model.Entidad + "\"", Arial14);
                titulo.Alignment = Element.ALIGN_CENTER;
                titulo.SpacingBefore = 10;
                titulo.SpacingAfter = 5;
                anchorCell.AddElement(titulo);
                anchorCell.BorderWidth = 0.5f;
                table.AddCell(anchorCell);
                doc.Add(table);
                doc.Add(new Paragraph("\n"));


                titulo = new Paragraph("NOMBRE DE LA OFICINA PRODUCTORA: " + model.Oficina + " \nCÓDIGO DE LA OFICINA: " + model.CodigOficina + "" + " \nNOMBRE DE FUNCIONARIO: " + model.Encargado + ""
                    + "\nCARGO DEL FUNCIONARIO: " + model.cargo + "", new iTextSharp.text.Font
                    (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                titulo.Alignment = Element.ALIGN_JUSTIFIED;
                titulo.SpacingAfter = 5;
                table = new PdfPTable(1);
                table.WidthPercentage = 100;
                anchorCell = new PdfPCell();
                anchorCell.AddElement(titulo);
                anchorCell.BorderWidth = 0.5f;
                table.AddCell(anchorCell);
                doc.Add(table);
                doc.Add(new Paragraph("\n"));
                titulo = new Paragraph("ASUNTOS DOCUMENTALES", new iTextSharp.text.Font
                   (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                titulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(titulo);


                List<EncuestaModel> temp = new List<EncuestaModel>();

                string serie = "", subserie = "asdfghjkl";
                int cont = -1;
                foreach (var x in encuesta)
                {
                    if (serie == "" || serie != x.Serie)
                    {
                        cont++;
                        temp.Add(x);
                        if (temp[cont].Tipologias == null)
                            temp[cont].Tipologias = new List<string>();
                        temp[cont].Tipologias.Add(x.tipologia);

                    }
                    else
                    {
                        if (subserie != x.subserie)
                        {
                            cont++;
                            temp.Add(x);
                            if (temp[cont].Tipologias == null)
                                temp[cont].Tipologias = new List<string>();
                            temp[cont].Tipologias.Add(x.tipologia);
                        }
                        else
                        {
                            temp[cont].Tipologias.Add(x.tipologia);
                        }
                    }
                    serie = x.Serie;
                    subserie = x.subserie;
                }
                serie = "";

                foreach (var x in temp)
                {
                    Phrase frase = new Phrase();
                    table = new PdfPTable(1);
                    table.WidthPercentage = 100;
                    anchorCell = new PdfPCell();
                    if (serie == "" || serie != x.Serie)
                    {
                        titulo = new Paragraph(x.Serie, new iTextSharp.text.Font
                  (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        titulo.Alignment = Element.ALIGN_JUSTIFIED;
                        anchorCell.BorderWidth = 0;
                        anchorCell.AddElement(titulo);
                        anchorCell.AddElement(new Paragraph("\n"));
                        doc.Add(new Paragraph("\n"));
                    }
                    anchorCell.AddElement(new Paragraph("1.SUBSERIE/ASUNTODOCUMENTAL:\n", new iTextSharp.text.Font
              (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    if (x.subserie != "")
                    {
                        anchorCell.AddElement(new Paragraph("\"" + x.subserie + "\"\n\n", new iTextSharp.text.Font
              (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    }
                    frase.Add(new Chunk("2.	PROCESO: ", new iTextSharp.text.Font
             (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    frase.Add(new Chunk(" \"" + x.Proceso + "\" \n", new iTextSharp.text.Font
              (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    frase.Add(new Chunk("3.	TIEMPO DE RETENCIÓN EN ARCHIVO DE GESTIÓN: ", new iTextSharp.text.Font
             (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    frase.Add(new Chunk(" \"" + x.Gestion + "\" \n", new iTextSharp.text.Font
              (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));

                    frase.Add(new Chunk("4.	TIEMPO DE RETENCIÓN EN ARCHIVO CENTRAL: ", new iTextSharp.text.Font
             (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    frase.Add(new Chunk(" \"" + x.Archivo + "\" \n", new iTextSharp.text.Font
              (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    frase.Add(new Chunk("5.	SOPORTE/ FORMATO:  ", new iTextSharp.text.Font
             (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    frase.Add(new Chunk(" \"" + x.Soporte + "\" \n", new iTextSharp.text.Font
              (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    frase.Add(new Chunk("6. DISPOSICIÓN FINAL:   ", new iTextSharp.text.Font
             (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    frase.Add(new Chunk(" \"" + x.DisposicionFinal + "\" \n", new iTextSharp.text.Font
              (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    frase.Add(new Chunk("7. TIPOLOGÍAS ASOCIADAS:   ", new iTextSharp.text.Font
            (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    for (var y = 0; y < x.Tipologias.Count; y++)
                    {
                        if (y == x.Tipologias.Count - 1)
                        {
                            frase.Add(new Chunk(" \"" + x.Tipologias[y] + "\"\n", new iTextSharp.text.Font
              (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        }
                        else
                        {
                            frase.Add(new Chunk(" \"" + x.Tipologias[y] + "\",", new iTextSharp.text.Font
              (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        }
                    }
                    frase.Add(new Chunk("8. OBSERVACIONES:   ", new iTextSharp.text.Font
                        (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    frase.Add(new Chunk(" \"" + x.Observaciones + "\" \n", new iTextSharp.text.Font
                        (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    frase.Add(new Chunk("9. RECOMENDACIONES:   ", new iTextSharp.text.Font
                        (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    frase.Add(new Chunk(" \"" + x.recomendaciones + "\" \n", new iTextSharp.text.Font
                        (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    frase.Add(new Chunk("10. SERIES MODIFICADAS:   ", new iTextSharp.text.Font
                                                (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    frase.Add(new Chunk(" \"" + x.SeriesModificadas + "\" \n", new iTextSharp.text.Font
                        (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    frase.Add(new Chunk("11. FUNCIONES RELACIONADAS:   ", new iTextSharp.text.Font
                                                (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    frase.Add(new Chunk(" \"" + x.Funcion + "\" \n", new iTextSharp.text.Font
                        (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    frase.Add(new Chunk("12. PROCEDIMIENTO:   ", new iTextSharp.text.Font
                                               (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    frase.Add(new Chunk(" \"" + x.Procedimiento + "\" \n", new iTextSharp.text.Font
                        (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    anchorCell.AddElement(frase);
                    anchorCell.BorderWidth = 0.5f;
                    table.AddCell(anchorCell);
                    doc.Add(table);
                    doc.Add(new Paragraph("\n"));
                    serie = x.Serie;

                }



                doc.Close();
                byte[] bytes = System.IO.File.ReadAllBytes(Server.MapPath(temporal));
                Font blackFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);
                using (MemoryStream stream = new MemoryStream())
                {
                    PdfReader reader = new PdfReader(bytes);
                    using (PdfStamper stamper = new PdfStamper(reader, stream))
                    {
                        int pages = reader.NumberOfPages;
                        for (int i = 1; i <= pages; i++)
                        {
                            ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Página " + i.ToString() + " de " + reader.NumberOfPages, new iTextSharp.text.Font
                                (iTextSharp.text.Font.FontFamily.HELVETICA, 9, iTextSharp.text.Font.NORMAL, BaseColor.GRAY)), 520f, 15f, 0);
                        }
                    }
                    bytes = stream.ToArray();
                }
                System.IO.File.WriteAllBytes(Server.MapPath(temporal), bytes);

                return File(Server.MapPath(temporal), "application/pdf");
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BaseDeDatos(EncuestaModel model)
        {

            CargarModelList();
            List<EncuestaModel> encuesta = (from N in modelList
                                            where N.Entidad == model.Entidad
                                            select N
                            ).ToList();
            DataTable dt = ConvertToDataTable(encuesta);
            dt.Columns.Remove("CodigoSerie");
            dt.Columns.Remove("Tipologias");
            dt.Columns.Remove("CodigoSubserie");
            dt.Columns["CodigOficina"].ColumnName = "Código de la oficina";
            dt.Columns["cargo"].ColumnName = "Cargo del encargado";
            dt.Columns["subserie"].ColumnName = "Subserie";
            dt.Columns["tipologia"].ColumnName = "Tipología/copia u original/Soporte";
            dt.Columns["Gestion"].ColumnName = "Tiempo en Gestión";
            dt.Columns["Archivo"].ColumnName = "Tiempo en Archivo";
            dt.Columns["Rol"].ColumnName = "Rol sobre la información";
            dt.Columns["Clasificacion"].ColumnName = "Clasificación de la información";
            dt.Columns["DisposicionFinal"].ColumnName = "Disposición final";
            dt.Columns["SeriesModificadas"].ColumnName = "Series modificadas";
            dt.Columns["recomendaciones"].ColumnName = "Recomendaciones";

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            using (var workbook = SpreadsheetDocument.Create(Server.MapPath("/prueba.xlsx"), DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();

                workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                foreach (System.Data.DataTable table in ds.Tables)
                {

                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                    sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                    uint sheetId = 1;
                    if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                    {
                        sheetId =
                            sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();
                    foreach (System.Data.DataColumn column in table.Columns)
                    {
                        columns.Add(column.ColumnName);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }


                    sheetData.AppendChild(headerRow);

                    foreach (System.Data.DataRow dsrow in table.Rows)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (String col in columns)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow[col].ToString()); //
                            newRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(newRow);
                    }

                }
            }
            return File(Server.MapPath("/prueba.xlsx"), "application/ms-excel", "informe.xlsx");
        }

        public JsonResult Serie(string Prefix, int oficina)
        {
            List<EncuestaModel> serie = new List<EncuestaModel>();
            if (System.IO.File.ReadAllBytes(Server.MapPath("/AsuntosFISCALÍAGENERAL.txt")).Length != 0)
            {
                string[] lines = System.IO.File.ReadAllLines(Server.MapPath("/AsuntosFISCALÍAGENERAL.txt"), Encoding.GetEncoding(1252));
                foreach (string line in lines)
                {
                    string[] TRDList = line.Split(';');
                    serie.Add(new EncuestaModel
                    {
                        CodigOficina = int.Parse(TRDList[0]),
                        Oficina = TRDList[1],
                        CodigoSerie = int.Parse(TRDList[2]),
                        Serie = TRDList[3],
                        CodigoSubserie = int.Parse(TRDList[4]),
                        subserie = TRDList[5],
                        Procedimiento = TRDList[6],
                    }); ;
                }
            }
            //Searching records from list using LINQ query  
            var NombreOficina = (from N in serie
                                 where N.CodigoSerie.ToString().Contains(Prefix)
                                 where N.CodigOficina == oficina
                                 select new { N.CodigoSerie, N.Serie }).Distinct();
            return Json(NombreOficina, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Subserie(string Prefix, int oficina, string codserie)
        {
            int cod = int.Parse(codserie.Split('-')[0]);
            List<EncuestaModel> serie = new List<EncuestaModel>();
            if (System.IO.File.ReadAllBytes(Server.MapPath("/AsuntosFISCALÍAGENERAL.txt")).Length != 0)
            {
                string[] lines = System.IO.File.ReadAllLines(Server.MapPath("/AsuntosFISCALÍAGENERAL.txt"), Encoding.GetEncoding(1252));
                foreach (string line in lines)
                {
                    string[] TRDList = line.Split(';');
                    serie.Add(new EncuestaModel
                    {
                        CodigOficina = int.Parse(TRDList[0]),
                        Oficina = TRDList[1],
                        CodigoSerie = int.Parse(TRDList[2]),
                        Serie = TRDList[3],
                        CodigoSubserie = int.Parse(TRDList[4]),
                        subserie = TRDList[5],
                        Procedimiento = TRDList[6],
                    }); ;
                }
            }
            //Searching records from list using LINQ query  
            var NombreOficina = (from N in serie
                                 where N.CodigoSubserie.ToString().Contains(Prefix)
                                 where N.CodigOficina == oficina
                                 where N.CodigoSerie == cod
                                 select new { N.CodigoSubserie, N.subserie, N.Procedimiento }).Distinct();
            return Json(NombreOficina, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Funcion(string prefix,string entidad, int oficina)
        {
            List<EncuestaModel> serie = new List<EncuestaModel>();
            StringComparison com;
            com = StringComparison.OrdinalIgnoreCase;
            if (System.IO.File.ReadAllBytes(Server.MapPath("/FuncionesFISCALÍAGENERAL.txt")).Length != 0)
            {
                string[] lines = System.IO.File.ReadAllLines(Server.MapPath("/FuncionesFISCALÍAGENERAL.txt"), Encoding.GetEncoding(1252));
                foreach (string line in lines)
                {
                    string[] TRDList = line.Split(';');
                    serie.Add(new EncuestaModel
                    {
                        CodigOficina = int.Parse(TRDList[0]),
                        CodigoSerie = int.Parse(TRDList[1]),
                        Funcion = TRDList[2],
                    }); ;
                }
            }
            //if (entidad.Contains("FISCAL"))
            //{

            //}

            //Searching records from list using LINQ query  
            var NombreOficina = (from N in serie
                                 where N.CodigOficina == oficina
                                 select new { N.CodigoSerie, N.Funcion }).Distinct();
            //if (Prefix != null)
            //    NombreOficina = (from N in serie
            //                     where N.CodigoSerie.ToString().Contains(Prefix)
            //                     where N.CodigOficina == oficina
            //                     select new { N.CodigoSerie, N.Funcion }).Distinct();

            return Json(NombreOficina, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Sistemas(string Prefix, int oficina)
        {
            List<EncuestaModel> serie = new List<EncuestaModel>();
            if (System.IO.File.ReadAllBytes(Server.MapPath("~/SistemasFiscalíaGeneralDeLaNación.txt")).Length != 0)
            {
                string[] lines = System.IO.File.ReadAllLines(Server.MapPath("~/SistemasFiscalíaGeneralDeLaNación.txt"), Encoding.GetEncoding(1252));
                foreach (string line in lines)
                {
                    string[] TRDList = line.Split(';');
                    serie.Add(new EncuestaModel
                    {
                        Funcion = TRDList[1],
                    }); ;
                }
            }
            ////Searching records from list using LINQ query  
            //var NombreOficina = (from N in serie
            //                     where N.CodigOficina == oficina
            //                     select new { N.CodigoSerie, N.Funcion }).Distinct();            

            return Json(serie, JsonRequestBehavior.AllowGet);
        }
    }


}

public class PDFFooter : PdfPageEventHelper
{
    PdfContentByte cb;
    PdfTemplate template;
    public DateTime FechaInforme { get; set; }

 


    // write on top of document
    public override void OnOpenDocument(PdfWriter writer, Document document)
    {

        base.OnOpenDocument(writer, document);
        cb = writer.DirectContent;
        template = cb.CreateTemplate(50, 50);
    }

    // write on start of each page
    public override void OnStartPage(PdfWriter writer, Document document)
    {
        base.OnStartPage(writer, document);
        Rectangle rect = new Rectangle(document.PageSize.Width - 20, document.PageSize.Height - 20, 18, document.BottomMargin); // you can resize rectangle 
        rect.EnableBorderSide(1);
        rect.EnableBorderSide(2);
        rect.EnableBorderSide(4);
        rect.EnableBorderSide(8);
        rect.BorderColor = BaseColor.BLACK;
        rect.BorderWidth = 1;
        document.Add(rect);
        rect = new Rectangle(document.PageSize.Width - 25, document.PageSize.Height - 40, document.PageSize.Width - 65, document.BottomMargin + 20); // you can resize rectangle 
        rect.EnableBorderSide(1);
        rect.EnableBorderSide(2);
        rect.EnableBorderSide(4);
        rect.EnableBorderSide(8);
        rect.BorderColor = BaseColor.BLACK;
        rect.BorderWidth = 0.5f;
        rect.BackgroundColor = BaseColor.LIGHT_GRAY;
        document.Add(rect);
        PdfPTable tab = new PdfPTable(1);
        tab.TotalWidth = document.PageSize.Width;
        PdfPCell cell = new PdfPCell(new Paragraph("FIRMA DEL FUNCIONARIO:                                                       FIRMA DEL ENCUESTADOR:", new iTextSharp.text.Font
                    (iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
        cell.Rotation = 90;
        cell.Border = 0;
        tab.AddCell(cell);
        tab.WriteSelectedRows(0, -1, document.PageSize.Width - 50, document.PageSize.Height - 190, writer.DirectContent);
    }

    // write on end of each page
    public override void OnEndPage(PdfWriter writer, Document document)
    {
        base.OnEndPage(writer, document);
        PdfPTable tabFot = new PdfPTable(2);
        PdfPCell cell;
        tabFot.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
        cell = new PdfPCell(new Paragraph(FechaInforme.ToString("D", new CultureInfo("es-ES")), new iTextSharp.text.Font
                    (iTextSharp.text.Font.FontFamily.HELVETICA, 9, iTextSharp.text.Font.NORMAL, BaseColor.GRAY)));
        cell.HorizontalAlignment = Element.ALIGN_LEFT;
        cell.Border = 0;
        tabFot.AddCell(cell);
        cell = new PdfPCell(new Paragraph(""));
        cell.Border = 0;
        tabFot.AddCell(cell);
        tabFot.WriteSelectedRows(0, -1, document.LeftMargin, writer.PageSize.GetBottom(document.BottomMargin), writer.DirectContent);
    }

    //write on close of document
    public override void OnCloseDocument(PdfWriter writer, Document document)
    {
        base.OnCloseDocument(writer, document);
    }
}