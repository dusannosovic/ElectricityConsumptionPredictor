namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WeatherTable", "LocalTime", c => c.DateTime());
            AlterColumn("dbo.WeatherTable", "Temperature", c => c.Double());
            AlterColumn("dbo.WeatherTable", "APressure", c => c.Double());
            AlterColumn("dbo.WeatherTable", "Pressure", c => c.Double());
            AlterColumn("dbo.WeatherTable", "PTencdency", c => c.Double());
            AlterColumn("dbo.WeatherTable", "Humidity", c => c.Int());
            AlterColumn("dbo.WeatherTable", "WindSpeed", c => c.Int());
            AlterColumn("dbo.WeatherTable", "HVisibility", c => c.Double());
            AlterColumn("dbo.WeatherTable", "DTemperature", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WeatherTable", "DTemperature", c => c.Double(nullable: false));
            AlterColumn("dbo.WeatherTable", "HVisibility", c => c.Double(nullable: false));
            AlterColumn("dbo.WeatherTable", "WindSpeed", c => c.Int(nullable: false));
            AlterColumn("dbo.WeatherTable", "Humidity", c => c.Int(nullable: false));
            AlterColumn("dbo.WeatherTable", "PTencdency", c => c.Double(nullable: false));
            AlterColumn("dbo.WeatherTable", "Pressure", c => c.Double(nullable: false));
            AlterColumn("dbo.WeatherTable", "APressure", c => c.Double(nullable: false));
            AlterColumn("dbo.WeatherTable", "Temperature", c => c.Double(nullable: false));
            AlterColumn("dbo.WeatherTable", "LocalTime", c => c.DateTime(nullable: false));
        }
    }
}
