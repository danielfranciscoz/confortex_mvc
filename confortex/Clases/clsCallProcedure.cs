using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Confortex.Clases
{
    public class clsCallProcedure : Controller
    {
        public JsonResult Call(String ProcedureName, Dictionary<String, String> parametros)
        {
            SqlConnection conexion = null;
            try
            {
                String TableName = "resultados";
                conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                SqlCommand comando = new SqlCommand(ProcedureName, conexion);

                SqlParameter parametro;
                foreach (KeyValuePair<String, String> valor in parametros)
                {
                    parametro = new SqlParameter();
                    parametro.ParameterName = valor.Key;
                    parametro.Value = valor.Value;

                    comando.Parameters.Add(parametro);
                }
                comando.CommandType = CommandType.StoredProcedure;

                conexion.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(comando);
                DataSet tablaResult = new DataSet();
                adapter.Fill(tablaResult, TableName);
                //String[,] Data = new String[tablaResult.Tables[TableName].Rows.Count, tablaResult.Tables[TableName].Columns.Count];
                String[] Columnas = new String[tablaResult.Tables[TableName].Columns.Count];
                Dictionary<String, String> keyValue = new Dictionary<String, String>();
                List<Dictionary<String, String>> listaColumns = new List<Dictionary<String, String>>();
                int ColIndex = 0;
                foreach (DataColumn columna in tablaResult.Tables[TableName].Columns)
                {
                    Columnas[ColIndex] = columna.Caption.Replace(".", "_");
                    ColIndex++;

                    keyValue.Add("mDataProp", columna.Caption.Replace(".", "_"));
                    keyValue.Add("sTitle", columna.Caption);

                    listaColumns.Add(keyValue);
                    keyValue = new Dictionary<String, String>();

                }

                keyValue = new Dictionary<String, String>();
                // Las filas se retornan con indices de columnas
                int RowIndex = 0;
                List<Dictionary<String, String>> lista = new List<Dictionary<String, String>>();

                foreach (DataRow fila in tablaResult.Tables[TableName].Rows)
                {
                    ColIndex = 0;
                    for (int i = 0; i < fila.ItemArray.Length; i++)
                    {
                        if (fila.IsNull(ColIndex))
                        {
                            keyValue.Add(Columnas[ColIndex], "0");
                            //Data[RowIndex, ColIndex] = "0";
                        }
                        else
                        {
                            keyValue.Add(Columnas[ColIndex], fila[ColIndex].ToString());
                            //Data[RowIndex, ColIndex] = fila[ColIndex].ToString();
                        }

                        ColIndex++;
                    }
                    lista.Add(keyValue);
                    keyValue = new Dictionary<String, String>();
                    RowIndex++;
                }

                conexion.Close();

                return Json(new { columns = listaColumns, data = lista });
            }
            catch (Exception ex)
            {
                if (conexion.State == ConnectionState.Open)
                {
                    conexion.Close();
                }             
                return Json(new { Message = new clsException(ex).Message() });
            }
        }

        public DataTable CallDT(String ProcedureName, Dictionary<String, String> parametros)
        {
            SqlConnection conexion = null;
            try
            {
                String TableName = "resultados";
                conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                SqlCommand comando = new SqlCommand(ProcedureName, conexion);

                SqlParameter parametro;
                foreach (KeyValuePair<String, String> valor in parametros)
                {
                    parametro = new SqlParameter();
                    parametro.ParameterName = valor.Key;
                    parametro.Value = valor.Value;

                    comando.Parameters.Add(parametro);
                }
                comando.CommandType = CommandType.StoredProcedure;

                conexion.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(comando);
                DataSet tablaResult = new DataSet();
                adapter.Fill(tablaResult, TableName);
                
                conexion.Close();
                DataTable data = new DataTable();
                data = tablaResult.Tables[TableName];

                return data;
            }
            catch (Exception ex)
            {
                if (conexion.State == ConnectionState.Open)
                {
                    conexion.Close();
                }
                return null;
            }
        }
    }
}