using Confortex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Confortex.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ConfortexEntities db = new ConfortexEntities();
        public ActionResult Error()
        {
            return View();
        }
        public ActionResult Index()
        {
            var user = User.Identity.GetUserName();
            var controlador = db.UsuarioRol.Where(w => w.UserId == user).Select(s=>s.Rol.Pantalla.Controlador).FirstOrDefault();

            return RedirectToAction("Index",controlador);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return PartialView();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return PartialView();
        }

   


        public ActionResult DeleteGeneral()
        {            
            return PartialView();
        }
    }
}