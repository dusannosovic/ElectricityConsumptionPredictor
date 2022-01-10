namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedcoststooptimizaddata : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OptimizedDatas", "Costs", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OptimizedDatas", "Costs");
        }
    }
}
