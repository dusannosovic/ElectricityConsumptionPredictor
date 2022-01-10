using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Model
{
    public class OptimizedDataPerHour
    {
        [Key]
        public DateTime DateAndTimeOfOptimization { get; set; }

        public virtual ICollection<OptimizedData> PwrPlantLoad { get; set; }
    }
}
