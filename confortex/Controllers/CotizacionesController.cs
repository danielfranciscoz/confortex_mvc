using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using Confortex.Models;
using Confortex.Clases;
using Confortex.Referencias;
using Microsoft.AspNet.Identity;
using ClosedXML.Excel;
using System.IO;

namespace Confortex.Controllers
{
    [Authorize]
    public class CotizacionesController : Controller
    {
        private ConfortexEntities db = new ConfortexEntities();
        int idCotizacion = 0;
        int idDescripcionHechura = 0;
        // GET: Cotizaciones

        [Accesso]
        public ActionResult Index()
        {
            var cotizacion = db.Cotizacion.Include(c => c.Cliente).Include(c => c.Estados);
            return View(cotizacion.ToList());
        }

        // GET: Cotizaciones/Details/5


        public FileResult ExportExcelentrega(int cotizacion)
        {

        
            clsCallProcedure procedimiento = new clsCallProcedure();

            String ProcedureName = "[dbo].[sp_ObtenerHojaEntrega]";
            Dictionary<String, String> parametros = new Dictionary<String, String>();
            parametros.Add("@IdCotizacion", cotizacion.ToString());



            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(procedimiento.CallDT(ProcedureName, parametros));
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Lista de Entrega Cotizacion no-" + cotizacion + ".xlsx");
                }
            }

        }

        // GET: Cotizaciones/Create
        public ActionResult Paso1(int cotizacion)
        {
            ViewBag.cotizacion = cotizacion;
            ViewBag.IdCliente = new SelectList(db.Cliente.Where(a => a.regAnulado == false), "IdCliente", "RazonSocial");
            ViewBag.dias = db.Cotizacion.Where(w => w.IdCotizacion == cotizacion).Select(s => s.diasEntrega).FirstOrDefault();
            var pas = db.Cotizacion.Where(w => w.IdCotizacion == cotizacion && (w.IdEstado == clsReferencias.Anulado || w.IdEstado == clsReferencias.Finalizado)).ToList();
            if(pas.Count > 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
                
            }

         
        }
        public ActionResult Paso2(int cotizacion)
        {
            ViewBag.cotizacion = cotizacion;
            var pas = db.Cotizacion.Where(w => w.IdCotizacion == cotizacion && (w.IdEstado == clsReferencias.EnBorrador || w.IdEstado == clsReferencias.Generado || w.IdEstado == clsReferencias.Aprobado)).ToList();
            if (pas.Count > 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Paso3(int cotizacion)
        {
            ViewBag.cotizacion = cotizacion;
            var pas = db.Cotizacion.Where(w => w.IdCotizacion == cotizacion && (w.IdEstado == clsReferencias.EnBorrador || w.IdEstado == clsReferencias.Generado || w.IdEstado == clsReferencias.Aprobado)).ToList();
            if (pas.Count > 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public JsonResult clienteEtapas(int idcotizacion)
        {
            var cliente = db.Cotizacion.Where(w => w.IdCotizacion == idcotizacion).Select(s => s.IdCliente).FirstOrDefault();
            var funciones = db.CotizacionFuncion.Where(w => w.IdCotizacion == idcotizacion).Select(s => s.Funcion.Orden).ToList();

            return Json(new { cliente = cliente, funciones = funciones });
        }

        // POST: Cotizaciones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult IdCotizacion()
        {
            Cotizacion id = db.Cotizacion.AsEnumerable().LastOrDefault();
            if (id == null)
            {
            idCotizacion = 1;
            }
            else
            {
            idCotizacion = id.IdCotizacion + 1;
            }
            return Json(new { IdCotizacion = idCotizacion });
        }

        // POST: Cotizaciones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.

        #region Etapa1 Generando la cotizacion Cotizacion en estado EnBorrador


        public ActionResult gestionPaso1(int IdCotizacion, int idCliente, double diasEntrega,CotizacionFuncion[] CotizacionFuncion)
        {
            var nuevo = db.Cotizacion.Where(w => w.IdCotizacion == IdCotizacion).Select(s => s.IdCotizacion).FirstOrDefault();

            if (nuevo == 0)
            {
                return Create(idCliente,diasEntrega, CotizacionFuncion);
            }
            else
            {
                return Edit(IdCotizacion,diasEntrega, idCliente, CotizacionFuncion);
            }

        }

        public JsonResult Create(int IdCliente,double diasEntrega, CotizacionFuncion[] CotizacionFuncion)
        {
            try
            {
                String mensaje = "";
                idCotizacion = db.Cotizacion.Max(w => w.IdCotizacion) + 1;
                try
                {
                    using (var dbContext = new ConfortexEntities())
                    {
                        dbContext.sp_GestionarCotizacion(idCotizacion,IdCliente, clsReferencias.EnBorrador, clsReferencias.INSERT,diasEntrega);
                    }

                    using (var dbContext = new ConfortexEntities())
                    {
                        dbContext.sp_AsignarUsuarioAudit(User.Identity.GetUserName());
                    }

                }
                catch (Exception ex)
                {
                    mensaje = new clsException(ex).Message();
                    idCotizacion = -1;
                }
                if (idCotizacion != -1)
                {
                    for (int i = 0; i < CotizacionFuncion.Length; i++)
                    {
                        CotizacionFuncion[i].IdCotizacion = idCotizacion;
                        CotizacionFuncion[i].FechaCreacion = System.DateTime.Now;
                        db.CotizacionFuncion.Add(CotizacionFuncion[i]);
                    }

                    db.SaveChanges();
                }
                else
                {
                    return Json(new { Message = mensaje });
                }

                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        public JsonResult Edit(int IdCotizacion, double diasEntrega, int IdCliente, CotizacionFuncion[] CotizacionFuncion)
        {
            try
            {
                idCotizacion = IdCotizacion;
                String mensaje = "";

                try
                {
                    using (var dbContext = new ConfortexEntities())
                    {
                        dbContext.sp_GestionarCotizacion(idCotizacion, IdCliente, clsReferencias.Ignore_Estate, clsReferencias.UPDATE,diasEntrega);
                    }

                    using (var dbContext = new ConfortexEntities())
                    {
                        dbContext.sp_AsignarUsuarioAudit(User.Identity.GetUserName());
                    }

                }
                catch (Exception ex)
                {
                    mensaje = new clsException(ex).Message();
                    idCotizacion = -1;
                }
                if (IdCotizacion != -1)
                {
                    List<CotizacionFuncion> cotizaciones = db.CotizacionFuncion.Where(w => w.IdCotizacion == idCotizacion).ToList();

                    for (int i = 0; i < CotizacionFuncion.Length; i++)
                    {
                        CotizacionFuncion cf = db.CotizacionFuncion.Find(new object[] { CotizacionFuncion[i].IdCotizacion, CotizacionFuncion[i].NombreFuncion });
                        if (cf == null)
                        {
                            CotizacionFuncion[i].IdCotizacion = idCotizacion;
                            CotizacionFuncion[i].FechaCreacion = System.DateTime.Now;
                            db.CotizacionFuncion.Add(CotizacionFuncion[i]);
                        }
                        else
                        {
                            cotizaciones = cotizaciones.Where(w => w.NombreFuncion != CotizacionFuncion[i].NombreFuncion).ToList();
                        }
                    }

                    db.SaveChanges();

                    foreach (CotizacionFuncion cot in cotizaciones)
                    {
                        db.CotizacionFuncion.Remove(cot);
                        db.SaveChanges();
                    }

                }
                else
                {
                    return Json(new { Message = mensaje });
                }


                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Cotizacion cotizacion = db.Cotizacion.Find(id);
                cotizacion.IdEstado = clsReferencias.Anulado;

                db.Entry(cotizacion).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        #endregion

        #region Etapa2 Generando Las Descripciones de Hechura de la cotizacion

        [HttpPost]
        public ActionResult CreateUpdateDescripcionHechura([Bind(Include = "IdDescripcionHechura,IdCotizacion,IdPieza,IdTela,IdColor,CantidadRequerida,Duracion,Descripcion")] DescripcionHechura dh, CombinacionDescripcionHechura[] cdh, AccesorioDescripcionHechura[] adh,DescripcionHechuraColor[] c)
        {

            try
            {

                int id = 0;
                id = SaveUpdateDescripcionHechura(dh);

                AsociarColores(c,id); 

                AsociarCombinaciones(cdh, id);

                AsociarMaterialesIndirectos(adh, id);
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        [HttpPost]
        public ActionResult DeleteDescripcionHechura([Bind(Include = "IdDescripcionHechura")] DescripcionHechura dh)
        {
            try
            {
                DescripcionHechura d = db.DescripcionHechura.Find(dh.IdDescripcionHechura);
                d.regAnulado = true;

                db.Entry(d).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        #endregion

        #region Etapa 2.1 Asociando colores a la Descripcion de la hechura
        private void AsociarColores(DescripcionHechuraColor[] c, int IdDescripcionHechura)
        {

            try
            {
                if ( c != null)
                {
                    List<DescripcionHechuraColor> colores = db.DescripcionHechuraColor.Where(w => w.IdDescripcionHechura == IdDescripcionHechura).ToList();

                    for (int i = 0; i < c.Length; i++)
                    {
                        if (colores.Where(W => W.IdColor == c[i].IdColor).ToList().Count == 0)
                        {
                            c[i].IdDescripcionHechura = IdDescripcionHechura;
                            c[i].FechaCreacion = System.DateTime.Now;
                            db.DescripcionHechuraColor.Add(c[i]);

                        }
                        else
                        {
                            colores = colores.Where(w => w.IdColor != c[i].IdColor).ToList();
                        }
                        db.SaveChanges();
                    }


                    db.DescripcionHechuraColor.RemoveRange(colores.ToArray());

                }
                else
                {
                    IEnumerable<DescripcionHechuraColor> colores = db.DescripcionHechuraColor.Where(W => W.IdDescripcionHechura == IdDescripcionHechura).ToArray();
                    db.DescripcionHechuraColor.RemoveRange(colores);

                }
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Etapa2.2 Asociando combinaciones a la Descripcion de la Hechura


        private void AsociarCombinaciones(CombinacionDescripcionHechura[] cdh, int IdDescripcionHechura)
        {

            try
            {
                if (cdh != null)
                {
                    List<CombinacionDescripcionHechura> combinaciones = db.CombinacionDescripcionHechura.Where(w => w.IdDescripcionHechura == IdDescripcionHechura).ToList();

                    for (int i = 0; i < cdh.Length; i++)
                    {
                        if (combinaciones.Where(W => W.IdCombinacion == cdh[i].IdCombinacion).ToList().Count == 0)
                        {
                            cdh[i].IdDescripcionHechura = IdDescripcionHechura;
                            cdh[i].FechaCreacion = System.DateTime.Now;
                            db.CombinacionDescripcionHechura.Add(cdh[i]);

                        }
                        else
                        {
                            combinaciones = combinaciones.Where(w => w.IdCombinacion != cdh[i].IdCombinacion).ToList();
                        }
                        db.SaveChanges();
                    }


                    db.CombinacionDescripcionHechura.RemoveRange(combinaciones.ToArray());

                }
                else
                {
                    IEnumerable<CombinacionDescripcionHechura> combinaciones = db.CombinacionDescripcionHechura.Where(W => W.IdDescripcionHechura == IdDescripcionHechura).ToArray();
                    db.CombinacionDescripcionHechura.RemoveRange(combinaciones);

                }
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Etapa2.3 Asociando MaterialesIndirectos a la Descripcion de la Hechura

        private void AsociarMaterialesIndirectos(AccesorioDescripcionHechura[] adh, int IdDescripcionHechura)
        {
            try
            {
                if (adh != null)
                {
                    AccesorioDescripcionHechura a = null;
                    List<AccesorioDescripcionHechura> accesorios = db.AccesorioDescripcionHechura.Where(w => w.IdDescripcionHechura == IdDescripcionHechura).ToList();

                    for (int i = 0; i < adh.Length; i++)
                    {
                        if (accesorios.Where(W => W.IdAccesorio == adh[i].IdAccesorio).ToList().Count == 0)
                        {
                            adh[i].FechaCreacion = System.DateTime.Now;
                            adh[i].IdDescripcionHechura = IdDescripcionHechura;
                            db.AccesorioDescripcionHechura.Add(adh[i]);
                        }
                        else
                        {
                            a = db.AccesorioDescripcionHechura.Find(new object[] { adh[i].IdAccesorio, adh[i].IdDescripcionHechura });
                            a.Cantidad = adh[i].Cantidad;

                            db.Entry(a).State = EntityState.Modified;

                            accesorios = accesorios.Where(w => w.IdAccesorio != adh[i].IdAccesorio).ToList();
                        }
                        db.SaveChanges();
                    }


                    db.AccesorioDescripcionHechura.RemoveRange(accesorios.ToArray());

                }
                else
                {
                    IEnumerable<AccesorioDescripcionHechura> accesorios = db.AccesorioDescripcionHechura.Where(W => W.IdDescripcionHechura == IdDescripcionHechura).ToArray();
                    db.AccesorioDescripcionHechura.RemoveRange(accesorios);

                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Etapa3 Actualizando Precios de Compra
        [HttpPost]
        public ActionResult EstablcerPrecio(int IdDescripcionHechura, float Precio)
        {
            try
            {
                DescripcionHechura dh = db.DescripcionHechura.Find(IdDescripcionHechura);
                dh.CostoUnit = Precio;

                db.Entry(dh).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        public ActionResult searchCostosEstimados()
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
                searchv = "0";
            }

            using (db)
            {
                var v = (from a in db.fn_PrecioSugerido(int.Parse(searchv.ToString())) select new { a.Orden, a.Concepto, a.CostoUnitario });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }

                totalRecords = v.Count();
                var data = v.Skip(skip).Take(pagesize).ToList();

                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult searchResumenDescripcionHechura()
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


            using (db)
            {
                var v = (from a in db.vw_ObtenerDetalleCotizacion select a);


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.IdCotizacion.ToString().Contains(searchv));
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

        #endregion

        #region Etapa4 Generando la Cotizacion
        [HttpPost]
        public ActionResult GenerarCotizacion(int IdCotizacion)
        {
            try
            {
                using (var dbContext = new ConfortexEntities())
                {
                    dbContext.sp_GestionarCotizacion(IdCotizacion, 0, clsReferencias.Generado, clsReferencias.Change_State,0);
                }
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        #endregion
        public ActionResult AprobarCotizacion(int IdCotizacion)
        {
            try
            {
                using (var dbContext = new ConfortexEntities())
                {
                    dbContext.sp_GestionarCotizacion(IdCotizacion, 0, clsReferencias.Aprobado, clsReferencias.Change_State,0);
                }
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        public ActionResult searchCotizaciones1()
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


            using (db)
            {
                var v = (from a in db.vw_ObtenerCotizaciones.Where(w=>w.Estado.Contains("Generado") || w.Estado.Contains("En")) select a);


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.CorreoContacto.Contains(searchv) || a.NombreContacto.Contains(searchv) || a.RazonSocial.Contains(searchv) || a.RUC.Contains(searchv) || a.IdCotizacion.ToString().Contains(searchv) || a.Cod_RA.Contains(searchv));
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
        public ActionResult searchCotizaciones2()
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


            using (db)
            {
                var v = (from a in db.vw_ObtenerCotizaciones.Where(w => w.Estado.Contains("Aprobado") ) select a);


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.CorreoContacto.Contains(searchv) || a.NombreContacto.Contains(searchv) || a.RazonSocial.Contains(searchv) || a.RUC.Contains(searchv) || a.IdCotizacion.ToString().Contains(searchv) || a.Cod_RA.Contains(searchv));
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
        public ActionResult searchCotizaciones3()
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


            using (db)
            {
                var v = (from a in db.vw_ObtenerCotizaciones.Where(w => w.Estado.Contains("Finalizado")) select a);


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.CorreoContacto.Contains(searchv) || a.NombreContacto.Contains(searchv) || a.RazonSocial.Contains(searchv) || a.RUC.Contains(searchv) || a.IdCotizacion.ToString().Contains(searchv) || a.Cod_RA.Contains(searchv));
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
        public ActionResult searchCotizaciones4()
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


            using (db)
            {
                var v = (from a in db.vw_ObtenerCotizaciones.Where(w => w.Estado.Contains("Anulado")) select a);


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.CorreoContacto.Contains(searchv) || a.NombreContacto.Contains(searchv) || a.RazonSocial.Contains(searchv) || a.RUC.Contains(searchv) || a.IdCotizacion.ToString().Contains(searchv) || a.Cod_RA.Contains(searchv));
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


        public ActionResult searchDescripcionHechura()
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


            using (db)
            {

                var v = (from a in db.DescripcionHechura.Where(w => w.regAnulado == false) select new { a.IdDescripcionHechura, a.IdCotizacion, a.IdPieza, a.IdTela, a.CantidadRequerida,Color = db.fn_ObtenerColoresDescHechura(a.IdDescripcionHechura), Pieza = a.Pieza.Nombre + " (" + a.Pieza.SexoPieza + ")", Tela = a.Tela.Nombre, a.Descripcion, a.Duracion });


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.IdCotizacion.ToString().Contains(searchv));
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

        public ActionResult searchColoresDescripcionHechura()
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
            if (searchv == null || searchv == "")
            {
                searchv = "0";
            }
            int search = Convert.ToInt32(searchv);


            using (db)
            {
                var v = (from a in db.DescripcionHechuraColor select new {a.IdDescripcionHechuraColor, a.IdDescripcionHechura, a.IdColor, Color = a.Color.Nombre});


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.IdDescripcionHechura == search);
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

        public ActionResult searchCombinacionesPieza()
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


            using (db)
            {
                var v = (from a in db.CombinacionDescripcionHechura select new { a.IdDescripcionHechura, a.IdCombinacion, a.Combinacion.Nombre }).Where(a => a.IdDescripcionHechura.ToString().Equals(searchv));


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.IdDescripcionHechura.ToString().Contains(searchv));
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

        public ActionResult searchMaterialIndirectoPieza()
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


            using (db)
            {
                var v = (from a in db.AccesorioDescripcionHechura select new { a.IdDescripcionHechura, a.IdAccesorio, a.Cantidad, a.Accesorio.Nombre }).Where(a => a.IdDescripcionHechura.ToString().Equals(searchv));


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.IdDescripcionHechura.ToString().Contains(searchv));
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


        public JsonResult ObtenerAuditoria(string CodRA)
        {
            clsCallProcedure procedimiento = new clsCallProcedure();

            String ProcedureName = "[dbo].[sp_ObtenerAuditoria]";
            Dictionary<String, String> parametros = new Dictionary<String, String>();
            parametros.Add("@CodRA", CodRA);

            return procedimiento.Call(ProcedureName, parametros);
        }
        private int SaveUpdateDescripcionHechura(DescripcionHechura dh)
        {
            int id = 0;
            if (dh.IdDescripcionHechura == 0)
            {
                dh.IdDescripcionHechura = db.DescripcionHechura.Max(w => w.IdDescripcionHechura) + 1;
                dh.cod_RA = Cod_RA.cod_RA();
                idDescripcionHechura = dh.IdDescripcionHechura;
                db.DescripcionHechura.Add(dh);
                id = dh.IdDescripcionHechura;
            }
            else
            {
                DescripcionHechura d = db.DescripcionHechura.Find(dh.IdDescripcionHechura);
                d.IdPieza = dh.IdPieza;
                d.IdTela = dh.IdTela;
                //d.IdColor = dh.IdColor;
                d.CantidadRequerida = dh.CantidadRequerida;
                d.Duracion = dh.Duracion;
                d.Descripcion = dh.Descripcion;
                idDescripcionHechura = dh.IdDescripcionHechura;
                db.Entry(d).State = EntityState.Modified;
                id = d.IdDescripcionHechura;
            }
            db.SaveChanges();

            return id;
        }

        public ActionResult ListEtapas()
        {
            return PartialView(db.Funcion.OrderBy(o => o.Orden).ToList());
        }

        public ActionResult CreateHechura()
        {
            ViewBag.IdTela = new SelectList(db.Tela.Where(w => w.regAnulado == false), "IdTela", "Nombre");
            ViewBag.IdColor = new SelectList(db.Color.Where(w => w.regAnulado == false), "IdColor", "Nombre");
            ViewBag.IdPieza = new SelectList(db.Pieza.Where(w => w.regAnulado == false).Select(a => new { a.IdPieza, Nombre = a.Nombre + " (" + a.SexoPieza + ")" }), "IdPieza", "Nombre");
            ViewBag.IdCombinacion = new SelectList(db.Combinacion.Where(w => w.regAnulado == false), "IdCombinacion", "Nombre");
            ViewBag.IdMaterial = new SelectList(db.Accesorio.Where(w => w.regAnulado == false && w.isAccesorio == true), "IdAccesorio", "Nombre");

            return PartialView();
        }

        public ActionResult ReporteCotizacion(string cotizacion)
        {
            int idc = Convert.ToInt16(cotizacion);
            var pas = db.Cotizacion.Where(w => w.IdCotizacion == idc && w.IdEstado == 7 ).ToList();
            if (pas.Count > 0)
            {
                return RedirectToAction("Index");
               
            }
            else
            {
                ReportViewerViewModel model = new ReportViewerViewModel();

                var user = User.Identity.GetUserName();
                var iduser = User.Identity.GetUserId();

                string content = Url.Content("~/Reportes/ASP/CRV.aspx?id=1&cotizacion=" + cotizacion + "&user=" + user);
                model.ReportPath = content;



                return View("ReporteCotizacion", model);

            }

          
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
