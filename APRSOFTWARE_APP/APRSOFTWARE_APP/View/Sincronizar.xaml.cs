using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Xamarin.Forms;
using System.Globalization;
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
               //CREAMOS CONEXION CON SQLSERVER              
               int id_apr = Modulo.GetIdAPR();
               Modulo.cnSqlite.Query<Clientes>("delete from lecturas");
               Modulo.SincronizarRutasAPP(id_apr);
              Modulo.SincronizarClientes(id_apr);          
              DateTime fecha=Modulo.GetUltimaFechaSincronizacion(id_apr);
               string mess= fecha.ToString("MMMM", CultureInfo.CreateSpecificCulture("es-ES"));
            Modulo.SincronizarLecturas(mess,fecha.Year,id_apr);
           }
           );
            lbl_mensaje.Text = "";
            await DisplayAlert("Informacion", "Datos sincronizados correctamente.", "ACEPTAR");
        }
    }
}