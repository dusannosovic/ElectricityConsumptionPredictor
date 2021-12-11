namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class powerplant : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PowerPlantsTable",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        MaxLoad = c.Int(nullable: false),
                        MinLoad = c.Int(nullable: false),
                        FuelConsumption = c.Int(nullable: false),
                        Co2Emmision = c.Int(nullable: false),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.Name);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PowerPlantsTable");
        }
    }
}
