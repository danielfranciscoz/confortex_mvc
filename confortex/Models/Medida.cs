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
    
    public partial class Medida
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Medida()
        {
            this.DetalleListadoProduccion = new HashSet<DetalleListadoProduccion>();
            this.PiezaTallaMedida_Standar = new HashSet<PiezaTallaMedida_Standar>();
            this.PiezaMedida = new HashSet<PiezaMedida>();
        }
    
        public int IdMedida { get; set; }
        public string Nombre { get; set; }
        public bool regAnulado { get; set; }
        public string cod_RA { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetalleListadoProduccion> DetalleListadoProduccion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PiezaTallaMedida_Standar> PiezaTallaMedida_Standar { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PiezaMedida> PiezaMedida { get; set; }
    }
}
