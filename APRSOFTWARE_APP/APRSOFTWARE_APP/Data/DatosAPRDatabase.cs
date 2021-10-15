using System;
using System.Collections.Generic;
using System.Text;

namespace APRSOFTWARE_APP.Data
{
   public class DatosAPRDatabase
    {

        public static int GetIdAPR()
        {
            int id;
            var consulta = ConexionLocal.cnSqlite.Query<DatosAPR>("select id_apr_global from datos_apr");
            if (consulta.Count == 0)
            {
                id = 0;
            }
            else
            {
                id = consulta[0].id_apr_global;
            }

            return id;
        }
    }
}
