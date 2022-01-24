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
            if (optimizationData.PowerPlantsToOptimize.Count == 0)
            {
                return;
            }
            var doubleVector = new DoubleVector();
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
                CrudOperations.AddOptimizedDataToTable(PrepareRenewable(prediction, optimizationData,optimizedDataPerHour,clouds,windSpeed));
            }
        }
        public List<OptimizedData> PrepareRenewable(Prediction prediction, OptimizationData optimizationData,OptimizedDataPerHour optimizedDataPerHour,float clouds,double windSpeed)
        {
            List<PowerPlant> powerPlants = optimizationData.PowerPlantsToOptimize;
            string optimizationType = optimizationData.OptimizationType;
            List<OptimizedData> optimizedData = new List<OptimizedData>();
            DateTime dt = new DateTime(1970,1,1);
            DateTime dtTemp = prediction.Date.AddDays(-1);
            double seconds = (dtTemp-dt).TotalSeconds;
            float weightFactor = optimizationData.WeightFactor;

            int sunPower = 0;
            int windPower = 0;
            int hydroPower = 0;
            float renewableCoefficient = 1;
            int powerPlantsMinPower = 0;
            int powerPlantMaxPower = 0;
            int renewableSources = 0;
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
                    int tempSunPowr= (int)((0.1  + 0.9 * Math.Abs(1 - (clouds / 100)))  * powerPlant.MaxLoad*InsolationAngle);
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
                    else if (windSpeed>22)
                    {
                        int tempWindPower = 0;
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
                    else 
                    {
                        int tempWindPower = 0;
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
                    powerPlantMaxPower += powerPlant.MaxLoad;
                }
            }
            if (renewableSources > prediction.Predicted - powerPlantsMinPower)
            {
                renewableCoefficient = (prediction.Predicted - powerPlantsMinPower) / renewableSources;
                //renewableSources = (int)(renewableSources * renewableCoefficient);
                renewableSources = 0;
                foreach (OptimizedData pwrPlant in optimizedData)
                {
                    pwrPlant.Load *= renewableCoefficient;
                    renewableSources += (int)pwrPlant.Load;
                }
            }
            if (prediction.Predicted - renewableSources > powerPlantMaxPower)
            {
                return new List<OptimizedData>();
            }
            foreach(OptimizedData opt in optimizedData)
            {
                if(opt.Type == "Hydro")
                {
                    opt.Costs = (int)(opt.Load * 0.5);
                }
            }
            List<PowerPlant> notrenewablePowrPlants = new List<PowerPlant>();
            notrenewablePowrPlants.AddRange(powerPlants.Where(s => s.Type == "Coil" || s.Type == "Oil" || s.Type == "Gas").ToList());
            //notrenewablePowrPlants.AddRange(powerPlants.Where(s => s.Type == "Oil").ToList());
            //notrenewablePowrPlants.AddRange(powerPlants.Where(s => s.Type == "Gas").ToList());
            optimizedDataPerHour.LoadToOptimize = prediction.Predicted;
            if (optimizationType == "CostOptimization")
            {
                
                float avgValueofFunctionGas;
                float avgValueofFunctionOil;
                float avgValueofFunctionCoal;
                float funcsum= 0 ;
                float number = 100;

                for(int i = 0;i<100;i++)
                {
                    float scaledData = Scale(i, 0,99, 5, 10);
                    funcsum += (float)(optimizationData.Multipler * ((optimizationData.Numerator * 8) / (scaledData * optimizationData.Denominator)) * optimizationData.FuelCostOil);
                }
                
                avgValueofFunctionOil = funcsum / number;
                funcsum = 0;
                for (int i = 0; i < 100; i++)
                {
                    float scaledData = Scale(i, 0, 99, 5, 10);
                    funcsum += (float)(optimizationData.Multipler * ((optimizationData.Numerator * 8) / (scaledData * optimizationData.Denominator)) * optimizationData.FuelCostGas);
                }
                avgValueofFunctionGas = funcsum / number;
                funcsum = 0;
                for (int i = 0; i < 100; i++)
                {
                    float scaledData = Scale(i, 0, 99, 5, 10);
                    funcsum += (float)(optimizationData.Multipler * ((optimizationData.Numerator * 8) / (scaledData * optimizationData.Denominator)) * optimizationData.FuelCostCoal);
                }
                avgValueofFunctionCoal = funcsum / number;

                List<OptimizedData> optData = CostOptimization(notrenewablePowrPlants, prediction, optimizedDataPerHour, prediction.Predicted - renewableSources, optimizationData,avgValueofFunctionCoal,avgValueofFunctionGas, avgValueofFunctionOil);
                for (int i = 0; i < notrenewablePowrPlants.Count; i++)
                {
                    float temp = Scale((float)optData[i].Load, notrenewablePowrPlants[i].MinLoad, notrenewablePowrPlants[i].MaxLoad, 5, 10);
                    double C02Func =  temp/8;
                    double tempC02 = Math.Pow(C02Func,2);
                    if (optData[i].Type == "Oil")
                    {
                        optData[i].Costs = (int)((optimizationData.Multipler*((optimizationData.Numerator*8) / (temp*optimizationData.Denominator)) * optimizationData.FuelCostOil)*optData[i].Load);
                        optData[i].C02 = (int)(((0.5+tempC02) * optimizationData.C02EmissionOil)*optData[i].Load);
                    }
                    else if (optData[i].Type == "Gas")
                    {
                        optData[i].Costs = (int)((optimizationData.Multipler*((optimizationData.Numerator*8) / (temp*optimizationData.Denominator)) * optimizationData.FuelCostGas)*optData[i].Load);
                        optData[i].C02 = (int)(((0.5 + tempC02) * optimizationData.C02EmissionGas) * optData[i].Load);
                    }
                    else
                    {
                        optData[i].Costs = (int)((optimizationData.Multipler*((optimizationData.Numerator*8) / (temp*optimizationData.Denominator)) * optimizationData.FuelCostCoal)*optData[i].Load);
                        optData[i].C02 = (int)(((0.5 + tempC02) * optimizationData.C02EmissionCoal) * optData[i].Load);
                    }

                }
                optimizedData.AddRange(optData);
                return optimizedData;
            }else if(optimizationType == "EmmisionOptimization")
            {

                float avgValueofFunctionGas;
                float avgValueofFunctionOil;
                float avgValueofFunctionCoal;
                float funcsum = 0;
                float number = 100;

                for (int i = 0; i < 100; i++)
                {
                    float scaledData = Scale(i, 0, 99, 5, 10);
                    double C02Func = scaledData / 8;
                    double tempC02 = Math.Pow(C02Func, 2);
                    funcsum += (float)(((0.5 + tempC02) * optimizationData.C02EmissionOil));
                }
                
                avgValueofFunctionOil = funcsum / number;
                funcsum = 0;
                for (int i = 0; i < 100; i++)
                {
                    float scaledData = Scale(i, 0, 99, 5, 10);
                    double C02Func = scaledData / 8;
                    double tempC02 = Math.Pow(C02Func, 2);
                    funcsum += (float)(((0.5 + tempC02) * optimizationData.C02EmissionGas));
                }
                
                avgValueofFunctionGas = funcsum / number;
                funcsum = 0;
                for (int i = 0; i < 100; i++)
                {
                    float scaledData = Scale(i, 0, 99, 5, 10);
                    double C02Func = scaledData / 8;
                    double tempC02 = Math.Pow(C02Func, 2);
                    funcsum += (float)(((0.5 + tempC02) * optimizationData.C02EmissionCoal));
                }
                avgValueofFunctionCoal = funcsum / number;
                List<OptimizedData> optData = C02Optimization(notrenewablePowrPlants, prediction, optimizedDataPerHour, prediction.Predicted - renewableSources, optimizationData,avgValueofFunctionCoal,avgValueofFunctionOil, avgValueofFunctionGas);
                for (int i = 0; i < notrenewablePowrPlants.Count; i++)
                {
                    float temp = Scale((float)optData[i].Load, notrenewablePowrPlants[i].MinLoad, notrenewablePowrPlants[i].MaxLoad, 5, 10);
                    double C02Func = temp/8;
                    double tempC02 = Math.Pow(C02Func, 2);
                    if (optData[i].Type == "Oil")
                    {
                        optData[i].Costs = (int)((optimizationData.Multipler * ((optimizationData.Numerator * 8) / (temp * optimizationData.Denominator)) * optimizationData.FuelCostOil) * optData[i].Load);
                        optData[i].C02 = (int)(((0.5 + tempC02) * optimizationData.C02EmissionOil) * optData[i].Load);
                    }
                    else if (optData[i].Type == "Gas")
                    {
                        optData[i].Costs = (int)((optimizationData.Multipler * ((optimizationData.Numerator * 8) / (temp * optimizationData.Denominator)) * optimizationData.FuelCostGas) * optData[i].Load);
                        optData[i].C02 = (int)(((0.5 + tempC02) * optimizationData.C02EmissionGas) * optData[i].Load);
                    }
                    else
                    {
                        optData[i].Costs = (int)((optimizationData.Multipler * ((optimizationData.Numerator * 8) / (temp * optimizationData.Denominator)) * optimizationData.FuelCostCoal) * optData[i].Load);
                        optData[i].C02 = (int)(((0.5 + tempC02) * optimizationData.C02EmissionCoal) * optData[i].Load);
                    }

                }
                optimizedData.AddRange(optData);
                return optimizedData;
            }
            else
            {
                float avgValueofFunctionGas;
                float avgValueofFunctionOil;
                float avgValueofFunctionCoal;
                float funcsum = 0;
                float number = 100;

                for (int i = 0; i < 100; i++)
                {
                    float scaledData = Scale(i, 0, 99, 5, 10);
                    funcsum += (float)(optimizationData.Multipler * ((optimizationData.Numerator * 8) / (scaledData * optimizationData.Denominator)) * optimizationData.FuelCostOil);
                }
                
                avgValueofFunctionOil = funcsum / number;
                funcsum = 0;
                for (int i = 0; i < 100; i++)
                {
                    float scaledData = Scale(i, 0, 99, 5, 10);
                    funcsum += (float)(optimizationData.Multipler * ((optimizationData.Numerator * 8) / (scaledData * optimizationData.Denominator)) * optimizationData.FuelCostGas);
                }
                
                avgValueofFunctionGas = funcsum / number;
                funcsum = 0;
                for (int i = 0; i < 100; i++)
                {
                    float scaledData = Scale(i, 0, 99, 5, 10);
                    funcsum += (float)(optimizationData.Multipler * ((optimizationData.Numerator * 8) / (scaledData * optimizationData.Denominator)) * optimizationData.FuelCostCoal);
                }
                avgValueofFunctionCoal = funcsum / number;
                //weightFactor = (float)0.5;
                float weightFC02 = 1 - weightFactor;
                List<OptimizedData> optimizedDataCost = new List<OptimizedData>();
                optimizedDataCost.AddRange(CostOptimization(notrenewablePowrPlants, prediction, optimizedDataPerHour, prediction.Predicted-renewableSources, optimizationData,avgValueofFunctionCoal,avgValueofFunctionGas,avgValueofFunctionOil));
                funcsum = 0;
                for (int i = 0; i < 100; i++)
                {
                    float scaledData = Scale(i, 0, 99, 5, 10);
                    double C02Func = scaledData / 8;
                    double tempC02 = Math.Pow(C02Func, 2);
                    funcsum += (float)(((0.5 + tempC02) * optimizationData.C02EmissionOil));
                }
                avgValueofFunctionOil = funcsum / number;
                funcsum = 0;
                for (int i = 0; i < 100; i++)
                {
                    float scaledData = Scale(i, 0, 99, 5, 10);
                    double C02Func = scaledData / 8;
                    double tempC02 = Math.Pow(C02Func, 2);
                    funcsum += (float)(((0.5 + tempC02) * optimizationData.C02EmissionGas));
                }
                avgValueofFunctionGas = funcsum / number;
                funcsum = 0;
                for (int i = 0; i < 100; i++)
                {
                    float scaledData = Scale(i, 0, 99, 5, 10);
                    double C02Func = scaledData / 8;
                    double tempC02 = Math.Pow(C02Func, 2);
                    funcsum += (float)(((0.5 + tempC02) * optimizationData.C02EmissionCoal));
                }
                avgValueofFunctionCoal = funcsum / number;
                List<OptimizedData> optimizedDataC02 = new List<OptimizedData>();
                optimizedDataC02.AddRange(C02Optimization(notrenewablePowrPlants, prediction, optimizedDataPerHour, prediction.Predicted - renewableSources, optimizationData, avgValueofFunctionCoal,avgValueofFunctionOil,avgValueofFunctionGas));
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
                for(int i =0; i < notrenewablePowrPlants.Count; i++) {
                    float temp = Scale((float)optimizedDataCost[i].Load, notrenewablePowrPlants[i].MinLoad, notrenewablePowrPlants[i].MaxLoad, 5, 10);
                    double C02Func = temp/8;
                    double tempC02 = Math.Pow(C02Func, 2);
                    if (optimizedDataCost[i].Type=="Oil")
                    {
                        optimizedDataCost[i].Costs = (int)((optimizationData.Multipler * ((optimizationData.Numerator * 8) / (temp * optimizationData.Denominator)) * optimizationData.FuelCostOil) * optimizedDataCost[i].Load);
                        optimizedDataCost[i].C02 = (int)(((0.5 + tempC02) * optimizationData.C02EmissionOil) * optimizedDataCost[i].Load);
                    }
                    else if (optimizedDataCost[i].Type == "Gas")
                    {
                        optimizedDataCost[i].Costs = (int)((optimizationData.Multipler * ((optimizationData.Numerator * 8) / (temp * optimizationData.Denominator)) * optimizationData.FuelCostGas) * optimizedDataCost[i].Load); ;
                        optimizedDataCost[i].C02 = (int)(((0.5 + tempC02) * optimizationData.C02EmissionGas) * optimizedDataCost[i].Load);
                    }
                    else
                    {
                        optimizedDataCost[i].Costs = (int)((optimizationData.Multipler * ((optimizationData.Numerator * 8) / (temp * optimizationData.Denominator)) * optimizationData.FuelCostCoal) * optimizedDataCost[i].Load);
                        optimizedDataCost[i].C02 = (int)(((0.5 + tempC02) * optimizationData.C02EmissionCoal) * optimizedDataCost[i].Load);
                    }
                    
                }
                optimizedData.AddRange(optimizedDataCost);
                return optimizedData;
            }

        }

        public List<OptimizedData> CostOptimization(List<PowerPlant> powerPlants, Prediction prediction,OptimizedDataPerHour optimizedDataPerHour, float valueToOptimize, OptimizationData optimizationData, float coalAvg, float gasAvg, float oilAvg)
        {
            List<OptimizedData> optimizedData = new List<OptimizedData>();
            //List<OptimizedData>optimizedData = new List<OptimizedData>();
            var costFunction = new DoubleVector();
            foreach (PowerPlant pwrPlant in powerPlants)
            {
                switch (pwrPlant.Type)
                {
                    case "Gas":
                        costFunction.Append(-gasAvg);
                        break;
                    case "Oil":
                        costFunction.Append(-oilAvg);
                        break;
                    default:
                        costFunction.Append(-coalAvg);
                        break;
                }
            }
            var lpProblem = new LinearProgrammingProblem(costFunction);
            var allPowr = new DoubleVector();
            foreach(PowerPlant pwrPlant in powerPlants)
            {
                allPowr.Append(1);
            }
            lpProblem.AddEqualityConstraint(allPowr, valueToOptimize);
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
            //System.Diagnostics.Debug.WriteLine("solution:" + solver.OptimalX.ToString());
            var solution = solver.OptimalX;
            for(int i = 0; i < powerPlants.Count; i++)
            {
                optimizedData.Add(new OptimizedData() { Index = i, Load = solution[i],Name = powerPlants[i].Name,Type = powerPlants[i].Type,OptDataPerHour = optimizedDataPerHour});
            }
            return optimizedData;
        }
        public List<OptimizedData> C02Optimization(List<PowerPlant> powerPlants, Prediction prediction, OptimizedDataPerHour optimizedDataPerHour, float valueToOptimize, OptimizationData optimizationData, float coalAvg, float oilAvg, float gasAvg)
        {
            List<OptimizedData> optimizedData = new List<OptimizedData>();
            var costFunction = new DoubleVector();
            foreach (PowerPlant pwrPlant in powerPlants)
            {
                switch (pwrPlant.Type)
                {
                    case "Gas":
                        costFunction.Append(-gasAvg);
                        break;
                    case "Oil":
                        costFunction.Append(-oilAvg);
                        break;
                    default:
                        costFunction.Append(-coalAvg);
                        break;
                }
            }
            var lpProblem = new LinearProgrammingProblem(costFunction);
            var allPowr = new DoubleVector();
            foreach (PowerPlant pwrPlant in powerPlants)
            {
                allPowr.Append(1);
            }
            lpProblem.AddEqualityConstraint(allPowr, valueToOptimize);
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
            //System.Diagnostics.Debug.WriteLine("solution:" + solver.OptimalX.ToString());
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
