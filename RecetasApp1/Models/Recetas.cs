using SQLite;
using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecetasApp1.Models
{
    [Table("Receta")]
    public class Receta
    {
        [PrimaryKey, AutoIncrement]
        public int IdReceta { get; set; }
        [MaxLength(40)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Elavoration { get; set; }
    }
    
}
