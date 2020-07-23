using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace APRSOFTWARE_APP
{
    class Clientes_listar
    {
        public int id_cliente { get; set; }
        public string nombre{ get; set; }
        public string apellido { get; set; }
        public string direccion { get; set; }
        public string num_medidor { get; set; }
        public int num_ruta { get; set; }

    }
}
