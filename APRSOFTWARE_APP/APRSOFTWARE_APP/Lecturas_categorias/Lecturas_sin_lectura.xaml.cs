using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace APRSOFTWARE_APP.Lecturas_categorias
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Lecturas_sin_lectura : ContentPage
    {
        List<Lecturas_clientes> Listado_clientes = new List<Lecturas_clientes>();
        public Lecturas_sin_lectura()
        {
            InitializeComponent();
            lbl_titulo_ruta.Text = Modulo.NombreRuta;
            CargarClientesSinLectura();
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
            await Navigation.PushAsync(new Lectura_agregar(lc.id_cliente, this.Title.ToString()));
        }
        void CargarClientesSinLectura()
        {
            //SELECIONAMOS CLIENTES DE LA RUTA PARA VERIFICAR CADA CLIENTE SI TIENE OBSERVACION O LECTURA YA INGRESADA  
           List<Clientes> ClientesEnRuta=Modulo.cnSqlite.Query<Clientes>("select*from clientes where nombre_ruta='"+Modulo.NombreRuta+"' and (estado='corte en tramite' or estado='activo' or estado='cortado') order by num_ruta asc");
           foreach (var Clientesruta in ClientesEnRuta)
            {
                var ClientesConLectura = Modulo.cnSqlite.Query<Lecturas_clientes>("select lecturas.mes,lecturas.id_cliente,lecturas.anio,clientes.nombre,clientes.apellido,clientes.num_ruta,clientes.direccion,clientes.num_medidor from lecturas inner join clientes on lecturas.id_cliente=clientes.id_cliente where lecturas.mes='" + Modulo.MesNombreActual + "' and lecturas.anio='" + Modulo.AnioActual + "' and clientes.nombre_ruta='" + Modulo.NombreRuta + "' and lecturas.id_cliente='"+ Clientesruta.id_cliente + "' order by clientes.num_ruta asc");
                if (ClientesConLectura.Count==0)
                {
                    Lecturas_clientes lc = new Lecturas_clientes
                    {
                        nombre = Clientesruta.nombre + " " + Clientesruta.apellido,
                        num_medidor = Clientesruta.num_medidor,
                        num_ruta = Clientesruta.num_ruta,
                        direccion = Clientesruta.direccion,
                        id_cliente = Clientesruta.id_cliente
                    };
                    Listado_clientes.Add(lc);
                }                
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