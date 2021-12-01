using DataBase.Controller;
using DataBase.Model;
using ExcelDataReader;
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
    public class HomeController : ApiController
    {
        [Route("api/Home/GetTicket")]
        [HttpGet]
        public string GetTicket()
        {
            return "dusan";
        }
        [Route("api/Home/PostFile")]
        [HttpPost]
        public string PostFile()
        {
            {
                    #region Variable Declaration  
                    string message = "";
                    HttpResponseMessage ResponseMessage = new HttpResponseMessage();
                    var httpRequest = HttpContext.Current.Request;
                    HttpPostedFile inputFile = null;
                    DataSet dsexcelRecords = new DataSet();

                    Stream fileStream = null;
                    IExcelDataReader reader = null;
                    List<Weather> weathers = new List<Weather>();
                    List<Load> loads = new List<Load>();
                    #endregion
                    inputFile = httpRequest.Files[0];
                    if (httpRequest.Files.Count > 0)
                    {
                        inputFile = httpRequest.Files[0];
                        fileStream = inputFile.InputStream;
                        if(inputFile !=null && fileStream != null)
                        {
                            if (inputFile.FileName.EndsWith(".xlsx"))
                            {
                                reader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                            }
                            else
                            {
                                message = "the file format is not supported";
                            }
                            dsexcelRecords = reader.AsDataSet();
                            reader.Close();
                            if(dsexcelRecords !=null && dsexcelRecords.Tables.Count > 0)
                            {
                                DataTable dtWeather = dsexcelRecords.Tables["weather"];
                                for(int i = 1; i < 10000; i++)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(dtWeather.Rows[i][0])))
                                    {
                                        Weather weather = new Weather();
                                        weather.Id = i;
                                        if (dtWeather.Rows[i][0] != null) {
                                        //string time = Convert.ToString(dtWeather.Rows[i][0]);
                                        //DateTime a;
                                        //DateTime.TryParse(time, out a);
                                        //weather.LocalTime = a;
                                        string a = Convert.ToString(dtWeather.Rows[i][0]);
                                        string[] split = a.Split(' ');
                                        string[] date = split[0].Split('.');
                                        string[] hours = split[1].Split(':');
                                        weather.LocalTime = new DateTime(Int32.Parse(date[2]), Int32.Parse(date[1]), Int32.Parse(date[0]), Int32.Parse(hours[0]), Int32.Parse(hours[1]), 0);
          
                                        }
                                        if (!string.IsNullOrEmpty(dtWeather.Rows[i][1].ToString())) {
                                            weather.Temperature = Convert.ToDouble(dtWeather.Rows[i][1]);
                                        }
                                        if (!string.IsNullOrEmpty(dtWeather.Rows[i][2].ToString())) {
                                            weather.APressure = Convert.ToDouble(dtWeather.Rows[i][2]);
                                        }
                                        if (!string.IsNullOrEmpty(dtWeather.Rows[i][3].ToString()))
                                        {
                                            weather.Pressure = Convert.ToDouble(dtWeather.Rows[i][3]);
                                        }
                                        if (!string.IsNullOrEmpty(dtWeather.Rows[i][4].ToString())) {
                                            weather.PTencdency = Convert.ToDouble(dtWeather.Rows[i][4]);
                                        }
                                        if (!string.IsNullOrEmpty(dtWeather.Rows[i][5].ToString())) {
                                            weather.Humidity = Convert.ToInt32(dtWeather.Rows[i][5]);
                                        }
                                        if (dtWeather.Rows[i][6] != null)
                                        {
                                            weather.WindDirection = Convert.ToString(dtWeather.Rows[i][6]);
                                        }
                                        if (!string.IsNullOrEmpty(dtWeather.Rows[i][7].ToString())) {
                                            weather.WindSpeed = Convert.ToInt32(dtWeather.Rows[i][7]);
                                        }
                                        if (dtWeather.Rows[i][10]!=null) {
                                            weather.Clouds = Convert.ToString(dtWeather.Rows[i][10]);
                                        }
                                        if (!string.IsNullOrEmpty(dtWeather.Rows[i][21].ToString()))
                                        {
                                        double b;
                                        double.TryParse(dtWeather.Rows[i][21].ToString(),out b);
                                        weather.HVisibility = b;
                                        //weather.HVisibility = Convert.ToDouble(dtWeather.Rows[i][21]);
                                        }
                                        if (!string.IsNullOrEmpty(dtWeather.Rows[i][22].ToString()))
                                        {
                                            weather.DTemperature = Convert.ToDouble(dtWeather.Rows[i][22]);
                                        }
                                        weathers.Add(weather);
                                    }
      

                                }
                            
                            DataTable dtLoad = dsexcelRecords.Tables["load"];
                            for (int i = 1; i < dtLoad.Rows.Count; i++)
                            {
                                Load load = new Load();
                                load.Id = i;
                                if (!string.IsNullOrEmpty(Convert.ToString(dtLoad.Rows[i][0])))
                                {
                                    string DateShort = Convert.ToString(dtLoad.Rows[i][0]);
                                    string TimeFrom = Convert.ToString(dtLoad.Rows[i][1]);
                                    string TimeTo = Convert.ToString(dtLoad.Rows[i][2]);
                                    string[] DShort = DateShort.Split(' ');
                                    string[] TFSplit = TimeFrom.Split(' ');
                                    string[] TTSplit = TimeTo.Split(' ');
                                    string[] Dshortsplited = DShort[0].Split('/');
                                    string[] TfSpliteed = TFSplit[1].Split(':');
                                    string[] TTsplited = TTSplit[1].Split(':');
                                    load.TimeFrom = new DateTime(Int32.Parse(Dshortsplited[2]), Int32.Parse(Dshortsplited[0]), Int32.Parse(Dshortsplited[1]), Int32.Parse(TfSpliteed[0]), Int32.Parse(TfSpliteed[1]), 0);
                                    load.TimeTo = new DateTime(Int32.Parse(Dshortsplited[2]), Int32.Parse(Dshortsplited[0]), Int32.Parse(Dshortsplited[1]), Int32.Parse(TTsplited[0]), Int32.Parse(TTsplited[1]), 0);
                                    load.LoadMWh = Convert.ToInt32(dtLoad.Rows[i][3].ToString());
                                }
                                loads.Add(load);
                            }
                        }
                            CrudOperations.AddEntites(loads, null,null);
                            
                        }
                    }


                    return "Uspesno";

            }

        }

        /* public ActionResult Add()
         {

         }*/
    }
}
