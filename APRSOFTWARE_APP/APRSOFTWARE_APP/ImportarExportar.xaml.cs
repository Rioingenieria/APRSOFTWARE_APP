using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace APRSOFTWARE_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImportarExportar : ContentPage
    {
        public ImportarExportar()
        {
            InitializeComponent();
            picker_mes.SelectedIndex = DateTime.Now.Month - 1;
            picker_opcion.SelectedIndex = 0;
            anio.Text = DateTime.Now.Year.ToString();
            lbl_mensaje.Text = "";
        }
        private async void btn_conectar_Clicked(object sender, EventArgs e)
        {
            if (picker_opcion.SelectedItem.ToString() == "Exportar")
            {
                if (picker_tabla.SelectedItem.ToString() == "Lecturas")
                {
                    var tabla_lecturas = Modulo.cnSqlite.Query<Lecturas>("select ifnull(id_lectura_app,'0') as id_lectura_app,ifnull(id_cliente,'0') as id_cliente,ifnull(nombre,'') as nombre,ifnull(fecha,'20220101') as fecha,ifnull(fecha_toma,'20220101') as fecha_toma,ifnull(mes,'no') as mes,ifnull(lectura_anterior,'0') as lectura_anterior,ifnull(lectura_actual,'0') as lectura_actual,ifnull(consumo,'0') as consumo,ifnull(promedio,'0') as promedio,ifnull(observacion,'no') as observacion from lecturas where mes='" + picker_mes.SelectedItem.ToString() + "' and anio='" + anio.Text.ToString() + "' order by id_cliente asc");
                    if (tabla_lecturas.Count() > 0)
                    {
                        if (Modulo.CrearConexionSQLServer() is false)
                        {
                            await DisplayAlert("Informacion", "No se ha podido establecer conexion con APRSoftware.", "ACEPTAR");
                            return;
                        }
                        lbl_mensaje.Text = "Exportando datos...No cierre la aplicacion.";
                        await Task.Run(() => { EnviarLecturasLocalesEnServidorSQLServer(tabla_lecturas); });
                    lbl_mensaje.Text = "Datos Enviados correctamente";  
                                            
                    }
                    else
                    {
                        await DisplayAlert("Informacion", "No existen lecturas ingresadas en mes y año seleccionado.", "ACEPTAR");
                    }
                }
            }
            else//IMPORTAR
            {
                if (Modulo.CrearConexionSQLServer() is false)
                {
                    await DisplayAlert("Informacion", "No se ha logrado conectar con APRSoftware.", "ACEPTAR");
                    return;
                }
                if (picker_tabla.SelectedItem.ToString() == "Clientes")
                {
                    Modulo.ImportarClientes();
                    await DisplayAlert("Informacion", "Clientes importados correctamente.", "ACEPTAR");
                }
                if (picker_tabla.SelectedItem.ToString() == "Rutas")
                {
                    Modulo.ImportarRutas();
                    await DisplayAlert("Informacion", "Rutas importadas correctamente.", "ACEPTAR");
                }
            }

        }
        private async void EnviarLecturasLocalesEnServidorSQLServer(List<Lecturas> lecturas_local)
        {
            int id_apr_global = Modulo.GetIdAPR();
            Modulo.cnSqlserver.Open();
            await Task.Run(() =>
            {
                foreach (var fila in lecturas_local)
                {
                    SqlDataAdapter dalectura = new SqlDataAdapter("select*from lecturas_app where mes='" + picker_mes.SelectedItem.ToString() + "' and año='" + anio.Text.ToString() + "' and id_cliente='" + fila.id_cliente + "' and id_apr_global='" + id_apr_global + "' order by id_cliente asc", Modulo.cnSqlserver);
                    DataTable dtlectura = new DataTable();
                    dalectura.Fill(dtlectura);
                    if (dtlectura.Rows.Count > 0)
                    {
                        try
                        {
                            SqlCommand insertar_lecturas = new SqlCommand("update lecturas_app set nombre=@nombre,fecha=@fecha,fecha_toma=@fecha_toma,mes=@mes,año=@año,lectura_anterior=@lectura_anterior,lectura_actual=@lectura_actual,consumo=@consumo,observacion=@observacion where id_cliente='" + fila.id_cliente + "' and id_apr_global='" + id_apr_global + "' and mes='" + picker_mes.SelectedItem.ToString() + "' and año='" + anio.Text + "'", Modulo.cnSqlserver);
                            insertar_lecturas.Parameters.AddWithValue("@nombre", fila.nombre);
                            insertar_lecturas.Parameters.AddWithValue("@fecha", fila.fecha);
                            insertar_lecturas.Parameters.AddWithValue("@fecha_toma", fila.fecha_toma.ToString("yyyyMMdd"));
                            insertar_lecturas.Parameters.AddWithValue("@mes", picker_mes.SelectedItem.ToString());
                            insertar_lecturas.Parameters.AddWithValue("@año", int.Parse(anio.Text.ToString()));
                            insertar_lecturas.Parameters.AddWithValue("@lectura_anterior", fila.lectura_anterior);
                            insertar_lecturas.Parameters.AddWithValue("@lectura_actual", fila.lectura_actual);
                            insertar_lecturas.Parameters.AddWithValue("@consumo", fila.consumo);
                            insertar_lecturas.Parameters.AddWithValue("@observacion", fila.observacion);
                            insertar_lecturas.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            if (Modulo.cnSqlserver.State == ConnectionState.Open)
                            {
                                Modulo.cnSqlserver.Close();
                                DisplayAlert("Excepcion", ex.Message, "ACEPTAR");
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            SqlCommand insertar_lecturas = new SqlCommand("insert into lecturas_app(id_apr_global,id_lectura,id_cliente,nombre,fecha,fecha_toma,mes,año,lectura_anterior,lectura_actual,consumo,observacion)values(@id_apr_global,@id_lectura,@id_cliente,@nombre,@fecha,@fecha_toma,@mes,@año,@lectura_anterior,@lectura_actual,@consumo,@observacion)", Modulo.cnSqlserver);
                            insertar_lecturas.Parameters.AddWithValue("@id_apr_global", id_apr_global);
                            insertar_lecturas.Parameters.AddWithValue("@id_lectura", fila.id_lectura_app);
                            insertar_lecturas.Parameters.AddWithValue("@id_cliente", fila.id_cliente);
                            insertar_lecturas.Parameters.AddWithValue("@nombre", fila.nombre);
                            insertar_lecturas.Parameters.AddWithValue("@fecha", fila.fecha);
                            insertar_lecturas.Parameters.AddWithValue("@fecha_toma", fila.fecha_toma.ToString("yyyyMMdd"));
                            insertar_lecturas.Parameters.AddWithValue("@mes", picker_mes.SelectedItem.ToString());
                            insertar_lecturas.Parameters.AddWithValue("@año", int.Parse(anio.Text.ToString()));
                            insertar_lecturas.Parameters.AddWithValue("@lectura_anterior", fila.lectura_anterior);
                            insertar_lecturas.Parameters.AddWithValue("@lectura_actual", fila.lectura_actual);
                            insertar_lecturas.Parameters.AddWithValue("@consumo", fila.consumo);
                            insertar_lecturas.Parameters.AddWithValue("@observacion", fila.observacion);
                            insertar_lecturas.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            if (Modulo.cnSqlserver.State == ConnectionState.Open)
                            {
                                Modulo.cnSqlserver.Close();
                                DisplayAlert("Excepcion", ex.Message, "ACEPTAR");
                            }
                        }
                    }
                }
            });
            Modulo.cnSqlserver.Close();
        }
        private void picker_tabla_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void CargarImportar()
        {
            picker_tabla.ItemsSource = null;
            var lista_importar = new List<String>() { "Clientes", "Rutas" };
            picker_tabla.ItemsSource = lista_importar;
        }
        private void CargarExportar()
        {
            picker_tabla.ItemsSource = null;
            var lista_exportar = new List<String>() { "Lecturas" };
            picker_tabla.ItemsSource = lista_exportar;
        }
        private void picker_opcion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (picker_opcion.SelectedItem.ToString() == "Exportar")
            {
                CargarExportar();
                picker_tabla.SelectedIndex = 0;
            }
            else
            {
                CargarImportar();
                picker_tabla.SelectedIndex = 0;
            }
        }
    }
}