using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Model
{
    public class PowerPlant
    {
        [Key]
        public string Name { get; set; }
        public int MaxLoad { get; set; }
        public int MinLoad { get; set; }
        public int FuelConsumption { get; set; }
        public int Co2Emmision { get; set; }
        public string Type { get; set; }

    }
}
