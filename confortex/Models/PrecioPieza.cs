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
    
    public partial class PrecioPieza
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PrecioPieza()
        {
            this.PrecioPiezaEntregaTicket = new HashSet<PrecioPiezaEntregaTicket>();
        }
    
        public int IdPrecioPieza { get; set; }
        public int IdPieza { get; set; }
        public string NombreFuncion { get; set; }
        public Nullable<double> Duracion { get; set; }
        public double PrecioMO { get; set; }
        public bool regAnulado { get; set; }
        public string cod_RA { get; set; }
    
        public virtual Funcion Funcion { get; set; }
        public virtual Pieza Pieza { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PrecioPiezaEntregaTicket> PrecioPiezaEntregaTicket { get; set; }
    }
}