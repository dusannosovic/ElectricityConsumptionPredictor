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
        public static DataTable CreateDataTableFromObjects<Weather>(List<Weather> weathers, string name = null)
        {
            var myType = typeof(Weather);
            if (name == null)
            {
                name = myType.Name;
            }
            DataTable dt = new DataTable(name);
            foreach (PropertyInfo info in myType.GetProperties())
            {
                if (info.Name != "Id")
                {
                    dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
                }
            }
            foreach (var item in weathers)
            {
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo info in myType.GetProperties())
                {
                    if (info.Name != "Id")
                    {
                        dr[info.Name] = info.GetValue(item);
                    }
                }
            }
            return dt;
        }
        public void TrainModel()
        {
            List<Weather> weathers = CrudOperations.GetAllWeather();
            List<Series> series = new List<Series>();
            float[,] a = new float[weathers.Count, 14];
            for(int i = 0; i < weathers.Count; i++)
            {
                a[i, 0] = (float)(weathers[i].LocalTime.Value.Year) ;
                a[i, 1] = (float)(weathers[i].LocalTime.Value.Month);
                a[i, 2] = (float)weathers[i].LocalTime.Value.Day;
                a[i, 3] = (float)weathers[i].LocalTime.Value.Hour;
                a[i, 4] = (float)weathers[i].LocalTime.Value.Minute;
                a[i, 5] = (float)weathers[i].Temperature;
                a[i, 6] = (float)weathers[i].APressure;
                a[i, 7] = (float)weathers[i].Pressure;
                a[i, 8] = (float)weathers[i].PTencdency;
                a[i, 9] = (float)weathers[i].Humidity;
                a[i, 10] = (float)weathers[i].WindSpeed;
                a[i, 11] = (float)weathers[i].Clouds;
                a[i, 12] = (float)weathers[i].HVisibility;
                a[i, 13] = (float)weathers[i].DTemperature;
            }
            var ar = Numpy.np.array(a);
            var trainx = Numpy.np.reshape(ar,(ar.shape[0], 1,ar.shape[1]));

            var model = new Sequential();
            
            model.Add(new Dense(100, 784, "relu"));

            model.Add(new Dense(100, 784, "relu"));
            model.Compile(optimizer: "sgd", loss: "categorical_crossentropy",metrics: new string[] { "accuracy" });
            
        }
    }
}
