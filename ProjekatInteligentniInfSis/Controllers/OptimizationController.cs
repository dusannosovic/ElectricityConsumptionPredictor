using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DataBase.Controller;
using DataBase.Model;
using OptimisationModel;

namespace ProjekatInteligentniInfSis.Controllers
{
    public class OptimizationController : ApiController
    {
        const float maxSunInsolation = (float)0.001;
        const float windTurbinePowerPerSquareMeter = (float)0.00034;
        [Route("api/Optimization/AddPowerPlant")]
        [HttpPost]
        public string AddPowerPlant(PowerPlant powerPlant)
        {
            var request = HttpContext.Current.Request;
            if (powerPlant.Type=="Solar") {
                powerPlant.MaxLoad = (int)(powerPlant.Area * maxSunInsolation);
                powerPlant.Eff = powerPlant.Eff / 100;
            }
            if (powerPlant.Type == "wind")
            {
                powerPlant.MaxLoad = (int)(powerPlant.Area * windTurbinePowerPerSquareMeter);
            }
            CrudOperations.AddPowerPlant(powerPlant);
            return "ok";
        }
        [Route("api/Optimization/GetPowerPlants")]
        [HttpGet]
        public List<PowerPlant> GetPowerPlants()
        {
            return CrudOperations.GetPowerPlants();
        }
        [Route("api/Optimization/SetOptimizationParameters")]
        [HttpPost]
        public string SetOptimizationParameters(OptimizationData pwrPlants) 
        {

            Optimisation opt = new Optimisation();
            //List<Tuple<int, PowerPlant>> powrPlant = new List<Tuple<int, PowerPlant>>();
            opt.MakeOptimization(pwrPlants);

            return "";
        }
        [Route("api/Optimization/GetAllDates")]
        [HttpGet]
        public List<string> GetAllDates()
        {
            List<string> d = new List<string>();
            List<DateTime> predictions = CrudOperations.GetPredictions().Select(s=>s.Date.Date).Distinct().ToList();
            foreach(DateTime dat in predictions)
            {
                d.Add(dat.Date.ToShortDateString());
            }
            return d;
        }
        [Route("api/Optimization/GetOptimizedDataPerHour")]
        [HttpGet]
        public List<OptimizedPerHourBanding> GetOptimizedDataPerHours()
        {
            List<OptimizedDataPerHour> data = CrudOperations.GetOptimizedDataPerHour();
            List<OptimizedPerHourBanding> optData = new List<OptimizedPerHourBanding>();
            foreach(OptimizedDataPerHour d in data)
            {
                OptimizedPerHourBanding optBanding = new OptimizedPerHourBanding();
                optBanding.DateAndTimeOfOptimization = d.DateAndTimeOfOptimization;
                optBanding.LoadToOptimize = d.LoadToOptimize;
                optBanding.PwrPlantLoad = new List<OptimizedBanding>();
                foreach(OptimizedData opt in d.PwrPlantLoad)
                {
                    OptimizedBanding optimizedBanding = new OptimizedBanding();
                    optimizedBanding.Costs = opt.Costs;
                    optimizedBanding.Index = opt.Index;
                    optimizedBanding.Load = opt.Load;
                    optimizedBanding.Type = opt.Type;
                    optimizedBanding.Name = opt.Name;
                    optBanding.PwrPlantLoad.Add(optimizedBanding);
                }
                optData.Add(optBanding);
            }
            return optData;

        }
        [Route("api/Optimization/GetCoilProduction")]
        [HttpGet]
        public int GetCoilProduction()
        {
            int temp = 0;
            foreach (OptimizedData opt in CrudOperations.GetOptimizedData().Where(s => s.Type == "Coil"))
            {
                temp += (int)opt.Load;
            }
            return temp;
        }

    }
}