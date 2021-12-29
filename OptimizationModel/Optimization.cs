using System;
using System.Collections.Generic;
using System.Text;
using DataBase.Controller;
using DataBase.Model;
using System.Net;
using Newtonsoft.Json.Linq;
using Extreme.Mathematics.Optimization;
using Extreme.Mathematics;


namespace OptimizationModel
{
    public class Optimization
    {
        public const double Imax = 0.001;
        public void MakeOptimization(OptimizationData optimizationData)
        {
            List<Prediction> predictions =  CrudOperations.GetPredictions();
            List<PowerPlant> powerPlants = CrudOperations.GetPowerPlants();
            foreach(Prediction prediction in predictions)
            {
                PrepareRenewable(prediction,powerPlants);
            }
        }
        public void PrepareRenewable(Prediction prediction,List<PowerPlant>powerPlants)
        {
            //var openWeatherAPI = new OpenWeatherAPI.OpenWeatherApiClient("06f80eb85672c812a1a6dd14d9619fc9");
            //var query = openWeatherAPI.Query("chicago");
            // System.Diagnostics.Debug.WriteLine("Oblacnost je {0}, a vetar je {1}",query.Clouds,query.Wind);
            string url = "http://api.openweathermap.org/data/2.5/weather?q=belgrade&appid=06f80eb85672c812a1a6dd14d9619fc9";
            var client = new WebClient();
            var content = client.DownloadString(url);
            dynamic data = JObject.Parse(content);
            int sunPower = 0;
            int windPower = 0;
            int hydroPower = 0;
            float renewableCoefficient = 1;
            int powerPlantsMinPower = 0;
            int renewableSources = 0;
            string clouds = data.clouds.all;
            string wind = data.wind.speed;
            double windSpeed = Convert.ToDouble(wind);
            System.Diagnostics.Debug.WriteLine("Clouds is "+clouds+"wind is "+wind);

            int hour = (prediction.Date.Hour + 5) % 24;
            float InsolationAngle = (float)Math.Sin(Scale(hour, 0, 23, -180, 180));

            if ( InsolationAngle< 0)
            {
                InsolationAngle = 0;
            }
            foreach (PowerPlant powerPlant in powerPlants) {
                if (powerPlant.Type=="Solar") {
                    sunPower += (int)((0.1 * Imax + 0.9 * (Imax * InsolationAngle * Math.Abs(1-(float.Parse(clouds)/100)))) * powerPlant.Eff * powerPlant.MaxLoad);
                }
            }
            foreach(PowerPlant powerPlant in powerPlants)
            {
                if (powerPlant.Type == "Wind")
                {
                    if ((1.1 * windSpeed - 2)/10 >1) {
                        windPower += powerPlant.MaxLoad;
                    }
                    else if((1.1 * windSpeed - 2) / 10 > 0)
                    {
                        windPower += (int)(powerPlant.MaxLoad * ((1.1 * windSpeed - 2) / 10));
                    }
                }
            }
            foreach(PowerPlant powerPlant in powerPlants)
            {
                if(powerPlant.Type == "Hydro")
                {
                    hydroPower += powerPlant.MaxLoad;
                }
            }
            renewableSources = hydroPower + windPower + sunPower; 
            foreach(PowerPlant powerPlant in powerPlants)
            {
                if(powerPlant.Type!="Solar"&& powerPlant.Type != "Hydro"&& powerPlant.Type != "Wind")
                {
                    powerPlantsMinPower += powerPlant.MinLoad;
                }
            }
            if (renewableSources > prediction.Predicted - powerPlantsMinPower)
            {
                renewableCoefficient = (prediction.Predicted - powerPlantsMinPower) / renewableSources;
            }

            CostOptimization(new List<PowerPlant>());
            var c = Vector.Create(1, 2);
        }

        public void CostOptimization(List<PowerPlant> powerPlants)
        {
            
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
