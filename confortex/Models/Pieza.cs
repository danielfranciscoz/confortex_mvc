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
    
    public partial class Pieza
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pieza()
        {
            this.DescripcionHechura = new HashSet<DescripcionHechura>();
            this.PiezaAccesorio = new HashSet<PiezaAccesorio>();
            this.PiezaTallaMedida_Standar = new HashSet<PiezaTallaMedida_Standar>();
            this.PiezaMedida = new HashSet<PiezaMedida>();
            this.PiezaTalla = new HashSet<PiezaTalla>();
            this.PrecioPieza = new HashSet<PrecioPieza>();
        }
    
        public int IdPieza { get; set; }
        public string Nombre { get; set; }
        public string SexoPieza { get; set; }
        public double CantidadTela { get; set; }
        public bool regAnulado { get; set; }
        public string cod_RA { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DescripcionHechura> DescripcionHechura { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PiezaAccesorio> PiezaAccesorio { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PiezaTallaMedida_Standar> PiezaTallaMedida_Standar { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PiezaMedida> PiezaMedida { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PiezaTalla> PiezaTalla { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PrecioPieza> PrecioPieza { get; set; }
    }
}