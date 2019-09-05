using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Confortex.Models;
using Confortex.Referencias;
using Confortex.Clases;
using Microsoft.AspNet.Identity;
using ClosedXML.Excel;
using System.IO;

namespace Confortex.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class ListadoProduccionController : Controller
    {
        private ConfortexEntities db = new ConfortexEntities();

        // GET: ListadoProduccion
        public ActionResult Index()
        {
            var sta = db.vw_ObtenerCotizaciones.Where(w => w.Estado.Contains("Finalizado") || w.Estado.Contains("Aprobado")).Select(s => new { id = s.IdCotizacion, name = "No-" + s.IdCotizacion + " " + s.RazonSocial + " (" + s.Estado + ")" + ((s.Pendiente>0)? " Pendiente de Asignar: "+s.Pendiente:"") }); ;
            ViewBag.IdCotizacion = new SelectList(sta, "id", "name");
            return View();
        }

        public ActionResult EntregaIndex()
        {
            var sta = db.vw_ObtenerCotizaciones.Where(w => w.Estado.Contains("Finalizado") || w.Estado.Contains("Aprobado")).Select(s => new { id = s.IdCotizacion, name = "No-" + s.IdCotizacion + " " + s.RazonSocial + " (" + s.Estado + ")" }); ;
            ViewBag.IdCotizacion = new SelectList(sta, "id", "name");
            return View();
        }
        // GET: ListadoProduccion/Details/5

        // GET: ListadoProduccion/Create
        public ActionResult Create(int hechura, int pieza, int ticket)
        {
            //ViewBag.IdDescripcionHechura = new SelectList(db.DescripcionHechura, "IdDescripcionHechura", "Descripcion");
            var cotizacion = db.DescripcionHechura.Where(w => w.IdDescripcionHechura == hechura).Select(s => s.IdCotizacion).FirstOrDefault();
            var tick = db.ListadoProduccion.Where(w => w.IdListadoProduccion == ticket).FirstOrDefault();
            ViewBag.IdTalla = new SelectList(db.PiezaTalla.Where(w => w.IdPieza == pieza && w.Talla.regAnulado == false).Select(w => new { w.IdTalla, w.Talla.Nombre }).OrderBy(o => o.Nombre), "IdTalla", "Nombre");
            ViewBag.IdMedida = new SelectList(db.PiezaMedida.Where(w => w.IdPieza == pieza && w.Medida.regAnulado == false).Select(w => new { w.IdMedida, w.Medida.Nombre }).OrderBy(o => o.Nombre), "IdMedida", "Nombre");
            ViewBag.IdFuncion = new SelectList(db.CotizacionFuncion.Where(w => w.IdCotizacion == cotizacion).Select(w => new { w.NombreFuncion }), "NombreFuncion", "NombreFuncion");
            ViewBag.IdColor = new SelectList(db.DescripcionHechuraColor.Where(w => w.IdDescripcionHechura == hechura).Select(w => new { w.Color.IdColor, w.Color.Nombre }).OrderBy(o => o.Nombre), "IdColor", "Nombre");

            ViewBag.Hechura = hechura;
            ViewBag.Pieza = pieza;
            ViewBag.Ticket = ticket;

            var descripcion = (from a in db.vw_ObtenerDetalleCotizacion.Where(w => w.IdDescripcionHechura == hechura) select new { a.Pieza, a.DescripcionHechuraOnlyCombinaciones, a.CantidadRequerida,a.Pendiente,a.ColorTela }).FirstOrDefault();
            //var asignado = db.ListadoProduccion.Where(w => w.IdDescripcionHechura == hechura && w.regAnulado == false && w.isReparacion == false).Sum(w => w.Cantidad);
            ViewBag.NombrePieza = descripcion.Pieza;
            ViewBag.DescripcionCompleta = descripcion.DescripcionHechuraOnlyCombinaciones;
            ViewBag.CantidadRequerida = descripcion.CantidadRequerida;
            ViewBag.Pendiente = descripcion.Pendiente;
            //ViewBag.Color = descripcion.ColorTela;
            return View();
        }


        public ActionResult ticketedit(int id)
        {
            var ticket = db.ListadoProduccion.Where(w => w.IdListadoProduccion == id).FirstOrDefault(); ;


            return Json(new
            {
                nombre = ticket.Nombre,
                observaciones = ticket.Observaciones,
                cantidad = ticket.Cantidad,
                talla = ticket.IdTalla,
                reparacion = ticket.isReparacion

            });
        }

        //public ActionResult Create2()
        //{
        //    return View();
        //}

        // POST: ListadoProduccion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(ListadoProduccion listadoProduccion, DetalleListadoProduccion[] medidas, TickectPrecioReparacion[] precioReparacion)
        {
            return GestionarTicket(listadoProduccion, medidas, clsReferencias.NoAction, precioReparacion);
        }

        // GET: ListadoProduccion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ListadoProduccion listadoProduccion = db.ListadoProduccion.Find(id);
            if (listadoProduccion == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdDescripcionHechura = new SelectList(db.DescripcionHechura, "IdDescripcionHechura", "Descripcion", listadoProduccion.IdDescripcionHechura);
            ViewBag.IdEstado = new SelectList(db.Estados, "IdEstado", "Nombre", listadoProduccion.IdEstado);
            ViewBag.IdTalla = new SelectList(db.Talla, "IdTalla", "Nombre", listadoProduccion.IdTalla);
            return View(listadoProduccion);
        }

        // POST: ListadoProduccion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(ListadoProduccion listadoProduccion, DetalleListadoProduccion[] medidas, TickectPrecioReparacion[] precioReparacion)
        {
            return GestionarTicket(listadoProduccion, medidas, clsReferencias.NoAction, precioReparacion);
        }

        // GET: ListadoProduccion/Delete/5

        // POST: ListadoProduccion/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed([Bind(Include = "IdListadoProduccion")]ListadoProduccion listadoProduccion)
        {
            return GestionarTicket(listadoProduccion, null, clsReferencias.DELETE, null);
        }


        public FileResult ExportExcelcorte(int cotizacion, int? ticket)
        {

            string ti = "";
            if (ticket != null)
            {
                ti = " Ticket no-" + ticket;
            }
            clsCallProcedure procedimiento = new clsCallProcedure();

            String ProcedureName = "[dbo].[sp_ObtenerHojaCorte]";
            Dictionary<String, String> parametros = new Dictionary<String, String>();
            parametros.Add("@IdCotizacion", cotizacion.ToString());
            parametros.Add("@NoTicket", ticket == null ? null : ticket.ToString());

           

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(procedimiento.CallDT(ProcedureName,parametros));
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Lista de Corte Cotizacion no-"+cotizacion+ti+".xlsx");
                }
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

        #region Busquedas

        public JsonResult ObtenerMedidaStandar(int IdPieza, int IdTalla)
        {
            clsCallProcedure procedimiento = new clsCallProcedure();

            String ProcedureName = "[dbo].[sp_ObtenerCatalogoStandard]";
            Dictionary<String, String> parametros = new Dictionary<String, String>();
            parametros.Add("@IdPieza", IdPieza.ToString());
            parametros.Add("@IdTalla", IdTalla.ToString());
            return procedimiento.Call(ProcedureName, parametros);

        }

        public ActionResult searchCotizaciones()
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
                var v = (from a in db.vw_ObtenerCotizaciones.Where(w => w.Estado == "Aprobado" || w.Estado == "Finalizado") select a);

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
                var v = (from a in db.vw_ObtenerDetalleCotizacion select new { a.IdCotizacion, a.IdDescripcionHechura, a.IdPieza, a.Pieza, a.DescripcionHechuraOnlyCombinaciones, a.CantidadRequerida,a.Pendiente });



                v = v.Where(a => a.IdCotizacion.ToString() == searchv);

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }
                totalRecords = v.Count();
                var data = v.Skip(skip).Take(pagesize).ToList();

                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult searchTickets()
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
                var v = (from a in db.fn_ObtenerTickets(int.Parse(searchv.ToString())) select a);

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }

                totalRecords = v.Count();
                var data = v.Skip(skip).Take(pagesize).ToList();

                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult searchMedidas()
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
                var v = (from a in db.DetalleListadoProduccion.Where(w => w.IdListadoProduccion.ToString() == searchv && w.regAnulado == false) select new { a.IdDetalleListadoProduccion, a.IdListadoProduccion, a.IdMedida, a.ListadoProduccion.IdTalla, vMedida = a.Medida, Talla = a.ListadoProduccion.Talla.Nombre, Medida = a.Medida1.Nombre });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }

                totalRecords = v.Count();
                var data = v.Skip(skip).Take(pagesize).ToList();

                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult searchReparacion()
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
                var v = (from a in db.TickectPrecioReparacion.Where(w => w.IdListadoProduccion.ToString() == searchv) select new { a.NombreFuncion, a.PrecioMO });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }

                totalRecords = v.Count();
                var data = v.Skip(skip).Take(pagesize).ToList();

                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult ReporteTicketsCotizacion(string cotizacion)
        {
            int idc = Convert.ToInt16(cotizacion);
            int idt = Convert.ToInt16(cotizacion);
            var pas = db.Cotizacion.Where(w => w.IdCotizacion == idc && w.IdEstado == 7).ToList();
            if (pas.Count > 0)
            {
                return RedirectToAction("Index");

            }
            else
            {
                ReportViewerViewModel model = new ReportViewerViewModel();

                var user = User.Identity.GetUserName();
                var iduser = User.Identity.GetUserId();

                string content = Url.Content("~/Reportes/ASP/CRV.aspx?id=3&cotizacion=" + cotizacion);
                model.ReportPath = content;



                return View("ReporteTicketsCotizacion", model);

            }


        }

        public ActionResult ReporteTickets(string cotizacion, string ticket)
        {
            int idc = Convert.ToInt16(cotizacion);
            int idt = Convert.ToInt16(ticket);
            var pas = db.Cotizacion.Where(w => w.IdCotizacion == idc && w.IdEstado == 7).ToList();
            if (pas.Count > 0)
            {
                return RedirectToAction("Index");

            }
            else
            {
                ReportViewerViewModel model = new ReportViewerViewModel();

                var user = User.Identity.GetUserName();
                var iduser = User.Identity.GetUserId();

                string content = Url.Content("~/Reportes/ASP/CRV.aspx?id=4&cotizacion=" + cotizacion + "&ticket=" + ticket);
                model.ReportPath = content;



                return View("ReporteTicket", model);

            }


        }

        public ActionResult ReporteTicketsCotizacionf(string cotizacion,string Ticket)
        {
            int idc = Convert.ToInt16(cotizacion);
            int idt = Convert.ToInt16(cotizacion);
            var pas = db.Cotizacion.Where(w => w.IdCotizacion == idc && w.IdEstado == 7).ToList();
            if (pas.Count > 0)
            {
                return RedirectToAction("Index");

            }
            else
            {
                ReportViewerViewModel model = new ReportViewerViewModel();

                var user = User.Identity.GetUserName();
                var iduser = User.Identity.GetUserId();

                string content = Url.Content("~/Reportes/ASP/CRV.aspx?id=5&cotizacion=" + cotizacion+"&ticket="+Ticket);
                model.ReportPath = content;



                return View("ReporteTicketsCotizacionf", model);

            }


        }

        public ActionResult ReporteTicketsf(string cotizacion, string ticket)
        {
            int idc = Convert.ToInt16(cotizacion);
            int idt = Convert.ToInt16(ticket);
            var pas = db.Cotizacion.Where(w => w.IdCotizacion == idc && w.IdEstado == 7).ToList();
            if (pas.Count > 0)
            {
                return RedirectToAction("Index");

            }
            else
            {
                ReportViewerViewModel model = new ReportViewerViewModel();

                var user = User.Identity.GetUserName();
                var iduser = User.Identity.GetUserId();

                string content = Url.Content("~/Reportes/ASP/CRV.aspx?id=6&cotizacion=" + cotizacion + "&ticket=" + ticket);
                model.ReportPath = content;



                return View("ReporteTicketf", model);

            }


        }



        #endregion

        #region Gestiones
        public JsonResult GestionarTicket(ListadoProduccion lp, DetalleListadoProduccion[] medidas, int accion, TickectPrecioReparacion[] precioReparacion)
        {
            try
            {
                ListadoProduccion ticket = null;
                if (accion == clsReferencias.NoAction)
                {
                    //Creando o editando el Ticket
                    int IdTicket = 0;

                    if (lp.IdListadoProduccion == 0)
                    {
                        IdTicket = db.ListadoProduccion.Max(w => w.IdListadoProduccion) + 1;
                        lp.IdListadoProduccion = IdTicket;
                        lp.IdEstado = clsReferencias.Generado;
                        lp.cod_RA = Cod_RA.cod_RA();
                        db.ListadoProduccion.Add(lp);

                        if (lp.isReparacion) //Si es una reparacion se gestionan los precios
                        {

                            for (int i = 0; i < precioReparacion.Length; i++)
                            {
                                precioReparacion[i].IdListadoProduccion = IdTicket;
                                precioReparacion[i].FechaCreacion = System.DateTime.Now;
                                db.TickectPrecioReparacion.Add(precioReparacion[i]);
                            }
                        }
                    }
                    else
                    {
                        ticket = db.ListadoProduccion.Find(lp.IdListadoProduccion);
                        IdTicket = ticket.IdListadoProduccion;
                        ticket.IdTalla = lp.IdTalla;
                        ticket.Nombre = lp.Nombre;
                        ticket.Cantidad = lp.Cantidad;
                        ticket.isReparacion = lp.isReparacion;
                        ticket.Observaciones = lp.Observaciones;

                        db.Entry(ticket).State = EntityState.Modified;

                        if (ticket.isReparacion)
                        {
                            List<TickectPrecioReparacion> precioTicket = db.TickectPrecioReparacion.Where(w => w.IdListadoProduccion == IdTicket).ToList();
                            TickectPrecioReparacion precio = null;
                            for (int i = 0; i < precioReparacion.Length; i++)
                            {
                                if (precioReparacion[i].IdListadoProduccion == 0)
                                {
                                    precioReparacion[i].IdListadoProduccion = IdTicket;
                                    precioReparacion[i].FechaCreacion = System.DateTime.Now;
                                    db.TickectPrecioReparacion.Add(precioReparacion[i]);
                                }
                                else
                                {
                                    precio = db.TickectPrecioReparacion.Find(new object[] { precioReparacion[i].IdListadoProduccion, precioReparacion[i].NombreFuncion });
                                    precio.PrecioMO = precioReparacion[i].PrecioMO;

                                    db.Entry(precio).State = EntityState.Modified;

                                    precioTicket = precioTicket.Where(w => w.NombreFuncion != precioReparacion[i].NombreFuncion).ToList();
                                }
                                db.SaveChanges();
                            }

                            db.TickectPrecioReparacion.RemoveRange(precioTicket.ToArray());
                        }
                    }

                    db.SaveChanges();

                    //Asignando medidas al ticket
                    List<DetalleListadoProduccion> ListaMedidas = db.DetalleListadoProduccion.Where(w => w.IdListadoProduccion == IdTicket && w.regAnulado == false).ToList();
                    DetalleListadoProduccion detalle = null;
                    if (medidas != null)
                    {
                        for (int i = 0; i < medidas.Length; i++)
                        {
                            if (medidas[i].IdListadoProduccion == 0 || medidas[i].IdDetalleListadoProduccion == 0)
                            {
                                medidas[i].IdListadoProduccion = IdTicket;
                                foreach (DetalleListadoProduccion temp in ListaMedidas.Where(w => w.IdMedida == medidas[i].IdMedida).ToList())
                                {
                                    if (temp != null)
                                    {
                                        temp.regAnulado = true;
                                        db.Entry(temp).State = EntityState.Modified;
                                    }
                                }
                                db.DetalleListadoProduccion.Add(medidas[i]);
                            }
                            else
                            {
                                detalle = db.DetalleListadoProduccion.Find(medidas[i].IdDetalleListadoProduccion);
                                detalle.IdMedida = medidas[i].IdMedida;
                                detalle.Medida = medidas[i].Medida;

                                db.Entry(medidas[i]).State = EntityState.Modified;
                                ListaMedidas = ListaMedidas.Where(w => w.IdDetalleListadoProduccion != medidas[i].IdDetalleListadoProduccion).ToList();
                            }
                            db.SaveChanges();
                        }

                        foreach (DetalleListadoProduccion det in ListaMedidas)
                        {
                            detalle = db.DetalleListadoProduccion.Find(det.IdDetalleListadoProduccion);
                            detalle.regAnulado = true;
                            db.Entry(detalle).State = EntityState.Modified;
                        }
                        db.SaveChanges();

                    }
                    else
                    {
                        foreach (DetalleListadoProduccion det in db.DetalleListadoProduccion.Where(w => w.IdListadoProduccion == IdTicket && w.regAnulado == false).ToList())
                        {
                            detalle = db.DetalleListadoProduccion.Find(det.IdDetalleListadoProduccion);
                            detalle.regAnulado = true;
                            db.Entry(detalle).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                    }


                }
                else
                {
                    //Eliminando el Ticket

                    ticket = db.ListadoProduccion.Find(lp.IdListadoProduccion);
                    ticket.regAnulado = true;

                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }
        #endregion


        #region Entrega de Tickets
        public ActionResult EntregaCreate()
        {
            var empleados = db.vw_ObtenerEmpleados.Where(w => w.regAnulado == false && w.TipoSalario == false).Select(s => new { s.IdEmpleado, Nombre = (s.codEmpleado + " - " + s.NombreContacto) }); ;
            ViewBag.Empleados = new SelectList(empleados, "IdEmpleado", "Nombre");
            return PartialView();
        }

        //Las entregas deben ser filtradas por pedidos (Cotizacion)
        public ActionResult searchEntregas()
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
                var v = (from a in db.vw_ObtenerEntregas select a);

                if (!(string.IsNullOrEmpty(searchv)))
                {
                    if (!searchv.Equals("-1"))
                    {
                        v = v.Where(a => a.IdCotizacion.ToString().Contains(searchv)).OrderByDescending(w=>w.IdEntregaTicket);
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

        [HttpPost]
        public ActionResult CreateEntrega(string Ticket, int IdEmpleado)
        {
            try
            {
                string NombreFuncion = Tticket(Ticket)[0];
                int IdTicket = int.Parse(Tticket(Ticket)[1]);
                if (NombreFuncion.Equals("C.CALIDAD"))
                {
                    return Json(new { Message = "El Ticket de Control de Calidad no puede ser entregado en el sistema." });
                }
                using (var dbContext = new ConfortexEntities())
                {
                    db.sp_EntregarTicket(IdTicket, NombreFuncion, IdEmpleado, 0);
                    return Json(new { Message = clsReferencias.Exito });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }

        }

        [HttpPost]
        public ActionResult DeleteEntrega(int IdEntrega)
        {
            try
            {
                using (var dbContext = new ConfortexEntities())
                {
                    db.sp_EntregarTicket(0, "", 0, IdEntrega);
                    return Json(new { Message = clsReferencias.Exito });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }

        }

        public String[] Tticket(string ticket)
        {
            //int tam = ticket.Length;
            //char[] t = ticket.ToCharArray();
            //string ti = "";
            //string funcion = "";
            //int p1 = 0;

            //for (int i = 3; i < tam; i++)
            //{
            //   if(t[i]=='-')
            //    {
            //        p1 = i;

            //    }

            //}

            //funcion =  ticket.Substring(4, p1-1);
            //ti = ticket.Substring(p1+1,tam);

            string[] corte = ticket.Split('-');
            string[] partes = new string[2];
            int j = 0;
            for (int i = 0; i < corte.Length; i++)
            {
                if (corte[i] != "CBT")
                {
                    partes[j] = corte[i];
                    j++;
                }
            }
            return partes;
        }

        #endregion
    }
}
