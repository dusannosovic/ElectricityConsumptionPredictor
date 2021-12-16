using DataBase.Controller;
using DataBase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ProjekatInteligentniInfSis.Controllers
{
    public class PredictionController : ApiController
    {
        // GET: Prediction
        [Route("api/Prediction/Predict")]
        [HttpGet]
        public string Predict(DateTime year ,long numberofdays)
        {
            List<Weather> weathers = new List<Weather>();
            for(int i = 0; i < numberofdays; i++)
            {
                year.AddDays(i);
                weathers.AddRange(CrudOperations.GetAllWeather().Where(s => s.LocalTime.Date == year.Date));
            }

            return "Uspesno";
        }
    }
}