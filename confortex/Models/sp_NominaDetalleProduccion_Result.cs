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
    
    public partial class sp_NominaDetalleProduccion_Result
    {
        public Nullable<int> No_Nomina { get; set; }
        public int IdDescripcionHechura { get; set; }
        public int No_Ticket { get; set; }
        public string Codigo { get; set; }
        public string NombreCompleto { get; set; }
        public string Produce { get; set; }
        public string TipoTicket { get; set; }
        public Nullable<int> Cantidad { get; set; }
        public double MOPieza { get; set; }
        public double MOCombinaciones { get; set; }
        public Nullable<double> Produccion { get; set; }
    }
}
