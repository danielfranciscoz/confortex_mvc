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
    
    public partial class Combinacion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Combinacion()
        {
            this.CombinacionDescripcionHechura = new HashSet<CombinacionDescripcionHechura>();
            this.PrecioCombinacion = new HashSet<PrecioCombinacion>();
        }
    
        public int IdCombinacion { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double CantidadTela { get; set; }
        public bool regAnulado { get; set; }
        public string cod_RA { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CombinacionDescripcionHechura> CombinacionDescripcionHechura { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PrecioCombinacion> PrecioCombinacion { get; set; }
    }
}
