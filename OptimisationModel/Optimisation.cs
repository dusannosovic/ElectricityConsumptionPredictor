using DataBase.Controller;
using DataBase.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CenterSpace.NMath.Core;

namespace OptimisationModel
{
    public class Optimisation
    {
        public const double Imax = 0.001;
        public void MakeOptimization(OptimizationData optimizationData)
        {
            DateTime dateFromString = Convert.ToDateTime(optimizationData.DateString);
            List<Prediction> predictions = CrudOperations.GetPredictions().Where(s => s.Date.Date == dateFromString.Date).ToList();
            //List<PowerPlant> powerPlants = CrudOperations.GetPowerPlants();
            CrudOperations.DeleteOptimizedData();
            string url = "https://api.openweathermap.org/data/2.5/onecall?lat=33.44&lon=-94.04&exclude=current,minutely,daily,alerts&appid=06f80eb85672c812a1a6dd14d9619fc9";
            JArray dataArray;
            JToken token;
            double windSpeed;
            float clouds;
            try
            {
                var client = new WebClient();
                var content = client.DownloadString(url);
                dynamic data = JObject.Parse(content);
                dataArray = data.hourly;
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Clouds is " + clouds + "wind is " + windSpeed);
                return;
            }

            foreach (Prediction prediction in predictions)
            {
                OptimizedDataPerHour optimizedDataPerHour = new OptimizedDataPerHour();
                optimizedDataPerHour.DateAndTimeOfOptimization = prediction.Date;
                token = dataArray[prediction.Date.Hour];
                clouds = (float)double.Parse(token["clouds"].ToString());
                windSpeed = double.Parse(token["wind_speed"].ToString());
                CrudOperations.AddOptimizedDataToTable(PrepareRenewable(prediction, optimizationData.PowerPlantsToOptimize,optimizedDataPerHour, optimizationData.OptimizationType, optimizationData.WeightFactor,clouds,windSpeed));
            }
        }
        public List<OptimizedData> PrepareRenewable(Prediction prediction, List<PowerPlant> powerPlants,OptimizedDataPerHour optimizedDataPerHour,string optimizationType,float weightFactor,float clouds,double windSpeed)
        {
            List<OptimizedData> optimizedData = new List<OptimizedData>();
            //var openWeatherAPI = new OpenWeatherAPI.OpenWeatherApiClient("06f80eb85672c812a1a6dd14d9619fc9");
            //var query = openWeatherAPI.Query("chicago");
            // System.Diagnostics.Debug.WriteLine("Oblacnost je {0}, a vetar je {1}",query.Clouds,query.Wind);
            DateTime dt = new DateTime(1970,1,1);
            DateTime dtTemp = prediction.Date.AddDays(-1);
            double seconds = (dtTemp-dt).TotalSeconds;
            //string url = string.Format("http://api.openweathermap.org/data/2.5/onecall/timemachine?lat=60.99&lon=30.9&dt={0}&appid=06f80eb85672c812a1a6dd14d9619fc9",seconds);
            //string url = "http://api.openweathermap.org/data/2.5/forecast?q=belgrade&appid=06f80eb85672c812a1a6dd14d9619fc9";
            
            string url = "https://api.openweathermap.org/data/2.5/onecall?lat=33.44&lon=-94.04&exclude=current,minutely,daily,alerts&appid=06f80eb85672c812a1a6dd14d9619fc9";
            /*JArray dataArray;
            JToken token;
            double windSpeed;
            float clouds;
            try {
                var client = new WebClient();
                var content = client.DownloadString(url);
                dynamic data = JObject.Parse(content);
                dataArray = data.hourly;
                token = dataArray[prediction.Date.Hour];
                clouds = (float)double.Parse(token["clouds"].ToString());
                windSpeed = double.Parse(token["wind_speed"].ToString());
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Clouds is " + clouds + "wind is " + windSpeed);
                return null;
            }*/
            //var client = new WebClient();
            //var content = client.DownloadString(url);
            //dynamic data = JObject.Parse(content);

            int sunPower = 0;
            int windPower = 0;
            int hydroPower = 0;
            float renewableCoefficient = 1;
            int powerPlantsMinPower = 0;
            int renewableSources = 0;

           
            //JArray dataArray = data.hourly;
            //JToken token = dataArray[prediction.Date.Hour];
            //float clouds = (float)double.Parse(token["clouds"].ToString());
            //double windSpeed = double.Parse(token["wind_speed"].ToString());

            //string wind = data.wind.speed;
            //double windSpeed = Convert.ToDouble(wind);
            System.Diagnostics.Debug.WriteLine("Clouds is " + clouds + "wind is " + windSpeed);

            int hour = (prediction.Date.Hour + 5) % 24;
            float angle = Scale(hour, 0, 23, (float)-Math.PI+(float)0.000000000001, (float)Math.PI);
            float InsolationAngle = (float)Math.Sin(angle);

            if (InsolationAngle < 0)
            {
                InsolationAngle = 0;
            }
            foreach (PowerPlant powerPlant in powerPlants)
            {
                if (powerPlant.Type == "Solar")
                {
                    int tempSunPowr= (int)((0.1 * Imax + 0.9 * (Imax * InsolationAngle * Math.Abs(1 - (clouds / 100)))) * powerPlant.Eff * powerPlant.MaxLoad);
                    sunPower += tempSunPowr;
                    optimizedData.Add(new OptimizedData { Index = optimizedData.Count, 
                        Load = tempSunPowr, 
                        Name = powerPlant.Name, 
                        OptDataPerHour = optimizedDataPerHour, 
                        Type = powerPlant.Type });
                    
                }
            }
            foreach (PowerPlant powerPlant in powerPlants)
            {
                if (powerPlant.Type == "Wind")
                {
                    if ((1.1 * windSpeed - 2) / 10 > 1)
                    {
                        windPower += powerPlant.MaxLoad;
                        optimizedData.Add(new OptimizedData
                        {
                            Index = optimizedData.Count,
                            Load = powerPlant.MaxLoad,
                            Name = powerPlant.Name,
                            OptDataPerHour = optimizedDataPerHour,
                            Type = powerPlant.Type
                        });
                    }
                    else if ((1.1 * windSpeed - 2) / 10 > 0)
                    {
                        int tempWindPower = (int)(powerPlant.MaxLoad * ((1.1 * windSpeed - 2) / 10));
                        windPower += tempWindPower;
                        optimizedData.Add(new OptimizedData
                        {
                            Index = optimizedData.Count,
                            Load = tempWindPower,
                            Name = powerPlant.Name,
                            OptDataPerHour = optimizedDataPerHour,
                            Type = powerPlant.Type
                        });
                    }
                }
            }
            foreach (PowerPlant powerPlant in powerPlants)
            {
                if (powerPlant.Type == "Hydro")
                {
                    hydroPower += powerPlant.MaxLoad;
                    optimizedData.Add(new OptimizedData
                    {
                        Index = optimizedData.Count,
                        Load = powerPlant.MaxLoad,
                        Name = powerPlant.Name,
                        OptDataPerHour = optimizedDataPerHour,
                        Type = powerPlant.Type
                    });
                }
            }
            renewableSources = hydroPower + windPower + sunPower;
            foreach (PowerPlant powerPlant in powerPlants)
            {
                if (powerPlant.Type != "Solar" && powerPlant.Type != "Hydro" && powerPlant.Type != "Wind")
                {
                    powerPlantsMinPower += powerPlant.MinLoad;
                }
            }
            if (renewableSources > prediction.Predicted - powerPlantsMinPower)
            {
                renewableCoefficient = (prediction.Predicted - powerPlantsMinPower) / renewableSources;
            }
            foreach(OptimizedData pwrPlant in optimizedData)
            {
                pwrPlant.Load *= renewableCoefficient;
            }
            List<PowerPlant> notrenewablePowrPlants = new List<PowerPlant>();
            notrenewablePowrPlants.AddRange(powerPlants.Where(s => s.Type == "Coil").ToList());
            notrenewablePowrPlants.AddRange(powerPlants.Where(s => s.Type == "Oil").ToList());
            notrenewablePowrPlants.AddRange(powerPlants.Where(s => s.Type == "Gas").ToList());
            if (optimizationType == "CostOptimization")
            {
                optimizedData.AddRange(CostOptimization(notrenewablePowrPlants, prediction, optimizedDataPerHour));
                return optimizedData;
            }else if(optimizationType == "EmmisionOptimization")
            {
                optimizedData.AddRange(C02Optimization(notrenewablePowrPlants, prediction, optimizedDataPerHour));
                return optimizedData;
            }
            else
            {
                weightFactor = (float)0.5;
                float weightFC02 = 1 - weightFactor;
                List<OptimizedData> optimizedDataCost = new List<OptimizedData>();
                optimizedDataCost.AddRange(CostOptimization(notrenewablePowrPlants, prediction, optimizedDataPerHour));
                List<OptimizedData> optimizedDataC02 = new List<OptimizedData>();
                optimizedDataC02.AddRange(C02Optimization(notrenewablePowrPlants, prediction, optimizedDataPerHour));
                foreach(OptimizedData optData in optimizedDataCost)
                {
                    optData.Load = optData.Load * weightFactor;
                }
                foreach(OptimizedData optData in optimizedDataC02)
                {
                    optData.Load = optData.Load * weightFC02;
                }
                for(int i = 0; i < optimizedDataCost.Count; i++)
                {
                    optimizedDataCost[i].Load += optimizedDataC02[i].Load;
                }
                optimizedData.AddRange(optimizedDataCost);
                return optimizedData;
            }

        }

        public List<OptimizedData> CostOptimization(List<PowerPlant> powerPlants, Prediction prediction,OptimizedDataPerHour optimizedDataPerHour)
        {
            List<OptimizedData> optimizedData = new List<OptimizedData>();
            //List<OptimizedData>optimizedData = new List<OptimizedData>();
            var costFunction = new DoubleVector();
            foreach (PowerPlant pwrPlant in powerPlants)
            {
                switch (pwrPlant.Type)
                {
                    case "Coil":
                        costFunction.Append(-1);
                        break;
                    case "Oil":
                        costFunction.Append(-2);
                        break;
                    default:
                        costFunction.Append(-3);
                        break;
                }
            }
            var lpProblem = new LinearProgrammingProblem(costFunction);
            var allPowr = new DoubleVector();
            foreach(PowerPlant pwrPlant in powerPlants)
            {
                allPowr.Append(1);
            }
            lpProblem.AddEqualityConstraint(allPowr, 200);
            for(int i = 0; i < powerPlants.Count; i++)
            {
                var tempDoubleVector = new DoubleVector();
                for(int j = 0; j< powerPlants.Count; j++)
                {
                    if (i == j)
                    {
                        tempDoubleVector.Append(1);
                    }
                    else
                    {
                        tempDoubleVector.Append(0);
                    }
                }
                lpProblem.AddUpperBoundConstraint(tempDoubleVector, powerPlants[i].MaxLoad);
                //lpProblem.AddUpperBound(i, powerPlants[i].MaxLoad);
                lpProblem.AddLowerBound(i, powerPlants[i].MinLoad);
                //lpProblem.AddLowerBoundConstraint(tempDoubleVector, powerPlants[i].MinLoad);
                //lpProblem.AddLowerBound(i, 0.0);
            }
            var solver = new PrimalSimplexSolver();
            solver.Solve(lpProblem);
            System.Diagnostics.Debug.WriteLine("solution:" + solver.OptimalX.ToString());
            var solution = solver.OptimalX;
            for(int i = 0; i < powerPlants.Count; i++)
            {
                optimizedData.Add(new OptimizedData() { Index = i, Load = solution[i],Name = powerPlants[i].Name,Type = powerPlants[i].Type,OptDataPerHour = optimizedDataPerHour});
            }
            return optimizedData;
        }
        public List<OptimizedData> C02Optimization(List<PowerPlant> powerPlants, Prediction prediction, OptimizedDataPerHour optimizedDataPerHour)
        {
            List<OptimizedData> optimizedData = new List<OptimizedData>();
            var costFunction = new DoubleVector();
            foreach (PowerPlant pwrPlant in powerPlants)
            {
                switch (pwrPlant.Type)
                {
                    case "Coil":
                        costFunction.Append(-3);
                        break;
                    case "Oil":
                        costFunction.Append(-2);
                        break;
                    default:
                        costFunction.Append(-1);
                        break;
                }
            }
            var lpProblem = new LinearProgrammingProblem(costFunction);
            var allPowr = new DoubleVector();
            foreach (PowerPlant pwrPlant in powerPlants)
            {
                allPowr.Append(1);
            }
            lpProblem.AddEqualityConstraint(allPowr, 200);
            for (int i = 0; i < powerPlants.Count; i++)
            {
                var tempDoubleVector = new DoubleVector();
                for (int j = 0; j < powerPlants.Count; j++)
                {
                    if (i == j)
                    {
                        tempDoubleVector.Append(1);
                    }
                    else
                    {
                        tempDoubleVector.Append(0);
                    }
                }
                lpProblem.AddUpperBoundConstraint(tempDoubleVector, powerPlants[i].MaxLoad);
                //lpProblem.AddUpperBound(i, powerPlants[i].MaxLoad);
                lpProblem.AddLowerBound(i, powerPlants[i].MinLoad);
                //lpProblem.AddLowerBoundConstraint(tempDoubleVector, powerPlants[i].MinLoad);
                //lpProblem.AddLowerBound(i, 0.0);
            }
            var solver = new PrimalSimplexSolver();
            solver.Solve(lpProblem);
            System.Diagnostics.Debug.WriteLine("solution:" + solver.OptimalX.ToString());
            var solution = solver.OptimalX;
            for (int i = 0; i < powerPlants.Count; i++)
            {
                optimizedData.Add(new OptimizedData() { Index = i, Load = solution[i], Name = powerPlants[i].Name, Type = powerPlants[i].Type, OptDataPerHour = optimizedDataPerHour });
            }
            return optimizedData;

        }

        private float Scale(float value, float min, float max, float minScale, float maxScale)
        {
            float scaled = minScale + (value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }
    }
}
