using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace APRSOFTWARE_APP.Data
{
    public class LecturasServidorDatabase
    {
        SqlConnection _cn=new SqlConnection();
        ConexionServidor CnServidor = new ConexionServidor();
        int IdAPRGlobal=0;
        public LecturasServidorDatabase()
        {
            IdAPRGlobal = DatosAPRDatabase.GetIdAPR();
        }
        public async Task<List<Lecturas>> GetLecturas(string mes,int anio)
        {
            return await Task.Run(() => {
                List<Lecturas> ListadoLecturas=new List<Lecturas>();
                _cn = CnServidor.AbrirConexion();
                SqlCommand CmdConsuoltaLecturas = new SqlCommand("select * from lecturas_app where id_apr_global=@id_apr_global and mes=@mes and año=@año order by id_cliente asc", _cn);
                CmdConsuoltaLecturas.Parameters.AddWithValue("@id_apr_global", IdAPRGlobal);
                CmdConsuoltaLecturas.Parameters.AddWithValue("@mes",mes);
                 CmdConsuoltaLecturas.Parameters.AddWithValue("@año",anio);
              
                using (var reader = CmdConsuoltaLecturas.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ListadoLecturas.Add(new Lecturas
                        {
                            id_cliente = Convert.ToInt32(reader["id_cliente"]),
                            nombre = Convert.ToString(reader["nombre"]),
                            fecha = Convert.ToDateTime(reader["fecha"]),
                            fecha_toma =Convert.ToDateTime(reader["fecha_toma"]),
                            mes = Convert.ToString(reader["mes"]),
                            anio= Convert.ToInt32(reader["año"]),
                            lectura_anterior=Convert.ToDecimal(reader["lectura_anterior"]),
                            lectura_actual = Convert.ToDecimal(reader["lectura_actual"]),
                            consumo=Convert.ToDecimal(reader["consumo"]),
                            observacion= Convert.ToString(reader["observacion"])
                        }); ; ;
                    }
                }
                _cn.Close  ();
                return ListadoLecturas;
            });
        }
        public async Task<Lecturas> GetLecturaByIdCliente(string mes, int anio,int idCliente)
        {
            return await Task.Run(() => {
               Lecturas Lectura = new Lecturas();
                _cn = CnServidor.AbrirConexion();
                SqlCommand CmdConsuoltaLecturas = new SqlCommand("select * from lecturas_app where id_apr_global=@id_apr_global and mes=@mes and año=@año and id_cliente=@id_cliente order by id_cliente asc", _cn);
                CmdConsuoltaLecturas.Parameters.AddWithValue("@id_apr_global", IdAPRGlobal);
                CmdConsuoltaLecturas.Parameters.AddWithValue("@mes", mes);
                CmdConsuoltaLecturas.Parameters.AddWithValue("@año", anio);
                CmdConsuoltaLecturas.Parameters.AddWithValue("@id_cliente", idCliente);
                using (var reader = CmdConsuoltaLecturas.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Lectura=new Lecturas
                        {
                            id_cliente = Convert.ToInt32(reader["id_cliente"]),
                            nombre = Convert.ToString(reader["nombre"]),
                            fecha = Convert.ToDateTime(reader["fecha"]),
                            fecha_toma = Convert.ToDateTime(reader["fecha_toma"]),
                            mes = Convert.ToString(reader["mes"]),
                            anio = Convert.ToInt32(reader["año"]),
                            lectura_anterior = Convert.ToDecimal(reader["lectura_anterior"]),
                            lectura_actual = Convert.ToDecimal(reader["lectura_actual"]),
                            consumo = Convert.ToDecimal(reader["consumo"]),
                            observacion = Convert.ToString(reader["observacion"])
                        }; ; ;
                    }
                }
                _cn.Close();
                return Lectura;
            });
        }

        public async Task<bool> InsertarLectura(Lecturas _lecturas)
        {
            return await Task.Run(() => { 
            bool IsExito;
            _cn=CnServidor.AbrirConexion();
            SqlCommand insertar_lecturas = new SqlCommand("insert into lecturas_app(id_apr_global,id_lectura,id_cliente,nombre,fecha,fecha_toma,mes,año,lectura_anterior,lectura_actual,consumo,observacion)values(@id_apr_global,@id_lectura,@id_cliente,@nombre,@fecha,@fecha_toma,@mes,@año,@lectura_anterior,@lectura_actual,@consumo,@observacion)",_cn);
            insertar_lecturas.Parameters.AddWithValue("@id_apr_global", IdAPRGlobal);
            insertar_lecturas.Parameters.AddWithValue("@id_lectura", _lecturas.id_lectura_app);
            insertar_lecturas.Parameters.AddWithValue("@id_cliente", _lecturas.id_cliente);
            insertar_lecturas.Parameters.AddWithValue("@nombre", _lecturas.nombre);
            insertar_lecturas.Parameters.AddWithValue("@fecha", _lecturas.fecha);
            insertar_lecturas.Parameters.AddWithValue("@fecha_toma", _lecturas.fecha_toma.ToString("yyyyMMdd"));
            insertar_lecturas.Parameters.AddWithValue("@mes", _lecturas.mes);
            insertar_lecturas.Parameters.AddWithValue("@año", _lecturas.anio);
            insertar_lecturas.Parameters.AddWithValue("@lectura_anterior", _lecturas.lectura_anterior);
            insertar_lecturas.Parameters.AddWithValue("@lectura_actual", _lecturas.lectura_actual);
            insertar_lecturas.Parameters.AddWithValue("@consumo", _lecturas.consumo);
            insertar_lecturas.Parameters.AddWithValue("@observacion", _lecturas.observacion);
           if (insertar_lecturas.ExecuteNonQuery()>0)
            {
                IsExito = true;
            }
            else
            {
                IsExito = false;
            }
            _cn.Close();
            return  IsExito;
            });
        }
      public async Task<bool> ActualizarLectura(Lecturas _lecturas)
        {
            return await Task.Run(() => { 
                bool IsExito;
                _cn = CnServidor.AbrirConexion();
                SqlCommand insertar_lecturas = new SqlCommand("update lecturas_app set nombre=@nombre,fecha=@fecha,fecha_toma=@fecha_toma,mes=@mes,año=@año,lectura_anterior=@lectura_anterior,lectura_actual=@lectura_actual,consumo=@consumo,observacion=@observacion where id_cliente='" + _lecturas.id_cliente + "' and id_apr_global='" + IdAPRGlobal + "' and mes='" + _lecturas.mes + "' and año='" + _lecturas.anio + "'", _cn);
                insertar_lecturas.Parameters.AddWithValue("@nombre", _lecturas.nombre);
                insertar_lecturas.Parameters.AddWithValue("@fecha", _lecturas.fecha);
                insertar_lecturas.Parameters.AddWithValue("@fecha_toma", _lecturas.fecha_toma.ToString("yyyyMMdd"));
                insertar_lecturas.Parameters.AddWithValue("@mes", _lecturas.mes);
                insertar_lecturas.Parameters.AddWithValue("@año", _lecturas.anio);
                insertar_lecturas.Parameters.AddWithValue("@lectura_anterior", _lecturas.lectura_anterior);
                insertar_lecturas.Parameters.AddWithValue("@lectura_actual", _lecturas.lectura_actual);
                insertar_lecturas.Parameters.AddWithValue("@consumo", _lecturas.consumo);
                insertar_lecturas.Parameters.AddWithValue("@observacion", _lecturas.observacion);
                insertar_lecturas.ExecuteNonQuery();
                if (insertar_lecturas.ExecuteNonQuery() > 0)
                {
                    IsExito = true;
                }
                else
                {
                    IsExito = false;
                }
               _cn.Close();
               return IsExito;
            });

        }
    }
}
