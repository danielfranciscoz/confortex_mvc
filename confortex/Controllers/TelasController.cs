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
using Confortex.Clases;
using Confortex.Referencias;

namespace Confortex.Controllers
{
    [Authorize]
    public class TelasController : Controller
    {
        private ConfortexEntities db = new ConfortexEntities();

        [Accesso]
        // GET: Telas
        public ActionResult Index()
        {
            return View();
        }

        // GET: Telas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tela tela = db.Tela.Find(id);
            if (tela == null)
            {
                return HttpNotFound();
            }
            return View(tela);
        }

        // GET: Telas/Create
        public ActionResult Create()
        {
            return PartialView();
        }

        // POST: Telas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(String Nombre, Double Precio)
        {
            try
            {
                db.sp_GestionarTela(0, Nombre, Precio, clsReferencias.INSERT);
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        // GET: Telas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { Message = clsReferencias.Id_NULL });
            }
            Tela tela = db.Tela.Find(id);
            if (tela == null)
            {
                return HttpNotFound();
            }
            return View(tela);
        }

        // POST: Telas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(int id, String Nombre, double Precio)
        {
            try
            {
                db.sp_GestionarTela(id, Nombre, Precio, clsReferencias.UPDATE);
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        // GET: Telas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tela tela = db.Tela.Find(id);
            if (tela == null)
            {
                return HttpNotFound();
            }
            return View(tela);
        }

        // POST: Telas/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                db.sp_GestionarTela(id, "", 0.00, clsReferencias.DELETE);
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
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

        public ActionResult searchTelas()
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
                var v = (from a in db.vw_ObtenerTelas select a);


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.Nombre.Contains(searchv));
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
