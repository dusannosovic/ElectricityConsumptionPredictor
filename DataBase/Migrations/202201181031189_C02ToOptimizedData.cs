namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class C02ToOptimizedData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OptimizedDatas", "C02", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OptimizedDatas", "C02");
        }
    }
}
