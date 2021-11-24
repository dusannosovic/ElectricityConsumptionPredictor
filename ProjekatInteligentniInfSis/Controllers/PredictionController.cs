using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjekatInteligentniInfSis.Controllers
{
    public class PredictionController : Controller
    {
        // GET: Prediction
        public ActionResult Index()
        {
            return View("Prediction");
        }
    }
}