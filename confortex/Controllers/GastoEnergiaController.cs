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
    public class GastoEnergiaController : Controller
    {
        private ConfortexEntities db = new ConfortexEntities();

        // GET: GastoEnergia
        public ActionResult Index()
        {
            return View(db.vw_ObtenerGastoMensual.ToList());
        }

        // GET: GastoEnergia/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GastoEnergia gastoEnergia = db.GastoEnergia.Find(id);
            if (gastoEnergia == null)
            {
                return HttpNotFound();
            }
            return View(gastoEnergia);
        }

        // GET: GastoEnergia/Create
        public ActionResult Create()
        {
            return PartialView();
        }

        // POST: GastoEnergia/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(GastoEnergia gastoEnergia)
        {
            try
            {
                gastoEnergia.FechaCreacion = System.DateTime.Now;
                db.GastoEnergia.Add(gastoEnergia);
                db.SaveChanges();
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }

        }

        // GET: GastoEnergia/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GastoEnergia gastoEnergia = db.GastoEnergia.Find(id);
            if (gastoEnergia == null)
            {
                return HttpNotFound();
            }
            return View(gastoEnergia);
        }

        // POST: GastoEnergia/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(GastoEnergia gastoEnergia)
        {
            try
            {
                GastoEnergia ge = db.GastoEnergia.Find(gastoEnergia.IdGastoEnergia);
                ge.MesAplica = gastoEnergia.MesAplica;
                ge.Monto = gastoEnergia.Monto;
                ge.FacturaNo = gastoEnergia.FacturaNo;

                db.Entry(ge).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        // GET: GastoEnergia/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GastoEnergia gastoEnergia = db.GastoEnergia.Find(id);
            if (gastoEnergia == null)
            {
                return HttpNotFound();
            }
            return View(gastoEnergia);
        }

        // POST: GastoEnergia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GastoEnergia gastoEnergia = db.GastoEnergia.Find(id);
            db.GastoEnergia.Remove(gastoEnergia);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult searchGastosEnergia()
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
                var v = (from a in db.vw_ObtenerGastoMensual select a);


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.MesAplica.Contains(searchv));
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

    }
}
