using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DataBase.Controller;
using DataBase.Model;

namespace ProjekatInteligentniInfSis.Controllers
{
    public class OptimizationController : ApiController
    {
        [Route("api/Optimization/AddPowerPlant")]
        [HttpPost]
        public string AddPowerPlant(PowerPlant powerPlant)
        {
           var request = HttpContext.Current.Request;
            CrudOperations.AddPowerPlant(powerPlant);
            return "ok";
        }
        [Route("api/Optimization/GetPowerPlants")]
        [HttpGet]
        public List<PowerPlant> GetPowerPlants()
        {
            return CrudOperations.GetPowerPlants();
        }

    }
}