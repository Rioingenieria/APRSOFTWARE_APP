
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.WebSockets;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using SQLite;
using System.Net.NetworkInformation;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;
namespace APRSOFTWARE_APP
{
   public class Modulo
    {
        public static SQLiteConnection cnSqlite = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "APPRSoftware.db3"));
        public static SqlConnection cnSqlserver;
        public static int id_usuario;
        public static string nombre_operador;
        public static string apellido_operador;
        //VARIABLES LECTURAS Y RUTAS GLOBAL
        public static string NombreRuta;
        public static string MesNombreActual;
        public static string MesNombreAnterior;
        public static int AnioAnterior;
        public static int AnioActual;
        public static DateTime FechaActual;
        public static bool ProbarConexionSQLServer(string ip, string puerto, string usuariobd, string contrasenabd, string nombrebd)     
        {
            cnSqlserver = new SqlConnection("Data source='" + ip + "'; initial catalog='" + nombrebd + "'; User id='" + usuariobd + "'; password='" + contrasenabd + "';");          

                return true;         
        }
        public static bool CrearConexionSQLServer()
        {
            var tabla_servidor = cnSqlite.Query<Servidor>("select * from servidor");
            if (tabla_servidor.Count() > 0)
            {
                try
                {
                    cnSqlserver = new SqlConnection("Data source=" + tabla_servidor[0].ip.ToString()+"; initial catalog=" + tabla_servidor[0].nombrebd.ToString() + "; User id=" + tabla_servidor[0].usuariobd.ToString() + "; password=" + tabla_servidor[0].contrasenabd.ToString() + ";");
                    return true;
                }
                catch (Exception)
                {                   
                    return false;
                }               
            }
            else
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
        }
        public static double Redondear(double numero,int decimales)
        {
            return Convert.ToInt32(Math.Pow(numero * 10, decimales + 1 / 2) ) / Math.Pow(10, decimales);
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
     


    }
}
