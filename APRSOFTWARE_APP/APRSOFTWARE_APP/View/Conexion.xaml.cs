using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using SQLite;
using Xamarin.Forms.Xaml;

namespace APRSOFTWARE_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Conexion : ContentPage
    {
        private SQLiteAsyncConnection _cnsqlite;
        public Conexion()
        {
            InitializeComponent();
            txt_nombre_bd.Text = "cayumapu";
            txt_contrasena.Text = "apr123";
            txt_puerto.Text = "1433";
            txt_usuario.Text = "apprsoft";
            _cnsqlite = DependencyService.Get<ISqlite>().GetConnection();
            var consulta = Modulo.cnSqlite.Query<Servidor>("select*from servidor");
            if (consulta.Count() > 0)
            {
                foreach (var item in consulta)
                {
                    txt_ip.Text = item.ip;
                    txt_nombre_bd.Text = item.nombrebd;
                    txt_puerto.Text = item.puerto;
                    txt_usuario.Text = item.usuariobd;
                    txt_contrasena.Text = item.contrasenabd;
                }
            }
        }
        private async void btn_guardar_clicked(Object sender, EventArgs e)
        {
            if (validarformulario())
            {
               await DisplayAlert("Informacion", "Todos los datos son obligatorios.", "ACEPTAR");
            }
            else
            {
                try
                {               
                Modulo.cnSqlite.DeleteAll<Servidor>();
                Modulo.cnSqlite.Query<Servidor>("insert into servidor(ip,puerto,usuariobd,contrasenabd,nombrebd)values(?,?,?,?,?)", txt_ip.Text, txt_puerto.Text, txt_usuario.Text, txt_contrasena.Text, txt_nombre_bd.Text);
                if (Modulo.CrearConexionSQLServer() is false)
                    {
                        await DisplayAlert("Informacion", "Error al conectar con el servidor", "ACEPTAR");
                        return;
                    }
                Modulo.cnSqlserver.Open();
                Modulo.cnSqlserver.Close();
               
                  await  DisplayAlert("Informacion", "Conexion exitosa con el servidor.", "ACEPTAR");
                }
                catch
                {
                    await DisplayAlert("Informacion", "Conexion con el servidor erronea. Verificar datos y conexion WIFI.", "ACEPTAR");
                }
                   
                
            }
        }
        private void btn_volver_clicked(Object sender, EventArgs e)
        {
            App.Current.MainPage.Navigation.PushModalAsync(new Login());
           
        }
        private bool validarformulario()
        {
            int contador = 0;
            if (string.IsNullOrEmpty(txt_ip.Text))
            {
                 contador++;
            }
            if (string.IsNullOrEmpty(txt_puerto.Text))
            {
                contador++;
            }
            if (string.IsNullOrEmpty(txt_usuario.Text))
            {
                contador++;
            }
            if (string.IsNullOrEmpty(txt_contrasena.Text))
            {
                contador++;
            }
            if (string.IsNullOrEmpty(txt_nombre_bd.Text))
            {
                contador++;
            }

            if (contador > 0)
            {
                return true;
            }
            else
            {
                return false;
            }   
        }
      
    }

}