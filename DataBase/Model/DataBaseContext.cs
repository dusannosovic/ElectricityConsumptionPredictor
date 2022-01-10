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
            modelBuilder.Entity<Load>().ToTable("LoadTable");
            modelBuilder.Entity<SunriseSunset>().ToTable("SunriseSunsetTable");
            modelBuilder.Entity<Weather>().ToTable("WeatherTable");
            modelBuilder.Entity<PowerPlant>().ToTable("PowerPlantsTable");
            modelBuilder.Entity<Prediction>().ToTable("PredictionsTable");

            modelBuilder.Entity<OptimizedData>().HasRequired(p => p.OptDataPerHour).WithMany(b => (ICollection<OptimizedData>)b.PwrPlantLoad);
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));

        }

        public DbSet<Weather> WeatherTable { get; set; }
        public DbSet<Load> LoadTable { get; set; }
        public DbSet<SunriseSunset> SunriseSunsetTable { get; set; }
        public DbSet<PowerPlant> PowerPlantsTable { get; set; }

        public DbSet<Prediction> PredictionsTable { get; set; }
        public DbSet<OptimizedData> OptimizedDataTable { get; set; }
        public DbSet<OptimizedDataPerHour> OptimizedDataTablePerHour { get; set; }


    }
}
