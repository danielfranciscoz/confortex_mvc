using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Confortex.Reportes.ASP
{
    public partial class CRV : System.Web.UI.Page
    {
        CrystalDecisions.CrystalReports.Engine.ReportDocument reportDocument = null;
        string fileName;

        protected void Page_Init(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            if (this.reportDocument != null)
            {
                this.reportDocument.Close();
                this.reportDocument.Dispose();
            }


            int IdR = int.Parse(Convert.ToString(Request.QueryString["id"]));
           
            Report(IdR);


        }
        private void Report(int id)
        {
            ReportDocument reportDocument = new ReportDocument();
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            string user = Convert.ToString(Request.QueryString["user"]);
            switch (id)
            {
                case 1:
                    string cotizacion = Convert.ToString(Request.QueryString["cotizacion"]);

                    reportDocument.Load(Server.MapPath("~/Reportes/rptPlantillaCotizacion.rpt"));
                    reportDocument.SetDatabaseLogon(builder.UserID.ToString(), builder.Password.ToString(), builder.DataSource.ToString(), builder.InitialCatalog.ToString(), true);
                    reportDocument.SetParameterValue("IdCotizacion", cotizacion);
                    reportDocument.SetParameterValue("Usuario", user);
                    ListCrystalReport.ID = "Cotizacion-No" + cotizacion;

                    break;
                case 2:
                    DateTime fecha = Convert.ToDateTime(Request.QueryString["Fecha"]);
                    bool isYear = Convert.ToBoolean(Request.QueryString["IsYear"]);

                    reportDocument.Load(Server.MapPath("~/Reportes/rptCostoMensual.rpt"));
                    reportDocument.SetDatabaseLogon(builder.UserID.ToString(), builder.Password.ToString(), builder.DataSource.ToString(), builder.InitialCatalog.ToString(), true);
                    reportDocument.SetParameterValue("@Fecha", fecha);
                    reportDocument.SetParameterValue("@IsYear", isYear);
                    reportDocument.SetParameterValue("Usuario", user);
                    ListCrystalReport.ID = "Informe de Costos";

                    break;
                case 3:
                    string coti = Convert.ToString(Request.QueryString["cotizacion"]);
                  

                    reportDocument.Load(Server.MapPath("~/Reportes/rptTickects.rpt"));
                    reportDocument.SetDatabaseLogon(builder.UserID.ToString(), builder.Password.ToString(), builder.DataSource.ToString(), builder.InitialCatalog.ToString(), true);
                    reportDocument.SetParameterValue("@IdCotizacion", coti);
                    reportDocument.SetParameterValue("@NoTicket", null);
                    ListCrystalReport.ID = "Tickets por pedido";

                    break;
                case 4:
                    string coti2 = Convert.ToString(Request.QueryString["cotizacion"]);
                    string ticket = Convert.ToString(Request.QueryString["ticket"]);

                    reportDocument.Load(Server.MapPath("~/Reportes/rptTickects.rpt"));
                    reportDocument.SetDatabaseLogon(builder.UserID.ToString(), builder.Password.ToString(), builder.DataSource.ToString(), builder.InitialCatalog.ToString(), true);
                    reportDocument.SetParameterValue("@IdCotizacion", coti2);
                    reportDocument.SetParameterValue("@NoTicket", ticket);
                    ListCrystalReport.ID = "Tickets No-"+ticket;

                    break;
                case 5:
                    string cotiz = Convert.ToString(Request.QueryString["cotizacion"]);
                    string ticketp = Convert.ToString(Request.QueryString["ticket"]);

                    reportDocument.Load(Server.MapPath("~/Reportes/rtpTicketFuncion.rpt"));
                    reportDocument.SetDatabaseLogon(builder.UserID.ToString(), builder.Password.ToString(), builder.DataSource.ToString(), builder.InitialCatalog.ToString(), true);
                    reportDocument.SetParameterValue("@IdCotizacion", cotiz);
                    if (ticketp.Equals("") || ticketp.Equals("null") || ticketp.Equals("undefined"))
                    {
                        reportDocument.SetParameterValue("@NoTicket", null);
                    }
                    else {
                        reportDocument.SetParameterValue("@NoTicket", ticketp);
                    }
                    ListCrystalReport.ID = "Tickets por pedido";

                    break;
                case 6:

                  
                    string nomina  = Convert.ToString(Request.QueryString["nomina"]);
                  //  string ticketp = Convert.ToString(Request.QueryString["ticket"]);

                    reportDocument.Load(Server.MapPath("~/Reportes/rptNomina.rpt"));
                    reportDocument.SetDatabaseLogon(builder.UserID.ToString(), builder.Password.ToString(), builder.DataSource.ToString(), builder.InitialCatalog.ToString(), true);
                    reportDocument.SetParameterValue("@IdNomina", nomina);
                    reportDocument.SetParameterValue("Usuario", user);
                    ListCrystalReport.ID = "Nomina No-" +nomina;

                    break;
                case 7:
                    
                    string cot = Convert.ToString(Request.QueryString["cotizacion"]);
                    //  string ticketp = Convert.ToString(Request.QueryString["ticket"]);

                    reportDocument.Load(Server.MapPath("~/Reportes/rptOrdenCompra.rpt"));
                    reportDocument.SetDatabaseLogon(builder.UserID.ToString(), builder.Password.ToString(), builder.DataSource.ToString(), builder.InitialCatalog.ToString(), true);
                    reportDocument.SetParameterValue("@IdCotizacion", cot);
                    reportDocument.SetParameterValue("Usuario", user);
                    ListCrystalReport.ID = "Requerimientos de Pedido-" + cot;

                    break;
            }

            ListCrystalReport.ReportSource = reportDocument;
            ListCrystalReport.EnableParameterPrompt = false;
            ListCrystalReport.HasToggleParameterPanelButton = false;
            ListCrystalReport.HasToggleGroupTreeButton = false;
            ListCrystalReport.DataBind();
        }
    }
    
}