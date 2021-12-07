using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PredictionModel
{
    public class Traning
    {
        public static DataTable CreateDataTableFromObjects<Weather>(List<Weather> weathers, string name = null)
        {
            var myType = typeof(Weather);
            if (name == null)
            {
                name = myType.Name;
            }
            DataTable dt = new DataTable(name);
            foreach (PropertyInfo info in myType.GetProperties())
            {
                if (info.Name != "Id")
                {
                    dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
                }
            }
            foreach (var item in weathers)
            {
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo info in myType.GetProperties())
                {
                    if (info.Name != "Id")
                    {
                        dr[info.Name] = info.GetValue(item);
                    }
                }
            }
            return dt;
        }
    }
}
