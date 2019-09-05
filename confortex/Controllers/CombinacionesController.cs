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
using System.Data.Entity.Core.Objects;
using Confortex.Clases;

namespace Confortex.Controllers
{
    [Authorize]
    public class CombinacionesController : Controller
    {
        private ConfortexEntities db = new ConfortexEntities();

        [Accesso]
        // GET: Combinaciones
        public ActionResult Index()
        {
            return View(db.Combinacion.ToList());
        }

        // GET: Combinaciones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Combinacion combinacion = db.Combinacion.Find(id);
            if (combinacion == null)
            {
                return HttpNotFound();
            }
            return View(combinacion);
        }

        // GET: Combinaciones/Create
        public ActionResult Create()
        {
            ViewBag.IdFuncion = new SelectList(db.Funcion, "Nombre", "Nombre");
            return PartialView();
        }

        // POST: Combinaciones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(String Nombre, String Descripcion, double CantidadTela, PrecioCombinacion[] preciocombinacion)
        {

            try
            {
                String mensaje = "";
                int idComb = 0;

                try
                {
                    using (db)
                    {
                        idComb = Convert.ToInt16(db.sp_GestionarCombinacion(0, Nombre, Descripcion, CantidadTela, clsReferencias.INSERT).SingleOrDefault().Value);
                    }

                }
                catch (Exception ex)
                {
                    mensaje = new clsException(ex).Message();
                    idComb = -1;
                }
                using (var dbcontext = new ConfortexEntities())
                {
                    if (idComb != -1)
                    {

                        for (int i = 0; i < preciocombinacion.Length; i++)
                        {

                            PrecioCombinacion cf = preciocombinacion[i];

                            cf.IdCombinacion = idComb;
                            cf.cod_RA = Cod_RA.cod_RA();

                            dbcontext.PrecioCombinacion.Add(cf);

                        }
                        dbcontext.SaveChanges();
                        return Json(new { Message = clsReferencias.Exito });

                    }
                    else
                    {
                        return Json(new { Message = mensaje });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

       

        // POST: Combinaciones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(int IdCombinacion, String Nombre, String Descripcion, double CantidadTela, PrecioCombinacion[] preciocombinacion)
        {
            try
            {
                String mensaje = "";

                try
                {
                    using (db)
                    {
                        db.sp_GestionarCombinacion(IdCombinacion, Nombre, Descripcion, CantidadTela, clsReferencias.UPDATE);
                    }

                }
                catch (Exception ex)
                {
                    mensaje = new clsException(ex).Message();
                }
                using (var dbcontext = new ConfortexEntities())
                {
                    //List<CombinacionFuncion> lista = combinacionfuncion.ToList();
                    List<PrecioCombinacion> cflist = dbcontext.PrecioCombinacion.Where(w => w.IdCombinacion == IdCombinacion && w.regAnulado == false).ToList();
                    //List<CombinacionFuncion> cflist = dbcontext.CombinacionFuncion.Where(w => w.IdCombinacion == IdCombinacion && w.regAnulado == false).Except(combinacionfuncion.AsEnumerable()).ToList();

                    PrecioCombinacion cf = null;
                    for (int i = 0; i < preciocombinacion.Length; i++)
                    {
                        if (preciocombinacion[i].IdPrecioCombinacion == 0) //Agregando Nuevo
                        {

                            cf = preciocombinacion[i];
                            cf.IdCombinacion = IdCombinacion;
                            cf.cod_RA = Cod_RA.cod_RA();
                            dbcontext.PrecioCombinacion.Add(cf);
                        }
                        else //Actualizando
                        {
                            cf = dbcontext.PrecioCombinacion.Find(preciocombinacion[i].IdPrecioCombinacion);

                            cf.Duracion = preciocombinacion[i].Duracion;
                            cf.PrecioUnitario = preciocombinacion[i].PrecioUnitario;
                            cf.NombreFuncion = preciocombinacion[i].NombreFuncion;

                            dbcontext.Entry(cf).State = EntityState.Modified;


                            cflist = cflist.Where(w => w.IdPrecioCombinacion != cf.IdPrecioCombinacion).ToList();
                        }
                        dbcontext.SaveChanges();
                    }

                    foreach (PrecioCombinacion combfun in cflist)
                    {
                       cf = dbcontext.PrecioCombinacion.Find(combfun.IdPrecioCombinacion);

                        cf.regAnulado = true;
                        dbcontext.Entry(cf).State = EntityState.Modified;

                    }
                    dbcontext.SaveChanges();

                    return Json(new { Message = clsReferencias.Exito });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        // GET: Combinaciones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Combinacion combinacion = db.Combinacion.Find(id);
            if (combinacion == null)
            {
                return HttpNotFound();
            }
            return View(combinacion);
        }

        // POST: Combinaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                db.sp_GestionarCombinacion(id, "", "", 0.00, clsReferencias.DELETE);
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

        public ActionResult searchCombinaciones()
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
                var v = (from a in db.vw_ObtenerCombinaciones select a);


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.Nombre.Contains(searchv) || a.Descripcion.Contains(searchv));
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

        public ActionResult searchCombinacionFuncion()
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
                var v = (from a in db.PrecioCombinacion.Where(w => w.regAnulado == false) select new { a.IdCombinacion, a.IdPrecioCombinacion, a.NombreFuncion, a.PrecioUnitario, a.Duracion, a.regAnulado });


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.IdCombinacion.ToString().Equals(searchv));
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
