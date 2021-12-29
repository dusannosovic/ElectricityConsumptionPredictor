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
            List<Prediction> predictions = CrudOperations.GetPredictions();
            List<PowerPlant> powerPlants = CrudOperations.GetPowerPlants();
            foreach (Prediction prediction in predictions)
            {
                PrepareRenewable(prediction, powerPlants);
            }
        }
        public void PrepareRenewable(Prediction prediction, List<PowerPlant> powerPlants)
        {
            //var openWeatherAPI = new OpenWeatherAPI.OpenWeatherApiClient("06f80eb85672c812a1a6dd14d9619fc9");
            //var query = openWeatherAPI.Query("chicago");
            // System.Diagnostics.Debug.WriteLine("Oblacnost je {0}, a vetar je {1}",query.Clouds,query.Wind);
            DateTime dt = new DateTime(1970,1,1);
            DateTime dtTemp = prediction.Date.AddDays(-1);
            double seconds = (dtTemp-dt).TotalSeconds;
            //string url = string.Format("http://api.openweathermap.org/data/2.5/onecall/timemachine?lat=60.99&lon=30.9&dt={0}&appid=06f80eb85672c812a1a6dd14d9619fc9",seconds);
            //string url = "http://api.openweathermap.org/data/2.5/forecast?q=belgrade&appid=06f80eb85672c812a1a6dd14d9619fc9";
            
            string url = "https://api.openweathermap.org/data/2.5/onecall?lat=33.44&lon=-94.04&exclude=current,minutely,daily,alerts&appid=06f80eb85672c812a1a6dd14d9619fc9";
            JArray dataArray;
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
                return;
            }
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
                    sunPower += (int)((0.1 * Imax + 0.9 * (Imax * InsolationAngle * Math.Abs(1 - (clouds / 100)))) * powerPlant.Eff * powerPlant.MaxLoad);
                }
            }
            foreach (PowerPlant powerPlant in powerPlants)
            {
                if (powerPlant.Type == "Wind")
                {
                    if ((1.1 * windSpeed - 2) / 10 > 1)
                    {
                        windPower += powerPlant.MaxLoad;
                    }
                    else if ((1.1 * windSpeed - 2) / 10 > 0)
                    {
                        windPower += (int)(powerPlant.MaxLoad * ((1.1 * windSpeed - 2) / 10));
                    }
                }
            }
            foreach (PowerPlant powerPlant in powerPlants)
            {
                if (powerPlant.Type == "Hydro")
                {
                    hydroPower += powerPlant.MaxLoad;
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

            var revenue = new DoubleVector(new double[] { 1, 2, 3 });
            var lpProblem = new LinearProgrammingProblem(revenue);
            CostOptimization(new List<PowerPlant>());

            var r1 = new DoubleVector(1, 3, 4, 5, 6, 7, 8, 9);
            
        }

        public void CostOptimization(List<PowerPlant> powerPlants)
        {

            System.Diagnostics.Debug.WriteLine("Trening start {0}", DateTime.Now);
        }
        public void C02Optimization()
        {

        }

        private float Scale(float value, float min, float max, float minScale, float maxScale)
        {
            float scaled = minScale + (value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }
    }
}
