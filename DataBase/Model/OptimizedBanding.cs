using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Model
{
    public class OptimizedBanding
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double Load { get; set; }
        public int? Costs { get; set; }
    }
}
