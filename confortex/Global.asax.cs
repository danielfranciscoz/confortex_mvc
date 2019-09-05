
using Confortex.Clases;
using Confortex.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;


[assembly: PreApplicationStartMethod(typeof(Confortex.MvcApplication), "Register")]
namespace Confortex
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public static void Register()
        {
            HttpApplication.RegisterModule(typeof(QueryString));
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            String url = Request.Url.AbsoluteUri;
            Response.Redirect("~/Home/Error");
            ConfortexEntities db = new ConfortexEntities();
            logErrores le = new logErrores();
            le.FechaHora = DateTime.Now;
            le.Descripcion = new clsException(exception).Message();
            le.URL = url;
            try
            {

                le.idUser = User.Identity.GetUserId();

            }
            catch (Exception)
            {
                le.idUser = null;
            }
            db.logErrores.Add(le);
            db.SaveChanges();


        }
    }
}
