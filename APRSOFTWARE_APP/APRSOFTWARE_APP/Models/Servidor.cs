using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace APRSOFTWARE_APP
{
    [Table("servidor")]
    public class Servidor
    {

        [PrimaryKey,AutoIncrement]
        public int id_servidor{ get; set; }
        [MaxLength(255)]
        public string ip { get; set; }
        [MaxLength(255)]
        public string puerto { get; set; }
        [MaxLength(255)]
        public string usuariobd { get; set; }
        [MaxLength(255)]
        public string contrasenabd { get; set; }
        [MaxLength(255)]
        public string nombrebd { get; set; }
        
    }
}
