using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Mvc;
using Confortex.Models;
using Confortex.Referencias;
using Confortex.Clases;
using Microsoft.AspNet.Identity;

namespace Confortex.Controllers
{
    [Authorize]
    public class NominaController : Controller
    {
        private ConfortexEntities db = new ConfortexEntities();
        private int IdNomina = 0;
        // GET: Nomina
        public ActionResult Index()
        {

            ViewBag.idnominan = db.Nomina.Max(w => w.IdNomina) + 1;
            return View();
        }


        public ActionResult empleado(int nomina, int empleado)
        {


            var salario = db.fn_ObtenerEmpleadosNomina(nomina).Where(a=>a.IdEmpleado == empleado).Select(s => s.SalarioBase).FirstOrDefault();
            return Json(new
            {
                viatico = salario / 4,
                salario = salario
            });
        }
        // GET: Nomina/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nomina nomina = db.Nomina.Find(id);
            if (nomina == null)
            {
                return HttpNotFound();
            }
            return View(nomina);
        }

        public JsonResult detallepaso1(int nomina)
        {
            DateTime fi = db.Nomina.Where(w => w.IdNomina == nomina && w.regAnulado == false).Select(s => s.FechaInicio).FirstOrDefault();
            DateTime ff = db.Nomina.Where(w => w.IdNomina == nomina && w.regAnulado == false).Select(s => s.FechaFin).FirstOrDefault();
            var referencia = db.Nomina.Where(w => w.IdNomina == nomina && w.regAnulado == false).Select(s => s.NombreReferencia).FirstOrDefault();
            var dias = db.NominaDiaLibre.Where(w => w.IdNomina == nomina).Select(a=> new {a.IdNomina,a.IdNominaDiaLibre,a.Concepto, FechaLibre = a.FechaLibre.Day + "/" + a.FechaLibre.Month + "/" + a.FechaLibre.Year });

            var fechainicio = fi.Day + "/" + fi.Month + "/" + fi.Year;
            if (fi.Year == 1) { fechainicio = ""; }
            var fechafin = ff.Day + "/" + ff.Month + "/" + ff.Year;
            if (ff.Year == 1) { fechafin = ""; }
            var observaciones = db.Nomina.Where(w => w.IdNomina == nomina && w.regAnulado == false).Select(s => s.Observaciones).FirstOrDefault();

            return Json(new { referencia = referencia, fechainicio = fechainicio, fechafin = fechafin, observaciones = observaciones, dias = dias });
        }

        public JsonResult detallepaso2(int nomina)
        {
            var dn = db.DetalleNomina.Where(w => w.IdNomina == nomina).FirstOrDefault();

            return Json(new { empleado = dn.IdEmpleado, ajuste = dn.Ajuste, transporte = dn.Transporte, horasextras = dn.HorasExtra, llegadastardes = dn.MinutosTarde, ausencias = dn.Ausencia, prestaciones = dn.Prestamo, inss = dn.INSS });
        }
        public ActionResult Paso1(int nomina)
        {
            ViewBag.Maxnomina = db.Nomina.Max(w => w.IdNomina) + 1;
            ViewBag.nomina = nomina;
            var pas = db.Nomina.Where(w => w.IdNomina == nomina && (w.IdEstado == clsReferencias.Anulado || w.IdEstado == clsReferencias.Pagado)).ToList();
            if (pas.Count > 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();

            }
        }
        // GET: Nomina/Create
        public ActionResult Paso2(int nomina)
        {

            ViewBag.nomina = nomina;
            var pas = db.Nomina.Where(w => w.IdNomina == nomina && (w.IdEstado == clsReferencias.Anulado || w.IdEstado == clsReferencias.Pagado)).ToList();
            if (pas.Count > 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();

            }
        }

        public ActionResult CreatePaso2(int nomina)
        {
            var per = db.fn_ObtenerEmpleadosNomina(nomina).Select(a => new { a.IdEmpleado, nombre = a.CodEmpleado + " - " + a.NombreCompleto + " (" + a.TipoNomina + ")" });

            ViewBag.IdEmpleado = new SelectList(per, "IdEmpleado", "nombre");
            return PartialView();
        }
        public ActionResult Create()
        {
            var Empleado = db.vw_ObtenerEmpleados.Where(w => w.regAnulado == false).Select(a => new { a.IdEmpleado, Nombre = a.codEmpleado + " - " + a.NombreContacto });
            ViewBag.IdEmpleado = new SelectList(Empleado, "IdEstado", "Nombre");
            return View();
        }

        // POST: Nomina/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
                #region Gestion de Nómina
        [HttpPost]
        public ActionResult Create(Nomina nomina,NominaDiaLibre[] diasLibres)
        {
            NominaDiaLibre nominadiaLibre;
           
            try
            {
                if (nomina.IdNomina == 0)
                {
                    IdNomina = db.Nomina.Max(w => w.IdNomina) + 1;
                }
                else {
                    IdNomina = nomina.IdNomina;
                }

                List<NominaDiaLibre> dias = db.NominaDiaLibre.Where(w=>w.IdNomina == IdNomina).ToList();

                using (var dbContext = new ConfortexEntities())
                {
                    if (nomina.IdNomina == 0)
                    {
                        dbContext.sp_GestionarNomina(IdNomina, nomina.NombreReferencia, nomina.FechaInicio, nomina.FechaFin, clsReferencias.EnBorrador, nomina.Observaciones, clsReferencias.INSERT);
                    }
                    else
                    {
                        dbContext.sp_GestionarNomina(IdNomina, nomina.NombreReferencia, nomina.FechaInicio, nomina.FechaFin, 0, nomina.Observaciones, clsReferencias.UPDATE);
                    }

                }

                if (diasLibres != null)
                {
                    for (int i = 0; i < diasLibres.Length; i++)
                    {
                        if (diasLibres[i].FechaLibre < nomina.FechaInicio || diasLibres[i].FechaLibre > nomina.FechaFin)
                        {
                            return Json(new { Message = "El día libre " + diasLibres[i].FechaLibre.Date.ToShortDateString() + " se encuentra fuera de los limites de pago de la Nómina, por lo tanto no puede ser agregado, debe corregir este error para continuar." });
                        }

                        if (diasLibres[i].IdNominaDiaLibre == 0)
                        {
                            diasLibres[i].IdNomina = IdNomina;
                            diasLibres[i].FechaCreacion = System.DateTime.Now;

                            db.NominaDiaLibre.Add(diasLibres[i]);
                        }
                        else {
                            nominadiaLibre = db.NominaDiaLibre.Find(diasLibres[i].IdNominaDiaLibre);
                            nominadiaLibre.Concepto = diasLibres[i].Concepto;

                            db.Entry(nominadiaLibre).State = EntityState.Modified;

                            dias = dias.Where(w => w.IdNominaDiaLibre != diasLibres[i].IdNominaDiaLibre).ToList();

                        }
                    }

                    db.NominaDiaLibre.RemoveRange(dias.ToArray());
                }
                else
                {
                    IEnumerable<NominaDiaLibre> diasT = db.NominaDiaLibre.Where(w => w.IdNomina == IdNomina);
                    db.NominaDiaLibre.RemoveRange(diasT);
                }
                db.SaveChanges();
                return Json(new { Message = clsReferencias.Exito, IdNomina = IdNomina });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        // POST: Nomina/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int Id)
        {
            try
            {
                using (var dbContext = new ConfortexEntities())
                {
                    dbContext.sp_GestionarNomina(Id, "", System.DateTime.Now, System.DateTime.Now, 0, "", clsReferencias.DELETE);
                }
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        [HttpPost]
        public ActionResult PagoNomina(int IdNomina)
        {
            try
            {
                using (var dbContext = new ConfortexEntities())
                {
                    dbContext.sp_GestionarNomina(IdNomina, "", System.DateTime.Now, System.DateTime.Now, clsReferencias.Pagado, "", clsReferencias.Pagar);
                }
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        [HttpPost]
        public ActionResult GenerarNomina(int IdNomina)
        {
            try
            {
                using (var dbContext = new ConfortexEntities())
                {
                    dbContext.sp_GestionarNomina(IdNomina, "", System.DateTime.Now, System.DateTime.Now, clsReferencias.Generado, "", clsReferencias.Generar);
                }
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        #endregion

        #region Gestion de Detalles de Nómina

        [HttpPost]
        public ActionResult CreateDetalle(DetalleNomina detalleNomina, OtroIngresoEgreso[] otroIngreso)
        {
            try
            {
                int IdDetalle = 0;
                bool isUpdate = false;

                using (var dbContext = new ConfortexEntities())
                {
                    if (detalleNomina.IdDetalleNomina == 0)
                    {
                        IdDetalle = db.DetalleNomina.Max(w => w.IdDetalleNomina) + 1;
                        dbContext.sp_GestionarDetalleNomina(IdDetalle, detalleNomina.IdNomina, detalleNomina.IdEmpleado, detalleNomina.Ajuste, detalleNomina.Transporte, detalleNomina.HorasExtra, detalleNomina.Ausencia, detalleNomina.MinutosTarde, detalleNomina.Prestamo, detalleNomina.INSS, clsReferencias.INSERT);
                    }
                    else
                    {
                        IdDetalle = detalleNomina.IdDetalleNomina;
                        dbContext.sp_GestionarDetalleNomina(detalleNomina.IdDetalleNomina, detalleNomina.IdNomina, detalleNomina.IdEmpleado, detalleNomina.Ajuste, detalleNomina.Transporte, detalleNomina.HorasExtra, detalleNomina.Ausencia, detalleNomina.MinutosTarde, detalleNomina.Prestamo, detalleNomina.INSS, clsReferencias.UPDATE);
                        isUpdate = true;
                    }

                }
                if (isUpdate)
                {
                    List<OtroIngresoEgreso> otros = db.OtroIngresoEgreso.Where(w => w.IdDetalleNomina == IdDetalle).ToList();
                    OtroIngresoEgreso oie = null;
                    if (otroIngreso != null)
                    {
                        for (int i = 0; i < otroIngreso.Length; i++)
                        {
                            if (otroIngreso[i].IdOtroIngresoEgreso == 0)
                            {
                                otroIngreso[i].IdDetalleNomina = IdDetalle;
                                otroIngreso[i].FechaCreacion = System.DateTime.Now;
                                db.OtroIngresoEgreso.Add(otroIngreso[i]);
                            }
                            else
                            {
                                oie = db.OtroIngresoEgreso.Find(otroIngreso[i].IdOtroIngresoEgreso);
                                oie.Concepto = otroIngreso[i].Concepto;
                                oie.Monto = otroIngreso[i].Monto;

                                db.Entry(oie).State = EntityState.Modified;

                                otros = otros.Where(w => w.IdOtroIngresoEgreso != oie.IdOtroIngresoEgreso).ToList();
                            }
                        }
                        db.SaveChanges();
                    }

                    foreach (OtroIngresoEgreso o in otros)
                    {
                        oie = db.OtroIngresoEgreso.Find(o.IdOtroIngresoEgreso);
                        oie.regAnulado = true;
                        db.Entry(oie).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
                else
                {
                    if (otroIngreso != null)
                    {
                        for (int i = 0; i < otroIngreso.Length; i++)
                        {
                            otroIngreso[i].IdDetalleNomina = IdDetalle;
                            otroIngreso[i].FechaCreacion = System.DateTime.Now;
                            db.OtroIngresoEgreso.Add(otroIngreso[i]);
                        }
                    }
                    db.SaveChanges();
                }

                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }


        public ActionResult DeleteDetalle(int IdDetalleNomina)
        {
            try
            {
                using (var dbContext = new ConfortexEntities())
                {
                    dbContext.sp_GestionarDetalleNomina(IdDetalleNomina, 0, 0, 0, 0, 0, 0, 0, 0, 0, clsReferencias.DELETE);
                }
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }
        #endregion


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Busquedas

        public ActionResult searchNomina()
        {

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var lenght = Request.Form.GetValues("length").FirstOrDefault();

            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var searchv = Request.Form.GetValues("search[value]").FirstOrDefault();
            int pagesize = lenght != null ? Convert.ToInt32(lenght) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int totalRecords = 0;
            if(searchv == "" || searchv == null)
            {
                searchv = "-1";
            }

            
            using (db)
            {
                var v = (from a in db.vw_ObtenerNomina select a );

                if (!(string.IsNullOrEmpty(searchv)))
                {
                    if (!searchv.Equals("-1"))
                    {
                        v = v.Where(a => a.Nomina.ToString().Equals(searchv) || a.Referencia.ToString().Contains(searchv)).OrderByDescending(w => w.Nomina);
                    }
                }
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }

                totalRecords = v.Count();
                var data = v.Skip(skip).Take(pagesize).ToList();

                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult searchDetalle()
        {

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var lenght = Request.Form.GetValues("length").FirstOrDefault();

            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var searchv = Request.Form.GetValues("search[value]").FirstOrDefault();
            int pagesize = lenght != null ? Convert.ToInt32(lenght) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int totalRecords = 0;


            if (searchv.ToString().Equals(""))
            {
                searchv = "-1";
            }

            using (db)
            {
                var v = (from a in db.fn_ObtenerTrabajadoresNomina(int.Parse(searchv.ToString())) select a);

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }

                totalRecords = v.Count();
                var data = v.Skip(skip).Take(pagesize).ToList();

                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);

            }
        }

        public string fecha(DateTime fecha)
        {
            return fecha.Day + "/" + fecha.Month + "/" + fecha.Year;

        }

        public ActionResult searchDiaslibres()
        {

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var lenght = Request.Form.GetValues("length").FirstOrDefault();

            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var searchv = Request.Form.GetValues("search[value]").FirstOrDefault();
            int pagesize = lenght != null ? Convert.ToInt32(lenght) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int totalRecords = 0;


            if (searchv.ToString().Equals(""))
            {
                searchv = "-1";
            }

            using (db)
            {
                var r = (from a in db.NominaDiaLibre select new {a.IdNomina,a.IdNominaDiaLibre,a.FechaLibre,a.Concepto });
                var v = (r.Select( a => new{a.IdNomina,a.IdNominaDiaLibre,FechaLibre = a.FechaLibre.Day + "/" + a.FechaLibre.Month + "/" + a.FechaLibre.Year, a.Concepto }));
                v.Where(w=>w.IdNomina==Convert.ToInt32(searchv));

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }

                totalRecords = v.Count();
                var data = v.Skip(skip).Take(pagesize).ToList();

                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult searchDetalleEmpleados()
        {

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var lenght = Request.Form.GetValues("length").FirstOrDefault();

            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var searchv = Request.Form.GetValues("search[value]").FirstOrDefault();
            int pagesize = lenght != null ? Convert.ToInt32(lenght) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int totalRecords = 0;


            if (searchv.ToString().Equals(""))
            {
                searchv = "-1";
            }

            using (db)
            {
                var v = (from a in db.vw_ObtenerDetallesNomina.Where(w => w.No_Nomina.ToString() == searchv) select new { a.ColillaNo, a.IdEmpleado, a.NombreCompleto,a.Produccion, a.Basico, a.C7mo, a.Viatico, a.Ajuste, a.Transporte, a.HorasExtra, a.TotalOtrosIngresos, a.Ausencia, a.MinutosTarde, a.Prestamo, a.INSS, a.TotalOtrosEgresos });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }

                totalRecords = v.Count();
                var data = v.Skip(skip).Take(pagesize).ToList();

                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult ReporteNomina(string nomina)
        {
            int idc = Convert.ToInt16(nomina);
            var pas = db.Nomina.Where(w => w.IdNomina == idc && (w.regAnulado == true || w.IdEstado == clsReferencias.EnBorrador)).ToList();
            if (pas.Count > 0)
            {
                return RedirectToAction("Index");

            }
            else
            {
                ReportViewerViewModel model = new ReportViewerViewModel();
                var user = User.Identity.GetUserName();
                var iduser = User.Identity.GetUserId();


                string content = Url.Content("~/Reportes/ASP/CRV.aspx?id=6&nomina=" + nomina + "&user=" + user);
                model.ReportPath = content;



                return View("ReporteNomina", model);

            }


        }



        public ActionResult searchDetalleNominaIngresos()
        {

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var lenght = Request.Form.GetValues("length").FirstOrDefault();

            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var searchv = Request.Form.GetValues("search[value]").FirstOrDefault();
            int pagesize = lenght != null ? Convert.ToInt32(lenght) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int totalRecords = 0;


            if (searchv.ToString().Equals(""))
            {
                searchv = "-1";
            }

            using (db)
            {
                var v = (from a in db.OtroIngresoEgreso.Where(w => w.IdDetalleNomina.ToString() == searchv && w.IsIngreso == true && w.regAnulado == false) select new { a.IdDetalleNomina, a.IdOtroIngresoEgreso, a.Monto, a.Concepto });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }

                totalRecords = v.Count();
                var data = v.Skip(skip).Take(pagesize).ToList();

                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult searchDetalleNominaEgresos()
        {

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var lenght = Request.Form.GetValues("length").FirstOrDefault();

            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var searchv = Request.Form.GetValues("search[value]").FirstOrDefault();
            int pagesize = lenght != null ? Convert.ToInt32(lenght) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int totalRecords = 0;


            if (searchv.ToString().Equals(""))
            {
                searchv = "-1";
            }

            using (db)
            {
                var v = (from a in db.OtroIngresoEgreso.Where(w => w.IdDetalleNomina.ToString() == searchv && w.IsIngreso == false && w.regAnulado == false) select new { a.IdDetalleNomina, a.IdOtroIngresoEgreso, a.Monto, a.Concepto });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }

                totalRecords = v.Count();
                var data = v.Skip(skip).Take(pagesize).ToList();

                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);

            }
        }

        #endregion
    }
}
