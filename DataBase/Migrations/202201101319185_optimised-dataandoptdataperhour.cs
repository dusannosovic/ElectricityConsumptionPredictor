namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class optimiseddataandoptdataperhour : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OptimizedDatas",
                c => new
                    {
                        Index = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Type = c.String(),
                        Load = c.Double(nullable: false),
                        OptDataPerHour_DateAndTimeOfOptimization = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Index)
                .ForeignKey("dbo.OptimizedDataPerHours", t => t.OptDataPerHour_DateAndTimeOfOptimization, cascadeDelete: true)
                .Index(t => t.OptDataPerHour_DateAndTimeOfOptimization);
            
            CreateTable(
                "dbo.OptimizedDataPerHours",
                c => new
                    {
                        DateAndTimeOfOptimization = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.DateAndTimeOfOptimization);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OptimizedDatas", "OptDataPerHour_DateAndTimeOfOptimization", "dbo.OptimizedDataPerHours");
            DropIndex("dbo.OptimizedDatas", new[] { "OptDataPerHour_DateAndTimeOfOptimization" });
            DropTable("dbo.OptimizedDataPerHours");
            DropTable("dbo.OptimizedDatas");
        }
    }
}
