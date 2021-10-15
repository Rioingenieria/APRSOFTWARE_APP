using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Globalization;
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
        string tab;
        DateTime fecha_inicial_promedio;
        DateTime fecha_actual;
        string accion;
        List<Lecturas_clientes> clientes = new List<Lecturas_clientes>();

        public Lectura_agregar(int ClienteInicial, string Tab_Seleccionada)
        {
            InitializeComponent();
            NumberFormatInfo formato = new CultureInfo("es-AR").NumberFormat;
            formato.CurrencyGroupSeparator = ".";
            formato.NumberDecimalSeparator = ",";
            lbl_nombre_ruta.Text = Modulo.NombreRuta;
            mesanterior = Modulo.MesNombreAnterior;
            anio_anterior = Modulo.AnioAnterior;
            anio_actual = Modulo.AnioActual;
            tab = Tab_Seleccionada;
            mesactual = Modulo.MesNombreActual;
            if (Tab_Seleccionada == "Con Lecturas")
            {
                clientes = Modulo.cnSqlite.Query<Lecturas_clientes>("select lecturas.mes,lecturas.id_cliente,lecturas.observacion,lecturas.anio,clientes.nombre,clientes.apellido,clientes.num_ruta,clientes.direccion,clientes.num_medidor,clientes.rut,clientes.estado from lecturas inner join clientes on lecturas.id_cliente=clientes.id_cliente where lecturas.mes='" + Modulo.MesNombreActual + "' and lecturas.anio='" + Modulo.AnioActual + "' and clientes.nombre_ruta='" + Modulo.NombreRuta + "' and (lecturas.observacion='Sin observacion' or lecturas.observacion is null) order by clientes.num_ruta asc");
            }
            else if (Tab_Seleccionada == "Con Observacion")
            {
                clientes = Modulo.cnSqlite.Query<Lecturas_clientes>("select lecturas.mes,lecturas.id_cliente,lecturas.anio,clientes.nombre,clientes.apellido,clientes.num_ruta,clientes.direccion,clientes.num_medidor,clientes.rut,clientes.estado from lecturas inner join clientes on lecturas.id_cliente=clientes.id_cliente where lecturas.mes='" + Modulo.MesNombreActual + "' and lecturas.anio='" + Modulo.AnioActual + "' and clientes.nombre_ruta='" + Modulo.NombreRuta + "' and lecturas.observacion<>'Sin observacion' order by clientes.num_ruta asc");
            }
            else
            {
                //SELECIONAMOS CLIENTES DE LA RUTA PARA VERIFICAR CADA CLIENTE SI TIENE OBSERVACION O LECTURA YA INGRESADA  
                List<Clientes> ClientesEnRuta = Modulo.cnSqlite.Query<Clientes>("select*from clientes where nombre_ruta='" + Modulo.NombreRuta + "' and (estado='corte en tramite' or estado='activo' or estado='cortado') order by num_ruta asc");
                foreach (var Clientesruta in ClientesEnRuta)
                {
                    var ClientesConLectura = Modulo.cnSqlite.Query<Lecturas_clientes>("select lecturas.mes,lecturas.id_cliente,lecturas.anio,clientes.nombre,clientes.apellido,clientes.num_ruta,clientes.direccion,clientes.num_medidor,clientes.rut,clientes.estado from lecturas inner join clientes on lecturas.id_cliente=clientes.id_cliente where lecturas.mes='" + Modulo.MesNombreActual + "' and lecturas.anio='" + Modulo.AnioActual + "' and clientes.nombre_ruta='" + Modulo.NombreRuta + "' and lecturas.id_cliente='" + Clientesruta.id_cliente + "' and (lecturas.observacion='Sin observacion' or lecturas.observacion is null) order by clientes.num_ruta asc");
                    if (ClientesConLectura.Count == 0)
                    {
                        Lecturas_clientes lc = new Lecturas_clientes
                        {
                            rut = Clientesruta.rut,
                            nombre = Clientesruta.nombre,
                            apellido = Clientesruta.apellido,
                            num_medidor = Clientesruta.num_medidor,
                            num_ruta = Clientesruta.num_ruta,
                            direccion = Clientesruta.direccion,
                            id_cliente = Clientesruta.id_cliente,
                            estado = Clientesruta.estado
                        };
                        clientes.Add(lc);
                    }
                }
            }
            numero_clientes_en_ruta = clientes.Count();
            fecha_actual = Modulo.FechaActual;
            int contador = -1;
            foreach (var item in clientes)
            {
                contador += 1;
                if (item.id_cliente == ClienteInicial)
                {
                    indice = contador;
                }
            }
            id_cliente = clientes[indice].id_cliente;

            CargarClienteEnPantalla(indice);
        }
        private void btn_left_Clicked(object sender, EventArgs e)
        {
            if (tab == "Sin Lectura")
            {
                Retroceder();
            }
            else
            {
                RetrocederConLecturauObservacion();
            }
        }
        private void Retroceder()
        {
            if (clientes.Count() == 1 && txt_consumo_actual.Text.ToString() != string.Empty)
            {
                RemoverClienteConLectura(indice);
                DisplayAlert("Informacion", "Lecturas ingresadas completamente.", "Aceptar");
                Navigation.PushAsync(new Lecturas_mainpage());
                return;
            }
            if (txt_consumo_actual.Text.ToString() != string.Empty)
            {
                RemoverClienteConLectura(indice);
                indice -= 1;
            }
            else
            {
                indice -= 1;
                if (indice<0)
                {
                    indice = clientes.Count()-1;
                }
            }
            CargarClienteEnPantalla(indice);
        }
        private void RetrocederConLecturauObservacion()
        {
            if (clientes.Count() == 0)
            {
                return;
            }
            if (indice == 0)
            {
                indice = clientes.Count() - 1;
            }
            else
            {
                indice -= 1;
            }
            CargarClienteEnPantalla(indice);
        }
        private void btn_right_Clicked(object sender, EventArgs e)
        {
            if (tab == "Sin Lectura")
            {
                Avanzar();
            }
            else
            {
                AvanzarConLecturauObservacion();
            }

        }
        private void RemoverClienteConLectura(int indice)
        {
            clientes.RemoveAt(indice);
        }
        private void Avanzar()
        {
            if (clientes.Count() == 1 && txt_consumo_actual.Text.ToString() != string.Empty)
            {
                RemoverClienteConLectura(indice);
                DisplayAlert("Informacion", "Lecturas ingresadas completamente.", "Aceptar");
                Navigation.PushAsync(new Lecturas_mainpage());
                return;
            }
            if (txt_consumo_actual.Text.ToString() != string.Empty)
            {
                RemoverClienteConLectura(indice);
                if (clientes.Count() == 1)
                {
                    indice = 0;
                }
                if (indice > clientes.Count() - 1)
                {
                    indice = 0;
                }
            }
            else
            {
                indice += 1;
                if (indice > clientes.Count() - 1)
                {
                    indice = 0;
                }
            }
            CargarClienteEnPantalla(indice);           
        }
        private void AvanzarConLecturauObservacion()
        {
            if (clientes.Count() == 0)
            {
                return;
            }
            indice += 1;
            if (indice > clientes.Count() - 1)
            {
                indice = 0;
            }
            CargarClienteEnPantalla(indice);

        }
        private void CargarClienteEnPantalla(int indice)
        {
            txt_lectura_actual.Text = "";
            txt_lectura_anterior.Text = "";
            txt_consumo_actual.Text = "";
            lbl_numero_ruta.Text = "N° en ruta: " + clientes[indice].num_ruta.ToString();
            txt_numero_servicio.Text = clientes[indice].id_cliente.ToString();
            txt_nombre.Text = clientes[indice].nombre.ToString() + " " + clientes[indice].apellido.ToString();
            //txt_rut.Text = clientes[indice].rut.ToString();
            txt_direccion.Text = clientes[indice].direccion.ToString();
            txt_numero_medidor.Text = clientes[indice].num_medidor.ToString();
            //txt_promedio_consumo.Text = CalcularPromedio().ToString();
            txt_ultimo_consumo.Text = "0";
            lbl_estado_cliente.Text = clientes[indice].estado.ToString().ToUpper();

            AsignarColorLabelEstadoCliente();
            ObtenerConsumoUltimoMes();
            if (tab == "Con Lecturas")
            {
                TabConLectura();
            }
            else if (tab == "Con Observacion")
            {
                TabConObservacion();
            }
            else
            {
                TabSinLectura();
            }
        }
        private void TabConLectura()
        {
            var consulta = Modulo.cnSqlite.Query<Lecturas>("select*from lecturas where mes='" + mesactual + "' and anio='" + anio_actual + "' and id_cliente='" + clientes[indice].id_cliente.ToString() + "'");
            if (consulta.Count() > 0)
            {
                accion = "update";
                txt_lectura_anterior.Text = consulta[0].lectura_anterior.ToString();
                txt_lectura_actual.Text = consulta[0].lectura_actual.ToString();
                txt_consumo_actual.Text = consulta[0].consumo.ToString();
                id_lectura_app = consulta[0].id_lectura_app;
                picker_observacion.SelectedItem = consulta[0].observacion.ToString();
            }
        }
        private void TabConObservacion()
        {
            var consulta = Modulo.cnSqlite.Query<Lecturas>("select*from lecturas where mes='" + mesactual + "' and anio='" + anio_actual + "' and id_cliente='" + clientes[indice].id_cliente.ToString() + "'");
            if (consulta.Count() > 0)
            {
                accion = "update";
                txt_lectura_anterior.Text = consulta[0].lectura_anterior.ToString();
                if (consulta[0].lectura_actual == null)
                {
                    txt_lectura_actual.Text = "";
                }
                else
                {
                    txt_lectura_actual.Text = consulta[0].lectura_actual.ToString();
                }
                if (consulta[0].consumo == null)
                {
                    txt_consumo_actual.Text = "";
                }
                else
                {
                    txt_consumo_actual.Text = consulta[0].consumo.ToString();
                }

                id_lectura_app = consulta[0].id_lectura_app;
                picker_observacion.SelectedItem = consulta[0].observacion;
            }
        }
        private void TabSinLectura()
        {

            var datos_anteriores = Modulo.cnSqlite.Query<Lecturas>("select lectura_actual from lecturas where mes='" + mesanterior + "' and anio='" + anio_anterior + "' and id_cliente='" + clientes[indice].id_cliente.ToString() + "'");
            if (datos_anteriores.Count() > 0)
            {
                txt_lectura_anterior.Text = datos_anteriores[0].lectura_actual.ToString();
            }
            else
            {
                txt_lectura_anterior.Text = "0";
            }


        }
        private void ObtenerConsumoUltimoMes()
        {
            List<Lecturas> ultimo_consumo = Modulo.cnSqlite.Query<Lecturas>("select*from lecturas where id_cliente='" + clientes[indice].id_cliente.ToString() + "' and mes='" + mesanterior + "' and anio='" + anio_anterior + "'");
            if (ultimo_consumo.Count() > 0)
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
            decimal promedio = 0;
            int contador = 0;
            fecha_inicial_promedio = fecha_actual.AddMonths(-6);
            var lecturas = Modulo.cnSqlite.Query<Lecturas>("select*from lecturas where id_cliente='" + clientes[indice].id_cliente + "' and fecha<'" + fecha_actual + "' and fecha>='" + fecha_inicial_promedio + "' and consumo is not null order by fecha asc");
            foreach (var item in lecturas)
            {
                promedio = promedio + Convert.ToDecimal(item.consumo);
                contador += 1;
            }
            if (contador == 0)
            {
                return 0;
            }
            promedio = Math.Round(promedio / contador, 2);

            return promedio;
        }
        private double CalcularVarianza()
        {
            //double promedio = double.Parse(txt_promedio_consumo.Text);
            double promedio =0;
            double varianza;
            return promedio;
            var lecturas = Modulo.cnSqlite.Query<Lecturas>("select*from lecturas where id_cliente='" + clientes[indice].id_cliente + "' and fecha<'" + fecha_actual + "' and fecha>='" + fecha_inicial_promedio + "' and consumo is not null order by fecha asc");
            int numero_muestra = 0;
            double sumatoria = 0;
            foreach (var item in lecturas)
            {
                numero_muestra += 1;
                double consumo = Convert.ToDouble(item.consumo);
                sumatoria += Math.Pow(consumo - promedio, 2);
            }
            varianza = sumatoria / (numero_muestra);
            return Math.Sqrt(Math.Pow(varianza, 2));
        }
        private void InsertarLectura()
        {
            decimal? lectura_actual;
            decimal? consumo;
            if (string.IsNullOrEmpty(txt_consumo_actual.Text))
            {
                lectura_actual = null;
                consumo = null;
            }
            else
            {
                lectura_actual = decimal.Parse(txt_lectura_actual.Text);
                consumo = decimal.Parse(txt_consumo_actual.Text);
            }
            Lecturas lectura = new Lecturas()
            {
                id_cliente = clientes[indice].id_cliente,
                nombre = clientes[indice].nombre,
                fecha = fecha_actual,
                fecha_toma = DateTime.Now,
                mes = mesactual,
                anio = anio_actual,
                lectura_anterior = decimal.Parse(txt_lectura_anterior.Text),
                lectura_actual = lectura_actual,
                consumo = consumo,
                //promedio = decimal.Parse(txt_promedio_consumo.Text),
                promedio = 0,
                observacion = picker_observacion.SelectedItem.ToString()
            };
            Modulo.cnSqlite.Insert(lectura);
        }
        private void InsertarObservacion()
        {
            decimal? lectura_actual;
            decimal? consumo;
            if (string.IsNullOrEmpty(txt_consumo_actual.Text))
            {
                lectura_actual = null;
                consumo = null;
            }
            else
            {
                lectura_actual = decimal.Parse(txt_lectura_actual.Text);
                consumo = decimal.Parse(txt_consumo_actual.Text);
            }

            Lecturas lectura = new Lecturas()
            {
                id_cliente = clientes[indice].id_cliente,
                nombre = clientes[indice].nombre,
                fecha = fecha_actual,
                fecha_toma = DateTime.Now,
                mes = mesactual,
                anio = anio_actual,
                lectura_anterior = decimal.Parse(txt_lectura_anterior.Text),
                lectura_actual = lectura_actual,
                consumo = consumo,
                //promedio = decimal.Parse(txt_promedio_consumo.Text),
                promedio = 0,
                observacion = picker_observacion.SelectedItem.ToString()
            };
            Modulo.cnSqlite.Insert(lectura);
        }
        private void ActualizarLectura()
        {
            decimal? lectura_actual;
            decimal? consumo;
            if (string.IsNullOrEmpty(txt_consumo_actual.Text))
            {
                lectura_actual = null;
                consumo = null;
            }
            else
            {
                lectura_actual = decimal.Parse(txt_lectura_actual.Text);
                consumo = decimal.Parse(txt_consumo_actual.Text);
            }
            Lecturas lectura = new Lecturas()
            {
                id_cliente = clientes[indice].id_cliente,
                nombre = clientes[indice].nombre + " " + clientes[indice].apellido,
                fecha_toma = DateTime.Now,
                mes = mesactual,
                anio = anio_actual,
                lectura_anterior = decimal.Parse(txt_lectura_anterior.Text),
                lectura_actual = lectura_actual,
                consumo = consumo,
                // promedio = decimal.Parse(txt_promedio_consumo.Text),
                promedio = 0,
                observacion = picker_observacion.SelectedItem.ToString()
            };
            Modulo.cnSqlite.Query<Lecturas>("update lecturas set id_cliente='" + lectura.id_cliente + "',nombre='" + lectura.nombre + "',fecha_toma='" + lectura.fecha_toma + "',mes='" + lectura.mes + "',anio='" + lectura.anio + "',lectura_anterior='" + lectura.lectura_anterior + "',lectura_actual='" + lectura.lectura_actual + "',consumo='" + lectura.consumo + "',promedio='" + lectura.promedio + "',observacion='" + lectura.observacion + "' where mes='" + mesactual + "' and anio='" + anio_actual + "' and id_cliente='" + clientes[indice].id_cliente + "'");
        }
        private void ActualizarObservacion()
        {
            Modulo.cnSqlite.Query<Lecturas>("update lecturas set observacion='" + picker_observacion.SelectedItem.ToString() + "' where id_lectura_app='" + id_lectura_app + "'");
        }
        private void txt_lectura_actual_Unfocused(object sender, FocusEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txt_lectura_actual.Text))
                {
                    return;
                }
                if (string.IsNullOrEmpty(txt_lectura_anterior.Text))
                {
                    DisplayAlert("Informacion", "Campo lectura anterior esta vacio,", "ACEPTAR");
                    return;
                }

                double consumo = double.Parse(txt_lectura_actual.Text.Replace('.', ',')) - double.Parse(txt_lectura_anterior.Text.Replace('.', ','));
                txt_lectura_actual.Text=txt_lectura_actual.Text.Replace('.', ',');
                txt_lectura_anterior.Text = txt_lectura_anterior.Text.Replace('.', ',');
                consumo = Math.Round(consumo, 2);
                if (consumo < 0)
                {
                    DisplayAlert("Informacion", "Lectura actual debe ser mayor a lectura anterior.", "Aceptar");
                    txt_lectura_actual.Text = "";
                    return;
                }
                txt_consumo_actual.Text = consumo.ToString();
                picker_observacion.SelectedItem = "Sin observacion";
                RegistrarLectura("lectura");
            }
            catch
            {
                DisplayAlert("Informacion", "Formatos de lecturas incorrectos.", "ACEPTAR");
            }
        }
        private void RegistrarLectura(string tipo)
        {
            if (tipo == "lectura")
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
            }
            if (tab == "Sin Lectura")
            {
                var consulta = Modulo.cnSqlite.Query<Lecturas>("select id_lectura_app,observacion from lecturas where mes='" + mesactual + "' and anio='" + anio_actual + "' and id_cliente='" + clientes[indice].id_cliente.ToString() + "'");

                if (consulta.Count > 0)
                {
                    accion = "update";
                    picker_observacion.SelectedItem = consulta[0].observacion.ToString();
                }
                else
                {
                    accion = "insert";
                    picker_observacion.SelectedIndex = 0;
                }
            }
            if (accion == "insert")
            {
                // DisplayAlert("Informacion", CalcularVarianza().ToString(), "ACEPTAR");
                //  DisplayAlert("Informacion", "insert", "ACEPTAR");
                InsertarLectura();
            }
            else
            {
                //DisplayAlert("Informacion", CalcularVarianza().ToString(), "ACEPTAR");
                //DisplayAlert("Informacion","update", "ACEPTAR");
                ActualizarLectura();
            }
        }

        private void btn_pendientes_Clicked(object sender, EventArgs e)
        {
            int contador = 0;
            Avanzar();
            contador += 1;
            if (contador > numero_clientes_en_ruta)
            {
                DisplayAlert("Informacion", "Se ha recorrido completamente la ruta y no se encuentran clientes sin lectura pendiente.", "ACEPTAR");
                return;
            }

            while (txt_consumo_actual.Text != "")
            {
                Avanzar();
            }
        }

        private void picker_observacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (picker_observacion.SelectedIndex > 0)
            {
                RegistrarLectura("observacion");
            }

        }

        private void btn_volver_Clicked(object sender, EventArgs e)
        {
       // Lecturas_categorias.Lecturas_sin_lectura.cargar
        }
    }
}