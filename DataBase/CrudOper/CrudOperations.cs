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
        public static async Task AddWeatherEntites(List<Weather> weathers)
        {
            using (var db = new DataBaseContext())
            {
                //db.Database.ExecuteSqlCommand("TRUNCATE TABLE [WeatherTable]");

                //db.SaveChanges();
                db.WeatherTable.AddRange(weathers);
                await db.SaveChangesAsync();
            }
        }
        public static void DeleteWeatherTable()
        {
            using (var db = new DataBaseContext())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [WeatherTable]");

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
        public static void AddPowerPlant(PowerPlant plant)
        {
            using(var db = new DataBaseContext())
            {
                db.PowerPlantsTable.Add(plant);
                db.SaveChanges();
            }
        }
        public static List<PowerPlant> GetPowerPlants()
        {
            using (var db = new DataBaseContext())
            {

                return db.PowerPlantsTable.ToList();
            }
        }
        public static void DeleteWeather()
        {
            using(var db = new DataBaseContext())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [WeatherTable]");
                db.SaveChanges();
            }
        }
        public static void AddPrediction(List<Prediction> predictions)
        {
            using (var db = new DataBaseContext())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [PredictionsTable]");
                db.SaveChanges();
                db.PredictionsTable.AddRange(predictions);
                db.SaveChanges();
            }
        }
        public static void  UpdatePrediction(List<Prediction> predictions)
        {
            using(var db = new DataBaseContext())
            {
                foreach (Prediction pred in predictions)
                {
                    if (db.PredictionsTable.Select(s=>s.Date).Contains(pred.Date))
                    {
                        db.PredictionsTable.Remove(pred);
                    }
                    db.PredictionsTable.Add(pred);
                }
                db.SaveChanges();
            }
        }
        public static List<Prediction> GetPredictions()
        {
            using (var db = new DataBaseContext())
            {
                return db.PredictionsTable.ToList();
            }
        }
        public static void DeletePredictionTable()
        {
            using(var db = new DataBaseContext())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [PredictionsTable]");
                db.SaveChanges();
            }
        }
        public static void AddOptimizedDataToTable(List<OptimizedData> optimizedData)
        {
            using(var db = new DataBaseContext())
            {
                db.OptimizedDataTable.AddRange(optimizedData);
                db.SaveChanges();
            }
        }
        public static void DeleteOptimizedData()
        {
            using(var db = new DataBaseContext())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [OptimizedDatas]");
                List<OptimizedDataPerHour> a = db.OptimizedDataTablePerHour.ToList();
                foreach(OptimizedDataPerHour temp in a)
                {
                    db.OptimizedDataTablePerHour.Remove(temp);
                }
                db.SaveChanges();
            }
        }
        public static List<OptimizedDataPerHour> GetOptimizedDataPerHour()
        {
            List<OptimizedDataPerHour> optimizedDataPerHours = new List<OptimizedDataPerHour>();
            using(var db = new DataBaseContext())
            {
                foreach(OptimizedDataPerHour optDat in db.OptimizedDataTablePerHour.ToList())
                {
                    optDat.PwrPlantLoad = db.OptimizedDataTable.Where(s => s.OptDataPerHour.DateAndTimeOfOptimization == optDat.DateAndTimeOfOptimization).ToList();
                    optimizedDataPerHours.Add(optDat);
                }

                return optimizedDataPerHours;
            }
        }
        public static List<OptimizedData> GetOptimizedData()
        {
            using(var db = new DataBaseContext())
            {
                return db.OptimizedDataTable.ToList();
            }
        }
    }
}
