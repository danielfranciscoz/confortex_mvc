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
    
    public partial class DescripcionHechuraColor
    {
        public int IdDescripcionHechuraColor { get; set; }
        public int IdDescripcionHechura { get; set; }
        public int IdColor { get; set; }
        public System.DateTime FechaCreacion { get; set; }
    
        public virtual Color Color { get; set; }
        public virtual DescripcionHechura DescripcionHechura { get; set; }
    }
}