using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace APRSOFTWARE_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListadoRutas : ContentPage
    {
        ObservableCollection<Ruta> Rutas_listados = new ObservableCollection<Ruta>();
        int mes_numero;
        string mesnombre_anterior;
        string mesnombre_actual;
        int anioo_anterior;
        int anioo_actual;
        DateTime fecha_actual;
        public ListadoRutas()
        {
            InitializeComponent();
            anio.Text = DateTime.Now.Year.ToString();
            picker_mes.SelectedIndex = DateTime.Now.Month - 1;
 
        }
        private void btn_mostrar_Clicked(object sender, EventArgs e)
        {
            anioo_actual = int.Parse(anio.Text);
            mes_numero = picker_mes.SelectedIndex;
            mesnombre_actual = picker_mes.SelectedItem.ToString();

            if (mes_numero == 0)
            {
                mesnombre_anterior = "diciembre";
                mes_numero = 12;
                anioo_anterior=anioo_actual-1;
            }
            else
            {
                picker_mes.SelectedIndex = mes_numero - 1;
                mesnombre_anterior = picker_mes.SelectedItem.ToString();
                picker_mes.SelectedIndex = picker_mes.SelectedIndex + 1;
                mes_numero += 1;
                anioo_anterior = anioo_actual;
            }
            fecha_actual = new DateTime(anioo_actual, mes_numero, 1);
            DateTime FechaAnterior = fecha_actual.AddMonths(-1);
            int idglobal =Modulo.GetIdAPR();
          // DateTime UltimaFecha=Modulo.GetUltimaFechaSincronizacion(idglobal);
           // if (UltimaFecha.Month>=FechaAnterior.Month && UltimaFecha.Year>=FechaAnterior.Year)
            //{
            //    DisplayAlert("Informacion", "Antes debe sincronizar la APP desde APRSoftware.", "ACEPTAR");
           // }
           // else 
           // {
                Rutas_listados.Clear();
                Rutas_listado.ItemsSource = Rutas_listados;
                CargarRutas();
           // }           
       
        }
        private async void CargarRutas()
            {         
            var tabla_lecturas = Modulo.cnSqlite.Query<Lecturas>("select*from lecturas where mes='" + mesnombre_anterior + "' and anio='" + anioo_anterior + "'");
            if (tabla_lecturas.Count() == 0)
            {
               await DisplayAlert("Informacion", "No existe lectura en el mes anterior.", "ACEPTAR");
                Rutas_listado.ItemsSource = null;
                return;
            }
            var tabla_rutas = Modulo.cnSqlite.Query<Rutas>("select nombre from rutas order by nombre collate nocase asc");
            if (tabla_rutas.Count() > 0)
            {
                foreach (var item in tabla_rutas)
                {
                    var clientes_por_ruta = Modulo.cnSqlite.Query<Clientes>("select*from clientes where nombre_ruta=? and (estado='corte en tramite' or estado='activo')",item.nombre);
                    int cantidad_clientes_ruta = clientes_por_ruta.Count();
                    int contador = 0;
                    foreach (var cliente in clientes_por_ruta)
                    {
                        var lecturas_mes = Modulo.cnSqlite.Query<Lecturas>("select*from lecturas where mes='" + mesnombre_actual + "' and anio='" + anioo_actual + "' and id_cliente='" + cliente.id_cliente + "'");
                        if (lecturas_mes.Count() > 0)
                        {
                            contador += 1;
                        }   
                    }
                    int pendientes = cantidad_clientes_ruta - contador;
                    string mensaje;
                    if (pendientes == 0)
                    {
                        if (contador==0)
                        {
                            mensaje = "Sin clientes";
                        }
                        else
                        {
                            mensaje = "Completada";
                        }                        
                    }
                    else
                    {
                        mensaje = "[Pendientes " + pendientes.ToString() + " de " + cantidad_clientes_ruta.ToString() + "]";
                    }
                    Ruta ruta = new Ruta()
                    {
                        nombre_ruta = item.nombre,
                        pendientes = mensaje
                    };
                    Rutas_listados.Add(ruta);
                }
  
                Rutas_listado.ItemsSource = Rutas_listados;
            }
            else
            {
               await DisplayAlert("Informacion", "No existen rutas para cargar. Se mostrararan clientes como sin ruta.", "ACEPTAR");
            }
        }

        private void titulo_ruta_Focused(object sender, FocusEventArgs e)
        {
            
        }

        private async void Rutas_listado_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }
            Ruta ruta = e.SelectedItem as Ruta;
            Modulo.NombreRuta = ruta.nombre_ruta.ToString();
            Modulo.MesNombreActual = mesnombre_actual;
            Modulo.MesNombreAnterior = mesnombre_anterior;
            Modulo.AnioActual = anioo_actual;
            Modulo.AnioAnterior = anioo_anterior;
            Modulo.FechaActual = fecha_actual;
            await Navigation.PushAsync(new Lecturas_mainpage());
        }

        private void Rutas_listado_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {

        }
    }
}