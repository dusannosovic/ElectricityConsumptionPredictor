namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class firstdatabasewithloadsunriseandweather : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LoadTable",
                c => new
                    {
                        TimeFrom = c.DateTime(nullable: false),
                        TimeTo = c.DateTime(nullable: false),
                        LoadMWh = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TimeFrom);
            
            CreateTable(
                "dbo.SunriseSunsetTable",
                c => new
                    {
                        Date = c.DateTime(nullable: false),
                        Sunrise = c.DateTime(nullable: false),
                        Sunset = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Date);
            
            CreateTable(
                "dbo.WeatherTable",
                c => new
                    {
                        LocalTime = c.DateTime(nullable: false),
                        Temperature = c.Double(nullable: false),
                        APressure = c.Double(nullable: false),
                        Pressure = c.Double(nullable: false),
                        PTencdency = c.Double(nullable: false),
                        Humidity = c.Int(nullable: false),
                        WindDirection = c.String(),
                        WindSpeed = c.Int(nullable: false),
                        Clouds = c.String(),
                        HVisibility = c.Double(nullable: false),
                        DTemperature = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.LocalTime);
            
            DropTable("dbo.TableEntities");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TableEntities",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Voltage = c.Single(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Name);
            
            DropTable("dbo.WeatherTable");
            DropTable("dbo.SunriseSunsetTable");
            DropTable("dbo.LoadTable");
        }
    }
}
