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
    
    public partial class NominaDiaLibre
    {
        public int IdNominaDiaLibre { get; set; }
        public int IdNomina { get; set; }
        public System.DateTime FechaLibre { get; set; }
        public string Concepto { get; set; }
        public System.DateTime FechaCreacion { get; set; }
    
        public virtual Nomina Nomina { get; set; }
    }
}
