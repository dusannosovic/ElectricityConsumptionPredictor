using DataBase.Controller;
using DataBase.Model;
using ExcelDataReader;
using PredictionModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ProjekatInteligentniInfSis.Controllers
{
    public class TraningController : ApiController
    {
        Traning tr = new Traning();
        // GET: Traning
        [Route("api/Traning/GetTable")]
        [HttpGet]
        public string GetTable(string dateTimeStart,string dateTimeEnd)
        {
            //Traning tr = new Traning();
            if(dateTimeStart!= null && dateTimeEnd != null)
            {

                return tr.TrainModel(dateTimeStart,dateTimeEnd);
                //return tr.TrainModel1(dateTimeStart, dateTimeEnd);
            }
            else
            {
                //return tr.TrainModel1(dateTimeStart, dateTimeEnd);
                return tr.TrainModel(null, null);
            }

            //tr.Predict();
            //return "Uspesno Treniran model";
        }
        [Route("api/Traning/Predict")]
        [HttpGet]
        public string Predict()
        {
            string a = tr.Predict();
            return a;
        }
    }
}