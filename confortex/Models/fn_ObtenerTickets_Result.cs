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
    
    public partial class fn_ObtenerTickets_Result
    {
        public Nullable<int> IdDescripcionHechura { get; set; }
        public Nullable<int> IdListadoProduccion { get; set; }
        public Nullable<int> IdTalla { get; set; }
        public string Talla { get; set; }
        public string Nombre { get; set; }
        public Nullable<int> Cantidad { get; set; }
        public string Estado { get; set; }
        public Nullable<bool> isReparacion { get; set; }
        public string Tipo { get; set; }
        public string Observaciones { get; set; }
    }
}
