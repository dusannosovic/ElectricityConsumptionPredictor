namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class delete_load_and_sunriseSunsetTable : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.LoadTable");
            DropTable("dbo.SunriseSunsetTable");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SunriseSunsetTable",
                c => new
                    {
                        Date = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Sunrise = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Sunset = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Date);
            
            CreateTable(
                "dbo.LoadTable",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimeFrom = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        TimeTo = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LoadMWh = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
