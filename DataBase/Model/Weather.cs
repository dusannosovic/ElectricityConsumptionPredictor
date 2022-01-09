using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Model
{
    public class Weather
    {
        public Weather()
        {

        }
        [Key]
        public int Id { get; set; }
        public DateTime LocalTime { get; set; }
        public Double? Temperature { get; set; }
        public Double? APressure { get; set; }
        public Double? Pressure { get; set; }
        public Double? PTencdency { get; set; }
        public Int32? Humidity { get; set; }
        public String WindDirection { get; set; }
        public Int32? WindSpeed { get; set; }
        public Double? Clouds { get; set; }
        public Double? HVisibility { get; set; }
        public Double? DTemperature { get; set; }
        public Double? SunriseSunset { get; set; }
        public Int32 Load { get; set; }
    }
}
