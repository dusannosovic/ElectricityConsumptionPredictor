namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class idload : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.LoadTable");
            AddColumn("dbo.LoadTable", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.LoadTable", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.LoadTable");
            DropColumn("dbo.LoadTable", "Id");
            AddPrimaryKey("dbo.LoadTable", "TimeFrom");
        }
    }
}
