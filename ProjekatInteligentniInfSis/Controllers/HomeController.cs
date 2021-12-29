using DataBase.Controller;
using DataBase.Model;
using ExcelDataReader;
using PredictionModel;
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
             
                    string message = "";
                    HttpResponseMessage ResponseMessage = new HttpResponseMessage();
                    var httpRequest = HttpContext.Current.Request;
                    HttpPostedFile inputFile = null;
                    DataSet dsexcelRecords = new DataSet();
                    PrepareData dataPreparer = new PrepareData();

                    Stream fileStream = null;
                    IExcelDataReader reader = null;
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
                            if(dsexcelRecords !=null && dsexcelRecords.Tables.Count > 0 && dsexcelRecords.Tables["load"]!=null)
                            {
                                await dataPreparer.AddDataToDatabase(dsexcelRecords,true);

                            }else if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                            {
                                await dataPreparer.AddDataToDatabase(dsexcelRecords, false);
                            }
                            
                        }
                    }


                return new HttpResponseMessage(HttpStatusCode.OK);


        }
        [Route("api/Home/GetNumberOfRow")]
        [HttpGet]
        public int NumberOfRow()
        {
            return CrudOperations.GetAllWeather().Count;
        }
        [Route("api/Home/DeleteWeather")]
        [HttpDelete]
        public void DeleteWeather()
        {
            CrudOperations.DeleteWeatherTable();
        }
    }
}
