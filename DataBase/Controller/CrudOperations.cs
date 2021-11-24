using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBase.Model;

namespace DataBase.Controller
{
    public class CrudOperations
    {
        public static void AddEntites(List<TableEntity> list)
        {
            using (var db = new DataBaseContext())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [TableEntities]");
                db.SaveChanges();
                db.TableEntities.AddRange(list);
                db.SaveChanges();
            }
        }
    }
}
