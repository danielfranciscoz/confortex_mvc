using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Confortex.Models;

namespace Confortex.Controllers
{
    [Authorize]
    public class PersonasController : Controller
    {
        private ConfortexEntities db = new ConfortexEntities();

        // GET: Empleados
        public ActionResult Index()
        {
            var persona = db.Persona.Include(p => p.Empleado).Include(p => p.OperadoraTelefonica);
            return View(persona.ToList());
        }

        // GET: Empleados/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = db.Persona.Find(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            return View(persona);
        }

        // GET: Empleados/Create
        public ActionResult Create()
        {
            ViewBag.IdPersona = new SelectList(db.Empleado, "IdEmpleado", "codEmpleado");
            ViewBag.IdOperadoraTelefonica = new SelectList(db.OperadoraTelefonica, "IdOperadoraTelefonica", "Nombre");
            return PartialView();
        }

        // POST: Empleados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       
        public ActionResult Create([Bind(Include = "IdPersona,PrimerNombre,SegundoNombre,PrimerApellido,SegundoApellido,Cedula,Direccion,Correo,Telefono,Celular,IdOperadoraTelefonica,EstadoCivil,cod_RA")] Persona persona)
        {
            if (ModelState.IsValid)
            {
                db.Persona.Add(persona);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdPersona = new SelectList(db.Empleado, "IdEmpleado", "codEmpleado", persona.IdPersona);
            ViewBag.IdOperadoraTelefonica = new SelectList(db.OperadoraTelefonica, "IdOperadoraTelefonica", "Nombre", persona.IdOperadoraTelefonica);
            return View(persona);
        }

        // GET: Empleados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = db.Persona.Find(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdPersona = new SelectList(db.Empleado, "IdEmpleado", "codEmpleado", persona.IdPersona);
            ViewBag.IdOperadoraTelefonica = new SelectList(db.OperadoraTelefonica, "IdOperadoraTelefonica", "Nombre", persona.IdOperadoraTelefonica);
            return PartialView();
        }

        // POST: Empleados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdPersona,PrimerNombre,SegundoNombre,PrimerApellido,SegundoApellido,Cedula,Direccion,Correo,Telefono,Celular,IdOperadoraTelefonica,EstadoCivil,cod_RA")] Persona persona)
        {
            if (ModelState.IsValid)
            {
                db.Entry(persona).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdPersona = new SelectList(db.Empleado, "IdEmpleado", "codEmpleado", persona.IdPersona);
            ViewBag.IdOperadoraTelefonica = new SelectList(db.OperadoraTelefonica, "IdOperadoraTelefonica", "Nombre", persona.IdOperadoraTelefonica);
            return View(persona);
        }

        // GET: Empleados/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = db.Persona.Find(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            return View(persona);
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Persona persona = db.Persona.Find(id);
            db.Persona.Remove(persona);
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
    }
}
