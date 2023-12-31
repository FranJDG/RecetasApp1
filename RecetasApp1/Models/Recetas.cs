﻿using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecetasApp1.Models
{
    [SQLite.Table("Receta")]
    public class Receta
    {
        [PrimaryKey, AutoIncrement]
        public int IdReceta { get; set; }
        [MaxLength(40)]
        public string Name { get; set; }
        public string Category { get; set; }
        public int Diners { get; set; }
        public int Time { get; set; }        
        public string Instructions { get; set; }
        public string ImagePath { get; set; }        
    }

    [SQLite.Table("Ingrediente")]
    public class Ingrediente
    {
        [PrimaryKey, AutoIncrement]
        public int IdI { get; set; }

        [MaxLength(30)]
        public string NameI { get; set; }
        [MaxLength(10)]
        public double Quantity { get; set; }
        public string Unit { get; set; }

        [ForeignKey("Receta")]
        public int RecetaId { get; set; }
    }

}
