using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
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
                    var tabla_lecturas = Modulo.cnSqlite.Query<Lecturas>("select*from lecturas where mes='" + picker_mes.SelectedItem.ToString() + "' and anio='" + anio.Text.ToString() + "'");
                    if (tabla_lecturas.Count() > 0)
                    {
                        if (Modulo.CrearConexionSQLServer() is false)
                        {
                            await DisplayAlert("Informacion", "No se ha podido establecer conexion con el APRSoftware.", "ACEPTAR");
                            return;
                        }
                        lbl_mensaje.Text = "Exportando datos...";
                        await Task.Run(()=> { EnviarLecturasLocalesEnServidorSQLServerAsync(tabla_lecturas); });
                        lbl_mensaje.Text = "";
                        await DisplayAlert("Informacion", "Lecturas enviadas correctamente a APRSoftware.", "ACEPTAR");
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
        private void EnviarLecturasLocalesEnServidorSQLServerAsync(List<Lecturas> lecturas_local)
        {           
                Modulo.cnSqlserver.Open();
                foreach (var fila in lecturas_local)
                {
                    SqlDataAdapter dalectura = new SqlDataAdapter("select*from lecturas where mes='" + picker_mes.SelectedItem.ToString() + "' and año='" + anio.Text.ToString() + "' and id_cliente='" + fila.id_cliente + "' and consumo is not null", Modulo.cnSqlserver);
                    DataTable dtlectura = new DataTable();
                    dalectura.Fill(dtlectura);
                    if (dtlectura.Rows.Count > 0)
                    {
                        //FUNCION POR DEFINIR SI ACTUALIZAR DESDE APP O NO
                    }
                    else
                    {
                        SqlCommand insertar_lecturas = new SqlCommand("insert into lecturas(id_cliente,lectura_anterior,lectura_actual,fecha_toma,mes,año,abono,estado,observacion,convenio)values(@id_cliente,@lectura_anterior,@lectura_actual,@fecha_toma,@mes,@año,@abono,@estado,@observacion,@convenio)", Modulo.cnSqlserver);
                        insertar_lecturas.Parameters.AddWithValue("@id_cliente", fila.id_cliente);
                        insertar_lecturas.Parameters.AddWithValue("@lectura_anterior", fila.lectura_anterior);
                        insertar_lecturas.Parameters.AddWithValue("@lectura_actual", fila.lectura_actual);
                        insertar_lecturas.Parameters.AddWithValue("@fecha_toma", fila.fecha_toma.ToString("yyyyMMdd"));
                        insertar_lecturas.Parameters.AddWithValue("@mes", picker_mes.SelectedItem.ToString());
                        insertar_lecturas.Parameters.AddWithValue("@año", int.Parse(anio.Text.ToString()));
                        insertar_lecturas.Parameters.AddWithValue("@abono", 0);
                        insertar_lecturas.Parameters.AddWithValue("@estado", "pendiente");
                        insertar_lecturas.Parameters.AddWithValue("@observacion", fila.observacion);
                    insertar_lecturas.Parameters.AddWithValue("@convenio", 0);
                    insertar_lecturas.ExecuteNonQuery();
                    }

                }
                Modulo.cnSqlserver.Close();          
            
        }
        private void picker_tabla_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }
        private void CargarImportar()
        {
            picker_tabla.ItemsSource = null;
            var lista_importar = new List<String>() {"Clientes","Rutas"};
            picker_tabla.ItemsSource = lista_importar;
        }
        private void CargarExportar()
        {
            picker_tabla.ItemsSource = null;
            var lista_exportar = new List<String>() {"Lecturas"};
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