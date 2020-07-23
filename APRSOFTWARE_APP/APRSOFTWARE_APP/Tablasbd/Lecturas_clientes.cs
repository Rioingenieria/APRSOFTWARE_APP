using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace APRSOFTWARE_APP
{

    [Table("lecturas_clientes")]
    class Lecturas_clientes
    {
        public int id_lectura_app { get; set; }
        public int id_cliente { get; set; }
        public string nombre { get; set; }
        public DateTime fecha { get; set; }
        public DateTime fecha_toma { get; set; }
        public string mes { get; set; }
        public int anio { get; set; }   
        public decimal lectura_anterior { get; set; }
        public decimal lectura_actual { get; set; }
        public decimal consumo { get; set; }
        public decimal promedio { get; set; }      
        public string observacion { get; set; }
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
