namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sunriseSunset1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WeatherTable", "SunriseSunset", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.WeatherTable", "SunriseSunset");
        }
    }
}
