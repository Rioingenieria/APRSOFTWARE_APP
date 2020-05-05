using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace APRSOFTWARE_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Sincronizar : ContentPage
    {
        public Sincronizar()
        {
            InitializeComponent();
            lbl_mensaje.Text = "";
        }

        private async void btn_sincronizar_Clicked(object sender, EventArgs e)
        {
            bool x =await DisplayAlert("INFORMACION", "Se borraran todas las Lecturas, Clientes y Rutas registradas en la APP y se volveran a sincronizar con APRSoftware. ¿Desea continuar?","No", "Si");
           if (x is true)
            {
                return;
            }
            if (Modulo.CrearConexionSQLServer() is false)
            {
                await DisplayAlert("Informacion", "Error al conectar con el servidor.", "ACEPTAR");
                return;
            }
            lbl_mensaje.Text = "Sincronizando con servidor..";
           await Task.Run(() =>
           {
                    //VACIAMOS TABLA DE RUTAS Y CLIENTES PARA VOLVWER A INSERTAR 
                    Modulo.cnSqlite.DeleteAll<Rutas>();
                     Modulo.cnSqlite.DeleteAll<Clientes>();
                     Modulo.cnSqlite.DeleteAll<Lecturas>();
                    //CREAMOS CONEXION CON SQLSERVER
              
                    //ABRIMOS CONEXION
                    Modulo.cnSqlserver.Open();
                    //SINCRONIZAMOS TABLA RUTAS
                    SqlDataAdapter darutas = new SqlDataAdapter("select id_ruta, nombre, descripcion from rutas", Modulo.cnSqlserver);
                     DataTable dtrutas = new DataTable();
                     darutas.Fill(dtrutas);
                     for (int i = 0; i < dtrutas.Rows.Count; i++)
                     {
                         Modulo.cnSqlite.Query<Rutas>("insert into rutas(id_ruta,nombre,descripcion) values (?,?,?)", dtrutas.Rows[i].ItemArray[0], dtrutas.Rows[i].ItemArray[1], dtrutas.Rows[i].ItemArray[2]);
                     }
                    //SINCRONIZAMOS TABLA CLIENTES
                    SqlDataAdapter daclientes = new SqlDataAdapter("select id_cliente,isnull(nombre,''),isnull(apellido,''),isnull(rut,''),isnull(domicilio,''),isnull(medidor,''),isnull(id_ruta,0),isnull(nombre_ruta,''),isnull(num_ruta,0),estado from clientes", Modulo.cnSqlserver);
                     DataTable dtclientes = new DataTable();
                     daclientes.Fill(dtclientes);
                     for (int i = 0; i < dtclientes.Rows.Count; i++)
                     {
                         Modulo.cnSqlite.Query<Clientes>("insert into clientes(id_cliente,nombre,apellido,rut,direccion,num_medidor,id_ruta,nombre_ruta,num_ruta,estado) values (?,?,?,?,?,?,?,?,?,?)", dtclientes.Rows[i].ItemArray[0], dtclientes.Rows[i].ItemArray[1], dtclientes.Rows[i].ItemArray[2], dtclientes.Rows[i].ItemArray[3], dtclientes.Rows[i].ItemArray[4], dtclientes.Rows[i].ItemArray[5], dtclientes.Rows[i].ItemArray[6], dtclientes.Rows[i].ItemArray[7], dtclientes.Rows[i].ItemArray[8], dtclientes.Rows[i].ItemArray[9]);
                     }
                    //SINCRONIZAMOS TABLA LECTURAS
                    SqlDataAdapter dalecturas = new SqlDataAdapter("select id_cliente,nombre,fecha,fecha_toma,mes,año,lectura_anterior,lectura_actual,consumo from lecturas where estado<>'saldo anterior'", Modulo.cnSqlserver);
                     DataTable dtlecturas = new DataTable();
                     dalecturas.Fill(dtlecturas);
                     for (int i = 0; i < dtlecturas.Rows.Count; i++)
                     {
                         Modulo.cnSqlite.Query<Lecturas>("insert into lecturas(id_cliente,nombre,fecha,fecha_toma,mes,anio,lectura_anterior,lectura_actual,consumo) values (?,?,?,?,?,?,?,?,?)", dtlecturas.Rows[i].ItemArray[0], dtlecturas.Rows[i].ItemArray[1], dtlecturas.Rows[i].ItemArray[2], dtlecturas.Rows[i].ItemArray[3], dtlecturas.Rows[i].ItemArray[4], dtlecturas.Rows[i].ItemArray[5], dtlecturas.Rows[i].ItemArray[6], dtlecturas.Rows[i].ItemArray[7], dtlecturas.Rows[i].ItemArray[8]);
                     }
                    //CERRAMOS CONEXION
                    Modulo.cnSqlserver.Close();
           }
           );
            lbl_mensaje.Text = "";
            await DisplayAlert("Informacion", "Datos sincronizados correctamente.", "ACEPTAR");
        }
    }
}