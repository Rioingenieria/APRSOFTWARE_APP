using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace APRSOFTWARE_APP
{
    [Table("clientes")]
    class Clientes
    {
        public int id_cliente { get; set; }
        public string nombre{ get; set; }
        public string apellido { get; set; }
        public string rut { get; set; }
        public string direccion { get; set; }
        public string num_medidor { get; set; }
        public int id_ruta { get; set; }
        public string nombre_ruta { get; set; }
        public int num_ruta { get; set; }
        public string estado { get; set; }
    }
}
