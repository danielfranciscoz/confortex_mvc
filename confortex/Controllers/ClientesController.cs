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
    public class ClientesController : Controller
    {
        private ConfortexEntities db = new ConfortexEntities();

        [Accesso]
        // GET: Clientes
        public ActionResult Index()
        {
            return View(db.Cliente.ToList());
        }

        // GET: Clientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Cliente.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // GET: Clientes/Create
        public ActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(string PrimerNombre, string SegundoNombre, string PrimerApellido, string SegundoApellido, string Cedula, string Direccion, string Correo, string Telefono, string Celular, int IdOperadoraTelefonica, bool EstadoCivil, string RazonSocial, string RUC, string TelefonoCliente, string DireccionCliente, int CantidadPersonal)
        {
            return Json(new { Message = GestionarCliente(0, PrimerNombre, SegundoNombre, PrimerApellido, SegundoApellido, Cedula, Direccion, Correo, Telefono, Celular, IdOperadoraTelefonica, EstadoCivil, RazonSocial, RUC, TelefonoCliente, DireccionCliente, CantidadPersonal) });
        }

        // GET: Clientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Cliente.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(int IdCliente, string PrimerNombre, string SegundoNombre, string PrimerApellido, string SegundoApellido, string Cedula, string Direccion, string Correo, string Telefono, string Celular, int IdOperadoraTelefonica, bool EstadoCivil, string RazonSocial, string RUC, string TelefonoCliente, string DireccionCliente, int CantidadPersonal)
        {
            return Json(new { Message = GestionarCliente(IdCliente, PrimerNombre, SegundoNombre, PrimerApellido, SegundoApellido, Cedula, Direccion, Correo, Telefono, Celular, IdOperadoraTelefonica, EstadoCivil, RazonSocial, RUC, TelefonoCliente, DireccionCliente, CantidadPersonal) });

        }

        // GET: Clientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Cliente.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]

        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Cliente cliente = db.Cliente.Find(id);
                cliente.regAnulado = true;
                db.SaveChanges();
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        public ActionResult searchCliente()
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
                var v = (from a in db.vw_ObtenerClientes select a);


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a =>

                        a.RazonSocial.Contains(searchv) ||
                         a.RUC.Contains(searchv) ||
                           a.NombreContacto.Contains(searchv) ||
                             a.CorreoContacto.Contains(searchv)
                        );
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
        private String GestionarCliente(int IdCliente, string PrimerNombre, string SegundoNombre, string PrimerApellido, string SegundoApellido, string Cedula, string Direccion, string Correo, string Telefono, string Celular, int IdOperadoraTelefonica, bool EstadoCivil, string RazonSocial, string RUC, string TelefonoCliente, string DireccionCliente, int CantidadPersonal)
        {
            string result = "";
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Persona persona;
                    Cliente cliente;
                    if (IdCliente == 0)
                    {
                        persona = new Persona();
                        cliente = new Cliente();
                    }
                    else
                    {
                        persona = db.Persona.Find(db.Cliente_Persona.Where(w => w.IdCliente == IdCliente && w.regAnulado == false).Select(s => s.IdPersona).FirstOrDefault());
                        cliente = db.Cliente.Find(IdCliente);
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


                    cliente.RazonSocial = RazonSocial;
                    cliente.RUC = RUC;
                    cliente.Telefono = TelefonoCliente;
                    cliente.Direccion = DireccionCliente;
                    cliente.CantidadEmpleados = CantidadPersonal;


                    if (IdCliente == 0)
                    {
                        db.Persona.Add(persona);
                        cliente.cod_RA = Cod_RA.cod_RA();
                        db.Cliente.Add(cliente);
                        //db.SaveChanges();

                        Cliente_Persona cliente_persona = new Cliente_Persona();
                        cliente_persona.IdCliente = cliente.IdCliente;
                        cliente_persona.IdPersona = persona.IdPersona;

                        db.Cliente_Persona.Add(cliente_persona);
                        //db.SaveChanges();

                    }
                    else
                    {
                        db.Entry(persona).State = EntityState.Modified;
                        db.Entry(cliente).State = EntityState.Modified;
                        //db.SaveChanges();
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
    }


}