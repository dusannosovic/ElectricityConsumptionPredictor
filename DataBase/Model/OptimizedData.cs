using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Model
{
    public class OptimizedData
    {
        [Key]
        public int Index { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double Load { get; set; }
        public int? Costs { get; set; }
        public int? C02 { get; set; }
        public virtual OptimizedDataPerHour OptDataPerHour { get; set; }
    }
}
