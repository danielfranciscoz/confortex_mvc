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
    
    public partial class CotizacionFuncion
    {
        public int IdCotizacion { get; set; }
        public string NombreFuncion { get; set; }
        public System.DateTime FechaCreacion { get; set; }
    
        public virtual Cotizacion Cotizacion { get; set; }
        public virtual Funcion Funcion { get; set; }
    }
}
