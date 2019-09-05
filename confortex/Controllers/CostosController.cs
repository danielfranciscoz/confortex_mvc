using Confortex.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Confortex.Controllers
{
    [Authorize]
    public class CostosController : Controller
    {
        // GET: Costos
        public ActionResult InformeCostos()
        {
            return View();
        }

        public ActionResult ReporteCostos(string fecha, bool isYear)
        {
      
                ReportViewerViewModel model = new ReportViewerViewModel();

                var user = User.Identity.GetUserName();
                var iduser = User.Identity.GetUserId();

                string content = Url.Content("~/Reportes/ASP/CRV.aspx?id=2&Fecha=" + fecha + "&IsYear=" + isYear+ "&user=" + user);
                model.ReportPath = content;

                return View("Reportecostos", model);            
        }
    }
}