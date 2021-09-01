using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace APRSOFTWARE_APP
{
    [Table("rutas")]
    class Rutas
    {
        public int id_ruta { get; set; }
        [MaxLength(255)]
        public string nombre { get; set; }
        [MaxLength(255)]
        public string descripcion { get; set; }

    }
}
