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
    public class TallasController : Controller
    {
        private ConfortexEntities db = new ConfortexEntities();

        [Accesso]
        // GET: Tallas
        public ActionResult Index()
        {
            return View();
        }

        // GET: Tallas/Details/5
        

        // GET: Tallas/Create
        public ActionResult Create()
        {
            return PartialView();
        }
        public ActionResult ListaTallas()
        {
            return PartialView(db.Talla.Where(w=>w.regAnulado==false).ToList());
        }

        // POST: Tallas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "Nombre,Descripcion")] Talla talla)
        {
            try
            {
                db.sp_GestionarTalla(0, talla.Nombre, talla.Descripcion, clsReferencias.INSERT);
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        // GET: Tallas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Talla talla = db.Talla.Find(id);
            if (talla == null)
            {
                return HttpNotFound();
            }
            return View(talla);
        }

        // POST: Tallas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public ActionResult Edit([Bind(Include = "IdTalla,Nombre,Descripcion")] Talla talla)
        {
            try
            {
                db.sp_GestionarTalla(talla.IdTalla, talla.Nombre, talla.Descripcion, clsReferencias.UPDATE);
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }

        }

        // GET: Tallas/Delete/5
        public ActionResult searchTallas()
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
                var v = (from a in db.Talla.Where(w => w.regAnulado == false) select new { a.IdTalla, a.Nombre, a.Descripcion, a.regAnulado });

                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.regAnulado == false && (a.Nombre.Contains(searchv) || a.Descripcion.Contains(searchv)));
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
        // POST: Tallas/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                db.sp_GestionarTalla(id, "", "", clsReferencias.DELETE);
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
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
