﻿using System;
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
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));
        }

        public DbSet<Weather> WeatherTable { get; set; }
        public DbSet<Load> LoadTable { get; set; }
        public DbSet<SunriseSunset> SunriseSunsetTable { get; set; }


    }
}
