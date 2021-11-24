using DataBase.Controller;
using DataBase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ProjekatInteligentniInfSis.Controllers
{
    public class HomeController : ApiController
    {
        [Route("api/Home/GetTicket")]
        [HttpGet]
        public string GetTicket()
        {
            return "dusan";
        }
       /* public ActionResult Add()
        {

        }*/
    }
}
