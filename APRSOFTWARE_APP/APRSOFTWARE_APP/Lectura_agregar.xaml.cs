using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace APRSOFTWARE_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Lectura_agregar : ContentPage
    {
        int numero_clientes_en_ruta;
        int id_cliente;
        int indice;
        string mesanterior;
        string mesactual;
        int anio_anterior;
        int anio_actual;
        int id_lectura_app;
        DateTime fecha_inicial_promedio;
        DateTime fecha_actual;
        string accion;
        List<Clientes> clientes = new List<Clientes>();
        public Lectura_agregar(string NombreRutaRecibido,string MesnombreActual,string MesnombreAnterior,int AnioAnterior,DateTime FechaActual,int AnioActual)
        {
            InitializeComponent();
            lbl_nombre_ruta.Text = NombreRutaRecibido;
            mesanterior = MesnombreAnterior;
            anio_anterior = AnioAnterior;
            anio_actual = AnioActual;
            mesactual = MesnombreActual;
            clientes=Modulo.cnSqlite.Query<Clientes>("select*from clientes where nombre_ruta='" + NombreRutaRecibido + "' order by num_ruta asc");
            numero_clientes_en_ruta = clientes.Count();
            fecha_actual = FechaActual;
            indice = 0;
            id_cliente = clientes[indice].id_cliente;
            CargarClienteEnPantalla();
           
        }
        private void btn_left_Clicked(object sender, EventArgs e)
        {
            Retroceder();
        }
        private void Retroceder()
        {
            indice -= 1;
            if (indice < 0)
            {
                DisplayAlert("Informacion", "Primer cliente. Se pasara al ultimo cliente.", "ACEPTAR");
                indice = numero_clientes_en_ruta - 1;
            }
            CargarClienteEnPantalla();
        }
        private void btn_right_Clicked(object sender, EventArgs e)
        {
            Avanzar();
        }
        private void Avanzar()
        {
            indice += 1;
            if (indice > numero_clientes_en_ruta - 1)
            {
                DisplayAlert("Informacion", "Ultimo cliente. Se volvera al primer cliente.", "ACEPTAR");
                indice = 0;
            }
            CargarClienteEnPantalla();
        }
        private void CargarClienteEnPantalla()
        {
            txt_lectura_actual.Text = "";
            txt_lectura_anterior.Text = "";
            txt_consumo_actual.Text = "";
            lbl_numero_ruta.Text= "N° en ruta: " + clientes[indice].num_ruta.ToString();
            txt_numero_servicio.Text = clientes[indice].id_cliente.ToString();
            txt_nombre.Text = clientes[indice].nombre.ToString() + " " + clientes[indice].apellido.ToString();
            txt_rut.Text = clientes[indice].rut.ToString();
            txt_direccion.Text = clientes[indice].direccion.ToString();
            txt_numero_medidor.Text = clientes[indice].num_medidor.ToString();
            txt_promedio_consumo.Text = CalcularPromedio().ToString();
            txt_ultimo_consumo.Text = "0";
            lbl_estado_cliente.Text= clientes[indice].estado.ToString().ToUpper();
            AsignarColorLabelEstadoCliente();
            ObtenerConsumoUltimoMes();
            var consulta=Modulo.cnSqlite.Query<Lecturas>("select*from lecturas where mes='" + mesactual + "' and anio='" + anio_actual + "' and id_cliente='" + clientes[indice].id_cliente.ToString() + "'");
           if (consulta.Count() > 0)
            {
                accion = "update";
                txt_lectura_anterior.Text = consulta[0].lectura_anterior.ToString();
                txt_lectura_actual.Text = consulta[0].lectura_actual.ToString();
                txt_consumo_actual.Text= consulta[0].consumo.ToString();
                id_lectura_app = consulta[0].id_lectura_app;
            }
            else
            {
                accion = "insert";
                var datos_anteriores = Modulo.cnSqlite.Query<Lecturas>("select*from lecturas where mes='" + mesanterior + "' and anio='" + anio_anterior + "' and id_cliente='" + clientes[indice].id_cliente.ToString() + "'");
                if (datos_anteriores.Count() > 0)
                {
                    txt_lectura_anterior.Text = datos_anteriores[0].lectura_actual.ToString();
                }
                else
                {
                    txt_lectura_anterior.Text = "0";
                }

            }
           
        }
        private void ObtenerConsumoUltimoMes()
        {
            List<Lecturas> ultimo_consumo=Modulo.cnSqlite.Query<Lecturas>("select*from lecturas where id_cliente='"+ clientes[indice].id_cliente.ToString() + "' and mes='"+mesanterior+"' and anio='"+anio_anterior+"'");
            if (ultimo_consumo.Count()>0)
            {
                txt_ultimo_consumo.Text = ultimo_consumo[0].consumo.ToString();
            }
            else
            {
                txt_ultimo_consumo.Text = "0";
            }
        }
        private void AsignarColorLabelEstadoCliente()
        {
            if (clientes[indice].estado.ToString() == "activo")
            {
                lbl_estado_cliente.TextColor = Color.FromHex("#79AB3A");
            }
            if (clientes[indice].estado.ToString() == "corte en tramite")
            {
                lbl_estado_cliente.TextColor = Color.FromHex("#D21414");
            }
            if (clientes[indice].estado.ToString() == "cortado")
            {
                lbl_estado_cliente.TextColor = Color.FromHex("#D21414");
            }
        }
        private decimal CalcularPromedio()
        {
            decimal promedio=0;
            int contador=0;
            fecha_inicial_promedio = fecha_actual.AddMonths(-6);
            var lecturas=Modulo.cnSqlite.Query<Lecturas>("select*from lecturas where id_cliente='"+ clientes[indice].id_cliente + "' and fecha<'"+fecha_actual+"' and fecha>='"+fecha_inicial_promedio+"' order by fecha asc");
            foreach (var item in lecturas)
            {
                promedio =promedio+ item.consumo;
                contador +=1;
            }
            if (contador == 0)
            {
                return 0;
            }
            promedio =Math.Round( promedio / contador,2);

            return promedio;
        }
        private double CalcularVarianza()
        {
            double promedio = double.Parse(txt_promedio_consumo.Text);
            double varianza;
            return promedio;
            var lecturas = Modulo.cnSqlite.Query<Lecturas>("select*from lecturas where id_cliente='" + clientes[indice].id_cliente + "' and fecha<'" + fecha_actual + "' and fecha>='" + fecha_inicial_promedio + "' order by fecha asc");
            int numero_muestra = 0;
            double sumatoria = 0;
            foreach (var item in lecturas)
            {
               numero_muestra += 1;
                double consumo = Convert.ToDouble(item.consumo);
                sumatoria += Math.Pow(consumo-promedio, 2);
            }
            varianza = sumatoria/(numero_muestra);
            return Math.Sqrt(Math.Pow(varianza,2));
        }
        private  void InsertarLectura()
        {
            Lecturas lectura = new Lecturas()
            {
                id_cliente = clientes[indice].id_cliente,
                nombre = clientes[indice].nombre,
                fecha=fecha_actual,
                fecha_toma = DateTime.Now,
                mes = mesactual,
                anio=anio_actual,
                lectura_anterior=decimal.Parse(txt_lectura_anterior.Text),
                lectura_actual=decimal.Parse(txt_lectura_actual.Text),
                consumo=decimal.Parse(txt_consumo_actual.Text),
                promedio=0,
                observacion=""
            };
            Modulo.cnSqlite.Insert(lectura);
        }
        private void ActualizarLectura()
        {
            Lecturas lectura = new Lecturas()
            {
                id_cliente = clientes[indice].id_cliente,
                nombre = clientes[indice].nombre +" "+clientes[indice].apellido,
                fecha_toma = DateTime.Now,
                mes = mesactual,
                anio = anio_actual,
                lectura_anterior = decimal.Parse(txt_lectura_anterior.Text),
                lectura_actual = decimal.Parse(txt_lectura_actual.Text),
                consumo = decimal.Parse(txt_consumo_actual.Text),
                promedio = 0,
                observacion = ""
            };
            Modulo.cnSqlite.Query<Lecturas>("update lecturas set id_cliente='"+lectura.id_cliente+"',nombre='" + lectura.nombre+"',fecha_toma='" + lectura.fecha_toma+"',mes='" + lectura.mes+"',anio='" + lectura.anio+"',lectura_anterior='" + lectura.lectura_anterior+"',lectura_actual='" + lectura.lectura_actual+"',consumo='" + lectura.consumo+"',promedio='" + lectura.promedio+"',observacion='" + lectura.observacion+"' where id_lectura_app='"+id_lectura_app+"'");      
        }
        private void txt_lectura_actual_Unfocused(object sender, FocusEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txt_lectura_anterior.Text))
                {
                    DisplayAlert("Informacion", "Campo lectura anterior esta vacio,", "ACEPTAR");
                    return;
                }

                decimal consumo = decimal.Parse(txt_lectura_actual.Text.Replace(',','.')) - decimal.Parse(txt_lectura_anterior.Text.Replace(',', '.'));
                if (consumo < 0)
                {
                    DisplayAlert("Informacion", "Lectura actual debe ser mayor a lectura anterior.", "Aceptar");
                    txt_lectura_actual.Text = "";
                    return;
                }
                txt_consumo_actual.Text = consumo.ToString();
                RegistrarLectura();
            }
            catch
            {
                DisplayAlert("Informacion", "Formatos de lecturas incorrectos.", "ACEPTAR");
            }
            
        }    
        private void RegistrarLectura()
        {
            if (string.IsNullOrEmpty(txt_lectura_anterior.Text))
            {
                DisplayAlert("Informacion", "No existe lectura anterior ingresada.", "ACEPTAR");
                return;
            }
            if (string.IsNullOrEmpty(txt_lectura_actual.Text))
            {
                DisplayAlert("Informacion", "No existe lectura actual ingresada.", "ACEPTAR");
                return;
            }
            if (string.IsNullOrEmpty(txt_consumo_actual.Text))
            {
                DisplayAlert("Informacion", "No existe consumo calculado.", "ACEPTAR");
                return;
            }
            if (accion == "insert")
            {
               // DisplayAlert("Informacion", CalcularVarianza().ToString(), "ACEPTAR");
                InsertarLectura();
            }
            else
            {
                //DisplayAlert("Informacion", CalcularVarianza().ToString(), "ACEPTAR");
                ActualizarLectura();
            }
        }

        private void btn_pendientes_Clicked(object sender, EventArgs e)
        {
            int contador = 0;
            Avanzar();
                contador += 1;
                if (contador>numero_clientes_en_ruta)
                {
                    DisplayAlert("Informacion","Se ha recorrido completamente la ruta y no se encuentran clientes sin lectura pendiente.", "ACEPTAR");
                    return;
                }

                while (txt_consumo_actual.Text !="")
                {
                    Avanzar();
                }                   
        }
    }
}