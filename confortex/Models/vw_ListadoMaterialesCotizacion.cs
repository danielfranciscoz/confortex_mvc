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
    
    public partial class vw_ListadoMaterialesCotizacion
    {
        public int IdCotizacion { get; set; }
        public int IdDescripcionHechura { get; set; }
        public int IdAccesorio { get; set; }
        public int IdPrecioAccesorio { get; set; }
        public string Material { get; set; }
        public Nullable<double> Cantidad { get; set; }
        public double PrecioActual { get; set; }
    }
}
