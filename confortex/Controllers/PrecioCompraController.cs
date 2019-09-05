using Confortex.Clases;
using Confortex.Models;
using Confortex.Referencias;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace Confortex.Controllers
{
    [Authorize]
    public class PrecioCompraController : Controller
    {
        private ConfortexEntities db = new ConfortexEntities();
        // GET: PrecioCompra
        [Accesso]
        public ActionResult Index()
        {
            var sta = db.vw_ObtenerCotizaciones.Where(w => w.Estado.Contains("Finalizado") || w.Estado.Contains("Aprobado")).Select(s => new { id = s.IdCotizacion, name = "No-" + s.IdCotizacion + " " + s.RazonSocial + " (" + s.Estado + ")" }); ;
            ViewBag.IdCotizacion = new SelectList(sta, "id", "name");
            return View();
        }

        [HttpPost]
        public ActionResult PrecioTela(int IdCotizacion, int IdTela, Boolean MantenerPrecio, double PrecioNuevo)
        {
            try
            {
                using (var dbContext = new ConfortexEntities())
                {
                    dbContext.sp_AsociarPreciosTela(IdCotizacion, IdTela, MantenerPrecio, PrecioNuevo);
                }
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {

                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        [HttpPost]
        public ActionResult PrecioAccesorio(int IdCotizacion, int IdAccesorio, Boolean MantenerPrecio, double PrecioNuevo)
        {
            try
            {
                using (var dbContext = new ConfortexEntities())
                {
                    dbContext.sp_AsociarPreciosAccesorio(IdCotizacion, IdAccesorio, MantenerPrecio, PrecioNuevo);
                }
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {

                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        public ActionResult RequerimientosCompra(string cotizacion)
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

                string content = Url.Content("~/Reportes/ASP/CRV.aspx?id=7&cotizacion=" + cotizacion+"&user="+ User.Identity.GetUserName());
                model.ReportPath = content;

                return View("RequerimientosCompra", model);

            }
        }

        public ActionResult searchPrecioConsignadoMateria()
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

                
                
                    var v2 = (from a in db.fn_ObtenerMIPrecioConsignado(Convert.ToInt16(searchv)) select a);
                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                    {
                        v2 = v2.OrderBy(sortColumn + " " + sortColumnDir);
                    }

                    totalRecords = v2.Count();
                    var data = v2.Skip(skip).Take(pagesize).ToList();
                    return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
                
            }
        }

        public ActionResult searchPrecioConsignadoTela()
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

                
                    var v = (from a in db.fn_ObtenerTelaPrecioConsignado(Convert.ToInt16(searchv)) select a);
                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                    {
                        v = v.OrderBy(sortColumn + " " + sortColumnDir);
                    }

                    totalRecords = v.Count();
                    var data = v.Skip(skip).Take(pagesize).ToList();
                    return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data }, JsonRequestBehavior.AllowGet);
              
            }
        }

    }
}