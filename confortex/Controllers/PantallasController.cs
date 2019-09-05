using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Confortex.Models;
using Microsoft.AspNet.Identity;

namespace Confortex.Controllers
{
    [Authorize]
    public class PantallasController : Controller
    {
        private ConfortexEntities db = new ConfortexEntities();

        [Accesso]
        // GET: Pantallas
        public ActionResult Index()
        {
            var pantalla = db.Pantalla.Include(p => p.Pantalla2);
            return View(pantalla.ToList());
        }

        // GET: Pantallas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pantalla pantalla = db.Pantalla.Find(id);
            if (pantalla == null)
            {
                return HttpNotFound();
            }
            return View(pantalla);
        }

        // GET: Pantallas/Create
        public ActionResult Create()
        {
            ViewBag.MenuPadre = new SelectList(db.Pantalla, "IdPantalla", "Nombre");
            return View();
        }

        // POST: Pantallas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdPantalla,Nombre,Recurso,isMenu,MenuPadre,Controlador,Vista,MenuOrden,cod_RA")] Pantalla pantalla)
        {
            if (ModelState.IsValid)
            {
                db.Pantalla.Add(pantalla);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MenuPadre = new SelectList(db.Pantalla, "IdPantalla", "Nombre", pantalla.MenuPadre);
            return View(pantalla);
        }

        // GET: Pantallas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pantalla pantalla = db.Pantalla.Find(id);
            if (pantalla == null)
            {
                return HttpNotFound();
            }
            ViewBag.MenuPadre = new SelectList(db.Pantalla, "IdPantalla", "Nombre", pantalla.MenuPadre);
            return View(pantalla);
        }

        // POST: Pantallas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdPantalla,Nombre,Recurso,isMenu,MenuPadre,Controlador,Vista,MenuOrden,cod_RA")] Pantalla pantalla)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pantalla).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MenuPadre = new SelectList(db.Pantalla, "IdPantalla", "Nombre", pantalla.MenuPadre);
            return View(pantalla);
        }

        // GET: Pantallas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pantalla pantalla = db.Pantalla.Find(id);
            if (pantalla == null)
            {
                return HttpNotFound();
            }
            return View(pantalla);
        }

        // POST: Pantallas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pantalla pantalla = db.Pantalla.Find(id);
            db.Pantalla.Remove(pantalla);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Menus()
        {

            var user = User.Identity.GetUserName();          
            var menus = db.fn_menu(user).OrderBy(i => i.IdPantalla);
            return PartialView("_MenusPartial", menus);
        }


        public ActionResult SubMenus(int id)
        {

            var user = User.Identity.GetUserName();

            var menus = db.fn_menu(user).Where(w => w.MenuPadre == id).OrderBy(i => i.IdPantalla);
           

            return PartialView("_SubMenusPartial", menus);
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
