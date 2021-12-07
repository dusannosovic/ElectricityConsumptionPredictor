namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class doublecloud : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WeatherTable", "Clouds", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WeatherTable", "Clouds", c => c.String());
        }
    }
}
