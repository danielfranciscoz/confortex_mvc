//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Confortex.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class vw_ObtenerEntregas
    {
        public int IdEntregaTicket { get; set; }
        public int IdCotizacion { get; set; }
        public int Ticket { get; set; }
        public string Actividad { get; set; }
        public string EntregadoPor { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public int Contabilizado { get; set; }
    }
}
