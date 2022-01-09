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
            if (weathers.Count > 0)
            {
                predictValues.Prediction(weathers);
                return "Uspesno previdjanje";
            }
            return "Neuspesno predvidjanje";
            
        }
        [Route("api/Prediction/GetPredictedValues")]
        [HttpGet]
        public List<Prediction> GetPredictedValues(DateTime dateStart, DateTime dateEnd)
        {
            return CrudOperations.GetPredictions().Where(s=>s.Date>=dateStart && s.Date<dateEnd.AddDays(1)).ToList();
        }
        [Route("api/Prediction/WriteToCsv")]
        [HttpPost]
        public string WriteToCsv(List<Prediction> predictions)
        {
            Predict predict = new Predict();
            predict.WriteToCsv(predictions);
            return "Uspesno upisano u csv Dokument";
        }
        [Route("api/Prediction/DeleteAllRecords")]
        [HttpDelete]
        public void DeleteAllRecords()
        {
            CrudOperations.DeletePredictionTable();
        }
    }
}