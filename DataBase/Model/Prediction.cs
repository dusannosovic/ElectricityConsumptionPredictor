using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Model
{
    public class Prediction
    {
        [Key]
        public DateTime Date { get; set; }
        public int Predicted { get; set; }
        public int? RealValue { get; set; }
    }
}
