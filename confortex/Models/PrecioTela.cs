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
    
    public partial class PrecioTela
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PrecioTela()
        {
            this.PrecioTelaDescripcionHechura = new HashSet<PrecioTelaDescripcionHechura>();
        }
    
        public int IdPrecioTela { get; set; }
        public int IdTela { get; set; }
        public double PrecioUnitario { get; set; }
        public bool regAnulado { get; set; }
        public string cod_RA { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PrecioTelaDescripcionHechura> PrecioTelaDescripcionHechura { get; set; }
        public virtual Tela Tela { get; set; }
    }
}
