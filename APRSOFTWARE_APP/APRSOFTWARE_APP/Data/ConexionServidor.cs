using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace APRSOFTWARE_APP.Data
{
    public class ConexionServidor
    {
        private SqlConnection cn = new SqlConnection("Data Source=198.38.91.9;Initial Catalog=adminAPR_aprcomunidad_pruebas;User Id=adminAPR_rio_pruebas;Password=Rio20199!*;");

        //private SqlConnection cn = new SqlConnection("Data Source=SQL5034.site4now.net;Initial Catalog=DB_A5734F_reducida;User Id=DB_A5734F_reducida_admin;Password=Reducida2019!;");
        public SqlConnection AbrirConexion()
        {

            if (cn.State == System.Data.ConnectionState.Closed)
            {
                cn.Open();
            }
            return cn;
        }
        public SqlConnection CerrarConexion()
        {
            if (cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
            return cn;
        }
    }
}
