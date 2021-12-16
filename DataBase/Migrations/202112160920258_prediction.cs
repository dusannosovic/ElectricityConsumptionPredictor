namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prediction : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PredictionsTable",
                c => new
                    {
                        Date = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Predicted = c.Int(nullable: false),
                        RealValue = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Date);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PredictionsTable");
        }
    }
}
