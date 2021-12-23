using DataBase.Controller;
using DataBase.Model;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ProjekatInteligentniInfSis.Controllers
{
    public class HomeController : ApiController
    {
        [Route("api/Home/GetTicket")]
        [HttpGet]
        public string GetTicket(int year,int month,int day, int numberofdays)
        {
            return "dusan";
        }
        [Route("api/Home/PostFile")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostFile()
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
                                DataTable dtLoad = dsexcelRecords.Tables["load"];
                                Dictionary<DateTime, Int32> Loads = ParseLoadToDictionary(dtLoad);
                                //int count = dtWeather.Rows.Count;
                                await CrudOperations.AddWeatherEntites(ParseWeather(dtWeather,Loads));

                                //CrudOperations.AddLoadEntites(ParseLoad(dtLoad));
                            }
                            //CrudOperations.AddLoadEntites(loads);
                            
                        }
                    }


                return new HttpResponseMessage(HttpStatusCode.OK);

            }

        }
        [Route("api/Home/GetNumberOfRow")]
        [HttpGet]
        public int NumberOfRow()
        {
            return CrudOperations.GetAllWeather().Count;
        }
        public List<Load> ParseLoad(DataTable dt)
        {
            List<Load> loads = new List<Load>();
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                Load load = new Load();
                load.Id = i;
                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][0])))
                {
                    string DateShort = Convert.ToString(dt.Rows[i][0]);
                    string TimeFrom = Convert.ToString(dt.Rows[i][1]);
                    string TimeTo = Convert.ToString(dt.Rows[i][2]);
                    string[] DShort = DateShort.Split(' ');
                    string[] TFSplit = TimeFrom.Split(' ');
                    string[] TTSplit = TimeTo.Split(' ');
                    string[] Dshortsplited = DShort[0].Split('/');
                    string[] TfSpliteed = TFSplit[1].Split(':');
                    string[] TTsplited = TTSplit[1].Split(':');
                    load.TimeFrom = new DateTime(Int32.Parse(Dshortsplited[2]), Int32.Parse(Dshortsplited[0]), Int32.Parse(Dshortsplited[1]), Int32.Parse(TfSpliteed[0]), Int32.Parse(TfSpliteed[1]), 0);
                    load.TimeTo = new DateTime(Int32.Parse(Dshortsplited[2]), Int32.Parse(Dshortsplited[0]), Int32.Parse(Dshortsplited[1]), Int32.Parse(TTsplited[0]), Int32.Parse(TTsplited[1]), 0);
                    load.LoadMWh = Convert.ToInt32(dt.Rows[i][3].ToString());
                }
                loads.Add(load);
            }
            return loads;
        }
        public Dictionary<DateTime,Int32> ParseLoadToDictionary(DataTable dt)
        {
            Dictionary<DateTime, Int32> Loads = new Dictionary<DateTime, Int32>();
            DateTime time;
            Int32 load;
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][0])))
                {
                    string DateShort = Convert.ToString(dt.Rows[i][0]);
                    DateTime Temp = Convert.ToDateTime(Convert.ToString(dt.Rows[i][1]));
                    string TimeFrom = Temp.TimeOfDay.ToString();
                    string TimeTo = Convert.ToString(dt.Rows[i][2]);
                    string[] DShort = DateShort.Split(' ');
                    string[] TFSplit = TimeFrom.Split(':');
                    string[] TTSplit = TimeTo.Split(' ');
                    string[] Dshortsplited = DShort[0].Split('/');
                    //string[] TfSpliteed = TFSplit[1].Split(':');
                    string[] TTsplited = TTSplit[1].Split(':');
                    time =  new DateTime(Int32.Parse(Dshortsplited[2]), Int32.Parse(Dshortsplited[0]), Int32.Parse(Dshortsplited[1]), Int32.Parse(TFSplit[0]), Int32.Parse(TFSplit[1]), 0);
                    //load.TimeTo = new DateTime(Int32.Parse(Dshortsplited[2]), Int32.Parse(Dshortsplited[0]), Int32.Parse(Dshortsplited[1]), Int32.Parse(TTsplited[0]), Int32.Parse(TTsplited[1]), 0);
                    load = Convert.ToInt32(dt.Rows[i][3].ToString());
                    Loads.Add(time, load);
                }
            }
            return Loads;

        }
        public List<Weather> ParseWeather(DataTable dtWeather, Dictionary<DateTime,Int32> loads)
        {
            List<Weather> weathers = new List<Weather>();
            for (int i = 1; i < dtWeather.Rows.Count; i++)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dtWeather.Rows[i][0])))
                {
                    Weather weather = new Weather();
                    weather.Id = i;
                    if (dtWeather.Rows[i][0] != null)
                    {
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
                    if (!string.IsNullOrEmpty(dtWeather.Rows[i][1].ToString()))
                    {
                        weather.Temperature = Convert.ToDouble(dtWeather.Rows[i][1]);
                    }
                    else
                    {
                        weather.Temperature = FindNumber(dtWeather, i, 1);
                    }
                    if (!string.IsNullOrEmpty(dtWeather.Rows[i][2].ToString()))
                    {
                        weather.APressure = Convert.ToDouble(dtWeather.Rows[i][2]);
                    }
                    else
                    {
                        weather.APressure = FindNumber(dtWeather, i, 2);
                    }
                    if (!string.IsNullOrEmpty(dtWeather.Rows[i][3].ToString()))
                    {
                        weather.Pressure = Convert.ToDouble(dtWeather.Rows[i][3]);
                    }
                    else
                    {
                        weather.Pressure = FindNumber(dtWeather, i, 3);
                    }
                    if (!string.IsNullOrEmpty(dtWeather.Rows[i][4].ToString()))
                    {
                        weather.PTencdency = Convert.ToDouble(dtWeather.Rows[i][4]);
                    }
                    else
                    {
                        weather.PTencdency = FindNumber(dtWeather, i, 4);
                    }
                    if (!string.IsNullOrEmpty(dtWeather.Rows[i][5].ToString()))
                    {
                        weather.Humidity = Convert.ToInt32(dtWeather.Rows[i][5]);
                    }
                    else
                    {
                        weather.Humidity = Convert.ToInt32(FindNumber(dtWeather, i, 5));
                    }
                    if (dtWeather.Rows[i][6] != null)
                    {
                        weather.WindDirection = Convert.ToString(dtWeather.Rows[i][6]);
                    }
                    if (!string.IsNullOrEmpty(dtWeather.Rows[i][7].ToString()))
                    {
                        weather.WindSpeed = Convert.ToInt32(dtWeather.Rows[i][7]);
                    }
                    else
                    {
                        weather.WindSpeed = Convert.ToInt32(FindNumber(dtWeather,i,7));
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(dtWeather.Rows[i][10])))
                    {
                        if (Convert.ToString(dtWeather.Rows[i][10]).Contains('%') && Convert.ToString(dtWeather.Rows[i][10]).Length < 6)
                        {

                            weather.Clouds = Double.Parse(Regex.Match(dtWeather.Rows[i][10].ToString(), @"\d+").ToString()) / 100;//https://stackoverflow.com/questions/4734116/find-and-extract-a-number-from-a-string
                        }
                        else if (Convert.ToString(dtWeather.Rows[i][10]).Contains("less"))
                        {
                            weather.Clouds = 0.05;
                        }
                        else if (Convert.ToString(dtWeather.Rows[i][10]).Contains("more"))
                        {
                            weather.Clouds = 0.95;
                        }
                        else if (Convert.ToString(dtWeather.Rows[i][10]).Contains("–"))
                        {
                            string[] a = Convert.ToString(dtWeather.Rows[i][10]).Split('–');
                            Double b = Convert.ToDouble(Regex.Match(a[0], @"\d+").ToString());
                            Double c = Convert.ToDouble(Regex.Match(a[1], @"\d+").ToString());
                            weather.Clouds = ((b + c) / 2) / 100;
                        }
                        else if (Convert.ToString(dtWeather.Rows[i][10]).Contains("no clouds"))
                        {
                            weather.Clouds = 0;
                        }
                        else if (Convert.ToString(dtWeather.Rows[i][10]).Contains("fog"))
                        {
                            weather.Clouds = 1;
                        }
                    }
                    else
                    {
                        int j = i;
                        int k = i;
                        while (string.IsNullOrEmpty(Convert.ToString(dtWeather.Rows[j][10])) && j >= 0)
                        {
                            j--;
                        }
                        while (string.IsNullOrEmpty(Convert.ToString(dtWeather.Rows[k][10])) && k < dtWeather.Rows.Count-1)
                        {
                            k++;
                        }
                        if (j == 0)
                        {
                            if (Convert.ToString(dtWeather.Rows[k][10]).Contains('%') && Convert.ToString(dtWeather.Rows[k][10]).Length < 6)
                            {

                                weather.Clouds = Double.Parse(Regex.Match(dtWeather.Rows[k][10].ToString(), @"\d+").ToString()) / 100;//https://stackoverflow.com/questions/4734116/find-and-extract-a-number-from-a-string
                            }
                            else if (Convert.ToString(dtWeather.Rows[k][10]).Contains("less"))
                            {
                                weather.Clouds = 0.05;
                            }
                            else if (Convert.ToString(dtWeather.Rows[k][10]).Contains("more"))
                            {
                                weather.Clouds = 0.95;
                            }
                            else if (Convert.ToString(dtWeather.Rows[k][10]).Contains("–"))
                            {
                                string[] a = Convert.ToString(dtWeather.Rows[k][10]).Split('–');
                                Double b = Convert.ToDouble(Regex.Match(a[0], @"\d+").ToString());
                                Double c = Convert.ToDouble(Regex.Match(a[1], @"\d+").ToString());
                                weather.Clouds = ((b + c) / 2) / 100;
                            }
                            else if (Convert.ToString(dtWeather.Rows[k][10]).Contains("no clouds"))
                            {
                                weather.Clouds = 0;
                            }
                            else if (Convert.ToString(dtWeather.Rows[k][10]).Contains("fog"))
                            {
                                weather.Clouds = 1;
                            }
                        }
                        else if (k == dtWeather.Rows.Count-1)
                        {
                            if (Convert.ToString(dtWeather.Rows[j][10]).Contains('%') && Convert.ToString(dtWeather.Rows[j][10]).Length < 6)
                            {

                                weather.Clouds = Double.Parse(Regex.Match(dtWeather.Rows[j][10].ToString(), @"\d+").ToString()) / 100;//https://stackoverflow.com/questions/4734116/find-and-extract-a-number-from-a-string
                            }
                            else if (Convert.ToString(dtWeather.Rows[j][10]).Contains("less"))
                            {
                                weather.Clouds = 0.05;
                            }
                            else if (Convert.ToString(dtWeather.Rows[j][10]).Contains("more"))
                            {
                                weather.Clouds = 0.95;
                            }
                            else if (Convert.ToString(dtWeather.Rows[j][10]).Contains("–"))
                            {
                                string[] a = Convert.ToString(dtWeather.Rows[j][10]).Split('–');
                                Double b = Convert.ToDouble(Regex.Match(a[0], @"\d+").ToString());
                                Double c = Convert.ToDouble(Regex.Match(a[1], @"\d+").ToString());
                                weather.Clouds = ((b + c) / 2) / 100;
                            }
                            else if (Convert.ToString(dtWeather.Rows[j][10]).Contains("no clouds"))
                            {
                                weather.Clouds = 0;
                            }
                            else if (Convert.ToString(dtWeather.Rows[j][10]).Contains("fog"))
                            {
                                weather.Clouds = 1;
                            }
                        }
                        else
                        {
                            Double x1;
                            Double x2;
                            if (Convert.ToString(dtWeather.Rows[j][10]).Contains('%') && Convert.ToString(dtWeather.Rows[j][10]).Length < 6)
                            {

                                x1 = Double.Parse(Regex.Match(dtWeather.Rows[j][10].ToString(), @"\d+").ToString()) / 100;//https://stackoverflow.com/questions/4734116/find-and-extract-a-number-from-a-string
                            }
                            else if (Convert.ToString(dtWeather.Rows[j][10]).Contains("less"))
                            {
                                x1 = 0.05;
                            }
                            else if (Convert.ToString(dtWeather.Rows[j][10]).Contains("more"))
                            {
                                x1 = 0.95;
                            }
                            else if (Convert.ToString(dtWeather.Rows[j][10]).Contains("–"))
                            {
                                string[] a = Convert.ToString(dtWeather.Rows[j][10]).Split('–');
                                Double b = Convert.ToDouble(Regex.Match(a[0], @"\d+").ToString());
                                Double c = Convert.ToDouble(Regex.Match(a[1], @"\d+").ToString());
                                x1 = ((b + c) / 2) / 100;
                            }
                            else if (Convert.ToString(dtWeather.Rows[j][10]).Contains("no clouds"))
                            {
                                x1 = 0;
                            }
                            else
                            {
                                x1 = 1;
                            }
                            if (Convert.ToString(dtWeather.Rows[k][10]).Contains('%') && Convert.ToString(dtWeather.Rows[k][10]).Length < 6)
                            {

                                x2 = Double.Parse(Regex.Match(dtWeather.Rows[k][10].ToString(), @"\d+").ToString()) / 100;//https://stackoverflow.com/questions/4734116/find-and-extract-a-number-from-a-string
                            }
                            else if (Convert.ToString(dtWeather.Rows[k][10]).Contains("less"))
                            {
                                x2 = 0.05;
                            }
                            else if (Convert.ToString(dtWeather.Rows[k][10]).Contains("more"))
                            {
                                x2 = 0.95;
                            }
                            else if (Convert.ToString(dtWeather.Rows[k][10]).Contains("–"))
                            {
                                string[] a = Convert.ToString(dtWeather.Rows[k][10]).Split('–');
                                Double b = Convert.ToDouble(Regex.Match(a[0], @"\d+").ToString());
                                Double c = Convert.ToDouble(Regex.Match(a[1], @"\d+").ToString());
                                x2 = ((b + c) / 2) / 100;
                            }
                            else if (Convert.ToString(dtWeather.Rows[k][10]).Contains("no clouds"))
                            {
                                x2 = 0;
                            }
                            else
                            {
                                x2 = 1;
                            }
                            weather.Clouds = (x1 + x2) / 2;
                        }
                    }
                    if (!string.IsNullOrEmpty(dtWeather.Rows[i][21].ToString()))
                    {
                        double b;
                        double.TryParse(dtWeather.Rows[i][21].ToString(), out b);
                        weather.HVisibility = b;
                        //weather.HVisibility = Convert.ToDouble(dtWeather.Rows[i][21]);
                    }
                    else
                    {
                        weather.HVisibility = FindNumber(dtWeather, i, 21);
                    }
                    if (!string.IsNullOrEmpty(dtWeather.Rows[i][22].ToString()))
                    {
                        weather.DTemperature = Convert.ToDouble(dtWeather.Rows[i][22]);
                    }
                    else
                    {
                        weather.DTemperature = FindNumber(dtWeather, i, 22);
                    }
                    if (loads.ContainsKey(weather.LocalTime))
                    {
                        weather.Load = loads[weather.LocalTime];
                    }
                    else
                    {
                        continue;
                    }
                    weathers.Add(weather);
                }
            }
            return weathers;
            }
        public double FindNumber(DataTable dtWeather, int i, int x)
        {
            int j = i;
            int k = i;
            while (string.IsNullOrEmpty(Convert.ToString(dtWeather.Rows[j][x])) && j >= 0)
            {
                j--;
            }
            while (string.IsNullOrEmpty(Convert.ToString(dtWeather.Rows[k][x])) && k < dtWeather.Rows.Count-1)
            {
                k++;
            }
            if (j == 0)
            {
                double b;
                double.TryParse(dtWeather.Rows[k][x].ToString(), out b);
                return b;
            }
            else if (k == dtWeather.Rows.Count-1)
            {
                double b;
                double.TryParse(dtWeather.Rows[j][x].ToString(), out b);
                return b;
            }
            else
            {
                double b;
                double c;
                double.TryParse(dtWeather.Rows[j][x].ToString(), out b);
                double.TryParse(dtWeather.Rows[k][x].ToString(), out c);
                return (b + c) / 2;
            }
        }

    }
}
