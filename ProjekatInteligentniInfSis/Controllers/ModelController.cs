using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjekatInteligentniInfSis.Controllers
{
    public class ModelController : Controller
    {
        public ActionResult Index()
        {
            return View("Model");
        }
    }
}