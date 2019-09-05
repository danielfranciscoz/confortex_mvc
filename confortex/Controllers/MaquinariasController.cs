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

namespace Confortex.Controllers
{
    [Authorize]
    public class MaquinariasController : Controller
    {
        private ConfortexEntities db = new ConfortexEntities();

        [Accesso]
        // GET: MaquinariaEquipoElectricos
        public ActionResult Index()
        {
            var maquinariaEquipoElectrico = db.MaquinariaEquipoElectrico.Include(m => m.Funcion);
            return View(maquinariaEquipoElectrico.ToList());
        }

        // GET: MaquinariaEquipoElectricos/Details/5
     
        // GET: MaquinariaEquipoElectricos/Create
        public ActionResult Create()
        {
            ViewBag.NombreFuncion = new SelectList(db.Funcion, "Nombre", "Nombre");
            return PartialView();
        }

        public ActionResult Precio()
        {
            return PartialView();
        }

        // POST: MaquinariaEquipoElectricos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "NombreEquipo,NombreFuncion,ConsumoPromedioKWh,CantidadExistencia,isMaquinaria")] MaquinariaEquipoElectrico mae)
        {

            try
            {

                if (!mae.isMaquinaria)
                {
                    mae.NombreFuncion = null;
                }
                db.MaquinariaEquipoElectrico.Add(mae);

                db.SaveChanges();
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }

        }
        [HttpPost]
        public JsonResult precioenergia()
        {
            var precio = db.PrecioEnergia.Where(w => w.regAnulado == false).Select(s => s.Precio).FirstOrDefault();
            return Json(new { precio = precio });
        }

        public ActionResult EnergiaElectria(double PrecioEnergia)
        {

            try
            {
                PrecioEnergia pe = new PrecioEnergia();
                pe.Precio = PrecioEnergia;
                pe.FechaRegistro = System.DateTime.Now;
                db.PrecioEnergia.Add(pe);
                db.SaveChanges();
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }

        }

        // GET: MaquinariaEquipoElectricos/Edit/5


        // POST: MaquinariaEquipoElectricos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "IdMaquinariaEquipoElectrico,NombreEquipo,NombreFuncion,ConsumoPromedioKWh,CantidadExistencia,isMaquinaria")] MaquinariaEquipoElectrico mae)
        {
            try
            {
                MaquinariaEquipoElectrico m = db.MaquinariaEquipoElectrico.Find(mae.IdMaquinariaEquipoElectrico);
                m.NombreEquipo = mae.NombreEquipo;
                if (!mae.isMaquinaria)
                {
                    m.NombreFuncion = null;
                }
                else
                {
                    m.NombreFuncion = mae.NombreFuncion;
                }
                m.ConsumoPromedioKWh = mae.ConsumoPromedioKWh;
                m.CantidadExistencia = mae.CantidadExistencia;
                m.isMaquinaria = mae.isMaquinaria;

                db.Entry(m).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        // GET: MaquinariaEquipoElectricos/Delete/5
 

        // POST: MaquinariaEquipoElectricos/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                MaquinariaEquipoElectrico m = db.MaquinariaEquipoElectrico.Find(id);
                m.regAnulado = true;
                db.Entry(m).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        public ActionResult searchMaquinarias()
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
                var v = (from a in db.MaquinariaEquipoElectrico.Where(w => w.regAnulado == false) select new { a.IdMaquinariaEquipoElectrico, a.NombreEquipo, a.NombreFuncion, a.CantidadExistencia,a.ConsumoPromedioKWh,a.isMaquinaria});


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.NombreFuncion.Contains(searchv) || a.NombreEquipo.Contains(searchv));
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
