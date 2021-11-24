using DataBase.Controller;
using DataBase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjekatInteligentniInfSis.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            List<TableEntity> tables = new List<TableEntity>();
            tables.Add(new TableEntity("35", DateTime.Now));
            tables.Add(new TableEntity("48", DateTime.Now));
            CrudOperations.AddEntites(tables);
            

            return View();
        }
       /* public ActionResult Add()
        {

        }*/
    }
}
