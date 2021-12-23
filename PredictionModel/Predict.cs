using DataBase.Controller;
using DataBase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredictionModel
{
    public class Predict
    {
        List<Weather> listOfWeathersToPredict;
        public float[,] RowDataX(List<Weather> weatherList)
        {
            float[,] a = new float[weatherList.Count, 13];
            listOfWeathersToPredict = weatherList;
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
            for (int i = 0; i < weatherList.Count; i++)
            {

                a[i, 0] = Scale(weatherList[i].LocalTime.Year, minYear, maxYear, 0, 1);
                a[i, 1] = Scale(weatherList[i].LocalTime.Month, minMonth, maxMonth, 0, 1);
                a[i, 2] = Scale((float)weatherList[i].LocalTime.DayOfWeek, minDay, maxDay, 0, 1);
                a[i, 3] = Scale(weatherList[i].LocalTime.Hour, minHour, maxHour, 0, 1);
                a[i, 4] = Scale((float)weatherList[i].Temperature, minTemperature, maxTemperature, 0, 1);
                a[i, 5] = Scale((float)weatherList[i].APressure, minAPressure, maxAPressure, 0, 1);
                a[i, 6] = Scale((float)weatherList[i].Pressure, minPressure, maxPressure, 0, 1);
                a[i, 7] = Scale((float)Convert.ToDouble(weatherList[i].PTencdency), minPTendency, maxPTendency, 0, 1);
                a[i, 8] = Scale((float)weatherList[i].Humidity, minHumidity, maxHumidity, 0, 1);
                a[i, 9] = Scale((float)weatherList[i].WindSpeed, minWindSpeed, maxWindSpeed, 0, 1);
                a[i, 10] = (float)weatherList[i].Clouds;
                a[i, 11] = Scale((float)weatherList[i].HVisibility, minHvisibility, maxHvisibility, 0, 1);
                a[i, 12] = Scale((float)weatherList[i].DTemperature, minDTemperature, maxDTemperature, 0, 1);
            }
            return a;
        }
        public void Prediction(List<Weather> weathers)
        {
            float[,] TestX = RowDataX(weathers);
            var ar = Numpy.np.array(TestX);
            var testx = Numpy.np.reshape(ar, (ar.shape[0], 1, ar.shape[1]));
            var model = Keras.Models.Model.LoadModel("C:/Users/Dusan/Desktop/Modeli");
            if (model != null)
            {
                var predict1 = model.Predict(testx);
                List<Prediction> predictions = LoadInverse(predict1, weathers);
                CrudOperations.AddPrediction(predictions);
            }

        }
        public List<Prediction> LoadInverse(Numpy.NDarray array, List<Weather> weathers)
        {
            var testPredict = Numpy.np.reshape(array, (array.shape[0]));
            float maxLoad = (float)Convert.ToDouble(CrudOperations.GetAllWeather().Select(s => s.Load).Max());
            float minLoad = (float)Convert.ToDouble(CrudOperations.GetAllWeather().Select(s => s.Load).Min());
            float test;
            Prediction temp;
            List<Prediction> predictions = new List<Prediction>();
            for(int i = 0;i<testPredict.size; i++)
            {
                test = (float)testPredict[i] * (maxLoad - minLoad) + minLoad;
                temp = new Prediction() { Date = weathers[i].LocalTime, Predicted = (int)test };
                predictions.Add(temp);
            }
            return predictions;
        }

        private float Scale(float value, float min, float max, float minScale, float maxScale)
        {
            float scaled = minScale + (value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }

    }

}

