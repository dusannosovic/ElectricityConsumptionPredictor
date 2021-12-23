using DataBase.Controller;
using DataBase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using PredictionModel;
using System.IO;

namespace ProjekatInteligentniInfSis.Controllers
{
    public class PredictionController : ApiController
    {
        // GET: Prediction
        [Route("api/Prediction/Predict")]
        [HttpGet]
        public string Predict(DateTime year ,long numberofdays)
        {
            Predict predictValues = new Predict();
            List<Weather> weathers = new List<Weather>();
            DateTime day;
            for(int i = 0; i < numberofdays; i++)
            {
                day = year.AddDays(i);
                weathers.AddRange(CrudOperations.GetAllWeather().Where(s => s.LocalTime.Date == day.Date));
            }
            predictValues.Prediction(weathers);

            return "Uspesno";
        }
        [Route("api/Prediction/GetPredictedValues")]
        [HttpGet]
        public List<Prediction> GetPredictedValues()
        {
            return CrudOperations.GetPredictions();
        }
        [Route("api/Prediction/WriteToCsv")]
        [HttpGet]
        public string WriteToCsv()
        {
            if (CrudOperations.GetPredictions().Count > 0)
            {
                StreamWriter sw = new StreamWriter("C:/Users/Dusan/source/repos/ElectricityConsumptionPredictor/predictions.csv", false);
                sw.Write("Time");
                sw.Write(",");
                sw.Write("Load");
                sw.Write(sw.NewLine);
                foreach (Prediction prediction in CrudOperations.GetPredictions())
                {
                    sw.Write(prediction.Date.ToString());
                    sw.Write(",");
                    sw.Write(prediction.Predicted.ToString());
                    sw.Write(sw.NewLine);
                }
                sw.Close();
                return "Uspesno upisano u csv";
            }
            else
            {
                return "Ne mozete upisati u Csv dokument";
            }

        }
    }
}