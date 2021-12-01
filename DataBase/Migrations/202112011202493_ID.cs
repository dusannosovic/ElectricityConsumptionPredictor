namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ID : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.WeatherTable");
            DropColumn("dbo.WeatherTable", "Index");
            AddColumn("dbo.WeatherTable", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.WeatherTable", "Id");

        }
        
        public override void Down()
        {
            AddColumn("dbo.WeatherTable", "Index", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.WeatherTable");
            DropColumn("dbo.WeatherTable", "Id");
            AddPrimaryKey("dbo.WeatherTable", "Index");
        }
    }
}
