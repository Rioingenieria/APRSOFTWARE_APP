using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace APRSOFTWARE_APP
{
    [Table("datos_apr")]
    class DatosAPR
    {
        [PrimaryKey, AutoIncrement]
        public int id_dato_apr { get; set; }
        public int id_apr_global { get; set; }
        public string razon_social{ get; set; }
        public string rut { get; set; }
        public string giro { get; set; }
        public string direccion { get; set; }
        public string comuna { get; set; }
        public string ciudad { get; set; }
        public string telefono { get; set; }
        public string correo { get; set; }
        public DateTime ultima_fecha_sincronizacion { get; set; }
    }
}
