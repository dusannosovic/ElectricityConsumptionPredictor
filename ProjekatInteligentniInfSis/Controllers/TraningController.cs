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
        // GET: Traning
        [Route("api/Traning/GetTable")]
        [HttpGet]
        public string GetTable()
        {
            DataTable dt = Traning.CreateDataTableFromObjects<Weather>(CrudOperations.GetAllWeather());

            return "uspesno";
        }
    }
}