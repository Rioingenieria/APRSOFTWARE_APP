using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
namespace APRSOFTWARE_APP.ViewModel
{
    class ClientesViewModel:BaseViewModel
    {
        private ObservableCollection<Lecturas_clientes> _listado_clientes;
        public ObservableCollection<Lecturas_clientes>  Listado_clientes
        {
            get { return _listado_clientes; }
            set
            {
                SetValue(ref this._listado_clientes, value);
                OnPropertyChanged("Listado_clientes");
            }
        }
        public ClientesViewModel()
        {
            ObservableCollection<Lecturas_clientes> listado = new ObservableCollection<Lecturas_clientes>();
            List<Clientes> ClientesEnRuta = Modulo.cnSqlite.Query<Clientes>("select*from clientes where nombre_ruta='" + Modulo.NombreRuta + "' and (estado='corte en tramite' or estado='activo' or estado='cortado') order by num_ruta asc");
            foreach (var Clientesruta in ClientesEnRuta)
            {
                var ClientesConLectura = Modulo.cnSqlite.Query<Lecturas_clientes>("select lecturas.mes,lecturas.id_cliente,lecturas.anio,clientes.nombre,clientes.apellido,clientes.num_ruta,clientes.direccion,clientes.num_medidor from lecturas inner join clientes on lecturas.id_cliente=clientes.id_cliente where lecturas.mes='" + Modulo.MesNombreActual + "' and lecturas.anio='" + Modulo.AnioActual + "' and clientes.nombre_ruta='" + Modulo.NombreRuta + "' and lecturas.id_cliente='" + Clientesruta.id_cliente + "' order by clientes.num_ruta asc");
                if (ClientesConLectura.Count == 0)
                {
                    Lecturas_clientes lc = new Lecturas_clientes
                    {
                        nombre = Clientesruta.nombre + " " + Clientesruta.apellido,
                        num_medidor = Clientesruta.num_medidor,
                        num_ruta = Clientesruta.num_ruta,
                        direccion = Clientesruta.direccion,
                        id_cliente = Clientesruta.id_cliente
                    };
                   listado.Add(lc);
                }
            }
            _listado_clientes = listado;
        }
     
    }
}
