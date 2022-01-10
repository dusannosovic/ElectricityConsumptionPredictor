namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedcoststooptimizaddatanullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OptimizedDatas", "Costs", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OptimizedDatas", "Costs", c => c.Int(nullable: false));
        }
    }
}
