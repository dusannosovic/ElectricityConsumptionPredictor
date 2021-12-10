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
using Numpy;
using Numpy.Models;
using PandasNet;
using Tensorflow.NumPy;

namespace PredictionModel
{
    public class Traning
    {
        public double trainPercentage = 0.85;
        public void TrainModel()
        {
            List<Weather> weathers = CrudOperations.GetAllWeather();
            int NumberOfWeather = CrudOperations.GetAllWeather().Count;
            int numberOfRowForTraining = (int)(trainPercentage * NumberOfWeather);
            float[,] TraningX = RowDataX(0, numberOfRowForTraining);
            float[] TraningY = RowDataY(0, numberOfRowForTraining);
            //float[,] TestX = RowDataX(numberOfRowForTraining, NumberOfWeather);
            //float[] TestY = RowDataY()
            var ar = Numpy.np.array(TraningX);
            var trainy = Numpy.np.array(TraningY);
            var trainx = Numpy.np.reshape(ar, (ar.shape[0], 1, ar.shape[1]));
            var model = new Sequential();
            
            model.Add(new Dense(10,null,"relu"));

            model.Add(new Dense(10, null, "relu"));
            model.Compile(optimizer: "sgd", loss: "categorical_crossentropy", metrics: new string[] { "accuracy" });
            model.Fit(trainx, trainy, 2, 100, 1);
            model.Save("C:/Users/Dusan/Desktop/Modeli");
        }
        public float[,] RowDataX(int first,int last)
        {
            float[,] a = new float[Math.Abs(last-first),14];
            List<Weather> weathers = CrudOperations.GetAllWeather();
            for (int i = first; i < last; i++)
            {
                a[i-first, 0] = (float)(weathers[i].LocalTime.Year);
                a[i-first, 1] = (float)(weathers[i].LocalTime.Month);
                a[i-first, 2] = (float)weathers[i].LocalTime.Day;
                a[i-first, 3] = (float)weathers[i].LocalTime.Hour;
                a[i-first, 4] = (float)weathers[i].LocalTime.Minute;
                a[i-first, 5] = (float)weathers[i].Temperature;
                a[i-first, 6] = (float)weathers[i].APressure;
                a[i-first, 7] = (float)weathers[i].Pressure;
                a[i-first, 8] = (float)weathers[i].PTencdency;
                a[i-first, 9] = (float)weathers[i].Humidity;
                a[i-first, 10] = (float)weathers[i].WindSpeed;
                a[i-first, 11] = (float)weathers[i].Clouds;
                a[i-first, 12] = (float)weathers[i].HVisibility;
                a[i-first, 13] = (float)weathers[i].DTemperature;
            }
            return a;
        }
        public float[] RowDataY(int first,int last)
        {
            float[] a = new float[Math.Abs(last - first)];
            List<Weather> weathers = CrudOperations.GetAllWeather();
            for(int i = first; i < last; i++)
            {
                a[i - first] = (float) weathers[i].Load;
            }
            return a;
        }
    }
}
