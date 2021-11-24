using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Model
{
    public class TableEntity
    {
        public TableEntity(string id, DateTime dateTime)
        {
            Name = id;
            DateTime = dateTime;
        }
        [Key]
        public string Name { get; set; }

        public float Voltage { get; set; }
        public DateTime DateTime { get; set;}
    }
}
