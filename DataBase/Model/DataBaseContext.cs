using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DataBase.Model
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext() : base("SystemMonitoringDatabase")
        {
           
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TableEntity>()
                .ToTable("TableEntities");
                
        }

        public DbSet<TableEntity> TableEntities { get; set; }
    }
}
