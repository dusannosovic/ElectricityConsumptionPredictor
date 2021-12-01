using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBase.Model;

namespace DataBase.Controller
{
    public class CrudOperations
    {
        public static void AddEntites(List<Load> loads, List<Weather> weathers, List<SunriseSunset> sunrises)
        {
            using (var db = new DataBaseContext())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [WeatherTable]");
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [LoadTable]");
                db.SaveChanges();
                //db.WeatherTable.AddRange(weathers);
                db.LoadTable.AddRange(loads);
                db.SaveChanges();
            }
        }
    }
}
