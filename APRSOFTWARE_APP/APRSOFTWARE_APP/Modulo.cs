
using SQLite;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.NetworkInformation;
namespace APRSOFTWARE_APP
{
    public class Modulo
    {
        public static SQLiteConnection cnSqlite = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "APPRSoftware.db3"));
        // public static SqlConnection cnSqlserver = new SqlConnection("Data Source=SQL5034.site4now.net;Initial Catalog=DB_A5734F_reducida;User Id=DB_A5734F_reducida_admin;Password=Reducida2019!;");
        public static SqlConnection cnSqlserver = new SqlConnection("Data Source=198.38.91.9;Initial Catalog=adminAPR_aprcomunidad_pruebas;User Id=adminAPR_rio_pruebas;Password=Rio20199!*;");

        public static int id_usuario;
        public static string nombre_operador = "USUARIO APR";
        public static string apellido_operador;
        //VARIABLES LECTURAS Y RUTAS GLOBAL
        public static string NombreRuta;
        public static string MesNombreActual;
        public static string MesNombreAnterior;
        public static int AnioAnterior;
        public static int AnioActual;
        public static DateTime FechaActual;
        public static bool CrearConexionSQLServer()
        {
            cnSqlserver = new SqlConnection("Data Source=198.38.91.9;Initial Catalog=adminAPR_aprcomunidad_pruebas;User Id=adminAPR_rio_pruebas;Password=Rio20199!*;");
            //cnSqlserver = new SqlConnection("Data Source=SQL5034.site4now.net;Initial Catalog=DB_A5734F_reducida;User Id=DB_A5734F_reducida_admin;Password=Reducida2019!;");
            return true;
        }
        public static void VaciarTablas()
        {
            cnSqlite.Query<Clientes>("delete from Clientes");
        }
        public static int GetIdAPR()
        {
            int id;
            var consulta = Modulo.cnSqlite.Query<DatosAPR>("select id_apr_global from datos_apr");
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
        public static DataTable GetDatosAPR(string rut)
        {
            cnSqlserver.Open();
            SqlDataAdapter dadatosAPR = new SqlDataAdapter("select*from datos_aprs where rut_empresa='" + rut + "'", cnSqlserver);
            DataTable dtDAtosAPR = new DataTable();
            dadatosAPR.Fill(dtDAtosAPR);
            cnSqlserver.Close();
            return dtDAtosAPR;
        }
        public static DateTime GetUltimaFechaSincronizacion(int id_apr_global)
        {
            cnSqlserver.Open();
            SqlDataAdapter dadatosAPR = new SqlDataAdapter("select ultima_fecha_sincronizacion from datos_aprs where id_apr_global='" + id_apr_global + "'", cnSqlserver);
            DataTable dtDAtosAPR = new DataTable();
            cnSqlserver.Close();
            dadatosAPR.Fill(dtDAtosAPR);
            return Convert.ToDateTime(dtDAtosAPR.Rows[0]["ultima_fecha_sincronizacion"]);
        }
        public static bool GetConexionInternet(string huesped = "http://www.google.com")
        {
            try
            {
                return new Ping().Send(huesped).Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
        public static void VerificarTablasBDSqlite()
        {
            cnSqlite.CreateTable<Servidor>();
            cnSqlite.CreateTable<Rutas>();
            cnSqlite.CreateTable<Lecturas>();
            cnSqlite.CreateTable<Clientes>();
            cnSqlite.CreateTable<Usuarios>();
            cnSqlite.CreateTable<DatosAPR>();
        }
        public static double Redondear(double numero, int decimales)
        {
            return Convert.ToInt32(Math.Pow(numero * 10, decimales + 1 / 2)) / Math.Pow(10, decimales);
        }
        public static void ImportarClientes()
        {
            cnSqlserver.Open();
            //ELIMINAMOS DATOS DE CLIENTES
            cnSqlite.DeleteAll<Clientes>();
            //SINCRONIZAMOS TABLA CLIENTES DESDE SQL SERVER
            SqlDataAdapter daclientes = new SqlDataAdapter("select id_cliente,isnull(nombre,''),isnull(apellido,''),isnull(rut,''),isnull(domicilio,''),isnull(medidor,''),isnull(id_ruta,0),isnull(nombre_ruta,''),isnull(num_ruta,0),estado from clientes", Modulo.cnSqlserver);
            DataTable dtclientes = new DataTable();
            daclientes.Fill(dtclientes);
            for (int i = 0; i < dtclientes.Rows.Count; i++)
            {
                cnSqlite.Query<Clientes>("insert into clientes(id_cliente,nombre,apellido,rut,direccion,num_medidor,id_ruta,nombre_ruta,num_ruta,estado) values (?,?,?,?,?,?,?,?,?,?)", dtclientes.Rows[i].ItemArray[0], dtclientes.Rows[i].ItemArray[1], dtclientes.Rows[i].ItemArray[2], dtclientes.Rows[i].ItemArray[3], dtclientes.Rows[i].ItemArray[4], dtclientes.Rows[i].ItemArray[5], dtclientes.Rows[i].ItemArray[6], dtclientes.Rows[i].ItemArray[7], dtclientes.Rows[i].ItemArray[8], dtclientes.Rows[i].ItemArray[9]);
            }
            cnSqlserver.Close();
        }
        public static void ImportarRutas()
        {
            cnSqlserver.Open();
            //ELIMINAMOS TABLA RUTAS
            cnSqlite.DeleteAll<Rutas>();
            //SINCRONIZAMOS TABLA RUTAS
            SqlDataAdapter darutas = new SqlDataAdapter("select id_ruta, nombre, descripcion from rutas", Modulo.cnSqlserver);
            DataTable dtrutas = new DataTable();
            darutas.Fill(dtrutas);
            for (int i = 0; i < dtrutas.Rows.Count; i++)
            {
                Modulo.cnSqlite.Query<Rutas>("insert into rutas(id_ruta,nombre,descripcion) values (?,?,?)", dtrutas.Rows[i].ItemArray[0], dtrutas.Rows[i].ItemArray[1], dtrutas.Rows[i].ItemArray[2]);
            }
            cnSqlserver.Close();
        }

        public static void SincronizarRutasAPP(int id_apr)
        {
            cnSqlite.DeleteAll<Rutas>();
            cnSqlserver.Open();
            //SINCRONIZAMOS TABLA RUTAS
            SqlDataAdapter darutas = new SqlDataAdapter("select id_ruta,nombre,isnull(descripcion,'') from rutas_app where id_apr_global='" + id_apr + "'", cnSqlserver);
            DataTable dtrutas = new DataTable();
            darutas.Fill(dtrutas);
            cnSqlserver.Close();
            for (int i = 0; i < dtrutas.Rows.Count; i++)
            {
                Modulo.cnSqlite.Query<Rutas>("insert into rutas(id_ruta,nombre,descripcion) values (?,?,?)", dtrutas.Rows[i].ItemArray[0], dtrutas.Rows[i].ItemArray[1], dtrutas.Rows[i].ItemArray[2]);
            }
        }
        public static void SincronizarClientes(int id_apr)
        {
            cnSqlserver.Open();
            //SINCRONIZAMOS TABLA CLIENTES
            SqlDataAdapter daclientes = new SqlDataAdapter("select id_cliente,isnull(nombre,''),isnull(apellido,''),isnull(rut,''),isnull(domicilio,''),isnull(medidor,''),isnull(id_ruta,0),isnull(nombre_ruta,''),isnull(num_ruta,0),estado from clientes where id_apr_global='" + id_apr + "'", cnSqlserver);
            DataTable dtclientes = new DataTable();
            daclientes.Fill(dtclientes);
            cnSqlserver.Close();
            for (int i = 0; i < dtclientes.Rows.Count; i++)
            {
                var cliente_local = cnSqlite.Query<Clientes>("select id_cliente from clientes where id_cliente='" + dtclientes.Rows[i][0] + "'");
                if (cliente_local.Count > 0)
                {
                    cnSqlite.Query<Clientes>("update clientes " +
                        "set nombre=?," +
                        "apellido=?," +
                        "rut=?," +
                        "direccion=?," +
                        "num_medidor=?," +
                        "id_ruta=?," +
                        "nombre_ruta=?," +
                        "num_ruta=?," +
                        "estado=? " +
                        "where id_cliente=?",
                        dtclientes.Rows[i][1],
                        dtclientes.Rows[i][2], 
                        dtclientes.Rows[i][3], 
                        dtclientes.Rows[i][4], 
                        dtclientes.Rows[i][5],
                        dtclientes.Rows[i][6], 
                        dtclientes.Rows[i][7], 
                        dtclientes.Rows[i][8],
                        dtclientes.Rows[i][9], 
                        cliente_local[0].id_cliente);
        }
                else
                {
                    cnSqlite.Query<Clientes>("insert into clientes(id_cliente,nombre,apellido,rut,direccion,num_medidor,id_ruta,nombre_ruta,num_ruta,estado) values (?,?,?,?,?,?,?,?,?,?)", dtclientes.Rows[i][0], dtclientes.Rows[i][1], dtclientes.Rows[i][2], dtclientes.Rows[i][3], dtclientes.Rows[i][4], dtclientes.Rows[i][5], dtclientes.Rows[i][6], dtclientes.Rows[i][7], dtclientes.Rows[i][8], dtclientes.Rows[i][9]);
                }
            }
        }
        public static void SincronizarLecturas(string mes, int anio, int id_apr_global)
        {
            cnSqlserver.Open();
            //SINCRONIZAMOS TABLA LECTURAS
            SqlDataAdapter dalecturas = new SqlDataAdapter("select id_cliente,nombre,fecha,fecha_toma,mes,año,lectura_anterior,lectura_actual,consumo,observacion from lecturas_app where id_apr_global='" + id_apr_global + "' and mes='" + mes + "' and año='" + anio + "'", cnSqlserver);
            DataTable dtlecturas = new DataTable();
            dalecturas.Fill(dtlecturas);
            cnSqlserver.Close();
            for (int i = 0; i < dtlecturas.Rows.Count; i++)
            {
                var lectura_local = cnSqlite.Query<Lecturas>("select id_lectura_app from lecturas where mes='" + mes + "' and anio='" + anio + "' and id_cliente='" + dtlecturas.Rows[i][0] + "'");
                if (lectura_local.Count > 0)
                {
                    cnSqlite.Query<Lecturas>("update lecturas set nombre='" + dtlecturas.Rows[i].ItemArray[1] + "',fecha='" + dtlecturas.Rows[i].ItemArray[2] + "',fecha_toma='" + dtlecturas.Rows[i].ItemArray[3] + "',mes='" + dtlecturas.Rows[i].ItemArray[4] + "',anio='" + dtlecturas.Rows[i].ItemArray[5] + "',lectura_anterior='" + dtlecturas.Rows[i].ItemArray[6] + "',lectura_actual='" + dtlecturas.Rows[i].ItemArray[7] + "',consumo='" + dtlecturas.Rows[i].ItemArray[8] + "' where id_lectura_app='" + lectura_local[0].id_lectura_app + "'");
                }
                else
                {
                    cnSqlite.Query<Lecturas>("insert into lecturas(id_cliente,nombre,fecha,fecha_toma,mes,anio,lectura_anterior,lectura_actual,consumo) values (?,?,?,?,?,?,?,?,?)", dtlecturas.Rows[i].ItemArray[0], dtlecturas.Rows[i].ItemArray[1], dtlecturas.Rows[i].ItemArray[2], dtlecturas.Rows[i].ItemArray[3], dtlecturas.Rows[i].ItemArray[4], dtlecturas.Rows[i].ItemArray[5], dtlecturas.Rows[i].ItemArray[6], dtlecturas.Rows[i].ItemArray[7], dtlecturas.Rows[i].ItemArray[8]);
                }
            }
        }

    }
}
