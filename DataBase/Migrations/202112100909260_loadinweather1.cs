namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class loadinweather1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WeatherTable", "Load", c => c.Int(nullable: false));
            AlterColumn("dbo.WeatherTable", "LocalTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WeatherTable", "LocalTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
            DropColumn("dbo.WeatherTable", "Load");
        }
    }
}
