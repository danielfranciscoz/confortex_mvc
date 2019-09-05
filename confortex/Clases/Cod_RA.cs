using Confortex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Confortex.Clases
{
    public class Cod_RA
    {
        private static ConfortexEntities db = new ConfortexEntities();

        public static string cod_RA() {
            return (db.Cod_RA_Table().FirstOrDefault().Cod_RA).ToString();
        }
        
    }
}