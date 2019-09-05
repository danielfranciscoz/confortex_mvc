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
    public class PiezasController : Controller
    {
        private ConfortexEntities db = new ConfortexEntities();

        [Accesso]
        // GET: Piezas
        public ActionResult Index()
        {
            return View();
        }

       
        public ActionResult MedidasStandar(int idpieza)
        {
            ViewBag.IdTalla = idpieza;
            var nombre = db.Pieza.Where(w => w.IdPieza == idpieza).Select(a => a.Nombre).FirstOrDefault();
            ViewBag.nombre = nombre;
            return View();
        }


        // GET: Piezas/Details/5


        // GET: Piezas/Create
        public ActionResult Create()
        {
            ViewBag.IdFuncion = new SelectList(db.Funcion, "Nombre", "Nombre");
            ViewBag.IdTalla = new SelectList(db.Talla, "Nombre", "Nombre");
            ViewBag.IdMedida = new SelectList(db.Medida, "Nombre", "Nombre");
            ViewBag.IdAccesorio = new SelectList(db.Accesorio.Where(w=>w.regAnulado==false && w.isAccesorio==false), "IdAccesorio", "Nombre");
            return PartialView();
        }

        // POST: Piezas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "IdPieza,Nombre,SexoPieza,CantidadTela")] Pieza p, PrecioPieza[] precioPieza, Medida[] medida, Talla[] talla, PiezaAccesorio[] piezaAccesorio)
        {
            try
            {

                //Agregando la Pieza

                p.cod_RA = Cod_RA.cod_RA();
                db.Pieza.Add(p);

                //Agregando el precio de la pieza
                for (int i = 0; i < precioPieza.Length; i++)
                {
                    PrecioPieza pp = precioPieza[i];
                    pp.IdPieza = p.IdPieza;

                    pp.cod_RA = Cod_RA.cod_RA();

                    db.PrecioPieza.Add(pp);
                }

                //Agregando todas las tallas a la Pieza
                for (int i = 0; i < talla.Length; i++)
                {
                    PiezaTalla pt = new PiezaTalla();
                    pt.IdTalla = talla[i].IdTalla;
                    pt.IdPieza = p.IdPieza;
                    pt.FechaCreacion = System.DateTime.Now;
                    db.PiezaTalla.Add(pt);
                }

                //Agregando todas las medidas a la pieza
                for (int i = 0; i < medida.Length; i++)
                {
                    PiezaMedida pm = new PiezaMedida();
                    pm.IdMedida = medida[i].IdMedida;
                    pm.IdPieza = p.IdPieza;
                    pm.FechaCreacion = System.DateTime.Now;
                    db.PiezaMedida.Add(pm);
                }

                //Agregando todos los insumos de produccion de la pieza
                for (int i = 0; i < piezaAccesorio.Length; i++)
                {
                    
                    piezaAccesorio[i].IdPieza = p.IdPieza;
                    piezaAccesorio[i].FechaCreacion = System.DateTime.Now;
                    db.PiezaAccesorio.Add(piezaAccesorio[i]);
                }

                db.SaveChanges();
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }


        [HttpPost]
        public ActionResult Edit([Bind(Include = "IdPieza,Nombre,SexoPieza,CantidadTela")] Pieza p, PrecioPieza[] precioPieza, Medida[] medida, Talla[] talla, PiezaAccesorio[] piezaAccesorio)
        {
            try
            {

                //Modificando la Pieza
                Pieza pieza = db.Pieza.Find(p.IdPieza);
                pieza.Nombre = p.Nombre;
                pieza.SexoPieza = p.SexoPieza;
                pieza.CantidadTela = p.CantidadTela;
                db.Entry(pieza).State = EntityState.Modified;

                List<PrecioPieza> pplist = db.PrecioPieza.Where(w => w.IdPieza == p.IdPieza && w.regAnulado == false).ToList();

                //Agregando el precio de la pieza
                PrecioPieza pp = null;
                for (int i = 0; i < precioPieza.Length; i++)
                {
                    if (precioPieza[i].IdPrecioPieza == 0) //Agregar nuevo detalle
                    {
                        pp = precioPieza[i];
                        pp.IdPieza = p.IdPieza;
                        pp.NombreFuncion = precioPieza[i].NombreFuncion;
                        pp.cod_RA = Cod_RA.cod_RA();
                        db.PrecioPieza.Add(pp);
                    }
                    else
                    {
                        pp = db.PrecioPieza.Find(precioPieza[i].IdPrecioPieza);
                        pp.PrecioMO = precioPieza[i].PrecioMO;
                        pp.Duracion = precioPieza[i].Duracion;

                        db.Entry(pp).State = EntityState.Modified;

                        pplist = pplist.Where(w => w.IdPrecioPieza != pp.IdPrecioPieza).ToList();
                    }
                    db.SaveChanges();
                }

                //Elimando las funciones que no retornaron
                pp = null;
                foreach (PrecioPieza pre in pplist)
                {
                    pp = db.PrecioPieza.Find(pre.IdPrecioPieza);
                    pp.regAnulado = true;
                    db.Entry(pp).State = EntityState.Modified;
                }
                db.SaveChanges();

                //Agregando todas las tallas a la Pieza
                List<PiezaTalla> piezastallas = db.PiezaTalla.Where(w => w.IdPieza == p.IdPieza).ToList();
                PiezaTalla pt = null;
                for (int i = 0; i < talla.Length; i++)
                {
                    if (piezastallas.Where(w => w.IdTalla == talla[i].IdTalla).ToList().Count == 0)
                    {
                        pt = new PiezaTalla();
                        pt.IdTalla = talla[i].IdTalla;
                        pt.IdPieza = pieza.IdPieza;
                        pt.FechaCreacion = System.DateTime.Now;
                        db.PiezaTalla.Add(pt);

                        db.SaveChanges();
                    }
                    else
                    {
                        piezastallas = piezastallas.Where(w => w.IdTalla != talla[i].IdTalla).ToList();
                    }
                }

                foreach (PiezaTalla piezaT in piezastallas)
                {
                    pt = db.PiezaTalla.Find(new Object[] { piezaT.IdPieza, piezaT.IdTalla });
                    db.PiezaTalla.Remove(pt);
                }
                db.SaveChanges();


                //Agregando todas las medidas a la pieza
                List<PiezaMedida> piezasmedidas = db.PiezaMedida.Where(w => w.IdPieza == p.IdPieza).ToList();
                PiezaMedida pm = null;

                for (int i = 0; i < medida.Length; i++)
                {
                    if (piezasmedidas.Where(w => w.IdMedida == medida[i].IdMedida).ToList().Count == 0)
                    {
                        pm = new PiezaMedida();
                        pm.IdMedida = medida[i].IdMedida;
                        pm.IdPieza = pieza.IdPieza;
                        pm.FechaCreacion = System.DateTime.Now;
                        db.PiezaMedida.Add(pm);

                        db.SaveChanges();
                    }
                    else
                    {
                        piezasmedidas = piezasmedidas.Where(w => w.IdMedida != medida[i].IdMedida).ToList();
                    }
                }

                foreach (PiezaMedida piezaM in piezasmedidas)
                {
                    pm = db.PiezaMedida.Find(new Object[] { piezaM.IdPieza, piezaM.IdMedida });
                    db.PiezaMedida.Remove(pm);
                }

                //Agregando todos los insumos de produccion de la pieza

                List<PiezaAccesorio> piezasAccesorios = db.PiezaAccesorio.Where(w => w.IdPieza == p.IdPieza).ToList();
                PiezaAccesorio pa = null;

                if (piezaAccesorio != null)
                {
                    for (int i = 0; i < piezaAccesorio.Length; i++)
                    {
                        if (piezasAccesorios.Where(w => w.IdAccesorio == piezaAccesorio[i].IdAccesorio).ToList().Count == 0)
                        {
                            piezaAccesorio[i].IdPieza = p.IdPieza;
                            piezaAccesorio[i].FechaCreacion = System.DateTime.Now;

                            db.PiezaAccesorio.Add(piezaAccesorio[i]);

                        }
                        else
                        {
                            pa = db.PiezaAccesorio.Find(new object[] { piezaAccesorio[i].IdPieza, piezaAccesorio[i].IdAccesorio });
                            pa.Cantidad = piezaAccesorio[i].Cantidad;

                            db.Entry(pa).State = EntityState.Modified;

                            piezasAccesorios = piezasAccesorios.Where(w => w.IdAccesorio != piezaAccesorio[i].IdAccesorio).ToList();
                        }
                        db.SaveChanges();
                    }

                    //foreach (PiezaAccesorio piezaA in piezasAccesorios)
                    //{
                    //    pa = db.PiezaAccesorio.Find(new Object[] { piezaA.IdPieza, piezaA.IdAccesorio });
                    //    db.PiezaAccesorio.Remove(pa);
                    //}
                    db.PiezaAccesorio.RemoveRange(piezasAccesorios.ToArray());
                    db.SaveChanges();
                }

                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Message = new clsException(ex).Message()
                });
            }
        }

        // GET: Piezas/Delete/5


        // POST: Piezas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Pieza pieza = db.Pieza.Find(id);
                pieza.regAnulado = true;
                db.Entry(pieza).State = EntityState.Modified;
                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }
        public ActionResult searchPiezas()
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
                var v = (from a in db.Pieza.Where(w => w.regAnulado == false) select new { a.IdPieza, a.Nombre, a.SexoPieza, a.CantidadTela });


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.Nombre.Contains(searchv) || a.SexoPieza.Contains(searchv));
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

        public ActionResult searchPrecioPieza()
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
                var v = (from a in db.PrecioPieza.Where(w => w.regAnulado == false) select new { a.IdPieza, a.IdPrecioPieza, a.Duracion, a.NombreFuncion, a.PrecioMO });


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.IdPieza.ToString().Equals(searchv.ToString()));
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

        public ActionResult searchPiezaTalla()
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
                var v = (from a in db.PiezaTalla select new { a.IdPieza, a.IdTalla, a.Talla.Nombre });


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.IdPieza.ToString().Equals(searchv.ToString()));
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

        public ActionResult searchPiezaMedida()
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
                var v = (from a in db.PiezaMedida select new { a.IdPieza, a.IdMedida, a.Medida.Nombre });


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.IdPieza.ToString().Equals(searchv.ToString()));
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

        public ActionResult searchPiezaInsumo()
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
                var v = (from a in db.PiezaAccesorio select new { a.IdPieza, a.Accesorio.Nombre, a.Cantidad,a.IdAccesorio });


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.IdPieza.ToString().Equals(searchv.ToString()));
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
