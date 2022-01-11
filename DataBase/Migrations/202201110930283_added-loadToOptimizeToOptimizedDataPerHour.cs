namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedloadToOptimizeToOptimizedDataPerHour : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OptimizedDataPerHours", "LoadToOptimize", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OptimizedDataPerHours", "LoadToOptimize");
        }
    }
}
