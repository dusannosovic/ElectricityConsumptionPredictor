namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addindexweather : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.WeatherTable");
            AddColumn("dbo.WeatherTable", "Index", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.WeatherTable", "Index");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.WeatherTable");
            DropColumn("dbo.WeatherTable", "Index");
            AddPrimaryKey("dbo.WeatherTable", "LocalTime");
        }
    }
}
