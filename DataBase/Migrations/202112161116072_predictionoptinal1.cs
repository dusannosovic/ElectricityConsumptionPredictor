namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class predictionoptinal1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PredictionsTable", "RealValue", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PredictionsTable", "RealValue", c => c.Int(nullable: false));
        }
    }
}
