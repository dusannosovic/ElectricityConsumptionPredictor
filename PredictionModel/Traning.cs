using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataBase.Controller;
using DataBase.Model;
using Keras.Layers;
using Keras.Models;
using Keras.Callbacks;
//using Numpy;
//using Numpy.Models;
//using PandasNet;
//using Tensorflow.NumPy;

namespace PredictionModel
{
    public class Traning
    {
        public double trainPercentage = 0.95;

        public string TrainModel(string dateTimeStart, string dateTimeEnd)
        {
            
            List<Weather> weathers = CrudOperations.GetAllWeather();
            int NumberOfWeather = CrudOperations.GetAllWeather().Count;
            int numberOfRowForTraining;
            int startRowForTraning;
            if(dateTimeStart==null && dateTimeEnd == null)
            {
                startRowForTraning = 0;
                numberOfRowForTraining = (int)(trainPercentage * NumberOfWeather);
            }
            else
            {
                try
                {
                    startRowForTraning = GetStartIndex(dateTimeStart);
                    numberOfRowForTraining = GetEndIndex(dateTimeEnd);
                    if (startRowForTraning > numberOfRowForTraining)
                    {
                        return "Brojevi se ne poklapaku";
                    }
                }
                catch
                {
                    return "Nepostojeci datumi";
                }
            }
            float[,] TraningX = RowDataX(startRowForTraning, numberOfRowForTraining);
            float[] TraningY = RowDataY(startRowForTraning, numberOfRowForTraining);
            //float[,] TestX = RowDataX(numberOfRowForTraining, NumberOfWeather);
            //float[] TestY = RowDataY()
            var ar = Numpy.np.array(TraningX);
            var trainy = Numpy.np.array(TraningY);
            var trainx = Numpy.np.reshape(ar, (ar.shape[0], 1, ar.shape[1]));
            //var trainx = ar;
            var model = new Sequential();
            var trainY = Numpy.np.reshape(trainy, (trainy.shape[0], 1, 1));
            var shape = new Keras.Shape(1, 13);

            model.Add(new Dense(8, activation: "sigmoid", kernel_initializer: "random_normal", input_shape: shape));
            model.Add(new Dense(6, activation: "sigmoid", kernel_initializer: "random_normal"));
            model.Add(new Dense(6, activation: "sigmoid", kernel_initializer: "random_normal"));
            model.Add(new Dense(6, activation: "sigmoid", kernel_initializer: "random_normal"));
            model.Add(new Dense(1, kernel_initializer: "random_normal"));
            model.Compile(optimizer: "adam", loss: "mean_squared_error", metrics: new string[] { "mean_squared_error" });
            System.Diagnostics.Debug.WriteLine("Trening start {0}", DateTime.Now);
            var csv_logger = new CSVLogger("C:/Users/Dusan/source/repos/ElectricityConsumptionPredictor/log.csv", ";", true);
            Callback[] callbacks = new Callback[] { csv_logger };
            model.Fit(trainx, trainY, 4, 400, 2, callbacks);
            System.Diagnostics.Debug.WriteLine("Trening stop {0}", DateTime.Now);
            model.Save("C:/Users/Dusan/Desktop/Modeli");
            return "Uspesno treniran model";
        }
        public string Predict()
        {
            List<Weather> weathers = CrudOperations.GetAllWeather();
            int NumberOfWeather = CrudOperations.GetAllWeather().Count;
            int numberOfRowForTraining = (int)(trainPercentage * NumberOfWeather);
            float[,] TestX = RowDataX(numberOfRowForTraining, NumberOfWeather);
            float[] TestY = RowDataY(numberOfRowForTraining, NumberOfWeather);
            var ar = Numpy.np.array(TestX);
            var ac = Numpy.np.array(TestY);
            var testx = Numpy.np.reshape(ar, (ar.shape[0],1, ar.shape[1]));
            var testy = Numpy.np.reshape(ac, (ac.shape[0]));

            var model = Keras.Models.Model.LoadModel("C:/Users/Dusan/Desktop/Modeli");
            var predict1 = model.Predict(testx);
            var predict = model.Evaluate(testx, testy);
           //System.Diagnostics.Debug.WriteLine("accuracy {0}", predict.)
            return LoadInverse(predict1);
        }


        public float[,] RowDataX(int first, int last)
        {
            float[,] a = new float[Math.Abs(last - first), 13];
            List<Weather> weathers = CrudOperations.GetAllWeather();
            float maxYear = (float)Convert.ToDouble(weathers.Select(s => s.LocalTime.Year).Max());
            float minYear = (float)Convert.ToDouble(weathers.Select(s => s.LocalTime.Year).Min());
            float maxMonth = (float)Convert.ToDouble(weathers.Select(s => s.LocalTime.Month).Max());
            float minMonth = (float)Convert.ToDouble(weathers.Select(s => s.LocalTime.Month).Min());
            float maxDay = (float)Convert.ToDouble(weathers.Select(s => s.LocalTime.DayOfWeek).Max());
            float minDay = (float)Convert.ToDouble(weathers.Select(s => s.LocalTime.DayOfWeek).Min());
            float maxHour = (float)Convert.ToDouble(weathers.Select(s => s.LocalTime.Hour).Max());
            float minHour = (float)Convert.ToDouble(weathers.Select(s => s.LocalTime.Hour).Min());
            float maxMinute = (float)Convert.ToDouble(weathers.Select(s => s.LocalTime.Minute).Max());
            float minMinute = (float)Convert.ToDouble(weathers.Select(s => s.LocalTime.Minute).Min());
            float maxTemperature = (float)Convert.ToDouble(weathers.Select(s => s.Temperature).Max());
            float minTemperature = (float)Convert.ToDouble(weathers.Select(s => s.Temperature).Min());
            float maxAPressure = (float)Convert.ToDouble(weathers.Select(s => s.APressure).Max());
            float minAPressure = (float)Convert.ToDouble(weathers.Select(s => s.APressure).Min());
            float maxPressure = (float)Convert.ToDouble(weathers.Select(s => s.Pressure).Max());
            float minPressure = (float)Convert.ToDouble(weathers.Select(s => s.Pressure).Min());
            float maxPTendency = (float)Convert.ToDouble(weathers.Select(s => s.PTencdency).Max());
            float minPTendency = (float)Convert.ToDouble(weathers.Select(s => s.PTencdency).Min());
            float maxHumidity = (float)Convert.ToDouble(weathers.Select(s => s.Humidity).Max());
            float minHumidity = (float)Convert.ToDouble(weathers.Select(s => s.Humidity).Min());
            float maxWindSpeed = (float)Convert.ToDouble(weathers.Select(s => s.WindSpeed).Max());
            float minWindSpeed = (float)Convert.ToDouble(weathers.Select(s => s.WindSpeed).Min());
            //double maxClouds = Convert.ToDouble(weathers.Select(s => s.Clouds).Max());
            //double minClouds = Convert.ToDouble(weathers.Select(s => s.Clouds).Min());
            float maxHvisibility = (float)Convert.ToDouble(weathers.Select(s => s.HVisibility).Max());
            float minHvisibility = (float)Convert.ToDouble(weathers.Select(s => s.HVisibility).Min());
            float maxDTemperature = (float)Convert.ToDouble(weathers.Select(s => s.DTemperature).Max());
            float minDTemperature = (float)Convert.ToDouble(weathers.Select(s => s.DTemperature).Min());
            for (int i = first; i < last; i++)
            {

                a[i - first, 0] = Scale(weathers[i].LocalTime.Year, minYear, maxYear, 0, 1);
                a[i - first, 1] = Scale(weathers[i].LocalTime.Month, minMonth, maxMonth, 0, 1);
                a[i - first, 2] = Scale((float)weathers[i].LocalTime.DayOfWeek, minDay, maxDay, 0, 1);
                a[i - first, 3] = Scale(weathers[i].LocalTime.Hour, minHour, maxHour, 0, 1);
                a[i - first, 4] = Scale((float)weathers[i].Temperature, minTemperature, maxTemperature, 0, 1);
                a[i - first, 5] = Scale((float)weathers[i].APressure, minAPressure, maxAPressure, 0, 1);
                a[i - first, 6] = Scale((float)weathers[i].Pressure, minPressure, maxPressure, 0, 1);
                a[i - first, 7] = Scale((float)Convert.ToDouble(weathers[i].PTencdency), minPTendency, maxPTendency, 0, 1);
                a[i - first, 8] = Scale((float)weathers[i].Humidity, minHumidity, maxHumidity, 0, 1);
                a[i - first, 9] = Scale((float)weathers[i].WindSpeed, minWindSpeed, maxWindSpeed, 0, 1);
                a[i - first, 10] = (float)weathers[i].Clouds;
                a[i - first, 11] = Scale((float)weathers[i].HVisibility, minHvisibility, maxHvisibility, 0, 1);
                a[i - first, 12] = Scale((float)weathers[i].DTemperature, minDTemperature, maxDTemperature, 0, 1);
            }
            return a;
        }
        public float[] RowDataY(int first, int last)
        {
            float[] a = new float[Math.Abs(last - first)];
            List<Weather> weathers = CrudOperations.GetAllWeather();
            float maxLoad = (float)Convert.ToDouble(weathers.Select(s => s.Load).Max());
            float minLoad = (float)Convert.ToDouble(weathers.Select(s => s.Load).Min());
            for (int i = first; i < last; i++)
            {
                a[i - first] = Scale((float)weathers[i].Load, minLoad, maxLoad, 0, 1);
            }
            return a;
        }
        public string LoadInverse(Numpy.NDarray array)
        {
            List<Tuple<float, float>> tuple = new List<Tuple<float, float>>();
            var testPredict = Numpy.np.reshape(array, (array.shape[0]));
            List<Weather> weathers = CrudOperations.GetAllWeather();
            int NumberOfWeather = CrudOperations.GetAllWeather().Count;
            int numberOfRowForTraining = (int)(trainPercentage * NumberOfWeather);
            float maxLoad = (float)Convert.ToDouble(CrudOperations.GetAllWeather().Select(s => s.Load).Max());
            float minLoad = (float)Convert.ToDouble(CrudOperations.GetAllWeather().Select(s => s.Load).Min());
            //float[] test = new float[testPredict.size];
            float test;
            float test2;
            float[] predict = RowDataY(numberOfRowForTraining, NumberOfWeather);
            for (int i = 0; i < testPredict.size; i++)
            {
                test = (float)testPredict[i] * (maxLoad - minLoad) + minLoad;
                test2 = (float)predict[i] * (maxLoad - minLoad) + minLoad;
                tuple.Add(new Tuple<float, float>(test, test2));
            }
            float sum = 0;
            foreach (Tuple<float, float> tuple1 in tuple)
            {
                sum += Math.Abs((tuple1.Item2 - tuple1.Item1) / tuple1.Item2);
            }
            System.Diagnostics.Debug.WriteLine("Absolut {0}%", (sum / tuple.Count) * 100);
            Double sumRmse = 0;
            foreach (Tuple<float, float> tuple1 in tuple)
            {
                sumRmse += Math.Pow((tuple1.Item2 - tuple1.Item1) / tuple1.Item2, 2);
            }
            System.Diagnostics.Debug.WriteLine("Squared {0}%", Math.Sqrt(sumRmse / tuple.Count) * 100);
            //float a = (float)testPredict[0];

            //string a =  float.Parse(testPredict.str[0]);
            //float[] ar = Numpy.np.List(testPredict);
            //testPredict.
            return String.Format("Absolut {0}%", (sum / tuple.Count) * 100) + string.Format("Squared {0}%", Math.Sqrt(sumRmse / tuple.Count) * 100);
        }
        public float getMaxLoad()
        {

            return (float)Convert.ToDouble(CrudOperations.GetAllWeather().Select(s => s.Load).Max());
        }
        public float getMinLoad()
        {
            return (float)Convert.ToDouble(CrudOperations.GetAllWeather().Select(s => s.Load).Max());
        }
        private float Scale(float value, float min, float max, float minScale, float maxScale)
        {
            float scaled = minScale + (value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }
        private int GetStartIndex(string starteDateTime)
        {
            List<Weather> weathers = CrudOperations.GetAllWeather();
            DateTime start = Convert.ToDateTime(starteDateTime);
            return weathers.IndexOf(weathers.Where(s=>s.LocalTime.Date==start.Date).First());
        }
        private int GetEndIndex(string endDateTime)
        {
            List<Weather> weathers = CrudOperations.GetAllWeather();
            DateTime start = Convert.ToDateTime(endDateTime);
            return weathers.IndexOf(weathers.Where(s => s.LocalTime.Date == start.Date).Last());
        }
    }
}
