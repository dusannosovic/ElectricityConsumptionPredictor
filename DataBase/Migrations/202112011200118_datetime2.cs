namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime2 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.LoadTable");
            DropPrimaryKey("dbo.SunriseSunsetTable");
            AlterColumn("dbo.LoadTable", "TimeFrom", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.LoadTable", "TimeTo", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SunriseSunsetTable", "Date", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SunriseSunsetTable", "Sunrise", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SunriseSunsetTable", "Sunset", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.WeatherTable", "LocalTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddPrimaryKey("dbo.LoadTable", "TimeFrom");
            AddPrimaryKey("dbo.SunriseSunsetTable", "Date");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.SunriseSunsetTable");
            DropPrimaryKey("dbo.LoadTable");
            AlterColumn("dbo.WeatherTable", "LocalTime", c => c.DateTime());
            AlterColumn("dbo.SunriseSunsetTable", "Sunset", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SunriseSunsetTable", "Sunrise", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SunriseSunsetTable", "Date", c => c.DateTime(nullable: false));
            AlterColumn("dbo.LoadTable", "TimeTo", c => c.DateTime(nullable: false));
            AlterColumn("dbo.LoadTable", "TimeFrom", c => c.DateTime(nullable: false));
            AddPrimaryKey("dbo.SunriseSunsetTable", "Date");
            AddPrimaryKey("dbo.LoadTable", "TimeFrom");
        }
    }
}
