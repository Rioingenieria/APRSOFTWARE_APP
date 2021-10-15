using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace APRSOFTWARE_APP.Data
{
    public class LecturasDatabase
    {
        SQLiteConnection CnLocal;
        public LecturasDatabase()
        {
            CnLocal =ConexionLocal.cnSqlite;
        }
        public List<Lecturas> GetLecturasLocales(string mes,int año)
        {
            var tabla_lecturas = Modulo.cnSqlite.Query<Lecturas>("select ifnull(id_lectura_app,'0') as id_lectura_app,ifnull(id_cliente,'0') as id_cliente,ifnull(nombre,'') as nombre,ifnull(fecha,'20220101') as fecha,ifnull(fecha_toma,'20220101') as fecha_toma,ifnull(mes,'no') as mes,ifnull(lectura_anterior,'0') as lectura_anterior,ifnull(lectura_actual,'0') as lectura_actual,ifnull(consumo,'0') as consumo,ifnull(promedio,'0') as promedio,ifnull(observacion,'no') as observacion from lecturas where mes=? and anio=? order by id_cliente asc",mes,año);
            return tabla_lecturas;
        }
    }
}
