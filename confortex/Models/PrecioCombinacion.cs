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
    
    public partial class PrecioCombinacion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PrecioCombinacion()
        {
            this.PrecioCombinacionEntregaTicket = new HashSet<PrecioCombinacionEntregaTicket>();
        }
    
        public int IdPrecioCombinacion { get; set; }
        public int IdCombinacion { get; set; }
        public string NombreFuncion { get; set; }
        public double Duracion { get; set; }
        public double PrecioUnitario { get; set; }
        public bool regAnulado { get; set; }
        public string cod_RA { get; set; }
    
        public virtual Combinacion Combinacion { get; set; }
        public virtual Funcion Funcion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PrecioCombinacionEntregaTicket> PrecioCombinacionEntregaTicket { get; set; }
    }
}
