using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace APRSOFTWARE_APP
{
    [Table("Usuarios")]
    class Usuarios
    {        
        public int id_usuario { get; set; }
        public string usuario { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string rut { get; set; }
        public string contrasena { get; set; }
    }
}
