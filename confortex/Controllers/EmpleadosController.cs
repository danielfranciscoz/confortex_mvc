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
    public class EmpleadosController : Controller
    {
        private ConfortexEntities db = new ConfortexEntities();

        [Accesso]
        // GET: Empleados
        public ActionResult Index()
        {
   
            return View();
        }

        // GET: Empleados/Details/5
       

        // GET: Empleados/Create
        public ActionResult Create()
        {
            ViewBag.Cargo = new SelectList(db.Cargo, "NombreCargo", "NombreCargo");
            ViewBag.IdEmpleado = new SelectList(db.Persona, "IdPersona", "PrimerNombre");
            return PartialView();
        }

        // POST: Empleados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public ActionResult Create(string PrimerNombre, string SegundoNombre, string PrimerApellido, string SegundoApellido, string Cedula, string Direccion, string Correo, string Telefono, string Celular, int IdOperadoraTelefonica, bool EstadoCivil, string CodEmpleado, int PersonasDependientes, string CelularFamiliar, string Cargo)
        {
            return Json(new { Message = GestionarEmpleado(0, PrimerNombre, SegundoNombre, PrimerApellido, SegundoApellido, Cedula, Direccion, Correo, Telefono, Celular, IdOperadoraTelefonica, EstadoCivil, CodEmpleado, PersonasDependientes, CelularFamiliar, Cargo) });
        }


        // POST: Empleados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public ActionResult Edit(int IdEmpleado, string PrimerNombre, string SegundoNombre, string PrimerApellido, string SegundoApellido, string Cedula, string Direccion, string Correo, string Telefono, string Celular, int IdOperadoraTelefonica, bool EstadoCivil, string CodEmpleado, int PersonasDependientes, string CelularFamiliar, string Cargo)
        {
            return Json(new { Message = GestionarEmpleado(IdEmpleado, PrimerNombre, SegundoNombre, PrimerApellido, SegundoApellido, Cedula, Direccion, Correo, Telefono, Celular, IdOperadoraTelefonica, EstadoCivil, CodEmpleado, PersonasDependientes, CelularFamiliar, Cargo) });
        }

        // GET: Empleados/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleado empleado = db.Empleado.Find(id);
            if (empleado == null)
            {
                return HttpNotFound();
            }
            return View(empleado);
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Empleado empleado = db.Empleado.Find(id);
                empleado.regAnulado = true;
                db.SaveChanges();
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

        private String GestionarEmpleado(int IdEmpleado, string PrimerNombre, string SegundoNombre, string PrimerApellido, string SegundoApellido, string Cedula, string Direccion, string Correo, string Telefono, string Celular, int IdOperadoraTelefonica, bool EstadoCivil, string CodEmpleado, int PersonasDependientes, string CelularFamiliar, string Cargo)
        {
            string result = "";

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Persona persona;
                    Empleado empleado;
                    if (IdEmpleado == 0)
                    {
                        persona = new Persona();
                        empleado = new Empleado();
                    }
                    else
                    {
                        persona = db.Persona.Find(IdEmpleado);
                        empleado = db.Empleado.Find(IdEmpleado);
                    }

                    persona.PrimerNombre = PrimerNombre;
                    persona.SegundoNombre = SegundoNombre;
                    persona.PrimerApellido = PrimerApellido;
                    persona.SegundoApellido = SegundoApellido;
                    persona.Cedula = Cedula;
                    persona.Direccion = Direccion;
                    persona.Correo = Correo;
                    persona.Telefono = Telefono;
                    persona.Celular = Celular;
                    persona.IdOperadoraTelefonica = IdOperadoraTelefonica;
                    persona.EstadoCivil = EstadoCivil;

                    empleado.codEmpleado = CodEmpleado;
                    empleado.PersonasDependientes = Byte.Parse("" + PersonasDependientes);
                    empleado.CelularFamiliar = CelularFamiliar;
                    empleado.Cargo = Cargo;

                    if (IdEmpleado == 0)
                    {
                        db.Persona.Add(persona);
                        db.SaveChanges(); // Guardo para obtener el id de la persona

                        empleado.IdEmpleado = persona.IdPersona;
                        empleado.cod_RA = Cod_RA.cod_RA();

                        db.Empleado.Add(empleado);
                    }
                    else
                    {
                        db.Entry(persona).State = EntityState.Modified;
                        db.Entry(empleado).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    transaction.Commit();
                    result = clsReferencias.Exito;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result = new clsException(ex).Message();
                }
            }
            return result;
        }

        public ActionResult searchEmpleados()
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
                var v = (from a in db.vw_ObtenerEmpleados select a);


                if ((string.IsNullOrEmpty(searchv)))
                {
                    searchv = "";
                }
                    v = v.Where(a => a.regAnulado == false && (a.PrimerNombre.Contains(searchv) ||
                    a.SegundoNombre.Contains(searchv) ||
                    a.PrimerApellido.Contains(searchv) ||
                    a.SegundoApellido.Contains(searchv) ||
                    a.Cedula.Contains(searchv) ||
                    a.Direccion.Contains(searchv) ||
                    a.Correo.Contains(searchv) ||
                    a.Telefono.Contains(searchv) ||
                    a.Celular.Contains(searchv) ||
                    a.OperadoraTelefonica.Contains(searchv) ||
                    a.Cargo.Contains(searchv) ||
                    a.DescSalario.Contains(searchv)));
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
