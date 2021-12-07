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
        public static void AddWeatherEntites(List<Weather> weathers)
        {
            using (var db = new DataBaseContext())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [WeatherTable]");

                db.SaveChanges();
                db.WeatherTable.AddRange(weathers);
                db.SaveChanges();
            }
        }
        public static void AddLoadEntites(List<Load> loads)
        {
            using(var db = new DataBaseContext())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [LoadTable]");
                db.SaveChanges();
                db.LoadTable.AddRange(loads);
                db.SaveChanges();
            }

            
        }
        public static List<Weather> GetAllWeather()
        {
            using (var db = new DataBaseContext())
            {
                return db.WeatherTable.ToList();
            }
        }
        public static List<Load> GetAllLoads()
        {
            using (var db = new DataBaseContext())
            {
                return db.LoadTable.ToList();
            }
        }
    }
}
