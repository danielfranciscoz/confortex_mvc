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
    
    public partial class Permiso
    {
        public string IdRol { get; set; }
        public int IdPantalla { get; set; }
        public int IdPermisoName { get; set; }
    
        public virtual Pantalla Pantalla { get; set; }
        public virtual PermisoName PermisoName { get; set; }
        public virtual Rol Rol { get; set; }
    }
}
