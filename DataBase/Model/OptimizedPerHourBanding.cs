using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Model
{
    public class OptimizedPerHourBanding
    {
        public DateTime DateAndTimeOfOptimization { get; set; }

        public int LoadToOptimize { get; set; }

        public List<OptimizedBanding> PwrPlantLoad { get; set; }
    }
}
