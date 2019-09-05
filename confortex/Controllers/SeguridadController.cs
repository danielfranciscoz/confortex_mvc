using Confortex.Clases;
using Confortex.Models;
using Confortex.Referencias;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Confortex.Controllers
{
   
    public class SeguridadController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ConfortexEntities db = new ConfortexEntities();
        // GET: Accesos
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult createuser()
        {
            return PartialView();
        }
        public ActionResult createpermiso()
        {
            ViewBag.Rol = new SelectList(db.Rol, "Id", "Name");
            ViewBag.IdPanta = new SelectList(db.Pantalla.Where(w=>w.isMenu==true), "IdPantalla", "Nombre");

            return PartialView();
        }
        public ActionResult createrol()
        {
            ViewBag.IdPantalla = new SelectList(db.Pantalla.Where(w=>w.isMenu==true),"IdPantalla","Nombre");
            return PartialView();
        }

        public ActionResult createroluser()
        {
            ViewBag.IdRol = new SelectList(db.Rol, "Id", "Name");
            ViewBag.IdUsuario = new SelectList(db.Usuario.Where(W=>W.regAnulado == false), "Username", "Username");
            return PartialView();
        }

        public ActionResult resetuser()
        {
            return PartialView();
        }
        public ActionResult deleteuser()
        {
            return PartialView();
        }
        public ActionResult deleterol()
        {
            return PartialView();
        }
        public ActionResult deleteroluser()
        {
            return PartialView();
        }
        public ActionResult deletepermiso()
        {
            return PartialView();
        }
        public SeguridadController()
        {
        }
        public SeguridadController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public ActionResult Register(string usuario,string pass)
        {
            try
            {
                var user = new ApplicationUser { UserName = usuario, Email = usuario+"@"+usuario};
                var result = UserManager.Create(user, pass);
                if (result.Succeeded)
                {
                    return Json(new
                    {
                        Message = clsReferencias.Exito
                    });

                }
                else
                {
                    return Json(new { Message = result.Errors});
                }
                  
            }
             catch (Exception er) {
                return Json(new { Message = new clsException(er).Message() });
            }


            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario

        }

        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
           
            var user = await UserManager.FindByNameAsync(model.User);
            if (user == null)
            {
                return Json(new { Message = "No se pudo realizar el guardado" });

            }
            string code = UserManager.GeneratePasswordResetToken(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id,code, model.Password);
            if (result.Succeeded)
            {
                return Json(new { Message = "Exito" });
            }
            else
            {
                return Json(new { Message = result.Errors });
            }
           
            
        }

        public ActionResult saveRol(Rol rol)
        {
            try
            {                
                db.Rol.Add(rol);
                db.SaveChanges();

                return Json(new
                {
                    Message = clsReferencias.Exito
                });

            }
            catch (Exception er)
            {
                return Json(new { Message = new clsException(er).Message() });
            }
        }
        public ActionResult saveRolUser(UsuarioRol rol)
        {
            try
            {
                db.UsuarioRol.Add(rol);
                db.SaveChanges();

                return Json(new
                {
                    Message = clsReferencias.Exito
                });

            }
            catch (Exception er)
            {
                return Json(new { Message = new clsException(er).Message() });
            }
        }

        public ActionResult savePermiso(Permiso rol)
        {
            try
            {
                db.Permiso.Add(rol);
                db.SaveChanges();

                return Json(new
                {
                    Message = clsReferencias.Exito
                });

            }
            catch (Exception er)
            {
                return Json(new { Message = new clsException(er).Message() });
            }
        }

        public ActionResult editRol(Rol rol)
        {
            try
            {

                db.Entry(rol).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new
                {
                    Message = clsReferencias.Exito
                });

            }
            catch (Exception er)
            {
                return Json(new { Message = new clsException(er).Message() });
            }
        }
        public ActionResult deleteeuser(string id)
        {
            try
            {
                Usuario us = db.Usuario.Find(id);
                us.regAnulado = true;
                db.Entry(us).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new
                {
                    Message = clsReferencias.Exito
                });

            }
            catch (Exception er)
            {
                return Json(new { Message = new clsException(er).Message() });
            }
        }

        public ActionResult deleteeRol(string id)
        {
            try
            {

                Rol rol = db.Rol.Find(id);
                db.Rol.Remove(rol);
                db.SaveChanges();
                return Json(new
                {
                    Message = clsReferencias.Exito
                });

            }
            catch (Exception er)
            {
                return Json(new { Message = new clsException(er).Message() });
            }
        }
        public ActionResult deleteeUserRol(UsuarioRol us)
        {
            try
            {
              
                UsuarioRol rol = db.UsuarioRol.Where(w=>w.UserId==us.UserId && w.RoleId==us.RoleId).FirstOrDefault();
                db.UsuarioRol.Remove(rol);
                db.SaveChanges();
                return Json(new
                {
                    Message = clsReferencias.Exito
                });

            }
            catch (Exception er)
            {
                return Json(new { Message = new clsException(er).Message() });
            }
        }

        public ActionResult deleteePermiso(Permiso permiso)
        {
            try
            {

                Permiso per = db.Permiso.Where(w => w.IdRol == permiso.IdRol && w.IdPantalla == permiso.IdPantalla).FirstOrDefault();
                db.Permiso.Remove(per);
                db.SaveChanges();
                return Json(new
                {
                    Message = clsReferencias.Exito
                });

            }
            catch (Exception er)
            {
                return Json(new { Message = new clsException(er).Message() });
            }
        }
        public ActionResult searchUsuarios()
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
                var v = (from a in db.Usuario.Where(w=>w.regAnulado==false) select  new {a.UserName,a.IdUsuario } );


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.UserName.Contains(searchv));
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


        public ActionResult searchRoles()
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
                var v = (from a in db.Rol select new { a.Id, a.Name,a.Pantalla.Nombre,a.IdStarPage });


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.Name.Contains(searchv));
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

        public ActionResult searchRolesUser()
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
                var v = (from a in db.UsuarioRol select new { a.RoleId,a.UserId,a.Rol.Name,a.Usuario.IdUsuario });


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.RoleId.Contains(searchv) || a.UserId.Contains(searchv));
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

        public ActionResult searchPermisos()
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
                var v = (from a in db.Permiso select new { a.IdRol,a.IdPantalla,a.Pantalla.Nombre,a.Rol.Name});


                if (!(string.IsNullOrEmpty(searchv)))
                {
                    v = v.Where(a => a.Name.Contains(searchv) || a.Nombre.Contains(searchv));
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