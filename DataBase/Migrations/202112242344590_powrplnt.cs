namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class powrplnt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PowerPlantsTable", "Area", c => c.Int(nullable: false));
            AddColumn("dbo.PowerPlantsTable", "Eff", c => c.Single(nullable: false));
            DropColumn("dbo.PowerPlantsTable", "FuelConsumption");
            DropColumn("dbo.PowerPlantsTable", "Co2Emmision");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PowerPlantsTable", "Co2Emmision", c => c.Int(nullable: false));
            AddColumn("dbo.PowerPlantsTable", "FuelConsumption", c => c.Int(nullable: false));
            DropColumn("dbo.PowerPlantsTable", "Eff");
            DropColumn("dbo.PowerPlantsTable", "Area");
        }
    }
}
