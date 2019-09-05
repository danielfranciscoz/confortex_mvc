<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ReportSite.Master" AutoEventWireup="true" CodeBehind="CRV.aspx.cs" Inherits="Confortex.Reportes.ASP.CRV" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.3500.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<asp:Content ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <CR:CrystalReportViewer ID="ListCrystalReport" runat="server" HasCrystalLogo="False"
        AutoDataBind="True" Height="50px" EnableParameterPrompt="false" EnableDatabaseLogonPrompt="false" ToolPanelWidth="200px"
        Width="350px" ToolPanelView="None"/>
    <input type="hidden" id="HidIDR" value="@Model.IdReport" runat="server" />
</asp:Content>
