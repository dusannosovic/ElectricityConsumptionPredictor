using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Model
{
    public class OptimizationData
    {
        public string DateString { get; set; }
        public string OptimizationType { get; set; }
        public int FuelCostGas { get; set; }
        public int C02EmissionGas { get; set; }
        public int FuelCostOil { get; set; }
        public int FuelCostCoal { get; set; }
        public int C02EmissionOil { get; set; }
        public int C02EmissionCoal { get; set; }
        public List<PowerPlant> PowerPlantsToOptimize { get; set; }
    }
}
