using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Confortex.Referencias
{
    public class clsReferencias
    {

        //Mensajes
        public static string Id_NULL = "El id es NULL";
        public static string Not_Found = "El registro no fué encontrado";
        public static string Exito = "Exito";

        //Acciones
        public static byte INSERT = 0;
        public static byte UPDATE = 1;
        public static byte DELETE = 2;
        public static byte Change_State = 3;
        public static byte NoAction = 4;
        public static byte Pagar = 4;
        public static byte Generar = 3;
        //Estados
        public static byte EnBorrador = 7;
        public static byte Generado = 1;
        public static byte Anulado = 4;
        public static byte Aprobado = 2;
        public static byte Finalizado = 15;
        public static byte Pagado = 16;
        public static byte Ignore_Estate = 0;

        public static byte Acceder = 5;
    }
}