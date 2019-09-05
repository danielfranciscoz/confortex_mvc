using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Mvc;
using Confortex.Models;
using System.Data.SqlClient;
using System.Configuration;
using Confortex.Clases;
using Confortex.Referencias;

namespace Confortex.Controllers
{
    [Authorize]
    public class PiezaTallaMedidaSController : Controller
    {
        private ConfortexEntities db = new ConfortexEntities();

        [Accesso]
        // GET: PiezaTallaMedida_Standars
        public ActionResult Index()
        {
            var piezaTallaMedida_Standar = db.PiezaTallaMedida_Standar.Include(p => p.Medida1).Include(p => p.Pieza).Include(p => p.Talla);
            return View(piezaTallaMedida_Standar.ToList());
        }

        public ActionResult Demo()
        {
            return View();
        }

        // GET: PiezaTallaMedida_Standars/Details/5


        // GET: PiezaTallaMedida_Standars/Create
        public ActionResult Create()
        {
            ViewBag.IdMedida = new SelectList(db.Medida, "IdMedida", "Nombre");
            ViewBag.IdPieza = new SelectList(db.Pieza, "IdPieza", "Nombre");
            ViewBag.IdTalla = new SelectList(db.Talla, "IdTalla", "Nombre");
            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(PiezaTallaMedida_Standar[] PiezaTallaMedida)
        {
            try
            {
                for (int k = 0; k < PiezaTallaMedida.Length; k++)
                {
                    PiezaTallaMedida_Standar p = db.PiezaTallaMedida_Standar.Find(new object[] { PiezaTallaMedida[k].IdPieza, PiezaTallaMedida[k].IdTalla, PiezaTallaMedida[k].IdMedida });
                    if (p == null)
                    {
                        if (PiezaTallaMedida[k].Medida > 0)
                        {
                            PiezaTallaMedida[k].cod_RA = Cod_RA.cod_RA();
                            db.PiezaTallaMedida_Standar.Add(PiezaTallaMedida[k]);
                        }
                    }
                    else
                    {
                        p.Medida = PiezaTallaMedida[k].Medida;
                        db.Entry(p).State = EntityState.Modified;
                    }

                    db.SaveChanges();
                }

                return Json(new { Message = clsReferencias.Exito });
            }
            catch (Exception ex)
            {
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        public ActionResult ModalInputs(int idpieza)
        {
            var medidas = db.PiezaMedida.Where(w => w.IdPieza == idpieza && w.Medida.regAnulado == false).ToList();

            return PartialView(medidas);
        }

        public JsonResult ObtenerCatalogo(int IdPieza)
        {
            clsCallProcedure procedimiento = new clsCallProcedure();

            String ProcedureName = "[dbo].[sp_ObtenerCatalogoStandard]";
            Dictionary<String, String> parametros = new Dictionary<String, String>();
            parametros.Add("@IdPieza", IdPieza.ToString());

            return procedimiento.Call(ProcedureName, parametros);

        }

        //public JsonResult Demo()
        //{
        //    var idpieza = 13;
        //    var columns = db.PiezaMedida.Where(w => w.IdPieza == idpieza).Select(s => s.Medida.Nombre).ToArray();


        //}

        // POST: PiezaTallaMedida_Standars/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //public ActionResult Create(int IdPieza, int IdTalla, MedidaValor[] medida)
        //{
        //    try
        //    {
        //        for (int k = 0; k < medida.Length; k++)
        //        {
        //            PiezaTallaMedida_Standar catalogo = new PiezaTallaMedida_Standar();
        //            catalogo.IdPieza = IdPieza;
        //            catalogo.IdTalla = IdTalla;
        //            catalogo.IdMedida = medida[k].IdMedida;
        //            catalogo.Medida = medida[k].Valor;
        //            catalogo.cod_RA = Cod_RA.cod_RA();
        //            db.PiezaTallaMedida_Standar.Add(catalogo);
        //            db.SaveChanges();
        //        }

        //        return Json(new { Message = clsReferencias.Exito });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Message = new clsException(ex).Message() });
        //    }
        //}

        // GET: PiezaTallaMedida_Standars/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PiezaTallaMedida_Standar piezaTallaMedida_Standar = db.PiezaTallaMedida_Standar.Find(id);
            if (piezaTallaMedida_Standar == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdMedida = new SelectList(db.Medida, "IdMedida", "Nombre", piezaTallaMedida_Standar.IdMedida);
            ViewBag.IdPieza = new SelectList(db.Pieza, "IdPieza", "Nombre", piezaTallaMedida_Standar.IdPieza);
            ViewBag.IdTalla = new SelectList(db.Talla, "IdTalla", "Nombre", piezaTallaMedida_Standar.IdTalla);
            return View(piezaTallaMedida_Standar);
        }

        // POST: PiezaTallaMedida_Standars/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]


        // GET: PiezaTallaMedida_Standars/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    PiezaTallaMedida_Standar piezaTallaMedida_Standar = db.PiezaTallaMedida_Standar.Find(id);
        //    if (piezaTallaMedida_Standar == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(piezaTallaMedida_Standar);
        //}

        //// POST: PiezaTallaMedida_Standars/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    PiezaTallaMedida_Standar piezaTallaMedida_Standar = db.PiezaTallaMedida_Standar.Find(id);
        //    db.PiezaTallaMedida_Standar.Remove(piezaTallaMedida_Standar);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}


        public ActionResult searchD(int idPieza)
        {
            var draw = "";
            var totalRecords = "";
            var searchv = idPieza;
            dynamic dresult = ObtenerCatalogo(Convert.ToInt32(searchv)).Data;


            return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = dresult.data }, JsonRequestBehavior.AllowGet);


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
