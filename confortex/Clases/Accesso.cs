using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Core.Objects;
using Confortex.Models;
using Confortex.Referencias;

namespace Confortex
{
 
    public class Accesso : AuthorizeAttribute 
    {
        private ConfortexEntities db = new ConfortexEntities();
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            Boolean acceso = false;
            String controlador = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
            String vista = httpContext.Request.RequestContext.RouteData.Values["action"].ToString();
            var user = httpContext.User.Identity.GetUserName();
        

            ObjectParameter outParam = new ObjectParameter("result", 0);

      
            db.Sp_PermisoMenu(controlador,vista, user, clsReferencias.Acceder, outParam);
            int p = int.Parse(outParam.Value.ToString());
            if (p > 0)
            {
                acceso = true;
            }

            return acceso;
           
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
           if (filterContext.HttpContext.Request.IsAuthenticated)
           {
              filterContext.HttpContext.Response.StatusCode = 403;
              filterContext.Result = new ViewResult { ViewName = "~/Views/shared/UnAccess.cshtml" };
          }
           else
           {
               base.HandleUnauthorizedRequest(filterContext);
           }
        }
    }
}