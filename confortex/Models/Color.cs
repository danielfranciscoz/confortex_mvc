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
    
    public partial class Color
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Color()
        {
            this.DescripcionHechuraColor = new HashSet<DescripcionHechuraColor>();
            this.ListadoProduccion = new HashSet<ListadoProduccion>();
        }
    
        public int IdColor { get; set; }
        public string Nombre { get; set; }
        public string CodigoHexadecimal { get; set; }
        public bool regAnulado { get; set; }
        public string cod_RA { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DescripcionHechuraColor> DescripcionHechuraColor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ListadoProduccion> ListadoProduccion { get; set; }
    }
}
