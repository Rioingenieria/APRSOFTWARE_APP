using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace APRSOFTWARE_APP.Lecturas_categorias
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Lecturas_con_lectura : ContentPage
    {
        List<Lecturas_clientes> Listado_clientes = new List<Lecturas_clientes>();
        public Lecturas_con_lectura()
        {
            
            InitializeComponent();
            lbl_titulo_ruta.Text = Modulo.NombreRuta;
        
            CargarClientesConLectura();
        }

        private void Clientes_listado_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {

        }

        private async void Clientes_listado_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }
            Lecturas_clientes lc = e.SelectedItem as Lecturas_clientes;
            await Navigation.PushAsync(new Lectura_agregar(lc.id_cliente,this.Title.ToString()));
        }
        void CargarClientesConLectura()
        {           
               var ClientesConLectura= Modulo.cnSqlite.Query<Lecturas_clientes>("select lecturas.mes,lecturas.id_cliente,lecturas.observacion,lecturas.anio,clientes.nombre,clientes.apellido,clientes.num_ruta,clientes.direccion,clientes.num_medidor from lecturas inner join clientes on lecturas.id_cliente=clientes.id_cliente where lecturas.mes='"+Modulo.MesNombreActual+"' and lecturas.anio='"+Modulo.AnioActual+"' and clientes.nombre_ruta='"+Modulo.NombreRuta+ "' and (lecturas.observacion='Sin observacion' or lecturas.observacion is null) order by clientes.num_ruta asc");
            foreach (var dr in ClientesConLectura)
            {
                Lecturas_clientes lc = new Lecturas_clientes 
                { 
                    nombre=dr.nombre +" "+dr.apellido,
                    num_medidor=dr.num_medidor,
                    num_ruta=dr.num_ruta,
                    direccion=dr.direccion,
                    id_cliente=dr.id_cliente
                };
                Listado_clientes.Add(lc);
            }
            Clientes_listado.ItemsSource = Listado_clientes;
        }   

        private void numero_medidor_Focused(object sender, FocusEventArgs e)
        {

        }

        private void nombre_cliente_Focused(object sender, FocusEventArgs e)
        {

        }

        private void numero_ruta_Focused(object sender, FocusEventArgs e)
        {

        }

        private void direccion_Focused(object sender, FocusEventArgs e)
        {

        }

        private void numero_servicio_Focused(object sender, FocusEventArgs e)
        {

        }
    }
}