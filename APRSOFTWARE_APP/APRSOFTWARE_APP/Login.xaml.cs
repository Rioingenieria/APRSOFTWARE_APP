using System;
using Xamarin.Forms;
using System.Data;
using System.Data.SqlClient;
using Xamarin.Forms.Xaml;
namespace APRSOFTWARE_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
            Modulo.VerificarTablasBDSqlite();
            CargarUsuariosEnPicker();
        }

        private void configuracion_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage.Navigation.PushModalAsync(new Conexion());
        }

        private void btn_entrar_Clicked(object sender, EventArgs e)
        {
            if (picker_usuarios.SelectedIndex !=-1)
            {
                var usuario=Modulo.cnSqlite.Query<Usuarios>("select*from usuarios where usuario='"+picker_usuarios.SelectedItem.ToString()+"'");
                Modulo.nombre_operador = usuario[0].nombre.ToString();
                Modulo.apellido_operador = usuario[0].apellido.ToString();
                App.Current.MainPage.Navigation.PushModalAsync(new Masterpage());
            }
            else
            {
                DisplayAlert("Informacion", "No existe usuario seleccionado.", "ACEPTAR");
            }
            
        }

        private void usuario_Clicked(object sender, EventArgs e)
        {
            if (Modulo.CrearConexionSQLServer() is true)
            {
                Modulo.cnSqlserver.Open();
                SqlDataAdapter dausuarios=new SqlDataAdapter("select id_usuario,usuario,isnull(nombre,''),isnull(apellido,''),isnull(rut,''),isnull(contraseña,'') from usuarios", Modulo.cnSqlserver);
                DataTable dtusuarios = new DataTable();
                dausuarios.Fill(dtusuarios);
                if (dtusuarios.Rows.Count>0)
                {                    
                    Modulo.cnSqlite.DeleteAll<Usuarios>();
                    foreach (DataRow item in dtusuarios.Rows)
                    {
                        Modulo.cnSqlite.Query<Usuarios>("insert into usuarios(id_usuario,usuario,nombre,apellido,rut,contrasena)values(?,?,?,?,?,?)",item.ItemArray[0],item.ItemArray[1], item.ItemArray[2], item.ItemArray[3], item.ItemArray[4], item.ItemArray[5]);
                    }
                    CargarUsuariosEnPicker();  
                    DisplayAlert("Informacion", "Datos de usuario actualizados desde APRSoftware.", "ACEPTAR");
                }
                else
                {
                    DisplayAlert("Informacion", "No existen usuarios ingresados en APRSoftware.", "ACEPTAR");
                }
                Modulo.cnSqlserver.Close();                
            }
            else
            {
                DisplayAlert("Informacion", "No se han podido obtener datos de usuario de APRSoftware.", "ACEPTAR");
            }
        }
        private void CargarUsuariosEnPicker()
        {
            var tabla_usuarios_local = Modulo.cnSqlite.Query<Usuarios>("select*from usuarios");
            picker_usuarios.Items.Clear();
            foreach (var item in tabla_usuarios_local)
            {
                picker_usuarios.Items.Add(item.usuario);
            }

        }
    }
}